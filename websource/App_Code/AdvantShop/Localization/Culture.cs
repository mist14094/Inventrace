//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Globalization;
using System.Threading;
using AdvantShop.Configuration;

namespace AdvantShop.Localization
{

    public class Culture
    {
        public enum ListLanguage
        {
            Russian = 0,
            English = 1,
            Ukrainian = 2,
        }

        public static ListLanguage Language
        {
            get
            {
                switch (Configuration.SettingsMain.Language)
                {
                    case "en":
                    case "en-US":
                        return ListLanguage.English;
                    case "ru":
                    case "ru-RU":
                        return ListLanguage.Russian;
                    case "uk":
                    case "uk-UA":
                        return ListLanguage.Ukrainian;

                    default:
                        return ListLanguage.Russian;
                }
            }
            set
            {
                Configuration.SettingsMain.Language = GetStringLangByEnum(value);
                InitializeCulture();
            }
        }

        public static void InitializeCulture()
        {
            ListLanguage s = Language;
            string lang = GetStringLangByEnum(s);
            if (!string.IsNullOrEmpty(lang))
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(lang);
            }
        }

        public static void InitializeCulture(string langValue)
        {
            string lang = langValue;
            if (!string.IsNullOrEmpty(lang))
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture(lang);
            }
        }

        private static string GetStringLangByEnum(ListLanguage lang)
        {
            switch (lang)
            {
                case ListLanguage.English:
                    return "en-US";
                case ListLanguage.Russian:
                    return "ru-RU";
                case ListLanguage.Ukrainian:
                    return "uk-UA";
                default:
                    return "ru-RU";
            }
        }

        public static string ConvertDate(DateTime d)
        {
            try
            {
                return d.ToString(SettingsMain.AdminDateFormat);
            }
            catch (FormatException)
            {
                return d.ToString(CultureInfo.CurrentCulture);
            }
        }

        public static string ConvertShortDate(DateTime d)
        {
            try
            {
                return d.ToString(SettingsMain.ShortDateFormat);
            }
            catch (FormatException)
            {
                return d.ToShortDateString();
            }
        }


        public static string ConvertDateFromString(string s)
        {
            DateTime d = DateTime.Parse(s, System.Globalization.CultureInfo.GetCultureInfo(GetStringLangByEnum(Language)));
            return d.ToString(Configuration.SettingsMain.AdminDateFormat);
        }
    }
}
