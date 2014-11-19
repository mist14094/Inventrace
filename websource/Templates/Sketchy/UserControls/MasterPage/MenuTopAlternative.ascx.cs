using System;
using AdvantShop.CMS;
using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using System.Text;
using System.Linq;

//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace Templates.Sketchy.UserControls.MasterPage
{
    public partial class MenuTopAlternative : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            searchBlock.Visible = true;// SettingsDesign.SearchBlockLocation == SettingsDesign.eSearchBlockLocation.CatalogMenu;
        }

        public string GetMenu()
        {
            var useCache = !Request.Url.AbsolutePath.Contains("err404.aspx");

            var rawUrl = Request.RawUrl;
            var cachename = CacheNames.GetMainMenuCacheObjectName() + "Alternative_" + rawUrl;

            if (useCache && !AdvantShop.Customers.CustomerSession.CurrentCustomer.RegistredUser && CacheManager.Contains(cachename))
            {
                return CacheManager.Get<string>(cachename);
            }
            if (useCache && AdvantShop.Customers.CustomerSession.CurrentCustomer.RegistredUser && CacheManager.Contains(cachename))
            {
                return CacheManager.Get<string>(cachename);
            }
        
            var result = new StringBuilder();

            var rootCategories = MenuService.GetEnabledChildMenuItemsByParentId(0, MenuService.EMenuType.Top, AdvantShop.Customers.CustomerSession.CurrentCustomer.RegistredUser ? EMenuItemShowMode.Authorized : EMenuItemShowMode.NotAuthorized).ToList();

        
            result.AppendFormat("<div class=\"tree-item\"><div class=\"tree-item-inside\"><a href=\"catalog\" class=\"tree-item-link tree-parent\">" + Resources.Resource.Client_MasterPage_Catalog + "</a>");

            result.AppendFormat("<div class=\"tree-submenu\">\r\n");
            result.Append("<div class=\"tree-submenu-category\">\r\n<div class=\"tree-submenu-column\">");

            foreach (var rootItem in CategoryService.GetChildCategoriesByCategoryIdForMenu(0).Where(cat => cat.Enabled))
            {
                result.AppendFormat("<a href='categories/{0}'>{1}</a>", rootItem.UrlPath, rootItem.Name);
            }

            result.Append("</div></div>\r\n");
            result.AppendFormat("</div>");

            result.AppendFormat("</div></div>");
            //**



            for (int rootIndex = 0; rootIndex < rootCategories.Count; ++rootIndex)
            {
                result.AppendFormat("<div class=\"{0}\"><div class=\"tree-item-inside\">", rawUrl.EndsWith(rootCategories[rootIndex].MenuItemUrlPath) ? "tree-item-selected" : "tree-item");

                result.AppendFormat("<a href=\"{0}\" class=\"{1}\"{2}>{3}</a>",
                                    rootCategories[rootIndex].MenuItemUrlPath,
                                    rootCategories[rootIndex].HasChild ? "tree-item-link tree-parent" : "tree-item-link",
                                    rootCategories[rootIndex].Blank ? " target=\"_blank\"" : string.Empty,
                                    rootCategories[rootIndex].MenuItemName);

                if (rootCategories[rootIndex].HasChild)
                {
                    result.AppendFormat("<div class=\"tree-submenu\">\r\n");
                    result.Append("<div class=\"tree-submenu-category\">\r\n<div class=\"tree-submenu-column\">");

                    foreach (var children in MenuService.GetEnabledChildMenuItemsByParentId(rootCategories[rootIndex].MenuItemID, MenuService.EMenuType.Top, AdvantShop.Customers.CustomerSession.CurrentCustomer.RegistredUser ? EMenuItemShowMode.Authorized : EMenuItemShowMode.NotAuthorized))
                    {
                        result.AppendFormat("<a href=\"{0}\"{1}>{2}</a>", children.MenuItemUrlPath,children.Blank ? " target=\"_blank\"" : string.Empty, children.MenuItemName);
                    }
                    result.Append("</div></div>\r\n");
                    result.AppendFormat("</div>");
                }

                //Пункт в главном меню закрывается
                result.AppendFormat("</div></div>");

                //if (rootIndex != rootCategories.Count - 1)
                //{
                //    result.AppendFormat("<div class=\"tree-item-split\"></div>");
                //}
            }

            string resultstring = result.ToString();

            if (useCache && !AdvantShop.Customers.CustomerSession.CurrentCustomer.RegistredUser)
                CacheManager.Insert(cachename, resultstring);
            else if (useCache && AdvantShop.Customers.CustomerSession.CurrentCustomer.RegistredUser)
                CacheManager.Insert(cachename, resultstring);

            return resultstring;
        }
    }
}