//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.SEO;
using AdvantShop.Taxes;
using Resources;

namespace ClientPages
{
    public partial class GiftCertificate_Page : AdvantShopClientPage
    {
        protected int OrderId = 0;
        protected string OrderNumber = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!SettingsOrderConfirmation.EnableGiftCertificateService)
                Response.Redirect("~/");

            mvGiftCertificate.SetActiveView(vMainGiftCertificateView);

            SetMeta(
                new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Client_GiftCertificate_Header)),
                string.Empty);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                txtEmailFrom.Text = CustomerSession.CurrentCustomer.RegistredUser
                                        ? CustomerSession.CurrentCustomer.EMail
                                        : string.Empty;
                lvPaymentMethods.DataSource = PaymentService.GetCertificatePaymentMethods();
                lvPaymentMethods.DataBind();
            }

            btnPrintOrder.OnClientClick =
                string.Format("javascript:open_printable_version('PrintOrder.aspx?OrderNumber={0}'); return false;",
                              OrderNumber);

            lnkToDefault.Href = AdvantShop.Core.UrlRewriter.UrlService.GetAbsoluteLink("");
        }

        private bool IsValidData()
        {
            bool boolIsValidPast =
                txtFrom.Text.IsNotEmpty()
                && txtTo.Text.IsNotEmpty()
                && ValidationHelper.IsValidEmail(txtEmail.Text)
                && ValidationHelper.IsValidEmail(txtEmailFrom.Text);

            float sum;
            if (!Single.TryParse(txtSum.Text.Trim(), out sum))
            {
                sum = sum * CurrencyService.CurrentCurrency.Value;
                if (sum < SettingsOrderConfirmation.MinimalPriceCertificate || sum > SettingsOrderConfirmation.MaximalPriceCertificate)
                {
                    boolIsValidPast = false;
                    ShowMessage(Notify.NotifyType.Error, Resource.Client_GiftCertificate_WrongSum);
                }
            }

            int paymentMethodId;
            if (!Int32.TryParse(hfPaymentMethod.Value, out paymentMethodId))
            {
                boolIsValidPast = false;
                ShowMessage(Notify.NotifyType.Error, Resource.Client_GiftCertificate_WrongPaymentMethod);
            }

            if (!validShield.IsValid())
            {
                ShowMessage(Notify.NotifyType.Error, Resource.Client_GiftCertificate_WrongCode);
                boolIsValidPast = false;
            }
            if (!boolIsValidPast)
                validShield.TryNew();

            return boolIsValidPast;
        }

        protected bool CreateCertificateOrder()
        {
            var certificate = new GiftCertificate
                {
                    CertificateCode = GiftCertificateService.GenerateCertificateCode(),
                    ToName = txtTo.Text,
                    FromName = txtFrom.Text,
                    Sum = Convert.ToSingle(txtSum.Text.Trim()) * CurrencyService.CurrentCurrency.Value,
                    CertificateMessage = txtMessage.Text,
                    Enable = true,
                    ToEmail = txtEmail.Text
                };

            var orderContact = new OrderContact
                {
                    Address = string.Empty,
                    City = string.Empty,
                    Country = string.Empty,
                    Name = string.Empty,
                    Zip = string.Empty,
                    Zone = string.Empty
                };

            float taxTotalPrice = 0;
            var orderTaxes = new List<TaxValue>();
            foreach (var taxId in GiftCertificateService.GetCertificateTaxesId())
            {
                var tax = TaxServices.GetTax(taxId);
                orderTaxes.Add(new TaxValue
                    {
                        TaxID = tax.TaxId,
                        TaxName = tax.Name,
                        TaxShowInPrice = tax.ShowInPrice,
                        TaxSum = tax.FederalRate
                    });
                taxTotalPrice += TaxServices.CalculateTax(certificate.Sum, tax);
            }

            float orderSum = certificate.Sum + taxTotalPrice;

            var payment = PaymentService.GetPaymentMethod(hfPaymentMethod.Value.TryParseInt());
            float paymentPrice = payment.Extracharge == 0 ? 0 : (payment.ExtrachargeType == ExtrachargeType.Fixed ? payment.Extracharge : payment.Extracharge / 100 * certificate.Sum + taxTotalPrice);

            var order = new Order
                {
                    OrderDate = DateTime.Now,
                    OrderCustomer = new OrderCustomer
                        {
                            CustomerID = CustomerSession.CurrentCustomer.Id,
                            Email = txtEmailFrom.Text,
                            FirstName = CustomerSession.CurrentCustomer.FirstName,
                            LastName = CustomerSession.CurrentCustomer.LastName,
                            CustomerIP = HttpContext.Current.Request.UserHostAddress
                        },
                    OrderCurrency = new OrderCurrency
                        {
                            CurrencyCode = CurrencyService.CurrentCurrency.Iso3,
                            CurrencyNumCode = CurrencyService.CurrentCurrency.NumIso3,
                            CurrencyValue = CurrencyService.CurrentCurrency.Value,
                            CurrencySymbol = CurrencyService.CurrentCurrency.Symbol,
                            IsCodeBefore = CurrencyService.CurrentCurrency.IsCodeBefore
                        },
                    OrderStatusId = OrderService.DefaultOrderStatus,
                    AffiliateID = 0,
                    ArchivedShippingName = Resource.Client_GiftCertificate_DeliveryByEmail,
                    PaymentMethodId = Convert.ToInt32(hfPaymentMethod.Value),
                    ArchivedPaymentName = payment.Name,
                    PaymentDetails = null,
                    Sum = orderSum + paymentPrice,
                    PaymentCost = paymentPrice,
                    OrderCertificates = new List<GiftCertificate>
                        {
                            certificate
                        },
                    TaxCost = taxTotalPrice,
                    Taxes = orderTaxes,
                    ShippingContact = orderContact,
                    BillingContact = orderContact,
                    Number = OrderService.GenerateNumber(1)
                };

            if (order.PaymentMethod.Type == PaymentType.QIWI)
            {
                order.PaymentDetails = new PaymentDetails() {Phone = txtPhone.Text};
            }

            OrderId = order.OrderID = OrderService.AddOrder(order);
            OrderNumber = order.Number = OrderService.GenerateNumber(order.OrderID);
            OrderService.UpdateNumber(order.OrderID, order.Number);
            OrderService.ChangeOrderStatus(order.OrderID, OrderService.DefaultOrderStatus);

            string email = txtEmailFrom.Text;

            string htmlOrderTable = OrderService.GenerateHtmlOrderCertificateTable(order.OrderCertificates,
                                                                                   CurrencyService.CurrentCurrency,
                                                                                   order.PaymentCost, order.TaxCost);


            string htmlMessage = SendMail.BuildMail(new ClsMailParamOnNewOrder
            {
                CustomerContacts = string.Empty,
                PaymentType = order.ArchivedPaymentName,
                ShippingMethod = order.ArchivedShippingName,
                CurrentCurrencyCode = CurrencyService.CurrentCurrency.Iso3,
                TotalPrice = order.Sum.ToString(),
                Comments = order.CustomerComment,
                Email = email,
                OrderTable = htmlOrderTable,
                OrderID = order.OrderID.ToString(),
                Number = order.Number
            });

            SendMail.SendMailNow(email, Resource.Client_OrderConfirmation_ReceivedOrder + " " + order.OrderID, htmlMessage, true);
            SendMail.SendMailNow(SettingsMail.EmailForOrders, Resource.Client_OrderConfirmation_ReceivedOrder + " " + order.OrderID, htmlMessage, true);

            return OrderId != 0;
        }

        protected void btnBuyGiftCertificate_Click(object sender, EventArgs e)
        {
            if (IsValidData() && CreateCertificateOrder())
            {
                mvGiftCertificate.SetActiveView(vFinalGiftCertificateView);
            }
        }
    }
}