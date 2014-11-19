//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Localization;
using System.Globalization;
using Resources;

public partial class Admin_UserControls_StoreLanguage : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
  
    }

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

    protected string RenderLanguageImage()
    {
        string result = "<img src=\"images/new_admin/lang/" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName + ".jpg\" class=\"lang-selected\" alt=\"{0}\"/>";
        
        if (Culture.Language == Culture.ListLanguage.English)
        {
            return string.Format(result, Resource.Global_Language_English);
        }
        if (Culture.Language == Culture.ListLanguage.Russian)
        {
            return string.Format(result, Resource.Global_Language_Russian);
        }
        return string.Empty;
    }
}