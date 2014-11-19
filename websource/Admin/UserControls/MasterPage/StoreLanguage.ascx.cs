//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Localization;
using System.Globalization;
using Resources;

namespace Admin.UserControls.MasterPage
{
    public partial class StoreLanguage : System.Web.UI.UserControl
    {
        protected void lnkEnglishLanguage_Click(object sender, EventArgs e)
        {
            Culture.Language = Culture.ListLanguage.English;
            Response.Redirect(Request.RawUrl);
        }

        protected void lnkRussianLanguage_Click(object sender, EventArgs e)
        {
            Culture.Language = Culture.ListLanguage.Russian;
            Response.Redirect(Request.RawUrl);
        }

        protected void lnkUkrainianLanguage_Click(object sender, EventArgs e)
        {
            Culture.Language = Culture.ListLanguage.Ukrainian;
            Response.Redirect(Request.RawUrl);
        }
        
        protected string RenderLanguageImage()
        {
            string result = "<img src=\"images/new_admin/lang/" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName + ".jpg\" class=\"lang-selected\" alt=\"{0}\"/>";

            switch (Culture.Language)
            {
                case Culture.ListLanguage.English:
                    return string.Format(result, Resource.Global_Language_English);
                case Culture.ListLanguage.Russian:
                    return string.Format(result, Resource.Global_Language_Russian);
                case Culture.ListLanguage.Ukrainian:
                    return string.Format(result, Resource.Global_Language_Ukrainian);
                default:
                    return string.Empty;
            }
        }
    }
}