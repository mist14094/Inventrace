//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Design;
using System.IO;
using AdvantShop.Helpers;

namespace AdvantShop.Core.UrlRewriter
{
    public class HttpTemplatesRewrite : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
        }

        #endregion
        
        private static void OnBeginRequest(object sender, EventArgs e)
        {

            // Check cn
            if (AppServiceStartAction.state != DataBaseService.PingDbState.NoError)
            {
                // Nothing here
                // just return
                return;
            }

            var app = (HttpApplication)sender;
            string strCurrentUrl = app.Request.RawUrl.ToLower().Trim();

            // Debug go first
            if (UrlService.CheckDebugAddress(strCurrentUrl))
            {
                return;
            }

            if (strCurrentUrl.Contains("/admin/"))
            {
                return;
            }

            string template = MobileHelper.IsMobileEnabled() ? "Mobile" : SettingsDesign.Template;
            if (template == TemplateService.DefaultTemplateId)
            {
                return;
            }

            MobileHelper.RedirectToMobile(app.Context);

            if (!app.Request.Path.Contains(".aspx") && !app.Request.Path.Contains("robots.txt")) return;
            string path = app.Request.Path.ToLower();
            if (app.Request.ApplicationPath != "/")
            {
                if (app.Request.ApplicationPath != null)
                    path = path.Replace(app.Request.ApplicationPath.ToLower(), "");
            }
            path = path.TrimStart('/').Split('?')[0];
            var strFilePath = string.Format("~/Templates/{0}/{1}", template, path);

            if (File.Exists(HttpContext.Current.Server.MapPath(strFilePath)))
            {
                app.Context.RewritePath(strFilePath + app.Request.Url.Query);
            }
        }
    }
}