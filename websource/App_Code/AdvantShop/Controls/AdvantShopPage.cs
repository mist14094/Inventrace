//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.SEO;
using AdvantShop.Core;
using System.IO;
using Resources;

namespace AdvantShop.Controls
{
    public class  AdvantShopPage : Page
    {
        protected override void InitializeCulture()
        {
            Localization.Culture.InitializeCulture();
            //Localization.Culture.InitializeCulture(AdvantshopConfigService.GetLocalization()); так сделано чтобы не долбиьтся в базу, а брать из конфига , НО тогда не работает переключение языков
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (AppServiceStartAction.state != DataBaseService.PingDbState.NoError)
            {
                SessionServices.StartSession(HttpContext.Current);
                return;
            }

            if (!File.Exists(SettingsGeneral.InstallFilePath))
            {
                Response.Redirect(UrlService.GetAbsoluteLink("install/default.aspx"));
            }
        }

        protected void Error404()
        {
            Server.Transfer(UrlService.GetAbsoluteLink("/err404.aspx"));
        }

        #region SetMeta

        protected void SetMeta(string pageTitle)
        {
            SetMetaTags(new MetaInfo(pageTitle));
        }

        protected MetaInfo SetMeta(MetaInfo meta, string name = null, string categoryName = null, string brandName = null, int page = 0)
        {
            MetaInfo newMeta = meta != null ? meta.DeepClone() : MetaInfoService.GetDefaultMetaInfo(); // Creating new object to modify - keeping original Meta for cache
			if (page > 1)
            {
                newMeta.Title += Resource.Client_Catalog_PageIs + page;
                newMeta.H1 += Resource.Client_Catalog_PageIs + page;
                newMeta.MetaDescription += Resource.Client_Catalog_PageIs + page;
            }
            SetMetaTags(MetaInfoService.GetFormatedMetaInfo(newMeta, name, categoryName, brandName));

 

            return newMeta;
        }

        private void SetMetaTags(MetaInfo meta)
        {
            var contr = (Literal)Page.Controls[0].FindControl("headMeta");
            if (contr == null) return;
            var strmeta = new StringBuilder();
            strmeta.Append("\n");
            strmeta.AppendFormat("<title>{0}</title>", meta.Title);
            strmeta.Append("\n");
            strmeta.AppendFormat("<meta name='Description' content='{0}'/>", meta.MetaDescription);
            strmeta.Append("\n");
            strmeta.AppendFormat("<meta name='Keywords' content='{0}'/>", meta.MetaKeywords);
            contr.Text = strmeta.ToString();
        }
        #endregion
    }
}