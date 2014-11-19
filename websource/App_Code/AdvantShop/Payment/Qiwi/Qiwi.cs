using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace AdvantShop.Payment
{
    public class Qiwi : PaymentMethod
    {
        public string ProviderID { get; set; }
        public string RestID { get; set; }
        public string Password { get; set; }
        public string ProviderName { get; set; }
        public string CurrencyCode { get; set; }
        public float CurrencyValue { get; set; }

        public override PaymentType Type
        {
            get { return PaymentType.QIWI; }
        }

        public override ProcessType ProcessType
        {
            get { return ProcessType.ServerRequest; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler; }
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
                               {QiwiTemplate.ProviderID, ProviderID},
                               {QiwiTemplate.RestID, RestID},
                               {QiwiTemplate.Password, Password},
                               {QiwiTemplate.ProviderName, ProviderName},
                               {QiwiTemplate.CurrencyCode, CurrencyCode},
                               {QiwiTemplate.CurrencyValue, CurrencyValue.ToString()}
                           };
            }
            set
            {
                ProviderID = value.ElementOrDefault(QiwiTemplate.ProviderID);
                RestID = value.ElementOrDefault(QiwiTemplate.RestID);
                Password = value.ElementOrDefault(QiwiTemplate.Password);
                ProviderName = value.ElementOrDefault(QiwiTemplate.ProviderName);
                CurrencyCode = value.ElementOrDefault(QiwiTemplate.CurrencyCode);

                float decVal;
                CurrencyValue = value.ContainsKey(QiwiTemplate.CurrencyValue) &&
                                float.TryParse(value[QiwiTemplate.CurrencyValue], out decVal)
                                    ? decVal
                                    : 1;
            }
        }

        public override string ProcessServerRequest(Order order)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(string.Format("https://w.qiwi.com/api/v2/prv/{0}/bills/{1}", Parameters[QiwiTemplate.ProviderID], order.OrderID.ToString()));
            webRequest.Headers["Authorization"] = "Basic " +
                                                  Convert.ToBase64String(
                                                    Encoding.UTF8.GetBytes(
                                                        (Parameters.ContainsKey(QiwiTemplate.RestID) ? Parameters[QiwiTemplate.RestID] : Parameters[QiwiTemplate.ProviderID])
                                                        + ":" + Parameters[QiwiTemplate.Password]));

            webRequest.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            webRequest.PreAuthenticate = true;
            webRequest.Method = "PUT";
            webRequest.Accept = "text/json";

            string data = string.Format("user={0}&amount={1}&ccy={2}&comment={3}&lifetime={4}",
                HttpUtility.UrlEncode("tel:+" + (order.PaymentDetails != null ? order.PaymentDetails.Phone: "")),
                                        (order.Sum / CurrencyValue).ToString("F2").Replace(",", "."),
                                        Parameters[QiwiTemplate.CurrencyCode],
                                        order.OrderID.ToString(),
                                        HttpUtility.UrlEncode(DateTime.Now.AddDays(45).ToString("yyyy-MM-ddTHH:mm:ss"))
                );

            var bytes = Encoding.UTF8.GetBytes(data);
            webRequest.ContentLength = bytes.Length;
            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            string result = "";

            try
            {
                using (var response = webRequest.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                result = reader.ReadToEnd();
                            }
                    }
                }
            }
            catch (WebException e)
            {
                using (var eResponse = e.Response)
                {
                    using (var eStream = eResponse.GetResponseStream())
                    {
                        if (eStream != null)
                            using (var reader = new StreamReader(eStream))
                            {
                                result = reader.ReadToEnd();
                            }
                    }
                }
            }

            if (result.IsNotEmpty())
            {
                var qiwiAnswer = JsonConvert.DeserializeObject<QiwiTemplate.QIWIAnswer>(result);

                if (qiwiAnswer.response.bill != null && qiwiAnswer.response.result_code == 0 && qiwiAnswer.response.bill.status == "waiting")
                {
                    return string.Format("https://w.qiwi.com/order/external/main.action?shop={0}&transaction={1}",
                                         Parameters[QiwiTemplate.ProviderID], order.OrderID);
                }
                else
                {
                    Debug.LogError(result);
                }
            }

            return string.Empty;
        }

        public override string ProcessResponse(HttpContext context)
        {
            HttpRequest req = context.Request;
            int orderID = 0;

            string result = "";

            if (req.Headers["Authorization"] == "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(Parameters[QiwiTemplate.ProviderID] + ":" + Parameters[QiwiTemplate.Password])) &&
                int.TryParse(req["bill_id"], out orderID) && req["status"] == "paid")
            {
                Order order = OrderService.GetOrder(orderID);
                if (order != null)
                {
                    OrderService.PayOrder(orderID, true);
                    result = "<?xml version=\"1.0\"?><result><result_code>0</result_code></result>";
                }
                else
                {
                    result = "<?xml version=\"1.0\"?><result><result_code>300</result_code></result>";
                }
            }
            else
            {
                result = "<?xml version=\"1.0\"?><result><result_code>150</result_code></result>";
            }

            context.Response.Clear();
            context.Response.ContentType = "text/xml";
            context.Response.Write(result);
            context.Response.End();
            return result;
        }
    }
}