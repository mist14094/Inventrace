using System;
using AdvantShop.Configuration;
using AdvantShop;

namespace Admin.UserControls.Settings
{
    public partial class SEOSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resources.Resource.Admin_CommonSettings_InvalidSEO;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            txtProductsHeadTitle.Text = SettingsSEO.ProductMetaTitle;
            txtProductsMetaKeywords.Text = SettingsSEO.ProductMetaKeywords;
            txtProductsMetaDescription.Text = SettingsSEO.ProductMetaDescription;
            txtProductsH1.Text = SettingsSEO.ProductMetaH1;
            txtProductsAdditionalDescription.Text = SettingsSEO.ProductAdditionalDescription;

            txtCategoriesHeadTitle.Text = SettingsSEO.CategoryMetaTitle;
            txtCategoriesMetaKeywords.Text = SettingsSEO.CategoryMetaKeywords;
            txtCategoriesMetaDescription.Text = SettingsSEO.CategoryMetaDescription;
            txtCategoriesMetaH1.Text = SettingsSEO.CategoryMetaH1;

            txtNewsHeadTitle.Text = SettingsSEO.NewsMetaTitle;
            txtNewsMetaKeywords.Text = SettingsSEO.NewsMetaKeywords;
            txtNewsMetaDescription.Text = SettingsSEO.NewsMetaDescription;
            txtNewsH1.Text = SettingsSEO.NewsMetaH1;

            txtStaticPageHeadTitle.Text = SettingsSEO.StaticPageMetaTitle;
            txtStaticPageMetaKeywords.Text = SettingsSEO.StaticPageMetaKeywords;
            txtStaticPageMetaDescription.Text = SettingsSEO.StaticPageMetaDescription;
            txtStaticPageH1.Text = SettingsSEO.StaticPageMetaH1;

            txtTitle.Text = SettingsSEO.DefaultMetaTitle;
            txtMetaKeys.Text = SettingsSEO.DefaultMetaKeywords;
            txtMetaDescription.Text = SettingsSEO.DefaultMetaDescription;

            txtGoogleAnalytics.Text = SettingsSEO.GoogleAnalyticsNumber;
            
            chbGoogleAnalytics.Checked = SettingsSEO.GoogleAnalyticsEnabled;

            chbGoogleAnalyticsApi.Checked = SettingsSEO.GoogleAnalyticsApiEnabled;
            txtGoogleAnalyticsAccountID.Text = SettingsSEO.GoogleAnalyticsAccountID;
            txtGoogleAnalyticsUserName.Text = SettingsSEO.GoogleAnalyticsUserName;
            txtGoogleAnalyticsPassword.Text = SettingsSEO.GoogleAnalyticsPassword;
            txtGoogleAnalyticsAPIKey.Text = SettingsSEO.GoogleAnalyticsAPIKey;

            txtCustomMetaString.Text = SettingsSEO.CustomMetaString;

        }
        public bool SaveData()
        {
			if (SettingsSEO.GoogleAnalyticsNumber != txtGoogleAnalytics.Text && chbGoogleAnalytics.Checked)
            {
                Trial.TrackEvent(Trial.TrialEvents.SetUpGoogleAnalytics, string.Empty);
            }
            SettingsSEO.ProductMetaTitle = txtProductsHeadTitle.Text;
            SettingsSEO.ProductMetaKeywords = txtProductsMetaKeywords.Text;
            SettingsSEO.ProductMetaDescription = txtProductsMetaDescription.Text;
            SettingsSEO.ProductMetaH1 = txtProductsH1.Text;
            SettingsSEO.ProductAdditionalDescription = txtProductsAdditionalDescription.Text;

            SettingsSEO.CategoryMetaTitle = txtCategoriesHeadTitle.Text;
            SettingsSEO.CategoryMetaKeywords = txtCategoriesMetaKeywords.Text;
            SettingsSEO.CategoryMetaDescription = txtCategoriesMetaDescription.Text;
            SettingsSEO.CategoryMetaH1 = txtCategoriesMetaH1.Text;

            SettingsSEO.NewsMetaTitle = txtNewsHeadTitle.Text;
            SettingsSEO.NewsMetaKeywords = txtNewsMetaKeywords.Text;
            SettingsSEO.NewsMetaDescription = txtNewsMetaDescription.Text;
            SettingsSEO.NewsMetaTitle = txtNewsHeadTitle.Text;
            SettingsSEO.NewsMetaH1 = txtNewsH1.Text;

            SettingsSEO.StaticPageMetaTitle = txtStaticPageHeadTitle.Text;
            SettingsSEO.StaticPageMetaKeywords = txtStaticPageMetaKeywords.Text;
            SettingsSEO.StaticPageMetaDescription = txtStaticPageMetaDescription.Text;
            SettingsSEO.StaticPageMetaH1 = txtStaticPageH1.Text;

            SettingsSEO.GoogleAnalyticsNumber = txtGoogleAnalytics.Text;            
            SettingsSEO.GoogleAnalyticsEnabled = chbGoogleAnalytics.Checked;

            SettingsSEO.DefaultMetaTitle = txtTitle.Text;
            SettingsSEO.DefaultMetaKeywords = txtMetaKeys.Text;
            SettingsSEO.DefaultMetaDescription = txtMetaDescription.Text;

            SettingsSEO.GoogleAnalyticsNumber = txtGoogleAnalytics.Text;
            SettingsSEO.GoogleAnalyticsEnabled = chbGoogleAnalytics.Checked;
            SettingsSEO.GoogleAnalyticsApiEnabled = chbGoogleAnalyticsApi.Checked;

            SettingsSEO.GoogleAnalyticsAccountID = txtGoogleAnalyticsAccountID.Text;
            SettingsSEO.GoogleAnalyticsUserName = txtGoogleAnalyticsUserName.Text;
            SettingsSEO.GoogleAnalyticsPassword = txtGoogleAnalyticsPassword.Text;
            SettingsSEO.GoogleAnalyticsAPIKey = txtGoogleAnalyticsAPIKey.Text;

            SettingsSEO.CustomMetaString = txtCustomMetaString.Text;
            LoadData();

            return true;
        }
    }
}