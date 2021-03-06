using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Taxes;
using Resources;

public partial class UserControls_FourthStep : System.Web.UI.UserControl
{
    public OrderConfirmationData PageData { get; set; }

    public class FourthStepNextEventArgs
    {
        public PaymentDetails PaymentDetails { get; set; }
        public string CustomerComment { get; set; }
    }

    public event Action<object, FourthStepNextEventArgs> NextStep;
    public void OnNextStep(FourthStepNextEventArgs arg)
    {
        if (NextStep != null) NextStep(this, arg);
    }

    public event Action<object, EventArgs> BackStep;
    public void OnBackStep(EventArgs arg)
    {
        if (BackStep != null) BackStep(this, arg);
    }

    public string Message { get; private set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (PageData != null)
            LoadData();

    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
    }

    protected void btnBackToPaymentType_Click(object sender, EventArgs e)
    {
        OnBackStep(EventArgs.Empty);
    }

    protected void btnRegUserConfirm_Click(object sender, EventArgs e)
    {
        if (PageData == null)
            Response.Redirect("orderconfirmation.aspx");

        if (UserOrderConfirmValidation())
        {
            PaymentDetails paymentDetails = null;

            if (PageData.SelectedPaymentType == PaymentType.Bill)
            {
                paymentDetails = new PaymentDetails
                                     {
                                         CompanyName = txtCompanyName.Text,
                                         INN = txtINN.Text
                                     };
            }
            else if (PageData.SelectedPaymentType == PaymentType.SberBank)
            {
                paymentDetails = new PaymentDetails
                                    {
                                        CompanyName = String.Empty,
                                        INN = txtINN2.Text,
                                    };
            }
            else if (PageData.SelectedPaymentType == PaymentType.QIWI)
            {
                paymentDetails = new PaymentDetails
                                    {
                                        Phone = txtPhone.Text
                                    };
            }


            OnNextStep(new FourthStepNextEventArgs { PaymentDetails = paymentDetails, CustomerComment = txtComments.Text });
        }
    }
    public string RenderSelectedOptions(string xml)
    {
        if (string.IsNullOrEmpty(xml))
        {
            return string.Empty;
        }

        var res = new StringBuilder("<div class=\"customoptions\" >");

        IList<EvaluatedCustomOptions> evlist = CustomOptionsService.DeserializeFromXml(xml);

        foreach (EvaluatedCustomOptions evco in evlist)
        {
            res.Append(evco.CustomOptionTitle);
            res.Append(": ");
            res.Append(evco.OptionTitle);
            res.Append(" ");
            if (evco.OptionPriceBc > 0)
            {
                res.Append("+");
                res.Append(CatalogService.GetStringPrice(evco.OptionPriceBc));
            }
            res.Append("<br />");
        }

        res.Append("</div>");

        return res.ToString();
    }

    private void LoadData()
    {
        //BillingContact info
        if (!PageData.BillingIsShipping)
        {
            if (PageData.BillingContact == null)
            {
                Response.Redirect("orderconfirmation.aspx");
                return;
            }
            lblBillingCountry.Text = PageData.BillingContact.Country;
            lblBillingCity.Text = PageData.BillingContact.City;
            lblBillingRegion.Text = PageData.BillingContact.RegionName;
            lblBillingAddress.Text = PageData.BillingContact.Address;
            lblBillingZip.Text = PageData.BillingContact.Zip;
        }
        else
        {
            if (PageData.ShippingContact == null)
            {
                Response.Redirect("orderconfirmation.aspx");
                return;
            }
            lblBillingCountry.Text = PageData.ShippingContact.Country;
            lblBillingCity.Text = PageData.ShippingContact.City;
            lblBillingRegion.Text = PageData.ShippingContact.RegionName;
            lblBillingAddress.Text = PageData.ShippingContact.Address;
            lblBillingZip.Text = PageData.ShippingContact.Zip;
        }

        if (PageData.ShippingContact == null)
        {
            Response.Redirect("orderconfirmation.aspx");
            return;
        }
        //ShippingContact info
        lblShippingCountry.Text = PageData.ShippingContact.Country;
        lblShippingRegion.Text = PageData.ShippingContact.RegionName;
        lblShippingCity.Text = PageData.ShippingContact.City;
        lblShippingAddress.Text = PageData.ShippingContact.Address;
        lblShippingZip.Text = PageData.ShippingContact.Zip;

        lblPaymentType.Text = PageData.SelectPaymentName;
        lblShippingMethod.Text = PageData.SelectShippingName + (PageData.ShippingOptionEx != null &&
                                 PageData.ShippingOptionEx.PickpointAddress.IsNotEmpty()
                                     ? "<br />" + PageData.ShippingOptionEx.PickpointAddress
                                     : string.Empty);


        pnlInfoForSberBank.Visible = PageData.SelectedPaymentType == PaymentType.SberBank;
        pnlInfoForBill2CompanyName.Visible =
            pnlInfoForBill2Inn.Visible = PageData.SelectedPaymentType == PaymentType.Bill;

        pnlPhoneForQiwi.Visible = PageData.SelectedPaymentType == PaymentType.QIWI;
        if (!Page.IsPostBack)
        {
            txtPhone.Text = PageData.Customer.Phone;
        }

        trCaptcha.Visible = SettingsMain.EnableCaptcha;
        UpdateBasket();
    }

