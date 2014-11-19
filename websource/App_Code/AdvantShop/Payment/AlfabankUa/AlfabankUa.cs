//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    public class AlfabankUa : PaymentMethod
    {
        public string PartnerId { get; set; }

        public override PaymentType Type
        {
            get { return PaymentType.AlfabankUa; }
        }
        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }
        public override Dictionary<string,string > Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {AlfabankUaTemplate.PartnerId, PartnerId},
                };
            }
            set
            {
                PartnerId = value.ElementOrDefault(AlfabankUaTemplate.PartnerId);
            }
        }

        public override void ProcessForm(Order order)
        {
            new PaymentFormHandler
            {
                FormName = "_xclick",
                Method = FormMethod.GET,
                Url = "http://www.alfabank.ua/credit/bpk/index.php" ,
                InputValues = new Dictionary<string, string>
                {
                    {"partner", PartnerId},
                    {"surname", order.OrderCustomer.LastName},
                    {"name", order.OrderCustomer.FirstName},
                    {"midname", ""},
                    {"phone", (order.OrderCustomer.MobilePhone.IsNotEmpty() ? order.OrderCustomer.MobilePhone : string.Empty)},
                    {"email", order.OrderCustomer.Email},
                    {"product", string.Join(",", order.OrderItems.Select(item => HttpUtility.HtmlEncode(item.Name)))}
                }
            }.Post();
        }

        public override string ProcessFormString(Order order, PaymentService.PageWithPaymentButton page)
        {
            return new PaymentFormHandler
            {
                FormName = "_xclick",
                Method = FormMethod.GET,
                Url = "http://www.alfabank.ua/credit/bpk/index.php",
                InputValues = new Dictionary<string, string>
                {
                    {"partner", PartnerId},
                    {"surname", order.OrderCustomer.LastName},
                    {"name", order.OrderCustomer.FirstName},
                    {"midname", ""},
                    {"phone", (order.OrderCustomer.MobilePhone.IsNotEmpty() ? order.OrderCustomer.MobilePhone : string.Empty)},
                    {"email", order.OrderCustomer.Email},
                    {"product", string.Join(",", order.OrderItems.Select(item => HttpUtility.HtmlEncode(item.Name)))}
                }
            }.ProcessRequest();
        }
    }
}