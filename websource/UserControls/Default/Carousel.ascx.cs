//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web.UI;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core;

public partial class UserControls_Default_Carousel : UserControl
{

    protected int CarouselsCount { get; set; }

    public string CssSlider { get; set; }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        CarouselsCount = CarouselService.GetCarouselsCount();
        if (CarouselsCount == 0 || SettingsDesign.CarouselVisibility == false)
        {
            this.Visible = false;
        }
    }
}