//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.CMS;
using AdvantShop.Core.Caching;

public partial class UserControls_MenuTop : System.Web.UI.UserControl
{
    protected string GetHtml()
    {
        if (!AdvantShop.Customers.CustomerSession.CurrentCustomer.RegistredUser && CacheManager.Contains(CacheNames.GetMainMenuCacheObjectName()))
        {
            return CacheManager.Get<string>(CacheNames.GetMainMenuCacheObjectName());
        }
        if (AdvantShop.Customers.CustomerSession.CurrentCustomer.RegistredUser && CacheManager.Contains(CacheNames.GetMainMenuAuthCacheObjectName()))
        {
            return CacheManager.Get<string>(CacheNames.GetMainMenuAuthCacheObjectName());
        }

        string result = string.Empty;

        foreach (var mItem in MenuService.GetEnabledChildMenuItemsByParentId(0, MenuService.EMenuType.Top, AdvantShop.Customers.CustomerSession.CurrentCustomer.RegistredUser ? EMenuItemShowMode.Authorized : EMenuItemShowMode.NotAuthorized))
        {
            if (mItem.NoFollow)
            {
                result += "<!--noindex-->";
            }

            result += string.Format("<a href=\"{0}\"{1}{3}>{2}</a>\n",
                mItem.MenuItemUrlPath,
                mItem.Blank ? " target=\"_blank\"" : string.Empty,
                mItem.MenuItemName,
                mItem.NoFollow ? " rel='nofollow'": string.Empty);

            if (mItem.NoFollow)
            {
                result += "<!--/noindex-->";
            }
        }

        if (!AdvantShop.Customers.CustomerSession.CurrentCustomer.RegistredUser)
            CacheManager.Insert(CacheNames.GetMainMenuCacheObjectName(), result);
        else if (AdvantShop.Customers.CustomerSession.CurrentCustomer.RegistredUser)
            CacheManager.Insert(CacheNames.GetMainMenuAuthCacheObjectName(), result);

        return result;
    }

}