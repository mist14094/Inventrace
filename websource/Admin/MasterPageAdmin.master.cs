//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using System.Web.UI;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Notifications;
using AdvantShop.Statistic;
using SquishIt.Framework;
using SquishIt.Framework.Css;
using SquishIt.Framework.JavaScript;
using SquishIt.Framework.Minifiers;
using SquishIt.Framework.Minifiers.CSS;
using SquishIt.Framework.Minifiers.JavaScript;

namespace Admin
{
    public partial class MasterPageAdmin : MasterPage
    {

        protected int newReviewsCount = 0;
        protected int newAdminMessage = 0;

        protected void Page_Load(object sender, EventArgs e)
        {

            lBase.Text = string.Format("<base href='{0}'/>",
                                       Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath +
                                       (!Request.ApplicationPath.EndsWith("/") ? "/" : string.Empty)
                                       + "admin/");


            MenuAdmin.CurrentCustomer = CustomerSession.CurrentCustomer;

            newAdminMessage = AdminMessagesService.GetNotViewedAdminMessagesCount();
            newReviewsCount = StatisticService.GetLastReviewsCount();

            adminMessages.CssClass = "top-part-right icon-mail " + (newAdminMessage > 0 ? "icon-selected" : "");
            adminMessages.Text = newAdminMessage > 0 ? newAdminMessage.ToString() : "";

            adminReviews.CssClass = "top-part-right icon-bubble " + (newReviewsCount > 0 ? "icon-selected" : "");
            adminReviews.Text = newReviewsCount > 0 ? newReviewsCount.ToString() : "";

            var _customer = CustomerSession.CurrentCustomer;
            if (_customer.CustomerRole == Role.Moderator)
            {
                var actions = RoleActionService.GetCustomerRoleActionsByCustomerId(_customer.Id);
                bool visible = actions.Any(a => a.Key == RoleActionKey.DisplayAdminMainPageStatistics && a.Enabled);

                StoreLanguage.Visible = visible;
                LastAdminMessages.Visible = visible;
                adminMessages.Visible = visible;

                adminReviews.Visible = actions.Any(a => a.Key == RoleActionKey.DisplayComments && a.Enabled);
            }

            //if (CustomerSession.CurrentCustomer.IsVirtual)
            //{
            //    lblIsDebug.Text = @"&nbsp;&nbsp;&nbsp; (Debug mode)";
            //    lblIsDebug.Visible = true;
            //    Image1.ImageUrl = "images/logo_red.gif";
            //}
            //else
            //{
            //    lblIsDebug.Visible = false;
            //    Image1.ImageUrl = "images/logo.jpg";
            //}
        }

