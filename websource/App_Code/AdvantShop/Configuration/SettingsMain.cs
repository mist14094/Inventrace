//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Web;
using AdvantShop.Helpers;

namespace AdvantShop.Configuration
{
    public class SettingsMain
    {
        public static bool EnableCaptcha
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["EnableCheckOrderConfirmCode"]); }
            set { SettingProvider.Items["EnableCheckOrderConfirmCode"] = value.ToString(); }
        }

        public static bool EnableAutoUpdateCurrencies
        {
            get { return SQLDataHelper.GetBoolean(SettingProvider.Items["EnableAutoUpdateCurrencies"]); }
            set { SettingProvider.Items["EnableAutoUpdateCurrencies"] = value.ToString(); }
        }

        public static string LogoImageName
        {
            get { return SettingProvider.Items["MainPageLogoFileName"]; }
            set { SettingProvider.Items["MainPageLogoFileName"] = value; }
        }

        public static string FaviconImageName
        {
            get { return SettingProvider.Items["MainFaviconFileName"]; }
            set { SettingProvider.Items["MainFaviconFileName"] = value; }
        }

        public static string SiteUrl
        {
            get { return SettingProvider.Items["ShopURL"]; }
            set { SettingProvider.Items["ShopURL"] = value; }
        }

        public static string ShopName
        {
            get { return SettingProvider.Items["ShopName"]; }
            set { SettingProvider.Items["ShopName"] = value; }
        }

        public static string LogoImageAlt
        {
            get { return SettingProvider.Items["ImageAltText"]; }
            set { SettingProvider.Items["ImageAltText"] = value; }
        }

        public static string Language
        {
            get { return SettingProvider.Items["Language"]; }
            set { SettingProvider.Items["Language"] = value; }
        }

        public static string AdminDateFormat
        {
            get { return SettingProvider.Items["AdminDateFormat"]; }
            set { SettingProvider.Items["AdminDateFormat"] = value; }
        }

        public static string ShortDateFormat
        {
            get { return SettingProvider.Items["ShortDateFormat"]; }
            set { SettingProvider.Items["ShortDateFormat"] = value; }
        }

        public static int SalerCountryId
        {
            get { return SQLDataHelper.GetInt(SettingProvider.Items["SalerCountryID"]); }
            set { SettingProvider.Items["SalerCountryID"] = value.ToString(); }
        }

        public static int SalerRegionId
        {
            get { return SQLDataHelper.GetInt(SettingProvider.Items["SalerRegionID"]); }
            set { SettingProvider.Items["SalerRegionID"] = value.ToString(); }
        }

        public static string Phone
        {
            get { return SettingProvider.Items["Phone"]; }
            set { SettingProvider.Items["Phone"] = value; }
        }

        public static string City
        {
            get { return SettingProvider.Items["City"]; }
            set { SettingProvider.Items["City"] = value; }
        }

        public static string SearchPage
        {
            get { return SettingProvider.Items["SearchPage"]; }
            set { SettingProvider.Items["SearchPage"] = value; }
        }

        public static string SearchArea
        {
            get { return SettingProvider.Items["SearchArea"]; }
            set { SettingProvider.Items["SearchArea"] = value; }
        }

    }
}