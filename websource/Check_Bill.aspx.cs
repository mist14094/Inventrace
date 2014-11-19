//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.itmcompany.ru
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Controls;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Taxes;
using Resources;

// TODO REWRITE

namespace ClientPages
{
    public partial class Check_Bill : AdvantShopClientPage
    {
        private Bill _bill;
        private Order _order;

        protected bool EmptyCheck
        {
            get { return OrderNumber == null || Bill == null; }
        }

        protected string OrderNumber
        {
            get { return Request["ordernumber"]; }
        }

        protected Order Order
        {
            get { return _order ?? (_order = OrderService.GetOrderByNumber(OrderNumber)); }
        }


        protected Bill Bill
        {
            get
            {
                if (_bill != null)
                    return _bill;

                if (Order.PaymentMethodId == 0)
                    return null;
                PaymentMethod method = PaymentService.GetPaymentMethod(Order.PaymentMethodId);
                if (!(method is Bill))
                    return null;
                _bill = (Bill) method;
                return _bill;
            }
        }

        protected override void InitializeCulture()
        {
            AdvantShop.Localization.Culture.InitializeCulture();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (EmptyCheck || Order == null)
                return;

            lblOrderID.Text = Order.OrderID.ToString();

            lblcompanyname.Text = Bill.CompanyName; // bill2.CompanyName;
            lTransactAccount.Text = Bill.TransAccount; // bill2.PayBill;
            lblinn.Text = Bill.INN;

            divKPP.Visible = Bill.KPP.IsNotEmpty();
            lblkpp.Text = Bill.KPP;
            lblbank.Text = Bill.BankName;
            lCorrespondentAccount.Text = Bill.CorAccount;
            lblbik.Text = Bill.BIK;

            lblProvider.Text = string.Format("ИНН {0}, {1} {2}, {3}{4}", Bill.INN,
                                             Bill.KPP.IsNotEmpty() ? "КПП " + Bill.KPP + "," : "", Bill.CompanyName,
                                             Bill.Address,
                                             ((string.IsNullOrEmpty(Bill.Telephone)) ? "" : ", тел. " + Bill.Telephone));


            lblDirector.Text = (string.IsNullOrEmpty(Bill.Director)) ? "______________________" : Bill.Director;
            lblAccountant.Text = (string.IsNullOrEmpty(Bill.Accountant)) ? "______________________" : Bill.Accountant;
            lblManager.Text = (string.IsNullOrEmpty(Bill.Manager)) ? "______________________" : Bill.Manager;

            lblDateTime.Text = (Order.OrderDate).ToString("dd.MM.yy");

            string userAddress = Order.BillingContact.Address;

            if (Order.PaymentDetails != null)
            {
                lblBuyer.Text = string.Format("{0}{1} {2}", (string.IsNullOrEmpty(Order.PaymentDetails.INN)) ? "" : ("ИНН " + Order.PaymentDetails.INN + " "), 
                                                            Order.PaymentDetails.CompanyName, userAddress);
            }
            else
            {
                lblBuyer.Text = userAddress;
            }
            
            rptOrderItems.DataBind();
            rCertificates.DataBind();
            trShipping.DataBind();

            float priceSum = 0;
            if (Order.OrderItems.Any())
            {
                priceSum = Order.OrderItems.Sum(oi => oi.Price * oi.Amount) + Order.ShippingCost + Order.PaymentCost;
            }
            else if(Order.OrderCertificates.Any())
            {
                priceSum = Order.OrderCertificates.Sum(cert => cert.Sum);
            }

            lbltotalprice.Text = CatalogService.GetStringPrice(priceSum, Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode);
            lTotalDiscount.Text = CatalogService.GetStringPrice(Order.TotalDiscount, Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode);
            lbltotalpricetopay.Text = CatalogService.GetStringPrice(Order.Sum, Order.OrderCurrency.CurrencyValue, Order.OrderCurrency.CurrencyCode);
            lbltotalprice2.Text = lbltotalpricetopay.Text;

            literalTaxCost.Text =
                BuildTaxTable(TaxServices.GetOrderTaxes(Order.OrderID).Where(p => p.TaxSum > 0).ToList(), Order.OrderCurrency);

            float sumToRender = Order.Sum / Order.OrderCurrency.CurrencyValue;
            var intPart = (int)(Math.Floor(sumToRender));
            int floatPart = sumToRender != 0
                                ? SQLDataHelper.GetInt(Math.Round(sumToRender - Math.Floor(sumToRender), 2) * 100)
                                : 0;

            string script = "<script>num2str(\'" + intPart + "\', \'str\');</script>";
            switch (floatPart%10)
            {
                case 1:
                    lbltotalkop.Text = floatPart.ToString("0#") + @" копейка";
                    break;
                case 2:
                case 3:
                case 4:
                    lbltotalkop.Text = floatPart.ToString("0#") + @" копейки";
                    break;
                default:
                    lbltotalkop.Text = floatPart.ToString("0#") + @" копеек";
                    break;
            }

            ClientScript.RegisterStartupScript(typeof (String), "A", script);
        }

        private static string BuildTaxTable(ICollection<TaxValue> taxes, OrderCurrency currency)
        {
            var sb = new StringBuilder();
            if (taxes.Count != 0)
            {
                foreach (var tax in taxes)
                {
                    sb.Append(
                        "<tr bgcolor=\"white\" runat=\"server\" id=\"rowNDS\"><td align=\"right\" width=\"82%\"><font class=\"sc\">");
                    sb.Append((tax.TaxShowInPrice ? Resource.Core_TaxServices_Include_Tax : "") + " " + tax.TaxName);
                    sb.Append(":</font></td><td align=\"right\" width=\"18%\"><font class=\"sc\"><b>");
                    sb.Append(CatalogService.GetStringPrice(tax.TaxSum, currency.CurrencyValue, currency.CurrencyCode));
                    sb.Append("</b></font></td></tr>");
                }
            }
            else
            {
                sb.Append(
                    "<tr bgcolor=\"white\" runat=\"server\" id=\"rowNDS\"><td align=\"right\" width=\"82%\"><font class=\"sc\">");
                sb.Append(Resource.Client_Bill2_NDSAlreadyIncluded);
                sb.Append("</font></td><td align=\"right\" width=\"18%\"><font class=\"sc\"><b>");
                sb.Append("Без НДС");
                sb.Append("</b></font></td></tr>");
            }

            return sb.ToString();
        }
    }
}