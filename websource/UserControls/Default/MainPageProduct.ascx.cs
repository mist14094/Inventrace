//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Design;

public partial class UserControls_MainPageProduct : System.Web.UI.UserControl
{
    public SettingsDesign.eMainPageMode Mode { set; get; }
    protected bool EnableRating = SettingsCatalog.EnableProductRating;
    protected CustomerGroup customerGroup = CustomerSession.CurrentCustomer.CustomerGroup;

    protected int ImageMaxHeight = SettingsPictureSize.SmallProductImageHeight;
    protected int ImageMaxWidth = SettingsPictureSize.SmallProductImageWidth;

    protected bool DisplayBuyButton = SettingsCatalog.DisplayBuyButton;
    protected bool DisplayMoreButton = SettingsCatalog.DisplayMoreButton;
    protected bool DisplayPreOrderButton = SettingsCatalog.DisplayPreOrderButton;

    protected string BuyButtonText = SettingsCatalog.BuyButtonText;
    protected string MoreButtonText = SettingsCatalog.MoreButtonText;
    protected string PreOrderButtonText = SettingsCatalog.PreOrderButtonText;

    protected int ItemCountInRow = SettingsDesign.CountProductInLine;
    protected float DiscountByTime = DiscountByTimeService.GetDiscountByTime();

    protected int ColorImageHeight = SettingsPictureSize.ColorIconHeightCatalog;
    protected int ColorImageWidth = SettingsPictureSize.ColorIconWidthCatalog;


    protected void Page_Load(object sender, EventArgs e)
    {
        SettingsDesign.eMainPageMode currentMode = !Demo.IsDemoEnabled || !CommonHelper.GetCookieString("structure").IsNotEmpty()
                      ? SettingsDesign.MainPageMode
                      : (SettingsDesign.eMainPageMode)Enum.Parse(typeof(SettingsDesign.eMainPageMode), CommonHelper.GetCookieString("structure"));


        if (SettingsDesign.Template == TemplateService.DefaultTemplateId && Mode != currentMode)
        {
            this.Visible = false;
            return;
        }

        var countNew = ProductOnMain.GetProductCountByType(ProductOnMain.TypeFlag.New);
        var countDiscount = ProductOnMain.GetProductCountByType(ProductOnMain.TypeFlag.Discount);
        var countBestseller = ProductOnMain.GetProductCountByType(ProductOnMain.TypeFlag.Bestseller);

        switch (Mode)
        {
            case SettingsDesign.eMainPageMode.Default:

                mvMainPageProduct.SetActiveView(viewDefault);

                //if (countBestseller == 0)
                //{
                //    ItemsCount = 3;
                //}
                //if (countNew == 0)
                //{
                //    ItemsCount = ItemsCount == 2 ? 3 : 6;
                //}
                //if (countDiscount == 0)
                //{
                //    ItemsCount = ItemsCount == 2 ? 3 : 6;
                //}

                if (countBestseller > 0)
                {
                    liBestsellers.Attributes.Add("class", "block width-for-" + ItemCountInRow); //SettingsDesign.CountLineOnMainPage);
                    lvBestSellers.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Bestseller, ItemCountInRow);
                    lvBestSellers.DataBind();
                }
                else
                {
                    liBestsellers.Visible = false;
                }
                if (countNew > 0)
                {
                    liNew.Attributes.Add("class", "block width-for-" + ItemCountInRow); //SettingsDesign.CountLineOnMainPage);
                    lvNew.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.New, ItemCountInRow);
                    lvNew.DataBind();
                }
                else
                {
                    liNew.Visible = false;
                }

                if (countDiscount > 0)
                {
                    liDiscount.Attributes.Add("class", "block block-last width-for-" + ItemCountInRow); //SettingsDesign.CountLineOnMainPage);
                    lvDiscount.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Discount, ItemCountInRow);
                    lvDiscount.DataBind();
                }
                else
                {
                    liDiscount.Visible = false;
                }

                break;
            case SettingsDesign.eMainPageMode.TwoColumns:
                mvMainPageProduct.SetActiveView(viewAlternative);

                if (countBestseller > 0)
                {
                    lvBestSellersAltervative.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Bestseller, ItemCountInRow);
                    lvBestSellersAltervative.DataBind();
                }
                else
                {
                    pnlBest.Visible = false;
                }

                if (countNew > 0)
                {
                    lvNewAlternative.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.New, ItemCountInRow);
                    lvNewAlternative.DataBind();
                }
                else
                {
                    pnlNew.Visible = false;
                }

                if (countDiscount > 0)
                {
                    lvDiscountAlternative.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Discount, ItemCountInRow);
                    lvDiscountAlternative.DataBind();
                }
                else
                {
                    pnlDiscount.Visible = false;
                }
                break;
            case SettingsDesign.eMainPageMode.ThreeColumns:
                mvMainPageProduct.SetActiveView(viewAlternative);

                if (countBestseller > 0)
                {
                    lvBestSellersAltervative.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Bestseller, ItemCountInRow);
                    lvBestSellersAltervative.DataBind();
                }
                else
                {
                    pnlBest.Visible = false;
                }

                if (countNew > 0)
                {
                    lvNewAlternative.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.New, ItemCountInRow);
                    lvNewAlternative.DataBind();
                }
                else
                {
                    pnlNew.Visible = false;
                }

                if (countDiscount > 0)
                {
                    lvDiscountAlternative.DataSource = ProductOnMain.GetProductsByType(ProductOnMain.TypeFlag.Discount, ItemCountInRow);
                    lvDiscountAlternative.DataBind();
                }
                else
                {
                    pnlDiscount.Visible = false;
                }
                break;

            default:
                throw new NotImplementedException();
        }
    }

        protected string RenderPictureTag(int productId, string strPhoto, string urlpath, string photoDesc, string productName)
        {
            string alt = photoDesc.IsNotEmpty() ? photoDesc : productName;
            return
                string.Format(
                    "<a href=\"{0}\" class=\"mp-pv-lnk\"><img src=\"{1}\" alt=\"{2}\" class=\"pv-photo p-photo scp-img\" {3} /></a>",
                    UrlService.GetLink(ParamType.Product, urlpath, productId), strPhoto.IsNotEmpty()
                                                                                   ? FoldersHelper.GetImageProductPath(ProductImageType.Small, strPhoto, false)
                                                                                   : "images/nophoto_small.jpg",
                    HttpUtility.HtmlEncode(alt), Mode == SettingsDesign.eMainPageMode.Default ? "width=100%" : "");
        }

    protected string RenderPriceTag(float price, float discount)
    {
        float totalDiscount = discount != 0 ? discount : DiscountByTime;
        return CatalogService.RenderPrice(price, totalDiscount, false, customerGroup);
    }
}