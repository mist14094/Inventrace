//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Orders;
using AdvantShop.Repository;
using AdvantShop.Taxes;
using Resources;
using AdvantShop.Controls;

namespace Admin
{
    public partial class ViewOrder : AdvantShopAdminPage
    {
        protected int OrderID = 0;
        protected string OrderNumber;
        protected bool IsPaid;
        protected float CurrencyValue;
        protected string CurrencyCode;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            if (!int.TryParse(Request["orderid"], out OrderID))
            {
                OrderID = OrderService.GetLastOrderId();
            }

            SetMeta(string.Format("{0} - {1} {2}", SettingsMain.ShopName, Resource.Admin_ViewOrder_ItemNum, OrderID));
            LoadOrder();
        }

        private void LoadTotal(Order order)
        {
            if (order.OrderCurrency != null)
            {
                CurrencyValue = order.OrderCurrency.CurrencyValue;
                CurrencyCode = order.OrderCurrency.CurrencyCode;
            }

            lblShippingPrice.Text = string.Format("+{0}", CatalogService.GetStringPrice(order.ShippingCost, CurrencyValue, CurrencyCode));

            var shippingContact = new CustomerContact();
            if (order.ShippingContact != null)
            {
                var region = RegionService.GetRegionIdByName(order.ShippingContact.Zone);
                shippingContact = new CustomerContact
                    {
                        CountryId = CountryService.GetCountryIdByName(order.ShippingContact.Country),
                        RegionId = region != 0 ? (int?)region : null
                    };
            }

            var billingContact = new CustomerContact();
            if (order.ShippingContact != null)
            {
                var region = RegionService.GetRegionIdByName(order.BillingContact.Zone);
                billingContact = new CustomerContact
                    {
                        CountryId = CountryService.GetCountryIdByName(order.BillingContact.Country),
                        RegionId = region != 0 ? (int?)region : null
                    };
            }

            float taxCost = 0;
            var taxedItems = new List<TaxValue>();


            if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                foreach (var item in order.OrderCertificates)
                {
                    foreach (var taxId in GiftCertificateService.GetCertificateTaxesId())
                    {
                        var tax = TaxServices.GetTax(taxId);
                        TaxValue taxedItem = taxedItems.Find(tv => tv.TaxID == tax.TaxId);
                        if (taxedItem != null)
                        {
                            taxedItem.TaxSum += TaxServices.CalculateTax(item.Sum, tax);
                        }
                        else
                        {
                            taxedItems.Add(new TaxValue
                                {
                                    TaxID = tax.TaxId,
                                    TaxName = tax.Name,
                                    TaxSum = TaxServices.CalculateTax(item.Sum, tax),
                                    TaxShowInPrice = tax.ShowInPrice
                                });
                        }
                    }
                }
                taxCost = taxedItems.Sum(tv => tv.TaxSum);
            }
            else
            {
                foreach (OrderItem item in order.OrderItems)
                {
                    if (item.ProductID == null) continue;
                    var t = TaxServices.GetTaxesForProduct((int)item.ProductID, billingContact, shippingContact);
                    foreach (TaxElement tax in t)
                    {
                        TaxValue taxedItem = taxedItems.Find(tv => tv.TaxID == tax.TaxId);
                        if (taxedItem != null)
                        {
                            taxedItem.TaxSum += TaxServices.CalculateTax(item, tax, order.OrderDiscount);
                        }
                        else
                        {
                            taxedItems.Add(new TaxValue
                                {
                                    TaxID = tax.TaxId,
                                    TaxName = tax.Name,
                                    TaxShowInPrice = tax.ShowInPrice,
                                    TaxSum = TaxServices.CalculateTax(item, tax, order.OrderDiscount)
                                });
                        }
                    }
                }
                taxCost = order.OrderItems.Sum(oi => TaxServices.CalculateTaxesTotal(oi, shippingContact, billingContact, order.OrderDiscount));
            }

