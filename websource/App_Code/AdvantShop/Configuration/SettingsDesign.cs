//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Helpers;

namespace AdvantShop.Configuration
{
    public class SettingsDesign
    {
        public enum eSearchBlockLocation
        {
            None = 0,
            TopMenu = 1,
            CatalogMenu = 2
        }
        
        public enum eMainPageMode
        {
            Default = 0,
            TwoColumns = 1,
            ThreeColumns = 2
        }

        #region Design settings in db
        
        public static string Template
        {
            get { return SettingProvider.Items["Template"]; }
            set { SettingProvider.Items["Template"] = value.ToString(); }
        }

        public static bool ShoppingCartVisibility
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["ShowShoppingCartOnMainPage"]); }
            set { SettingProvider.Items["ShowShoppingCartOnMainPage"] = value.ToString(); }
        }

        public static bool EnableZoom
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["EnabledZoom"]); }
            set { SettingProvider.Items["EnabledZoom"] = value.ToString(); }
        }
        
        #endregion

        #region Current template settings in template.config

        public static string Theme
        {
            get { return TemplateSettingsProvider.Items["Theme"]; }
            set { TemplateSettingsProvider.Items["Theme"] = value.ToString(); }
        }

        public static string ColorScheme
        {
            get { return TemplateSettingsProvider.Items["ColorScheme"]; }
            set { TemplateSettingsProvider.Items["ColorScheme"] = value.ToString(); }
        }

        public static string BackGround
        {
            get { return TemplateSettingsProvider.Items["BackGround"]; }
            set { TemplateSettingsProvider.Items["BackGround"] = value.ToString(); }
        }

        public static eSearchBlockLocation SearchBlockLocation
        {
            get 
            {
                var result = eSearchBlockLocation.CatalogMenu;
                Enum.TryParse<eSearchBlockLocation>(TemplateSettingsProvider.Items["SearchBlockLocation"], out result);
                return result;
            }
            set { TemplateSettingsProvider.Items["SearchBlockLocation"] = value.ToString(); }
        }

        public static eMainPageMode MainPageMode
        {
            get 
            { 
                var mainPageMode = eMainPageMode.Default;
                Enum.TryParse<eMainPageMode>(TemplateSettingsProvider.Items["MainPageMode"], out mainPageMode);
                return mainPageMode;
            }
            set { TemplateSettingsProvider.Items["MainPageMode"] = value.ToString(); }
        }
        
        public static string CarouselAnimation
        {
            get { return TemplateSettingsProvider.Items["CarouselAnimation"]; }
            set { TemplateSettingsProvider.Items["CarouselAnimation"] = value; }
        }

        public static int CarouselAnimationSpeed
        {
            get
            {
                int intTempResult = -1;
                Int32.TryParse(TemplateSettingsProvider.Items["CarouselAnimationSpeed"], out intTempResult);
                return intTempResult;
            }
            set { TemplateSettingsProvider.Items["CarouselAnimationSpeed"] = value.ToString(); }
        }

        public static int CarouselAnimationDelay
        {
            get
            {
                int intTempResult = -1;
                Int32.TryParse(TemplateSettingsProvider.Items["CarouselAnimationDelay"], out intTempResult);
                return intTempResult;
            }
            set { TemplateSettingsProvider.Items["CarouselAnimationDelay"] = value.ToString(); }
        }        

        /// <summary>
        /// ShowSeeProductOnMainPage
        /// </summary>
        public static bool RecentlyViewVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["RecentlyViewVisibility"]); }
            set { TemplateSettingsProvider.Items["RecentlyViewVisibility"] = value.ToString(); }
        }
        
        /// <summary>
        /// ShowNewsOnMainPage
        /// </summary>
        public static bool NewsVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["NewsVisibility"]); }
            set { TemplateSettingsProvider.Items["NewsVisibility"] = value.ToString(); }
        }

        /// <summary>
        /// ShowNewsSubscriptionOnMainPage
        /// </summary>
        public static bool NewsSubscriptionVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["NewsSubscriptionVisibility"]); }
            set { TemplateSettingsProvider.Items["NewsSubscriptionVisibility"] = value.ToString(); }
        }

        /// <summary>
        /// ShowVotingOnMainPage
        /// </summary>
        public static bool VotingVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["VotingVisibility"]); }
            set { TemplateSettingsProvider.Items["VotingVisibility"] = value.ToString(); }
        }

        /// <summary>
        /// ShowStatusCommentOnMainPage
        /// </summary>
        public static bool CheckOrderVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["CheckOrderVisibility"]); }
            set { TemplateSettingsProvider.Items["CheckOrderVisibility"] = value.ToString(); }
        }

        /// <summary>
        /// ShowFilterInCatalog
        /// </summary>
        public static bool FilterVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["FilterVisibility"]); }
            set { TemplateSettingsProvider.Items["FilterVisibility"] = value.ToString(); }
        }
        
        /// <summary>
        /// ShowCurrencyOnMainPage
        /// </summary>
        public static bool CurrencyVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["CurrencyVisibility"]); }
            set { TemplateSettingsProvider.Items["CurrencyVisibility"] = value.ToString(); }
        }

        /// <summary>
        /// ShowMainPageProductsOnMainPage
        /// </summary>
        public static bool MainPageProductsVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["MainPageProductsVisibility"]); }
            set { TemplateSettingsProvider.Items["MainPageProductsVisibility"] = value.ToString(); }
        }
        
        /// <summary>
        /// GiftSertificateBlock
        /// </summary>
        public static bool GiftSertificateVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["GiftSertificateVisibility"]); }
            set { TemplateSettingsProvider.Items["GiftSertificateVisibility"] = value.ToString(); }
        }

        /// <summary>
        /// WishList
        /// </summary>
        public static bool WishListVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["WishListVisibility"]); }
            set { TemplateSettingsProvider.Items["WishListVisibility"] = value.ToString(); }
        }

        /// <summary>
        /// CarouseltVisibility
        /// </summary>
        public static bool CarouselVisibility
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["CarouselVisibility"]); }
            set { TemplateSettingsProvider.Items["CarouselVisibility"] = value.ToString(); }
        }

        public static int CountProductInLine
        {
            get
            {
                if (Demo.IsDemoEnabled &&  CommonHelper.GetCookieString("ProductsCount").TryParseInt() != 0)
                {
                    return CommonHelper.GetCookieString("ProductsCount").TryParseInt();
                }
                else
                {
                    return TemplateSettingsProvider.Items["CountProductInLine"].TryParseInt();
                }
                
            }
            set
            {
                if (Demo.IsDemoEnabled)
                {
                    CommonHelper.SetCookie("ProductsCount", value.ToString());
                }
                else
                {
                    TemplateSettingsProvider.Items["CountProductInLine"] = value.ToString();    
                }
                
            }
        }        

        public static bool EnableSocialShareButtons
        {
            get { return SQLDataHelper.GetBoolean(TemplateSettingsProvider.Items["EnableSocialShareButtons"]); }
            set { TemplateSettingsProvider.Items["EnableSocialShareButtons"] = value.ToString(); }
        }

        #endregion;
    }
}