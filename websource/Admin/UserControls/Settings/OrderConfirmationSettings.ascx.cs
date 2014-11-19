using System;
using AdvantShop.Configuration;
using AdvantShop.Payment;

namespace Admin.UserControls.Settings
{
    public partial class OrderConfirmationSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resources.Resource.Admin_CommonSettings_InvalidOrderConfirmation;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            txtQRUserID.Text = SettingsOrderConfirmation.QrUserId;
            cbAmountLimitation.Checked = SettingsOrderConfirmation.AmountLimitation;
            txtMinimalPrice.Text = SettingsOrderConfirmation.MinimalOrderPrice.ToString("#0.00") ?? "0.00";
            txtMaximalPricecertificate.Text = SettingsOrderConfirmation.MaximalPriceCertificate.ToString("#0.00") ?? "0.00";
            txtMinimalPriceCertificate.Text = SettingsOrderConfirmation.MinimalPriceCertificate.ToString("#0.00") ?? "0.00";
            ckbEnableGiftCertificateService.Checked = SettingsOrderConfirmation.EnableGiftCertificateService;

            ckbBuyInOneClick.Checked = SettingsOrderConfirmation.BuyInOneClick;
            txtFirstText.Text = SettingsOrderConfirmation.BuyInOneClick_FirstText;
            txtSecondText.Text = SettingsOrderConfirmation.BuyInOneClick_FinalText;

            chkShowStatusInfo.Checked = SettingsOrderConfirmation.PrintOrder_ShowStatusInfo;
            chkShowMap.Checked = SettingsOrderConfirmation.PrintOrder_ShowMap;

            rbGoogleMap.Checked = SettingsOrderConfirmation.PrintOrder_MapType == "googlemap";
            rbYandexMap.Checked = SettingsOrderConfirmation.PrintOrder_MapType == "yandexmap";
        }

        public bool SaveData()
        {
            bool isCorrect = true;

            SettingsOrderConfirmation.BuyInOneClick = ckbBuyInOneClick.Checked;
            SettingsOrderConfirmation.BuyInOneClick_FirstText = txtFirstText.Text;
            SettingsOrderConfirmation.BuyInOneClick_FinalText = txtSecondText.Text;

            SettingsOrderConfirmation.QrUserId = txtQRUserID.Text;
            SettingsOrderConfirmation.AmountLimitation = cbAmountLimitation.Checked;

            SettingsOrderConfirmation.PrintOrder_ShowStatusInfo = chkShowStatusInfo.Checked;
            SettingsOrderConfirmation.PrintOrder_ShowMap = chkShowMap.Checked;
            SettingsOrderConfirmation.PrintOrder_MapType = rbGoogleMap.Checked ? "googlemap" : "yandexmap";


            if (SettingsOrderConfirmation.EnableGiftCertificateService != ckbEnableGiftCertificateService.Checked)
            {
                var method = PaymentService.GetPaymentMethodByType(PaymentType.GiftCertificate);
                if (method == null && ckbEnableGiftCertificateService.Checked)
                {
                    PaymentService.AddPaymentMethod(new PaymentGiftCertificate
                        {
                            Enabled = true,
                            Name = Resources.Resource.Client_GiftCertificate,
                            Description = Resources.Resource.Payment_GiftCertificateDescription,
                            SortOrder = 0
                        });
                }
                else if (method != null && !ckbEnableGiftCertificateService.Checked)
                {
                    PaymentService.DeletePaymentMethod(method.PaymentMethodID);
                    SettingsDesign.GiftSertificateVisibility = false;
                }
            }
            SettingsOrderConfirmation.EnableGiftCertificateService = ckbEnableGiftCertificateService.Checked;

        

            float price = 0;
            if (float.TryParse(txtMaximalPricecertificate.Text, out price) && price >= 0)
            {
                SettingsOrderConfirmation.MaximalPriceCertificate = price;
            }
            else
            {
                ErrMessage += Resources.Resource.Admin_CommonSettings_CertificateMaxPriceError;
                isCorrect = false;
            }

            if (float.TryParse(txtMinimalPriceCertificate.Text, out price) && price >= 0)
            {
                SettingsOrderConfirmation.MinimalPriceCertificate = price;
            }
            else
            {
                ErrMessage += Resources.Resource.Admin_CommonSettings_CertificateMinPriceError;
                isCorrect = false;
            }


            if (float.TryParse(txtMinimalPrice.Text, out price) && price >= 0)
            {
                SettingsOrderConfirmation.MinimalOrderPrice = price;
            }
            else
            {
                ErrMessage += Resources.Resource.Admin_CommonSettings_OrderMinPriceError;
                isCorrect = false;
            }

            LoadData();

            return isCorrect;
        }
    }
}