//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using AdvantShop.Core;

namespace AdvantShop.Security
{
    /// <summary>
    /// Summary description for HttpAuthorization
    /// </summary>
    public class HttpAuthorization : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.AuthenticateRequest += context_AuthenticateRequest;
        }

        private static readonly string[] AuthFileTypes = new[] { ".aspx", ".ashx", ".asmx" };

        void context_AuthenticateRequest(object sender, EventArgs e)
        {
            // Check cn first
            if (AppServiceStartAction.state != DataBaseService.PingDbState.NoError)
            {
                // Nothing here
                // just return
                return;
            }

            var app = (HttpApplication)sender;

            // Check debug
            if (Core.UrlRewriter.UrlService.CheckDebugAddress(app.Request.RawUrl.ToLower().Trim()))
            {
                return;
            }

            if (app.Request.FilePath.ToLower().EndsWith(AuthFileTypes) && app.Context.Session != null)
            {
                AuthorizeService.LoadUserCookies();
            }
        }

        public void Dispose()
        {

        }
    }
}