            lvTaxes.DataSource = taxedItems;
            lvTaxes.DataBind();

            float prodTotal = 0;
            if (order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                prodTotal = order.OrderCertificates.Sum(item => item.Sum);
            }
            else
            {
                prodTotal = order.OrderItems.Sum(oi => oi.Price * oi.Amount);
            }

            float totalDiscount = 0;

            totalDiscount += order.OrderDiscount > 0 ? Convert.ToSingle(Math.Round(prodTotal / 100 * order.OrderDiscount, 2)) : 0;

            lblTotalOrderPrice.Text = CatalogService.GetStringPrice(prodTotal, CurrencyValue, CurrencyCode);

            lblOrderDiscount.Text = string.Format("-{0}", CatalogService.GetStringPrice(totalDiscount, CurrencyValue, CurrencyCode));
            lblOrderDiscountPercent.Text = order.OrderDiscount + @"%";
            trDiscount.Visible = order.OrderDiscount != 0;

            liPaymentPrice.Visible = order.PaymentCost != 0;
            lblPaymentPrice.Text = CatalogService.GetStringPrice(order.PaymentCost, CurrencyValue, CurrencyCode);

            if (order.Certificate != null)
            {
                trCertificatePrice.Visible = order.Certificate.Price != 0;
                lblCertificatePrice.Text = string.Format("-{0}", CatalogService.GetStringPrice(order.Certificate.Price));
                totalDiscount += order.Certificate.Price;
            }

            if (order.Coupon != null)
            {
                trCoupon.Visible = order.Coupon.Value != 0;
                switch (order.Coupon.Type)
                {
                    case CouponType.Fixed:
                        var productsPrice =
                            order.OrderItems.Where(p => p.IsCouponApplied).Sum(p => p.Price * p.Amount);
                        totalDiscount += productsPrice >= order.Coupon.Value ? order.Coupon.Value : productsPrice;
                        lblCoupon.Text = String.Format("-{0} ({1})", CatalogService.GetStringPrice(totalDiscount, CurrencyValue, CurrencyCode),
                                                       order.Coupon.Code);
                        break;
                    case CouponType.Percent:
                        totalDiscount +=
                            order.OrderItems.Where(p => p.IsCouponApplied).Sum(
                                p => order.Coupon.Value * p.Price / 100 * p.Amount);
                        lblCoupon.Text = String.Format("-{0} ({1}%) ({2})", CatalogService.GetStringPrice(totalDiscount, CurrencyValue, CurrencyCode),
                                                       CatalogService.FormatPriceInvariant(order.Coupon.Value),
                                                       order.Coupon.Code);
                        break;
                }
            }

