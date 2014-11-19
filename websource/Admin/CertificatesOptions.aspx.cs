//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Taxes;

namespace Admin
{
    public partial class CertificatesOptions : AdvantShopAdminPage
    {
        protected List<int> GiftCertificatePaymentMethods;
        protected List<int> GiftCertificateTaxes; 

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1} - {2}", SettingsMain.ShopName, lblHead.Text, lblSubHead.Text));
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            GiftCertificatePaymentMethods = new List<int>(GiftCertificateService.GetCertificatePaymentMethodsID());
            lvPaymentMethods.DataSource = PaymentService.GetAllPaymentMethods(false);
            lvPaymentMethods.DataBind();

            GiftCertificateTaxes = GiftCertificateService.GetCertificateTaxesId();
            lvTaxes.DataSource = TaxServices.GetTaxes();
            lvTaxes.DataBind();
        }

        protected void btnSaveClick(object sender, EventArgs e)
        {
            var PaymentMethodsList = new List<int>();
            foreach (ListViewItem item in lvPaymentMethods.Items)
            {
                int id;
                if (((CheckBox)item.FindControl("ckbActive")).Checked && Int32.TryParse(((HiddenField)item.FindControl("hfPaymentId")).Value, out id))
                {
                    PaymentMethodsList.Add(id);
                }
            }

            GiftCertificateService.SaveCertificatePaymentMethods(PaymentMethodsList);

            var TaxesList = new List<int>();
            foreach (ListViewItem item in lvTaxes.Items)
            {
                int id;
                if (((CheckBox)item.FindControl("ckbActive")).Checked && Int32.TryParse(((HiddenField)item.FindControl("hfTaxId")).Value, out id))
                {
                    TaxesList.Add(id);
                }
            }
            GiftCertificateService.SaveCertificateTaxes(TaxesList);
        }
    }
}