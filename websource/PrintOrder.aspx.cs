//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Controls;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using Resources;
using AdvantShop.Configuration;

namespace ClientPages
{
    public partial class PrintOrder : AdvantShopClientPage
    {
        protected bool showStatusInfo = SettingsOrderConfirmation.PrintOrder_ShowStatusInfo;
        protected bool showMap = SettingsOrderConfirmation.PrintOrder_ShowMap;
        protected string mapType = SettingsOrderConfirmation.PrintOrder_MapType;
        protected string mapAdress;

        protected Order order;
        protected OrderCurrency ordCurrency = null;
        protected bool ShowOptions = true;

        protected override void InitializeCulture()
        {
            AdvantShop.Localization.Culture.InitializeCulture();
        }

        protected string RenderSelectedOptions(IList<EvaluatedCustomOptions> evlist)
        {
            if (evlist == null || evlist.Count == 0)
                return "&nbsp;";

            var html = new StringBuilder();
            html.Append("<ul>");

            foreach (EvaluatedCustomOptions ev in evlist)
            {
                html.Append(string.Format("<li>{0}: {1}</li>", ev.CustomOptionTitle, ev.OptionTitle));
            }

            html.Append("</ul>");
            return html.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request["ordernumber"]))
            {
                Response.Redirect("default.aspx");
            }

