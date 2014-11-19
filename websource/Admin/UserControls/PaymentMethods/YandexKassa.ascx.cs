using System;
using System.Collections.Generic;
using System.IO;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Helpers;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class YandexKassaControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid ||
                       ValidateFormData(
                           new[] { txtShopID, txtScID, txtCurrencyValue }, //, txtCertificate
                           new[] {txtCurrencyValue}, new[] {txtShopID})
                           ? new Dictionary<string, string>
                               {
                                   {YandexKassaTemplate.ShopID, txtShopID.Text},
                                   {YandexKassaTemplate.ScID, txtScID.Text},
                                   {YandexKassaTemplate.CurrencyValue, txtCurrencyValue.Text},
                                   {YandexKassaTemplate.YaPaymentType, ddlPaymentType.SelectedValue ?? ddlPaymentType.Items[0].Value},
                                   {YandexKassaTemplate.FileCertificate, txtCertificate.Text},
                                   {YandexKassaTemplate.Password, txtPassword.Text},
                               }
                           : null;
            }
            set
            {
                txtShopID.Text = value.ElementOrDefault(YandexKassaTemplate.ShopID);
                txtScID.Text = value.ElementOrDefault(YandexKassaTemplate.ScID);
                txtCurrencyValue.Text = value.ElementOrDefault(YandexKassaTemplate.CurrencyValue);
                txtCertificate.Text = value.ElementOrDefault(YandexKassaTemplate.FileCertificate);
                txtPassword.Text = value.ElementOrDefault(YandexKassaTemplate.Password);
                if (ddlPaymentType.Items.FindByValue(value.ElementOrDefault(YandexKassaTemplate.YaPaymentType)) != null)
                {
                    ddlPaymentType.SelectedValue = value.ElementOrDefault(YandexKassaTemplate.YaPaymentType);
                }
            }
        }

        public override void SaveOtherData()
        {
            if (fuCertificate.HasFile)
            {
                FileHelpers.CreateDirectory(SettingsGeneral.AbsolutePath + @"\App_Data\YandexKassa\");

                string fileName = string.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(fuCertificate.FileName));

                fuCertificate.SaveAs(SettingsGeneral.AbsolutePath + @"\App_Data\YandexKassa\" + fileName);

                if (!string.IsNullOrEmpty(txtCertificate.Text))
                {
                    FileHelpers.DeleteFile(SettingsGeneral.AbsolutePath + @"\App_Data\YandexKassa\" + txtCertificate.Text);
                }

                txtCertificate.Text = fileName;
            }
        }
    }
}