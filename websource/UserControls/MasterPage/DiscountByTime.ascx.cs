using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Helpers;

public partial class UserControls_MasterPage_DiscountByTime : System.Web.UI.UserControl
{
    protected string popupText;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (DiscountByTimeService.ShowPopup && CommonHelper.GetCookieString("discountbytime").IsNullOrEmpty()
                && DiscountByTimeService.GetDiscountByTime() != 0)
        {
            popupText = DiscountByTimeService.PopupText;
            CommonHelper.SetCookie("discountbytime", "true", new TimeSpan(12, 0, 0), true);
        }
        else
        {
            this.Visible = false;
        }
    }
}