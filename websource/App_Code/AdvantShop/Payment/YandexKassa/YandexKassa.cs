//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Xml;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    public class YandexKassa : PaymentMethod
    {
        public string ShopId { get; set; }
        public string ScId { get; set; }
        public float CurrencyValue { get; set; }
        public string FileCertificate { get; set; }
        public string YaPaymentType { get; set; }
        public string Password { get; set; }

        public override PaymentType Type
        {
            get { return PaymentType.YandexKassa; }
        }

        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler; }
        }

        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.FailUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }

        public override string NotificationUrl
        {
            get
            {
                return "https://demo.advantshop.net/yandexkassa/" + StringHelper.EncodeTo64(base.NotificationUrl);
            }
        }
        
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {YandexKassaTemplate.ShopID, ShopId},
                               {YandexKassaTemplate.ScID, ScId},
                               {YandexKassaTemplate.CurrencyValue, CurrencyValue.ToString(CultureInfo.InvariantCulture)},
                               {YandexKassaTemplate.FileCertificate, FileCertificate},
                               {YandexKassaTemplate.YaPaymentType, YaPaymentType},
                               {YandexKassaTemplate.Password, Password}
                           };
            }
            set
            {
                ShopId = value.ElementOrDefault(YandexKassaTemplate.ShopID);
                ScId = value.ElementOrDefault(YandexKassaTemplate.ScID);
                YaPaymentType = value.ElementOrDefault(YandexKassaTemplate.YaPaymentType);
                Password = value.ElementOrDefault(YandexKassaTemplate.Password);
                FileCertificate = value.ElementOrDefault(YandexKassaTemplate.FileCertificate);
                float decVal;
                CurrencyValue = value.ContainsKey(YandexKassaTemplate.CurrencyValue) &&
                                float.TryParse(value[YandexKassaTemplate.CurrencyValue], NumberStyles.Float, CultureInfo.InvariantCulture, out decVal)
                                    ? decVal
                                    : 1;
            }
        }

        public override void ProcessForm(Order order)
        {
            new PaymentFormHandler
            {
                Url = "https://money.yandex.ru/eshop.xml",//https://demomoney.yandex.ru/eshop.xml
                InputValues =
                {
                    {"shopId", ShopId},
                    {"scid", ScId},
                    {"sum", (order.Sum/CurrencyValue).ToString("F2").Replace(",", ".")},
                    {"customerNumber", order.OrderCustomer.CustomerID.ToString().Normalize()},
                    {"orderNumber", order.OrderID.ToString(CultureInfo.InvariantCulture).Normalize()},
                    {"shopSuccessURL", HttpUtility.UrlEncode(SuccessUrl)},
                    {"shopFailURL", HttpUtility.UrlEncode(FailUrl)},
                    {"cps_email", order.OrderCustomer.Email ?? string.Empty},
                    {"paymentType", YaPaymentType},
                    {
                        "cps_phone",
                        order.OrderCustomer.MobilePhone.IsNotEmpty() &&
                        order.OrderCustomer.MobilePhone.All(char.IsDigit) &&
                        order.OrderCustomer.MobilePhone.Length <= 15
                            ? order.OrderCustomer.MobilePhone
                            : string.Empty
                    }
                }
            }.Post();
        }

        public override string ProcessFormString(Order order, PaymentService.PageWithPaymentButton page)
        {
            return new PaymentFormHandler
            {
                Url = "https://money.yandex.ru/eshop.xml",//https://demomoney.yandex.ru/eshop.xml
                Page = page,
                Method = FormMethod.POST,
                InputValues =
                {
                    {"shopId", ShopId},
                    {"scid", ScId},
                    {"sum", (order.Sum/CurrencyValue).ToString("F2").Replace(",", ".")},
                    {"customerNumber", order.OrderCustomer.CustomerID.ToString().Normalize()},
                    {"orderNumber", order.OrderID.ToString(CultureInfo.InvariantCulture).Normalize()},
                    {"shopSuccessURL", HttpUtility.UrlEncode(SuccessUrl)},
                    {"shopFailURL", HttpUtility.UrlEncode(FailUrl)},
                    {"cps_email", order.OrderCustomer.Email ?? string.Empty},
                    {"paymentType", YaPaymentType},
                    {
                        "cps_phone",
                        order.OrderCustomer.MobilePhone.IsNotEmpty() &&
                        order.OrderCustomer.MobilePhone.All(char.IsDigit) &&
                        order.OrderCustomer.MobilePhone.Length <= 15
                            ? order.OrderCustomer.MobilePhone
                            : string.Empty
                    }
                }
            }.ProcessRequest();
        }

        public override string ProcessResponse(HttpContext context)
        {
            var typeRequest = TypeRequestYandex.checkOrder;
            var processingResult = ProcessingResult.ErrorParsing;
            var invoiceId = string.Empty;

            try
            {
                if (context.Request.ContentType.Equals("application/pkcs7-mime", StringComparison.InvariantCultureIgnoreCase))
                {
                    ProcessingPkcs7(context, ref processingResult, ref typeRequest, ref invoiceId);
                }
                else
                {
                    ProcessingMd5(context, ref processingResult, ref typeRequest, ref invoiceId);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                processingResult = ProcessingResult.Exception;
            }

            var result = RendAnswer(typeRequest, processingResult, invoiceId);
            context.Response.Clear();
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.ContentType = "application/xml";
            context.Response.Write(result);
            context.Response.End();
            return result;
        }

        private string RendAnswer(TypeRequestYandex typeRequest, ProcessingResult processingResult, string invoiceId)
        {
            return string.Format(
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?><{0}Response performedDatetime=\"{1}\" code=\"{2}\" invoiceId=\"{3}\" shopId=\"{4}\"/>",
                typeRequest, DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fzzz"), (int)processingResult, invoiceId, ShopId);
        }

        private bool IsCheckFields(Dictionary<string, string> parameters, TypeRequestYandex typeRequest)
        {
            decimal orderSumAmount;

            if (parameters["shopId"].Equals(ShopId, StringComparison.InvariantCultureIgnoreCase) &&
                parameters["invoiceId"].IsNotEmpty() && parameters["invoiceId"].All(char.IsDigit) &&
                parameters["orderNumber"].IsNotEmpty() && parameters["orderNumber"].All(char.IsDigit) &&
                parameters["orderSumAmount"].IsNotEmpty() &&
                decimal.TryParse(parameters["orderSumAmount"], NumberStyles.Float, CultureInfo.InvariantCulture, out orderSumAmount))
            {
                var ord = OrderService.GetOrder(parameters["orderNumber"].TryParseInt());

                if (ord != null &&
                    // Если это запрос "Уведомление о переводе", которые могут повторяться несколько раз (упомянуто в документации),
                    // тогда неважно заказ был уже отмечен оплаченным или уже отменен
                    (typeRequest == TypeRequestYandex.paymentAviso || (!ord.Payed && ord.OrderStatusId != OrderService.CanceledOrderStatus)) &&
                    ord.OrderCustomer.CustomerID.ToString().Normalize().Equals(parameters["customerNumber"], StringComparison.InvariantCultureIgnoreCase) &&
                    orderSumAmount >= Math.Round((decimal) (ord.Sum/CurrencyValue), 2))
                {
                    return true;
                }
            }
            return false;
        }

        #region PKCS#7

        private void ProcessingPkcs7(HttpContext context, ref ProcessingResult processingResult,
            ref TypeRequestYandex typeRequest, ref string invoiceId)
        {
            string inputData;
            using (var stream = context.Request.InputStream)
            {
                using (var reader = new StreamReader(stream))
                {
                    inputData = reader.ReadToEnd();
                }
            }

            if (inputData != null)
            {
                inputData = inputData.Replace("\n", "").Replace("-----BEGIN PKCS7-----", "").Replace("-----END PKCS7-----", "");

                var certificate1 = new X509Certificate2(SettingsGeneral.AbsolutePath + @"\App_Data\YandexKassa\" + FileCertificate);
                var signedCms = new SignedCms();
                signedCms.Decode(Convert.FromBase64String(inputData));
                try
                {
                    signedCms.CheckSignature(new X509Certificate2Collection(certificate1), true);
                }
                catch (System.Security.Cryptography.CryptographicException)
                {
                    processingResult = ProcessingResult.ErrorAuthorize;
                }

                if (processingResult != ProcessingResult.ErrorAuthorize)
                {
                    var parameters = ReadParametersPkcs7(Encoding.UTF8.GetString(signedCms.ContentInfo.Content),
                        ref typeRequest);

                    invoiceId = parameters["invoiceId"];

                    if (IsCheckFields(parameters, typeRequest))
                    {
                        if (typeRequest == TypeRequestYandex.paymentAviso)
                            OrderService.PayOrder(parameters["orderNumber"].TryParseInt(), true);

                        processingResult = ProcessingResult.Success;
                    }
                    else
                    {
                        processingResult = typeRequest == TypeRequestYandex.checkOrder
                            ? ProcessingResult.TranslationFailure
                            : ProcessingResult.ErrorParsing;

                    }
                }
            }
        }

        private Dictionary<string, string> ReadParametersPkcs7(string strData, ref TypeRequestYandex typeRequest)
        {
            var parameters = new Dictionary<string, string>();

            using (var sr = new StringReader(strData))
            using (var reader = new XmlTextReader(sr))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "checkOrderRequest" ||
                                reader.Name == "paymentAvisoRequest")
                            {
                                if (reader.Name == "checkOrderRequest")
                                    typeRequest = TypeRequestYandex.checkOrder;
                                if (reader.Name == "paymentAvisoRequest")
                                    typeRequest = TypeRequestYandex.paymentAviso;

                                if (reader.HasAttributes)
                                    while (reader.MoveToNextAttribute())
                                        parameters.Add(reader.Name, reader.Value);
                            }
                            break;
                    }
                }
            }

            return parameters;
        }

        #endregion

        #region NVP/MD5

        private void ProcessingMd5(HttpContext context, ref ProcessingResult processingResult,
            ref TypeRequestYandex typeRequest, ref string invoiceId)
        {
            var parameters = ReadParametersMd5(context, ref typeRequest);

            invoiceId = parameters["invoiceId"];

            if (IsCheckMd5(parameters))
            {
                if (IsCheckFields(parameters, typeRequest))
                {
                    if (typeRequest == TypeRequestYandex.paymentAviso)
                        OrderService.PayOrder(parameters["orderNumber"].TryParseInt(), true);

                    processingResult = ProcessingResult.Success;
                }
                else
                {
                    processingResult = typeRequest == TypeRequestYandex.checkOrder
                        ? ProcessingResult.TranslationFailure
                        : ProcessingResult.ErrorParsing;

                }
            }
            else
                processingResult = ProcessingResult.ErrorAuthorize;
        }

        private Dictionary<string, string> ReadParametersMd5(HttpContext context, ref TypeRequestYandex typeRequest)
        {
            if (context.Request.Form["action"].IsNotEmpty())
            {
                if (context.Request.Form["action"].Equals("checkOrder", StringComparison.InvariantCultureIgnoreCase))
                    typeRequest = TypeRequestYandex.checkOrder;
                else if (context.Request.Form["action"].Equals("paymentAviso", StringComparison.InvariantCultureIgnoreCase))
                    typeRequest = TypeRequestYandex.paymentAviso;
            }
            return context.Request.Form.AllKeys.ToDictionary(key => key, key => context.Request.Form[key]);
        }

        private bool IsCheckMd5(Dictionary<string, string> parameters)
        {
            return parameters["md5"].ToLower() ==
                   string.Format("{0};{1};{2};{3};{4};{5};{6};{7}",
                       parameters["action"],
                       parameters["orderSumAmount"],
                       parameters["orderSumCurrencyPaycash"],
                       parameters["orderSumBankPaycash"],
                       parameters["shopId"],
                       parameters["invoiceId"],
                       parameters["customerNumber"],
                       Password).Md5(false);
        }

        #endregion

        private enum TypeRequestYandex
        {

            //Do not change the register
            checkOrder,
            paymentAviso
        }

        private enum ProcessingResult : int
        {
            Success = 0,
            ErrorAuthorize = 1,
            TranslationFailure = 100,
            ErrorParsing = 200,
            Exception = 1000
        }
    }
}