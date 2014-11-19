//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Text;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.Caching;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.SaasData;
using SquishIt.Framework;
using SquishIt.Framework.Css;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Minifiers.CSS;
using SquishIt.Framework.Minifiers.JavaScript;

namespace ClientPages
{
    public partial class MasterPage : System.Web.UI.MasterPage, IMasterPage
    {
        protected StringBuilder NotifyMessage = new StringBuilder();

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Request.UserAgent != null &&
                (Request.UserAgent.Contains("MSIE 6.0") || Request.UserAgent.Contains("MSIE 7.0") ||
                 Request.UserAgent.Contains("MSIE 8.0")) && !Request.UserAgent.Contains("ms-word") && !Request.UserAgent.Contains("ms-excel") && !Request.UserAgent.Contains("ms-outlook") && !Request.UserAgent.Contains("ms-office"))
            {
                Response.Redirect("~/ie6/default.aspx", true);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            form.Action = Request.RawUrl;

            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.IsWorkingNow)
            {
                Response.Redirect(UrlService.GetAbsoluteLink("/app_offline.html"));
            }

            searchBlock.Visible = (SettingsDesign.SearchBlockLocation == SettingsDesign.eSearchBlockLocation.TopMenu &&
                                   SettingsDesign.MainPageMode == SettingsDesign.eMainPageMode.Default);
            demoFeedBack.Visible = Demo.IsDemoEnabled || Trial.IsTrialEnabled;
            Logo.ImgSource = FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, false);

            SettingsDesign.eMainPageMode currentMode = !Demo.IsDemoEnabled ||
                                                       !CommonHelper.GetCookieString("structure").IsNotEmpty()
                                                           ? SettingsDesign.MainPageMode
                                                           : (SettingsDesign.eMainPageMode)
                                                             Enum.Parse(typeof (SettingsDesign.eMainPageMode),
                                                                        CommonHelper.GetCookieString("structure"));
            toMobile.Visible = MobileHelper.IsMobileBrowser();
            if (currentMode == SettingsDesign.eMainPageMode.Default)
            {
                menuTop.Visible = true;
                searchBig.Visible = false;
                menuCatalog.Visible = true;
                menuTopMainPage.Visible = false;

                liViewCss.Text = "<link rel=\"stylesheet\" href=\"css/views/default.css\" >";
            }
            else if (currentMode == SettingsDesign.eMainPageMode.TwoColumns)
            {
                menuTop.Visible = false;
                searchBig.Visible = (SettingsDesign.SearchBlockLocation == SettingsDesign.eSearchBlockLocation.TopMenu);
                menuCatalog.Visible = false;
                searchBlock.Visible = false;
                menuTopMainPage.Visible = true;

                liViewCss.Text = "<link rel=\"stylesheet\" href=\"css/views/twocolumns.css\" >";
            }
            else if (currentMode == SettingsDesign.eMainPageMode.ThreeColumns)
            {
                menuTop.Visible = false;
                searchBig.Visible = (SettingsDesign.SearchBlockLocation == SettingsDesign.eSearchBlockLocation.TopMenu);
                menuCatalog.Visible = false;
                menuTopMainPage.Visible = true;

                liViewCss.Text = "<link rel=\"stylesheet\" href=\"css/views/threecolumns.css\" >";
            }
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!FileHelpers.IsCombineCssJsExsist(false))
            {
                CacheManager.RemoveByPattern("squishit_");
            }

            headStyle.Text = Bundle.Css()
                                   .Add("~/css/normalize.css")
                                   .Add("~/css/advcss/modal.css")
                                   .Add("~/css/advcss/notify.css")

                                   .Add("~/css/jq/jquery.cloud-zoom.css")
                                   .Add("~/css/jq/jquery-ui-1.8.17.custom.css")
                                   .Add("~/css/jq/jquery.autocomplete.css")
                                   .Add("~/css/jq/jquery.fancybox-1.3.4.css")

                                   .Add("~/css/theme.css")
                                   .Add("~/css/constructor.css")
                                   .Add("~/css/carousel.css")
                                   .Add("~/css/forms.css")
                                   .Add("~/css/styles.css")
                                   .Add("~/css/styles-extra.css")
                                   .Add("~/css/validator.css")
                                   .Add("~/js/plugins/jpicker/css/jpicker.css")
                                   .Add("~/js/plugins/upper/css/upper.css")
                                   .Add("~/js/plugins/expander/css/expander.css")
                                   .Add("~/js/plugins/vote/css/vote.css")
                                   .Add("~/js/plugins/progress/css/progress.css")
                                   .Add("~/js/plugins/compare/css/compare.css")
                                   .Add("~/js/plugins/spinbox/css/spinbox.css")
                                   .Add("~/js/plugins/cart/css/cart.css")
                                   .Add("~/js/plugins/scrollbar/css/scrollbar.css")
                                   .Add("~/js/plugins/tabs/css/tabs.css")
                                   .Add("~/js/plugins/videos/css/videos.css")
                                   .Add("~/js/plugins/flexslider/css/flexslider.css")
                                   .Add("~/js/plugins/sizeColorPickerDetails/css/sizeColorPickerDetails.css")
                                   .Add("~/js/plugins/sizeColorPickerCatalog/css/sizeColorPickerCatalog.css")
                                   .Add("~/js/plugins/cexpander/css/cexpander.css")

                                   .Add("~/js/jspage/details/css/details.css")

                                   .WithMinifier(MinifierFactory.Get<CSSBundle, YuiCompressor>())
                                   .Render("~/css/combined_#.css");

            headScript.Text = Bundle.JavaScript()
                                    .Add("~/js/jq/jquery-1.7.1.min.js")
                                    .Add("~/js/modernizr.custom.js")
                                    .Add("~/js/ejs_fulljslint.js")
                                    .Add("~/js/ejs.js")
                                    .WithMinifier(MinifierFactory.Get<JavaScriptBundle, YuiMinifier>())
                                    .Render("js/combined_#.js");

            bottomScript.Text = Bundle.JavaScript()
                                      .Add("~/js/fix/PIEInit.js")
                                      .Add("~/js/localization/" + SettingsMain.Language + "/lang.js")
                                      .Add("~/js/string.format-1.0.js")

                                      .Add("~/js/advantshop.js")
                                      .Add("~/js/services/Utilities.js")
                                      .Add("~/js/services/scriptsManager.js")
                                      .Add("~/js/services/jsuri-1.1.1.js")
                                      .Add("~/js/services/offers.js")

                                      .Add("~/js/jq/jquery-ui-1.8.17.custom.min.js")
                                      .Add("~/js/jq/jquery.cloud-zoom.1.0.2.js")
                                      .Add("~/js/jq/jquery.cookie.js")
                                      .Add("~/js/jq/jquery.metadata.js")
                                      .Add("~/js/jq/jquery.fancybox-1.3.4.js")

                                      .Add("~/js/jq/jquery.jcarousel.min.js")
                                      .Add("~/js/jq/jquery.placeholder.js")
                                      .Add("~/js/jq/jquery.validate.js")
                                      .Add("~/js/jq/jquery.autocomplete.js")
                                      .Add("~/js/jq/jquery.raty.js")
                                      .Add("~/js/jq/jquery.mousewheel.js")

                                      .Add("~/js/advjs/advBuyInOneClick.js")
                                      .Add("~/js/advjs/advDetectTouch.js")
                                      .Add("~/js/advjs/advFeedback.js")
                                      .Add("~/js/advjs/advNotify.js")
                                      .Add("~/js/advjs/advModal.js")
                                      .Add("~/js/advjs/advMoveCaret.js")
                                      .Add("~/js/advjs/advGiftCertificate.js")
                                      .Add("~/js/advjs/advMyAccount.js")
                                      .Add("~/js/advjs/advOrderConfirmation.js")
                                      .Add("~/js/advjs/advReviews.js")
                                      .Add("~/js/advjs/advUtils.js")

                                      .Add("~/js/plugins/cart/cart.js")
                                      .Add("~/js/plugins/compare/compare.js")
                                      .Add("~/js/plugins/expander/expander.js")
                                      .Add("~/js/plugins/jpicker/jpicker.js")
                                      .Add("~/js/plugins/progress/progress.js")
                                      .Add("~/js/plugins/reviews/reviews.js")
                                      .Add("~/js/plugins/scrollbar/scrollbar.js")
                                      .Add("~/js/plugins/spinbox/spinbox.js")
                                      .Add("~/js/plugins/tabs/tabs.js")
                                      .Add("~/js/plugins/upper/upper.js")
                                      .Add("~/js/plugins/vote/vote.js")
                                      .Add("~/js/plugins/videos/videos.js")
                                      .Add("~/js/plugins/flexslider/flexslider.js")
                                      .Add("~/js/plugins/sizeColorPickerDetails/sizeColorPickerDetails.js")
                                      .Add("~/js/plugins/sizeColorPickerCatalog/sizeColorPickerCatalog.js")
                                      .Add("~/js/plugins/imagePicker/imagePicker.js")
                                      .Add("~/js/plugins/cexpander/cexpander.js")

                                      .Add("~/js/jspage/details/details.js")

                                      .Add("~/js/common.js")
                                      .Add("~/js/constructor.js")
                                      .Add("~/js/dopostback.js")
                                      .Add("~/js/validateInit.js")
                                      .WithMinifier(MinifierFactory.Get<JavaScriptBundle, YuiMinifier>())
                                      .Render("~/js/combined_#.js");

            headCustomMeta.Text = SettingsSEO.CustomMetaString;
        }


        public void ShowMessage(Notify.NotifyType notifyType, string message)
        {
            NotifyMessage.Append(Notify.ShowMessage(notifyType, message));
        }

        protected void toMobileLink_Click(object sender, EventArgs e)
        {
            MobileHelper.DeleteDesktopForcedCookie();
            MobileHelper.RedirectToMobile(Context);
        }
    }
}