    private void UpdateBasket()
    {
        try
        {
            var shpCart = ShoppingCartService.CurrentShoppingCart;

            if (shpCart.Count > 0)
            {
                lvOrderList.DataSource = shpCart;
                lvOrderList.DataBind();
                lvOrderList.Visible = true;
                pnlSummary.Visible = true;
            }
            else
            {
                lvOrderList.Visible = false;
                pnlSummary.Visible = false;
            }

            float totalPrice = shpCart.TotalPrice;
            float discountOnTotalPrice = shpCart.DiscountPercentOnTotalPrice;
            float totalDiscount = shpCart.TotalDiscount;
            float shippingPrice = PageData.SelectShippingRate;


            lblTotalOrderPrice.Text = CatalogService.GetStringPrice(shpCart.TotalPrice);

            if (discountOnTotalPrice > 0)
            {
                discountRow.Visible = true;
                lblOrderDiscount.Text = string.Format(
                    "<span class=\"per\">-{0}%</span><span class=\"price\">{1}</span>", discountOnTotalPrice,
                    CatalogService.GetStringPrice(totalPrice * discountOnTotalPrice / 100));
            }
            else
            {
                discountRow.Visible = false;
            }

            if (shpCart.Certificate != null)
            {
                certificateRow.Visible = true;
                lblCertificatePrice.Text = String.Format("-{0}",
                                                         CatalogService.GetStringPrice(shpCart.Certificate.Sum, 1,
                                                                                       shpCart.Certificate.CertificateOrder.OrderCurrency.CurrencyCode,
                                                                                       shpCart.Certificate.CertificateOrder.OrderCurrency.CurrencyValue));
            }

            if (shpCart.Coupon != null)
            {
                couponRow.Visible = true;
                if (shpCart.TotalPrice < shpCart.Coupon.MinimalOrderPrice)
                {
                    lblCouponPrice.Text = String.Format(Resource.Client_OrderConfirmation_CouponMessage,
                                                CatalogService.GetStringPrice(shpCart.Coupon.MinimalOrderPrice));
                }
                else
                {
                    if (totalDiscount == 0)
                    {
                        lblCouponPrice.Text = String.Format("-{0} ({1})", CatalogService.GetStringPrice(0), shpCart.Coupon.Code);
                    }
                    else
                    {
                        switch (shpCart.Coupon.Type)
                        {
                            case CouponType.Fixed:
                                lblCouponPrice.Text = String.Format("-{0} ({1})", CatalogService.GetStringPrice(totalDiscount), shpCart.Coupon.Code);
                                break;
                            case CouponType.Percent:
                                lblCouponPrice.Text = String.Format("-{0} ({1}%) ({2})", CatalogService.GetStringPrice(totalDiscount),
                                                                    CatalogService.FormatPriceInvariant(shpCart.Coupon.Value), shpCart.Coupon.Code);
                                break;
                        }
                    }
                }
            }

            if (PageData.SelectedPaymentType == PaymentType.CashOnDelivery && PageData.ShippingOptionEx != null)
                shippingPrice = PageData.ShippingOptionEx.PriceCash;

            var billingContact = PageData.BillingContact;
            var shippingContact = PageData.ShippingContact;
            float taxesTotal = shpCart.Sum(item => TaxServices.CalculateTaxesTotal(item, shippingContact, billingContact, shpCart.DiscountPercentOnTotalPrice));
            var taxesItems = GetTaxItems(shpCart, shippingContact, billingContact, shpCart.DiscountPercentOnTotalPrice);

            var paymentExtraCharge = PageData.SelectedPaymentExtracharge;
            paymentExtraChargeRow.Visible = paymentExtraCharge != 0;
            if (PageData.SelectedPaymentExtrachargeType == ExtrachargeType.Percent)
            {
                paymentExtraCharge = paymentExtraCharge * (totalPrice + shippingPrice - totalDiscount) / 100;
            }

            lPaymentCost.Text = paymentExtraCharge > 0 ? Resource.Client_OrderConfirmation_PaymentCost : Resource.Client_OrderConfirmation_PaymentDiscount;
            
            float dblTotalPrice = (totalPrice + shippingPrice + taxesTotal - totalDiscount + paymentExtraCharge);

            lblTotalOrderPrice.Text = CatalogService.GetStringPrice(totalPrice);
            lblShippingPrice.Text = "+" + CatalogService.GetStringPrice(shippingPrice);
            lblPaymentExtraCharge.Text = (paymentExtraCharge > 0 ? "+": "") + CatalogService.GetStringPrice(paymentExtraCharge);

            if (taxesItems.Count > 0)
            {
                literalTaxCost.Text = BuildTaxTable(taxesItems, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Iso3, Resource.Admin_ViewOrder_Taxes);
            }
            lblTotalPrice.Text = CatalogService.GetStringPrice(dblTotalPrice > 0 ? dblTotalPrice : 0);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Message = ex.Message + " at UpdateBasket";
        }
    }

