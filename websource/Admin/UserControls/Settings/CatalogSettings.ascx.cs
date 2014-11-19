using System;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Helpers;
using Resources;

namespace Admin.UserControls.Settings
{
    public partial class CatalogSettings : System.Web.UI.UserControl
    {
        public string ErrMessage = Resources.Resource.Admin_CommonSettings_InvalidCatalog;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            ddlDefaultCurrency.DataSource = SqlDataSource2;
            ddlDefaultCurrency.DataTextField = "Name";
            ddlDefaultCurrency.DataValueField = "CurrencyIso3";
            ddlDefaultCurrency.DataBind();

            ddlDefaultCurrency.SelectedValue = SettingsCatalog.DefaultCurrencyIso3;
            txtProdPerPage.Text = SettingsCatalog.ProductsPerPage.ToString();
            cbEnableProductRating.Checked = SettingsCatalog.EnableProductRating;
            cbEnableCompareProducts.Checked = SettingsCatalog.EnableCompareProducts;
            cbEnableCatalogViewChange.Checked = SettingsCatalog.EnabledCatalogViewChange;
            cbEnableSearchViewChange.Checked = SettingsCatalog.EnabledSearchViewChange;
            ckbModerateReviews.Checked = SettingsCatalog.ModerateReviews;
            chkAllowReviews.Checked = SettingsCatalog.AllowReviews;

            cbExluderingFilters.Checked = SettingsCatalog.ExluderingFilters;
            cbShowStockAvailability.Checked = SettingsCatalog.ShowStockAvailability;

            cbComplexFilter.Checked = SettingsCatalog.ComplexFilter;
            txtSizesHeader.Text = SettingsCatalog.SizesHeader;
            txtColorsHeader.Text = SettingsCatalog.ColorsHeader;

            txtColorPictureWidthCatalog.Text = SettingsPictureSize.ColorIconWidthCatalog.ToString();
            txtColorPictureHeightCatalog.Text = SettingsPictureSize.ColorIconHeightCatalog.ToString();
            txtColorPictureWidthDetails.Text = SettingsPictureSize.ColorIconWidthDetails.ToString();
            txtColorPictureHeightDetails.Text = SettingsPictureSize.ColorIconHeightDetails.ToString();



            ddlCatalogView.SelectedValue = ((int)SettingsCatalog.DefaultCatalogView).ToString();
            ddlSearchView.SelectedValue = ((int)SettingsCatalog.DefaultSearchView).ToString();
            chkCompressBigImage.Checked = SettingsCatalog.CompressBigImage;

            txtBlockOne.Text = SettingsCatalog.RelatedProductName;
            txtBlockTwo.Text = SettingsCatalog.AlternativeProductName;

            txtBuyButtonText.Text = SettingsCatalog.BuyButtonText;
            txtMoreButtonText.Text = SettingsCatalog.MoreButtonText;
            txtPreOrderButtonText.Text = SettingsCatalog.PreOrderButtonText;

            cbDisplayBuyButton.Checked = SettingsCatalog.DisplayBuyButton;
            cbDisplayMoreButton.Checked = SettingsCatalog.DisplayMoreButton;
            cbDisplayPreOrderButton.Checked = SettingsCatalog.DisplayPreOrderButton;

            ckbShowCategoriesInBottomMenu.Checked = SettingsCatalog.DisplayCategoriesInBottomMenu;

            cbShowProductsCount.Checked = SettingsCatalog.ShowProductsCount;

            chkEnableZoom.Checked = SettingsDesign.EnableZoom;
        }


        public bool SaveData()
        {
            if (!ValidateData())
                return false;

            SettingsCatalog.DefaultCurrencyIso3 = ddlDefaultCurrency.SelectedValue;
            SettingsCatalog.ProductsPerPage = SQLDataHelper.GetInt(txtProdPerPage.Text);
            SettingsCatalog.EnableProductRating = cbEnableProductRating.Checked;
            SettingsCatalog.EnableCompareProducts = cbEnableCompareProducts.Checked;
            SettingsCatalog.EnabledCatalogViewChange = cbEnableCatalogViewChange.Checked;
            SettingsCatalog.EnabledSearchViewChange = cbEnableSearchViewChange.Checked;
            SettingsCatalog.DefaultCatalogView = (SettingsCatalog.ProductViewMode)SQLDataHelper.GetInt(ddlCatalogView.SelectedValue);
            SettingsCatalog.DefaultSearchView = (SettingsCatalog.ProductViewMode)SQLDataHelper.GetInt(ddlSearchView.SelectedValue);
            SettingsCatalog.DisplayCategoriesInBottomMenu = ckbShowCategoriesInBottomMenu.Checked;
            SettingsCatalog.CompressBigImage = chkCompressBigImage.Checked;
            SettingsCatalog.ModerateReviews = ckbModerateReviews.Checked;
            SettingsCatalog.AllowReviews = chkAllowReviews.Checked;

            SettingsCatalog.ExluderingFilters = cbExluderingFilters.Checked;
            SettingsCatalog.ShowStockAvailability = cbShowStockAvailability.Checked;

            SettingsCatalog.ComplexFilter = cbComplexFilter.Checked;
            SettingsCatalog.SizesHeader = txtSizesHeader.Text;
            SettingsCatalog.ColorsHeader = txtColorsHeader.Text;

            SettingsPictureSize.ColorIconWidthCatalog = txtColorPictureWidthCatalog.Text.TryParseInt();
            SettingsPictureSize.ColorIconHeightCatalog = txtColorPictureHeightCatalog.Text.TryParseInt();
            SettingsPictureSize.ColorIconWidthDetails = txtColorPictureWidthDetails.Text.TryParseInt();
            SettingsPictureSize.ColorIconHeightDetails = txtColorPictureHeightDetails.Text.TryParseInt();

            SettingsCatalog.RelatedProductName = txtBlockOne.Text;
            SettingsCatalog.AlternativeProductName = txtBlockTwo.Text;

            SettingsCatalog.BuyButtonText = txtBuyButtonText.Text;
            SettingsCatalog.MoreButtonText = txtMoreButtonText.Text;
            SettingsCatalog.PreOrderButtonText = txtPreOrderButtonText.Text;

            SettingsCatalog.DisplayBuyButton = cbDisplayBuyButton.Checked;
            SettingsCatalog.DisplayMoreButton = cbDisplayMoreButton.Checked;
            SettingsCatalog.DisplayPreOrderButton = cbDisplayPreOrderButton.Checked;

            SettingsCatalog.ShowProductsCount = cbShowProductsCount.Checked;

            SettingsDesign.EnableZoom = chkEnableZoom.Checked;

            LoadData();
            return true;
        }

        private bool ValidateData()
        {
            if (string.IsNullOrEmpty(ddlDefaultCurrency.SelectedValue))
            {
                ErrMessage = "";
                return false;
            }

            if (string.IsNullOrEmpty(txtProdPerPage.Text))
            {
                ErrMessage = "";
                return false;
            }

            int ti;
            if (!int.TryParse(txtProdPerPage.Text, out ti))
            {
                ErrMessage = Resource.Admin_CommonSettings_NoNumberPerPage;
                return false;
            }
            return true;
        }

        protected void SqlDataSource2_Init(object sender, EventArgs e)
        {
            SqlDataSource2.ConnectionString = Connection.GetConnectionString();
        }

        protected void btnDoindex_Click(object sender, EventArgs e)
        {
            AdvantShop.FullSearch.LuceneSearch.CreateAllIndexInBackground();
            lbDone.Visible = true;
        }

    }
}