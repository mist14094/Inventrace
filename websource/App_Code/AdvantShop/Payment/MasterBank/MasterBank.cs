//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    /// <summary>
    /// Summary description for WebMiney
    /// </summary>
    public class MasterBank : PaymentMethod
    {
        public string Terminal { get; set; }

        //public string ResultUrl { get; set; }
        //public string SuccessUrl { get; set; }
        //public FormMethod SuccessUrlMethod { get; set; }

        ////public string FailUrl { get; set; }
        //public FormMethod FailUrlMethod { get; set; }

        public override PaymentType Type
        {
            get { return PaymentType.MasterBank; }
        }
        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }
        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl; }
        }
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {MasterBankTemplate.Terminal, Terminal}
                           };
            }
            set
            {
                Terminal = value.ElementOrDefault(MasterBankTemplate.Terminal);
            }
        }
        public override void ProcessForm(Order order)
        {

            //1.	AMOUNT (����� � ������. ����������� ������ � �����)
            //2.	ORDER (���������� ����� ������. ������ ���� ����������. �������������� ��� ���������� �������. ��������� ������ ����� ������� 6-32 ��������.)
            //3.	MERCH_URL (URL, ������� ������������� �� ������ ������ � �������. ���� �� �����, ������� �� ���� �������� ���������)
            //4.	TERMINAL (��� ���������, ������������� ������)
            //5.	COUNTRY (������. ����������� ����������, ���� �������� ��������� �� � ������)
            //6.	TIMESTAMP (����� ���������� �������� � GMT (-4 ���� �� �����������). ������ YYYYMMDDHHMMSS)
            //7.	SIGN (�������� �������. ��������� �� ���������: md5(TERMINAL. TIMESTAMP.ORDER.AMOUNT.<��������� �����>) ����� ����� ����������� � �������� ������������)

            new PaymentFormHandler
                {
                    Url = "https://pay.masterbank.ru/acquiring",
                    InputValues = new Dictionary<string, string>
                                      {
                                          {"AMOUNT", order.Sum.ToString()},
                                          {"ORDER", order.OrderID.ToString()},
                                          {"MERCH_URL", SuccessUrl},
                                          {"TERMINAL", ""},
                                          {"COUNTRY", ""},
                                          {"TIMESTAMP", DateTime.Now.ToString("YYYYMMDDHHmmSS")},
                                          {"SIGN", GetMd5Hash(MD5.Create(),"" + order.OrderID.ToString() + order.Sum.ToString())}
                                      }
                }.Post();
        }

        public override string ProcessFormString(Order order, PaymentService.PageWithPaymentButton page)
        {
            //1.	AMOUNT (����� � ������. ����������� ������ � �����)
            //2.	ORDER (���������� ����� ������. ������ ���� ����������. �������������� ��� ���������� �������. ��������� ������ ����� ������� 6-32 ��������.)
            //3.	MERCH_URL (URL, ������� ������������� �� ������ ������ � �������. ���� �� �����, ������� �� ���� �������� ���������)
            //4.	TERMINAL (��� ���������, ������������� ������)
            //5.	COUNTRY (������. ����������� ����������, ���� �������� ��������� �� � ������)
            //6.	TIMESTAMP (����� ���������� �������� � GMT (-4 ���� �� �����������). ������ YYYYMMDDHHMMSS)
            //7.	SIGN (�������� �������. ��������� �� ���������: md5(TERMINAL. TIMESTAMP.ORDER.AMOUNT.<��������� �����>) ����� ����� ����������� � �������� ������������)
            return new PaymentFormHandler
             {
                 Url = "https://pay.masterbank.ru/acquiring",
                 Page = page,
                 InputValues = new Dictionary<string, string>
                                      {
                                          {"AMOUNT", order.Sum.ToString()},
                                          {"ORDER", order.OrderID.ToString()},
                                          {"MERCH_URL", SuccessUrl},
                                          {"TERMINAL", ""},
                                          {"COUNTRY", ""},
                                          {"TIMESTAMP", DateTime.Now.ToString("YYYYMMDDHHmmSS")},
                                          {"SIGN", GetMd5Hash(MD5.Create(), Terminal + order.OrderID.ToString() + order.Sum.ToString())}
                                      }
             }.ProcessRequest();
        }

        public override string ProcessResponse(HttpContext context)
        {

            //1.	RESULT (��������� ��������. 0 � �������� 2 � ��������� 3 � ����������� �������� )
            //2.	RC (��� ������ ISO8583)
            //3.	CURRENCY (������)
            //4.	ORDER
            //5.	RRN (����� �������� � �������� �������)
            //6.	INT_REF (���������� ��� ��������)
            //7.	AUTHCODE (��� �����������. ����� �������������)
            //8.	PAN (��������������� ����� �����)
            //9.	TRTYPE (��� ��������. 0 � ����������� (��������� ������ ������������), 21 � ���������� �������, 24 � �������.)
            //10.	TIMESTAMP
            //11.	SIGN (������������ ��� ������������ �������)
            //12.	AMOUNT

            var req = context.Request;
            if (!CheckData(req))
                return NotificationMessahges.InvalidRequestData;


            var paymentNumber = req["ORDER"];
            int orderID;
            if (int.TryParse(paymentNumber, out orderID) &&
                OrderService.GetOrder(orderID) != null)
            {
                var order = OrderService.GetOrder(orderID);
                if (order != null && req["RESULT"] == "0" && req["AMOUNT"] == string.Format("{0:0.00}", order.Sum))
                {
                    OrderService.PayOrder(orderID, true);
                    return NotificationMessahges.SuccessfullPayment(order.Number);
                }
            }
            return NotificationMessahges.Fail;
        }

        public bool CheckData(HttpRequest req)
        {
            //var fields = new string[]
            //                 {
            //                     "LMI_PAYEE_PURSE",
            //                     "LMI_PAYMENT_AMOUNT",
            //                     "LMI_PAYMENT_NO",
            //                     "LMI_MODE",
            //                     "LMI_SYS_INVS_NO",
            //                     "LMI_SYS_TRANS_NO",
            //                     "LMI_SYS_TRANS_DATE",
            //                     "LMI_SECRET_KEY",
            //                     "LMI_PAYER_PURSE",
            //                     "WMIdLMI_PAYER_WM"
            //                 };

            //;
            //return (!fields.Any(val => string.IsNullOrEmpty(req[val]))
            //    && fields.Aggregate<string, StringBuilder, string>(new StringBuilder(), (str, field) => str.Append(field == "LMI_SECRET_KEY" ? SecretKey : req[field]), Strings.ToString).Md5(true) != req["LMI_HASH"]);
            return true;
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}