    private static Dictionary<TaxElement, float> GetTaxItems(ShoppingCart shoppingCart, CustomerContact shippingContact, CustomerContact billingContact, float discountPercent)
    {
        var taxesItems = new Dictionary<TaxElement, float>();

        foreach (var item in shoppingCart)
        {
            var t = (List<TaxElement>)TaxServices.GetTaxesForProduct(item.Offer.ProductId, billingContact, shippingContact);
            foreach (var tax in t)
            {
                if (taxesItems.ContainsKey(tax))
                {
                    taxesItems[tax] += TaxServices.CalculateTax((OrderItem)item, tax, discountPercent);
                }
                else
                {
                    taxesItems.Add(tax, TaxServices.CalculateTax((OrderItem)item, tax, discountPercent));
                }
            }
        }
        return taxesItems;
    }

    private static string BuildTaxTable(Dictionary<TaxElement, float> taxes, float currentCurrencyRate, string currentCurrencyIso3, string message)
    {
        return BuildTaxTable(
            taxes.Select(tax => new TaxValue { TaxName = tax.Key.Name, TaxID = tax.Key.TaxId, TaxSum = tax.Value, TaxShowInPrice = tax.Key.ShowInPrice }).
                ToList(), currentCurrencyRate, currentCurrencyIso3, message);
    }

    private static string BuildTaxTable(IEnumerable<TaxValue> taxes, float currentCurrencyRate, string currentCurrencyIso3, string message)
    {
        var sb = new StringBuilder();
        foreach (TaxValue tax in taxes)
        {
            sb.AppendFormat("<div><span class=\"fullcart-summary-text\">{0}:</span> ",
                (tax.TaxShowInPrice ? Resource.Core_TaxServices_Include_Tax : "") + " " + tax.TaxName);
            sb.AppendFormat("<span class=\"price\">{0}{1}</span></div>",
                (tax.TaxShowInPrice ? "" : "+"),
                CatalogService.GetStringPrice(tax.TaxSum, currentCurrencyRate, currentCurrencyIso3));
        }
        if (sb.Length == 0)
        {
            sb.AppendFormat("<div><span class=\"basket-txt\">{0}</span>",
                message);
            sb.AppendFormat("<span class=\"price\">{0}</span></div>",
                CatalogService.GetStringPrice(0, currentCurrencyRate, currentCurrencyIso3));
        }
        return sb.ToString();
    }

    private bool UserOrderConfirmValidation()
    {
        bool boolValidationResult = true;

        // Validation ----------------------------------------------------

        if (SettingsMain.EnableCaptcha && !dnfValid.IsValid())
        {
            boolValidationResult = false;
            ((AdvantShopClientPage)this.Page).ShowMessage(Notify.NotifyType.Error, Resource.Client_Registration_CodeDiffrent);
        }

        // Validation shoping cart

        var shpCrt = ShoppingCartService.CurrentShoppingCart;

        foreach (var item in shpCrt)
        {
            if (item.Amount > item.Offer.Amount && SettingsOrderConfirmation.AmountLimitation && !item.Offer.Product.AllowPreOrder)
            {
                boolValidationResult = false;
                ((AdvantShopClientPage)this.Page).ShowMessage(Notify.NotifyType.Error, Resource.Client_Items_Count_Error);
            }
        }

        return boolValidationResult;
    }


    protected string RenderPhoto(string photoName, string name)
    {
        return string.Format("<img src=\"{0}\" alt=\"{1}\" />",
                             photoName.IsNotEmpty()
                                 ? FoldersHelper.GetImageProductPath(ProductImageType.Small, photoName, false)
                                 : "images/nophoto_xsmall.jpg",
                             name);
    }

}