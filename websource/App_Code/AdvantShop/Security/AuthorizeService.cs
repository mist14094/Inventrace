//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Specialized;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;

namespace AdvantShop.Security
{
    [Serializable]
    public class AuthorizeService
    {
        private static string MCookieCollectionName
        {
            get { return HttpUtility.UrlEncode(SettingsMain.SiteUrl); }
        }

        public static bool CheckAdminCookies()
        {
            if (HttpContext.Current.Request.Cookies[MCookieCollectionName] != null)
            {
                var newCookie = HttpContext.Current.Request.Cookies[MCookieCollectionName];

                if (newCookie != null)
                    return LoginAdmin(newCookie["cUserName"], newCookie["cUserPWD"], true);
            }
            return false;
        }

        public static bool LoginAdmin(string email, string password, bool isHash)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var customer = CustomerService.GetCustomerByEmailAndPassword(email, password, isHash);
                if (customer == null)
                    return false;

                if (!customer.IsAdmin)
                    return false;

                DeleteCookie();
                WriteCookie(customer);
                return true;
            }
            return false;
        }

        public static void LoadUserCookies()
        {
            var collection = CommonHelper.GetCookieCollection(MCookieCollectionName);
            if (collection != null)
            {
                if (!AuthorizeTheUser(collection["cUserName"], collection["cUserPWD"], true))
                {
                    if (String.IsNullOrEmpty(collection["cUserId"]) || collection["cUserId"].TryParseGuid() == Guid.Empty)
                    {
                        CustomerSession.CreateAnonymousCustomerGuid();
                    }
                }
            }
            else
            {
                CustomerSession.CreateAnonymousCustomerGuid();
            }
        }
        
        public static void DeleteCookie()
        {
            CommonHelper.DeleteCookie(MCookieCollectionName);
        }

        public static void WriteCookie(Customer customer)
        {
            var collection = new NameValueCollection
                                    {
                                        {"cUserName", customer.EMail},
                                        {"cUserPWD", customer.Password},
                                        {"cUserId", customer.Id.ToString()}
                                    };
            CommonHelper.SetCookieCollection(MCookieCollectionName, collection, new TimeSpan(7, 0, 0, 0));
        }

        public static bool AuthorizeTheUser(string email, string password, bool isHash)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return false;

            if (HttpContext.Current.Session["isAuthorize"] != null && (bool)HttpContext.Current.Session["isAuthorize"]) return true;

            var isDebugAccount = Secure.IsDebugAccount(email, password);
            HttpContext.Current.Session["isDebug"] = isDebugAccount;

            if (isDebugAccount)//, false, false))
            {
                //HttpContext.Current.Session["isDebug"] = true;
                HttpContext.Current.Session["isAuthorize"] = true;
                Secure.AddUserLog("sa", true, true);
                return true;
            }

            var oldCustomerId = CustomerSession.CustomerId;
            var customer = CustomerService.GetCustomerByEmailAndPassword(email, password, isHash);
            if (customer == null)
            {
                //DeleteCookie();
                //CustomerSession.CreateAnonymousCustomerGuid();
                return false;
            }
            HttpContext.Current.Session["isAuthorize"] = true;
            DeleteCookie();
            WriteCookie(customer);
            Secure.AddUserLog(customer.EMail, true, customer.EMail == "admin");

            ShoppingCartService.MergeShoppingCarts(oldCustomerId, customer.Id);
            return true;
        }
    }
}