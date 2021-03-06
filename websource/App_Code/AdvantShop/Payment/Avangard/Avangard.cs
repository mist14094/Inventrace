//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    public class Avangard : PaymentMethod
    {
        public override PaymentType Type
        {
            get { return PaymentType.Avangard; }
        }

        public override ProcessType ProcessType
        {
            get { return ProcessType.ServerRequest; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl; }
        }

        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.CancelUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {AvangardTemplate.ShopId, ShopId},
                               {AvangardTemplate.ShopPassword, ShopPassword}
                           };
            }
            set
            {
                if (value.ContainsKey(AvangardTemplate.ShopId))
                    ShopId = value[AvangardTemplate.ShopId];
                ShopPassword = value.ElementOrDefault(AvangardTemplate.ShopPassword);
            }
        }

        public string ShopId { get; set; }

        public string ShopPassword { get; set; }

        public override string ProcessServerRequest(Order order)
        {
            var responseTicket = GetTicket(order);
            if (responseTicket.ResponseCode != 0)
            {
                return string.Empty;
            }

            PaymentService.SaveOrderpaymentInfo(order.OrderID, this.PaymentMethodID, AvangardTemplate.Ticket, responseTicket.Ticket);
            PaymentService.SaveOrderpaymentInfo(order.OrderID, this.PaymentMethodID, AvangardTemplate.OkCode, responseTicket.OkCode);
            PaymentService.SaveOrderpaymentInfo(order.OrderID, this.PaymentMethodID, AvangardTemplate.FailureCode, responseTicket.FailureCode);

            return string.Format("https://www.avangard.ru/iacq/pay?ticket={0}", responseTicket.Ticket);
        }

        public override string ProcessResponse(HttpContext context)
        {
            string failNotification = "<span style=\"color:red;font-size:14px;\">������ �� ���������: ����� ����� � �������� �����. ������ � �������� ������, ������� �������� ������ �����.</span>";
            HttpRequest req = context.Request;

            if (string.IsNullOrEmpty(req["result_code"]))
            {
                return failNotification;
            }

            var additionalInfo = PaymentService.GetOrderIdByMaymentIdAndCode(this.PaymentMethodID, req["result_code"]);
            if (additionalInfo.Name == AvangardTemplate.FailureCode)
            {
                return failNotification;
            }

            var order = OrderService.GetOrder(additionalInfo.OrderId);
            if (order != null)
            {
                OrderService.PayOrder(order.OrderID, true);
                return "<span style=\"color:green; font-size:14px; \">������ ���������</span>";
                //NotificationMessahges.SuccessfullPayment(order.OrderID.ToString());
            }
            return failNotification;
        }

        private AvangardResponse GetTicket(Order order)
        {
            var result = new AvangardResponse();

            string sum = (Math.Round(order.Sum) * 100).ToString();

            var requestXmlString =
                "<?xml version=\"1.0\" encoding=\"windows-1251\"?>" +
                "<NEW_ORDER>" +
                "<SHOP_ID>{0}</SHOP_ID>" +
                "<SHOP_PASSWD>{1}</SHOP_PASSWD>" +
                "<AMOUNT>{2}</AMOUNT>" +
                "<ORDER_NUMBER>{3}</ORDER_NUMBER>" +
                "<ORDER_DESCRIPTION>{4}</ORDER_DESCRIPTION>" +
                "<LANGUAGE>{5}</LANGUAGE>" +
                "<BACK_URL>{6}</BACK_URL>" +
                "<CLIENT_NAME>{7}</CLIENT_NAME>" +
                "<CLIENT_ADDRESS>{8}</CLIENT_ADDRESS>" +
                "<CLIENT_EMAIL>{9}</CLIENT_EMAIL>" +
                "<CLIENT_PHONE>{10}</CLIENT_PHONE>" +
                "<CLIENT_IP>{11}</CLIENT_IP>" +
                "</NEW_ORDER >";

            //System.Net.HttpWebRequest adds the header 'HTTP header "Expect: 100-Continue"' to every request unless you explicitly ask it not to by setting this static property to false:
            ServicePointManager.Expect100Continue = false;

            var postData = string.Format(requestXmlString,
               ShopId,
               ShopPassword,
               sum,
               order.OrderID.ToString(),
               Translit(GetOrderDescription(order.Number)),
               CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
               this.SuccessUrl,
               Translit(order.OrderCustomer.FirstName + " " + order.OrderCustomer.LastName),
               Translit(order.BillingContact.Country + "," + order.BillingContact.City + "," + order.BillingContact.Address),
               order.OrderCustomer.Email,
               order.OrderCustomer.MobilePhone,
               order.OrderCustomer.CustomerIP
               );

            WebRequest request = WebRequest.Create("https://www.avangard.ru/iacq/h2h/reg?xml=" + HttpUtility.UrlEncode(postData));
            request.Method = "GET";

            using (var reader = new StreamReader(request.GetResponse().GetResponseStream(), Encoding.GetEncoding("windows-1251")))
            {
                var responseFromServer = reader.ReadToEnd();

                using (var xmlReader = XmlReader.Create(new StringReader(responseFromServer)))
                {
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            switch (xmlReader.Name)
                            {
                                case "id":
                                    int id;
                                    if (xmlReader.Read() && int.TryParse(xmlReader.Value, out id))
                                    {
                                        result.Id = id;
                                    }
                                    break;
                                case "ticket":
                                    if (xmlReader.Read())
                                    {
                                        result.Ticket = xmlReader.Value;
                                    }
                                    break;
                                case "ok_code":
                                    if (xmlReader.Read())
                                    {
                                        result.OkCode = xmlReader.Value;
                                    }
                                    break;
                                case "failure_code":
                                    if (xmlReader.Read())
                                    {
                                        result.FailureCode = xmlReader.Value;
                                    }
                                    break;
                                case "response_code":
                                    int responseCode;
                                    if (xmlReader.Read() && int.TryParse(xmlReader.Value, out responseCode))
                                    {
                                        result.ResponseCode = responseCode;
                                    }
                                    break;
                                case "response_message":
                                    if (xmlReader.Read())
                                    {
                                        result.ResponseMessage = xmlReader.Value;
                                    }
                                    break;
                            }
                        }
                    }
                }

                reader.Close();
            }

            return result;
        }

        private string Translit(string input)
        {
            var dictionary = new Dictionary<string, string>
                {
                    {"�","a"},
                    {"�","b"},
                    {"�","v"},
                    {"�","g"},
                    {"�","d"},
                    {"�","e"},
                    {"�","jo"},
                    {"�","zh"},
                    {"�","z"},
                    {"�","i"},
                    {"�","j"},
                    {"�","k"},
                    {"�","l"},
                    {"�","m"},
                    {"�","n"},
                    {"�","o"},
                    {"�","p"},
                    {"�","r"},
                    {"�","s"},
                    {"�","t"},
                    {"�","u"},
                    {"�","f"},
                    {"�","h"},
                    {"�","c"},
                    {"�","ch"},
                    {"�","sh"},
                    {"�","shh"},
                    {"�",""},
                    {"�","y"},
                    {"�","'"},
                    {"�","je"},
                    {"�","ju"},
                    {"�","ja"}
                };



            var output = string.Empty;
            input = input.ToLower();
            for (int i = 0; i < input.Length; i++)
            {
                output += dictionary.ContainsKey(input[i].ToString())
                              ? dictionary[input[i].ToString()].ToString()
                              : input[i].ToString();
            }
            return output;
        }
    }
}