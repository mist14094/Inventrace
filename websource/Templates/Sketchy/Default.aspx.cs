//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Configuration;
using AdvantShop.Controls;

namespace Templates.Sketchy
{
    public partial class DefaultPage : AdvantShopPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            news.Visible = SettingsDesign.NewsVisibility;
            voting.Visible = SettingsDesign.VotingVisibility;
            checkOrder.Visible = SettingsDesign.CheckOrderVisibility;
            carousel.Visible = SettingsDesign.CarouselVisibility;
            mainPageProduct.Visible = SettingsDesign.MainPageProductsVisibility;
            giftCertificate.Visible = SettingsDesign.GiftSertificateVisibility && SettingsOrderConfirmation.EnableGiftCertificateService;
            SetMeta(null, string.Empty);
        }
    }
}