            float sum = taxCost + prodTotal + order.ShippingCost + order.PaymentCost - totalDiscount;
            lblTotalPrice.Text = CatalogService.GetStringPrice(sum < 0 ? 0 : sum, CurrencyValue, CurrencyCode);
        }

        private void LoadOrder()
        {
            Order order = OrderService.GetOrder(OrderID);
            if (order != null)
            {
                lnkExportToExcel.NavigateUrl = "../HttpHandlers/ExportOrderExcel.ashx?OrderID=" + order.OrderID;
                lnkEditOrder.NavigateUrl = "EditOrder.aspx?OrderID=" + order.OrderID;

                OrderNumber = order.Number;
                lblOrderId.Text = order.OrderID.ToString();
                lblOrderDate.Text = AdvantShop.Localization.Culture.ConvertDate(order.OrderDate);
                lblOrderNumber.Text = order.Number;

                IsPaid = order.PaymentDate != null && order.PaymentDate != DateTime.MinValue;

                if (order.OrderCustomer != null)
                {
                    if (CustomerService.GetCustomer(order.OrderCustomer.CustomerID).Id != Guid.Empty)
                    {
                        lnkCustomerName.Text = order.OrderCustomer.FirstName + @" " + order.OrderCustomer.LastName;
                        lnkCustomerName.NavigateUrl = @"viewcustomer.aspx?customerid=" + order.OrderCustomer.CustomerID;
                        lnkCustomerEmail.Text = order.OrderCustomer.Email;
                        lnkCustomerEmail.NavigateUrl = "mailto:" + order.OrderCustomer.Email;
                    }
                    else
                    {
                        lblCustomerEmail.Text = order.OrderCustomer.Email;
                        lblCustomerName.Text = order.OrderCustomer.FirstName + @" " + order.OrderCustomer.LastName;
                    }

                    lblCustomerPhone.Text = order.OrderCustomer.MobilePhone;
                }

                if (order.ShippingContact != null)
                {
                    lblShippingCountry.Text = order.ShippingContact.Country;
                    lblShippingCity.Text = order.ShippingContact.City;
                    lblShippingRegion.Text = order.ShippingContact.Zone;
                    lblShippingZipCode.Text = order.ShippingContact.Zip;
                    lblShippingAddress.Text = order.ShippingContact.Address;

                    if (!string.IsNullOrEmpty(order.ShippingContact.Country) && !string.IsNullOrEmpty(order.ShippingContact.City)
                        && !string.IsNullOrEmpty(order.ShippingContact.Zone) && !string.IsNullOrEmpty(order.ShippingContact.Address))
                    {
                        lnkShippingAddressOnMap.NavigateUrl = (SettingsOrderConfirmation.PrintOrder_MapType == "googlemap"
                                                                   ? "https://maps.google.com/maps?ie=UTF8&z=15&q="
                                                                   : "http://maps.yandex.ru/?text=") +
                                                              HttpUtility.UrlEncode(order.ShippingContact.Country + "," + order.ShippingContact.Zone + "," +
                                                                                    order.ShippingContact.City + "," + order.ShippingContact.Address);
                    }
                    else
                    {
                        lnkShippingAddressOnMap.Visible = false;
                    }
                }

                if (order.BillingContact != null)
                {
                    lblBuyerCountry.Text = order.BillingContact.Country;
                    lblBuyerRegion.Text = order.BillingContact.Zone;
                    lblBuyerCity.Text = order.BillingContact.City;
                    lblBuyerZip.Text = order.BillingContact.Zip;
                    lblBuyerAddress.Text = order.BillingContact.Address;

                    if (!string.IsNullOrEmpty(order.BillingContact.Country) && !string.IsNullOrEmpty(order.BillingContact.City)
                        && !string.IsNullOrEmpty(order.BillingContact.Zone) && !string.IsNullOrEmpty(order.BillingContact.Address))
                    {
                        lnkBuyerAddressOnMap.NavigateUrl = (SettingsOrderConfirmation.PrintOrder_MapType == "googlemap"
                                                                ? "https://maps.google.com/maps?ie=UTF8&z=15&q="
                                                                : "http://maps.yandex.ru/?text=") +
                                                           HttpUtility.UrlEncode(order.BillingContact.Country + "," + order.BillingContact.Zone + "," +
                                                                                 order.BillingContact.City + "," + order.BillingContact.Address);
                    }
                    else
                    {
                        lnkBuyerAddressOnMap.Visible = false;
                    }


                }

                lblShippingMethodName.Text = order.ArchivedShippingName + (order.OrderPickPoint != null ? "<br />" + order.OrderPickPoint.PickPointAddress : "");
                lblPaymentMethodName.Text = order.PaymentMethodName;

                var statusesList = OrderService.GetOrderStatuses();
                if (statusesList != null && statusesList.Any(status => status.StatusID == order.OrderStatus.StatusID))
                {
                    ddlViewOrderStatus.DataSource = statusesList;
                    ddlViewOrderStatus.DataBind();
                    ddlViewOrderStatus.SelectedValue = order.OrderStatus.StatusID.ToString();
                }
                else
                {
                    ddlViewOrderStatus.Items.Add(new ListItem(order.OrderStatus.StatusName, order.OrderStatus.StatusID.ToString()));
                    ddlViewOrderStatus.SelectedValue = order.OrderStatus.StatusID.ToString();
                }
                ddlViewOrderStatus.Attributes["data-orderid"] = order.OrderID.ToString();

                pnlOrderNumber.Attributes["style"] = "border-left-color: #" + order.OrderStatus.Color;

                if (order.OrderCertificates == null || order.OrderCertificates.Count == 0)
                {
                    lvOrderItems.DataSource = order.OrderItems;
                    lvOrderItems.DataBind();
                    lvOrderCertificates.Visible = false;
                }
                else
                {
                    lvOrderCertificates.DataSource = order.OrderCertificates;
                    lvOrderCertificates.DataBind();
                    lvOrderItems.Visible = false;
                }



                if (order.Certificate != null)
                {
                    //lCertificateCode.Text = order.Certificate.Code;
                    //pnlCertificateCode.Visible = !string.IsNullOrEmpty(order.Certificate.Code);
                }

                lblUserComment.Text = string.IsNullOrEmpty(order.CustomerComment)
                                          ? Resource.Admin_OrderSearch_NoComment
                                          : order.CustomerComment;

                txtAdminOrderComment.Text = string.Format("{0}", order.AdminOrderComment);
                txtStatusComment.Text = string.Format("{0}", order.StatusComment);

                txtStatusComment.Attributes["data-orderid"] = order.OrderID.ToString();
                txtAdminOrderComment.Attributes["data-orderid"] = order.OrderID.ToString();

                LoadTotal(order);
            }
            else
            {
                Response.Redirect("OrderSearch.aspx");
            }
        }

        protected string RenderSelectedOptions(IList<EvaluatedCustomOptions> evlist)
        {
            if (evlist.Count == 0)
            {
                return string.Empty;
            }

            var html = new StringBuilder();

            foreach (EvaluatedCustomOptions ev in evlist)
            {
                html.Append(string.Format("<div>{0}: {1}</div>", ev.CustomOptionTitle, ev.OptionTitle));
            }

            return html.ToString();
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            if (int.TryParse(Request["orderid"], out OrderID))
            {
                OrderService.DeleteOrder(OrderID);
                Response.Redirect("ordersearch.aspx");
            }
        }

        protected string RenderPicture(int productId, int photoId)
        {
            Photo photo;
            if (photoId != 0 && (photo = PhotoService.GetPhoto(photoId)) != null)
            {
                return String.Format("<img src='{0}'/>", FoldersHelper.GetImageProductPath(ProductImageType.XSmall, photo.PhotoName, true));
            }

            var p = ProductService.GetProduct(productId);
            if (p == null || string.IsNullOrEmpty(p.Photo))
            {
                return string.Format("<img src='{0}' alt=\"\"/>", AdvantShop.Core.UrlRewriter.UrlService.GetAbsoluteLink("images/nophoto_xsmall.jpg"));
            }

            return String.Format("<img src='{0}'/>", FoldersHelper.GetImageProductPath(ProductImageType.XSmall, p.Photo, true));
        }

        protected string RenderPaidButtons()
        {
            return
                string.Format(
                    "<label><input type=\"radio\" {0} name=\"g-checkout\" value=\"1\" onclick=\"setOrderPaid(1,{2})\"/>" + Resource.Admin_ViewOrder_Paid + "</label>" +
                    "<label><input type=\"radio\" {1} name=\"g-checkout\" value=\"0\" onclick=\"setOrderPaid(0,{2})\"/>" + Resource.Admin_ViewOrder_NotPaid + "</label>",
                    IsPaid ? "checked=\"checked\"" : string.Empty,
                    !IsPaid ? "checked=\"checked\"" : string.Empty,
                    OrderID);
        }
    }
}