        public void Page_PreRender(object sender, EventArgs e)
        {
            //Customer _customer = CustomerSession.CurrentCustomer;
            //if (_customer.CustomerRole == Role.Moderator)
            //{
            //    var actions = RoleActionService.GetCustomerRoleActionsByCustomerId(_customer.Id);
            //    OnLineUsers.Visible = actions.Any(a => a.Key == RoleActionKey.DisplayOrders && a.Enabled);
            //}

            //OnLineUsers.Visible &= SettingsMain.EnableUserOnline;

            if (!FileHelpers.IsCombineCssJsExsist(true))
            {
                CacheManager.RemoveByPattern("squishit_");
            }

            headStyle.Text = Bundle.Css()
                                   .Add("../css/validator.css")
                                   .Add("../css/normalize.css")
                                   .Add("../css/advcss/modal.css")
                                   .Add("../css/jq/jquery.autocomplete.css")
                                   .Add("../js/plugins/progress/css/progress.css")
                                   .Add("../js/plugins/jpicker/css/jpicker.css")
                                   .Add("css/jquery.tooltip.css")
                                   .Add("css/AdminStyle.css")
                                   .Add("css/advcss/notify.css")
                                   .Add("css/catalogDataTreeStyles.css")
                                   .Add("css/exportFeedStyles.css")
                                   .Add("js/plugins/tooltip/css/tooltip.css")
                                   .Add("js/plugins/placeholder/css/placeholder.css")
                                   .Add("js/plugins/radiolist/css/radiolist.css")
                                   .Add("js/plugins/chart/css/chart.css")
                                   .Add("js/plugins/noticeStatistic/css/noticeStatistic.css")
                                   .Add("js/jspage/adminmessages/css/styles.css")
                                   .Add("css/new_admin/buttons.css")
                                   .Add("css/new_admin/dropdown-menu.css")
                                   .Add("css/new_admin/icons.css")
                                   .Add("css/new_admin/admin.css")
                                   .Add("css/new_admin/pagenumber.css")
                                   .WithMinifier(MinifierFactory.Get<CSSBundle, YuiCompressor>())
                                   .Render("css/combined_admin_#.css");

            // combine javascript

            headScript.Text = Bundle.JavaScript()
                                    .Add("../js/jq/jquery-1.7.1.min.js")
                                    .Add("../js/modernizr.custom.js")
                                    .Add("../js/localization/" + SettingsMain.Language + "/lang.js")
                                    .Add("../js/ejs_fulljslint.js")
                                    .Add("../js/ejs.js")
                                    .WithMinifier(MinifierFactory.Get<JavaScriptBundle, YuiMinifier>())
                                    .Render("js/combined_admin_head_#.js");

            bottomScript.Text = Bundle.JavaScript()
                                      .Add("../js/fix/PIEInit.js")
                                      .Add("../js/jq/jquery.validate.js")
                                      .Add("../js/validateInit.js")
                                      .Add("../js/string.format-1.0.js")
                                      .Add("../js/jq/jquery.autocomplete.js")
                                      .Add("../js/jq/jquery.metadata.js")
                                      .Add("../js/advjs/advNotify.js")
                                      .Add("../js/advjs/advModal.js")
                                      .Add("../js/advjs/advTabs.js")
                                      .Add("../js/advjs/advUtils.js")
                                      .Add("../js/advantshop.js")
                                      .Add("../js/services/Utilities.js")
                                      .Add("../js/services/scriptsManager.js")
                                      .Add("../js/services/jsuri-1.1.1.js")
                                      .Add("../js/plugins/progress/progress.js")
                                      .Add("../js/plugins/jpicker/jpicker.js")
                                      .Add("js/customValidate.js")
                                      .Add("js/smallThings.js")
                                      .Add("js/jspage/adminmessages/adminmessages.js")
                                      .Add("js/jspage/vieworder.js")
                                      .Add("js/jspage/default.js")
                                      .Add("js/jquery.cookie.min.js")
                                      .Add("js/jquery.qtip.min.js")
                                      .Add("js/jquery.tooltip.min.js")
                                      .Add("js/slimbox2.js")
                                      .Add("js/jquery.history.js")
                                      .Add("js/jquerytimer.js")
                                      .Add("js/admin.js")
                                      .Add("js/grid.js")
                                      .Add("js/plugins/tooltip/tooltip.js")
                                      .Add("js/plugins/placeholder/placeholder.js")
                                      .Add("js/plugins/chart/jquery.flot.js")
                                      .Add("js/plugins/chart/chart.js")
                                      .Add("js/plugins/radiolist/radiolist.js")
                                      .Add("js/plugins/noticeStatistic/noticeStatistic.js")
                                      .Add("js/masterpage/adminsearch.js")
                                      .Add("js/masterpage/saasIndicator.js")
                                      .Add("js/masterpage/ordersCount.js")

                                      .WithMinifier(MinifierFactory.Get<JavaScriptBundle, YuiMinifier>()).Render("js/combined_admin_bottom_#.js");
        }

        protected void lnkExit_Click(object sender, EventArgs e)
        {
            CustomerSession.CreateAnonymousCustomerGuid();
            Session["isDebug"] = false;
            CommonHelper.DeleteCookie(HttpUtility.UrlEncode(SettingsMain.SiteUrl));
            Response.Redirect("~/");
        }
    }
}