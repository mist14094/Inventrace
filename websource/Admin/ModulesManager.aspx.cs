//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using Resources;

namespace Admin
{
    public partial class ModulesManager : AdvantShopAdminPage
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_ModuleManager_Header));
            lTrialMode.Visible = Trial.IsTrialEnabled;
        }

    }
}