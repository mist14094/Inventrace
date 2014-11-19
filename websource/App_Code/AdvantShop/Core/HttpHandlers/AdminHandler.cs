using System.Web;
using System.Web.SessionState;
using AdvantShop.Customers;
using AdvantShop.Security;


namespace AdvantShop.Core.HttpHandlers
{
    public abstract class AdminHandler : IHttpHandler, IRequiresSessionState
    {
        protected AdminHandler()
        {
            Localization.Culture.InitializeCulture();
        }

        public bool Authorize(HttpContext context)
        {
         if (! (CustomerSession.CurrentCustomer.IsAdmin
             || CustomerSession.CurrentCustomer.CustomerRole == Role.Moderator && RoleAccess.Check(CustomerSession.CurrentCustomer, context.Request.Url.ToString().ToLower())
             || CustomerSession.CurrentCustomer.IsVirtual))
            {
                context.Response.Clear();
                context.Response.StatusCode = 403;
                context.Response.Status = "403 Forbidden";
                return false;
            }
            return true;
        }
        
        public void ProcessRequest(HttpContext context)
        {
            //throw new System.NotImplementedException();
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}