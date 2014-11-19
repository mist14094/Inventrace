//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using Resources;

public partial class Admin_RitmzSettings : AdvantShopAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_RitmzSettings_Header));
    }

    protected void btnSave_Click(object sevder, EventArgs e)
    {
        RitmzSettings.SaveData();
    }
}