            try
            {
                int orderId = OrderService.GetOrderIdByNumber(Request["ordernumber"]);

                lblOrderID.Text = Resource.Admin_ViewOrder_ItemNum + orderId;

                if (!IsPostBack)
                {
                    order = OrderService.GetOrder(orderId);

                    if (order != null)
                    {
                        ordCurrency = order.OrderCurrency;
                        lOrderDate.Text = AdvantShop.Localization.Culture.ConvertDate(order.OrderDate);

                        if (order.OrderCertificates == null || order.OrderCertificates.Count == 0)
                        {
                            lblShipping.Text = @"<b>" + Resource.Admin_ViewOrder_Name + @"</b>&nbsp;" +
                                               order.ShippingContact.Name;
                            lblShipping.Text += @"<br />";
                            lblShipping.Text += @"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>" + Resource.Admin_ViewOrder_Country +
                                                @"</b>&nbsp;" + order.ShippingContact.Country + @"<br />";
                            lblShipping.Text += @"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>" + Resource.Admin_ViewOrder_City +
                                                @"</b>&nbsp;" + order.ShippingContact.City + @"<br />";

                            if (order.ShippingContact.Zone != null)
                            {
                                lblShipping.Text += @"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>" + Resource.Admin_ViewOrder_Zone +
                                                    @"</b>&nbsp;" + order.ShippingContact.Zone.Trim() + @"<br />";
                            }

                            if (order.ShippingContact.Zip != null)
                            {
                                lblShipping.Text += @"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>" + Resource.Admin_ViewOrder_Zip +
                                                    @"</b>&nbsp;" + order.ShippingContact.Zip.Trim() + @"<br />";
                            }

                            lblShipping.Text += @"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>" + Resource.Admin_ViewOrder_Address +
                                                @"</b>&nbsp;" + order.ShippingContact.Address + @"<br />";


                            lblBilling.Text = @"<b>" + Resource.Admin_ViewOrder_Name + @"</b>&nbsp;" +
                                              order.BillingContact.Name;
                            lblBilling.Text += @"<br />";
                            lblBilling.Text += @"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>" + Resource.Admin_ViewOrder_Country +
                                               @"</b>&nbsp;" + order.BillingContact.Country + @"<br />";
                            lblBilling.Text += @"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>" + Resource.Admin_ViewOrder_City +
                                               @"</b>&nbsp;" + order.BillingContact.City + @"<br />";

                            if (order.BillingContact.Zone != null)
                            {
                                lblBilling.Text += @"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>" + Resource.Admin_ViewOrder_Zone +
                                                   @"</b>&nbsp;" + order.BillingContact.Zone.Trim() + @"<br />";
                            }

                            if (order.BillingContact.Zip != null)
                            {
                                lblBilling.Text += @"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>" + Resource.Admin_ViewOrder_Zip +
                                                   @"</b>&nbsp;" + order.BillingContact.Zip.Trim() + @"<br />";
                            }

                            lblBilling.Text += @"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>" + Resource.Admin_ViewOrder_Address +
                                               @"</b>&nbsp;" + order.BillingContact.Address + @"<br />";
                        }
                        else
                        {
                            trBilling.Visible = false;
                            trShipping.Visible = false;
                        }

                        lblShippingMethod.Text = @"<b>" + order.ArchivedShippingName + @"</b>";

                        if (order.OrderPickPoint != null)
                        {
                            lblShippingMethod.Text += @"</br><b>" + order.OrderPickPoint.PickPointAddress + @"</b>";
                        }

                        lblPaymentType.Text = @"<b>" + order.PaymentMethodName + @"</b>";

                        // ------------------------------------------------------------------------------------


                        float productPrice = order.OrderCertificates != null && order.OrderCertificates.Count > 0
                                                 ? order.OrderCertificates.Sum(item => item.Sum)
                                                 : order.OrderItems.Sum(item => item.Amount*item.Price);
                        float totalDiscount = order.OrderDiscount;

                        lblProductPrice.Text = CatalogService.GetStringPrice(productPrice, ordCurrency.CurrencyValue,
                                                                             ordCurrency.CurrencyCode);

                        trDiscount.Visible = order.OrderDiscount != 0;
                        lblOrderDiscount.Text = string.Format("-{0}",
                                                              CatalogService.GetStringDiscountPercent(productPrice,
                                                                                                      totalDiscount,
                                                                                                      ordCurrency
                                                                                                          .CurrencyValue,
                                                                                                      order
                                                                                                          .OrderCurrency
                                                                                                          .CurrencySymbol,
                                                                                                      order
                                                                                                          .OrderCurrency
                                                                                                          .IsCodeBefore,
                                                                                                      CurrencyService
                                                                                                          .CurrentCurrency
                                                                                                          .PriceFormat,
                                                                                                      false));

                        if (order.Certificate != null)
                        {
                            trCertificate.Visible = true;
                            lblCertificate.Text = string.Format("-{0}",
                                                                CatalogService.GetStringPrice(order.Certificate.Price,
                                                                                              ordCurrency.CurrencyValue,
                                                                                              ordCurrency.CurrencyCode));
                        }
                        else
                        {
                            trCertificate.Visible = false;
                        }

                        if (order.Coupon != null)
                        {
                            trCoupon.Visible = true;
                            switch (order.Coupon.Type)
                            {
                                case CouponType.Fixed:
                                    lblCoupon.Text = String.Format("-{0} ({1})",
                                                                   CatalogService.GetStringPrice(order.Coupon.Value, ordCurrency.CurrencyValue, ordCurrency.CurrencyCode),
                                                                   order.Coupon.Code);
                                    break;
                                case CouponType.Percent:
                                    lblCoupon.Text = String.Format("-{0} ({1}%) ({2})",
                                                                   CatalogService.GetStringPrice(productPrice * order.Coupon.Value / 100, ordCurrency.CurrencyValue, ordCurrency.CurrencyCode),
                                                                   CatalogService.FormatPriceInvariant(order.Coupon.Value), 
                                                                   order.Coupon.Code);
                                    break;
                            }
                        }
                        else
                        {
                            trCoupon.Visible = false;
                        }

                        lblShippingPrice.Text = string.Format("+{0}",
                                                              CatalogService.GetStringPrice(order.ShippingCost,
                                                                                            order.OrderCurrency
                                                                                                 .CurrencyValue,
                                                                                            order.OrderCurrency
                                                                                                 .CurrencyCode));
                        PaymentRow.Visible = order.PaymentCost != 0;
                        lblPaymentPrice.Text = string.Format("+{0}",
                                                             CatalogService.GetStringPrice(order.PaymentCost,
                                                                                           order.OrderCurrency
                                                                                                .CurrencyValue,
                                                                                           order.OrderCurrency
                                                                                                .CurrencyCode));


                        List<TaxValue> taxedItems = TaxServices.GetOrderTaxes(order.OrderID);
                        literalTaxCost.Text = TaxServices.BuildTaxTable(taxedItems, order.OrderCurrency.CurrencyValue,
                                                                        order.OrderCurrency.CurrencyCode,
                                                                        Resource.Admin_ViewOrder_Taxes);
                        lblTotalPrice.Text = CatalogService.GetStringPrice(order.Sum, order.OrderCurrency.CurrencyValue,
                                                                           order.OrderCurrency.CurrencyCode);

                        // ------------------------------------------------------------------------------------

                        if (string.IsNullOrEmpty(order.CustomerComment))
                        {
                            lblUserComments.Visible = false;
                        }
                        else
                        {
                            lblUserComments.Text = Resource.Client_PrintOrder_YourComment +
                                                   @" <br />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + order.CustomerComment;
                            lblUserComments.Visible = true;
                        }

                        if (order.OrderCertificates == null || order.OrderCertificates.Count == 0)
                        {
                            lvOrderItems.DataSource = order.OrderItems;
                            lvOrderItems.DataBind();

                            ShowOptions =
                                order.OrderItems.Any(
                                    item =>
                                    item.SelectedOptions.Any() || item.Color.IsNotEmpty() || item.Size.IsNotEmpty());

                            lvOrderItems.FindControl("tdOptions").Visible = ShowOptions;

                        }
                        else
                        {
                            lvOrderGiftCertificates.DataSource = order.OrderCertificates;
                            lvOrderGiftCertificates.DataBind();
                        }


                        mapAdress = order.ShippingContact.Country + "," + order.ShippingContact.Zone + "," +
                                    order.ShippingContact.City + "," + order.ShippingContact.Address;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }
}