//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;

namespace AdvantShop.Core.UrlRewriter
{
    public class HttpUrlRewrite : IHttpModule
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
            var app = (HttpApplication)sender;
            string strCurrentUrl = app.Request.RawUrl.ToLower().Trim();

            //app.Context.Response.TrySkipIisCustomErrors = true;

            // Debug go first
            if (UrlService.CheckDebugAddress(strCurrentUrl))
            {
                // Nothing here
                // just return
                return;
            }

            // Check cn
            if (AppServiceStartAction.state != DataBaseService.PingDbState.NoError)
            {
                // Nothing here
                // just return
                return;
            }

            // Check original pictures
            if (strCurrentUrl.Contains("/pictures/product/original/"))
            {
                app.Context.RewritePath("~/err404.aspx");
                return;
            }

            // Social 
            string social = UrlService.Social.Find(strCurrentUrl.Contains);
            if (social != null)
            {
                app.Response.RedirectPermanent("~/social/catalogsocial.aspx?type=" + social.Split('-').Last());
            }

            // Check exportfeed
            //if (strCurrentUrl.Contains("exportfeed.aspx") || strCurrentUrl.Contains("exportfeeddet.aspx"))
            //    return;

            // Payment return url
            if (strCurrentUrl.Contains("/paymentreturnurl/"))
            {
                app.Context.RewritePath("~/PaymentReturnUrl.aspx?PaymentMethodID=" + app.Request.Path.Split(new[] { "/paymentreturnurl/" }, StringSplitOptions.None).LastOrDefault()
                                        + (string.IsNullOrWhiteSpace(app.Request.Url.Query) ? string.Empty : "&" + app.Request.Url.Query.Trim('?')));
                return;
            }
            if (strCurrentUrl.Contains("/paymentnotification/"))
            {
                app.Context.RewritePath("~/HttpHandlers/PaymentNotification.ashx?PaymentMethodID=" + app.Request.Path.Split(new[] { "/paymentnotification/" }, StringSplitOptions.None).LastOrDefault()
                    + (string.IsNullOrWhiteSpace(app.Request.Url.Query) ? string.Empty : "&" + app.Request.Url.Query.Trim('?')));
                return;
            }

            // Seek in url table
            foreach (var key in UrlService.UrlTable.Keys.Where(strCurrentUrl.Split('?')[0].EndsWith))
            {
                app.Context.RewritePath(UrlService.UrlTable[key] + (string.IsNullOrWhiteSpace(app.Request.Url.Query)
                                                                            ? string.Empty
                                                                            : (UrlService.UrlTable[key].Contains("?") ? "&" : "?") + app.Request.Url.Query.Trim('?')));
                return;
            }


            //// Storage
            //string storage = UrlService.Storages.Find(strCurrentUrl.Contains);
            //if (storage != null)
            //{
            //    var index = strCurrentUrl.IndexOf(storage, StringComparison.Ordinal);
            //    string tail = app.Request.RawUrl.Substring(index + storage.Length);
            //    string pathNew = string.Format("~{0}{1}", storage, tail);
            //    app.Context.RewritePath(pathNew);
            //    return;
            //}

            string path = strCurrentUrl;
            if (app.Request.ApplicationPath != "/")
            {
                if (app.Request.ApplicationPath != null)
                    path = path.Replace(app.Request.ApplicationPath.ToLower(), "");
            }

            //301 redirect if need
            string extention = Path.GetExtension(path);
            if (UrlService.ExtentionNotToRedirect.Contains(extention))
                return;

            if (SettingsSEO.Enabled301Redirects && !path.Contains("/admin/"))
            {
                string newUrl = UrlService.GetRedirect301(path.Trim('/'), app.Request.Url.AbsoluteUri.Trim('/'));
                if (newUrl.IsNotEmpty())
                {
                    app.Response.RedirectPermanent(newUrl);
                    return;
                }
            }

            //if (strCurrentUrl != "/" && strCurrentUrl.EndsWith("/"))
            //    app.Response.RedirectPermanent("~/" + strCurrentUrl.Trim('/'));


            //if (UrlService.HasExtention(strCurrentUrl))
            //    return;

            if (path.Contains("/module/"))
            {
                var moduleStringId =
                    Modules.ModulesService.GetModuleStringIdByUrlPath(path.Split('/').LastOrDefault());
                if (moduleStringId.IsNotEmpty())
                {
                    app.Context.RewritePath("~/modulepage.aspx?moduleid=" + moduleStringId);
                    return;
                }
            }

            var modules = AttachedModules.GetModules(AttachedModules.EModuleType.UrlRewrite);
            if (modules != null)
            {
                foreach (var moduleType in modules)
                {
                    var moduleObject = (IModuleUrlRewrite) Activator.CreateInstance(moduleType, null);
                    string newUrl = path;
                    if (moduleObject.CheckAlive() && moduleObject.RewritePath(path, ref newUrl))
                    {
                        app.Context.RewritePath(newUrl);
                        return;
                    }
                }
            }


            var param = UrlService.ParseRequest(path);
            if (param != null)
            {
                UrlService.RedirectTo(app, param);
            }
            else if (path.IsNotEmpty() && path != "/" && !path.Contains(".") && !path.Contains("?"))
            {
                Debug.LogError(new HttpException(404, "Can't get url: " + app.Context.Request.RawUrl + "path: '" + path + "'"));
                app.Context.RewritePath("~/err404.aspx");
            }
        }
    }
}