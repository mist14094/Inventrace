using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Security;
using System;
using System.Globalization;

public partial class UserControls_MasterPage_TopPanel : System.Web.UI.UserControl
{

    protected string wishlistCount = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {

        pnlWishList.Visible = SettingsDesign.WishListVisibility;
        shoppingCart.Visible = SettingsDesign.ShoppingCartVisibility;
        aCreateTrial.Visible = Demo.IsDemoEnabled;
        lbLoginAsAdmin.Visible = Trial.IsTrialEnabled;
        
        Customer curentCustomer = CustomerSession.CurrentCustomer;
        //pnlLogin.Visible = !curentCustomer.RegistredUser;

        pnlConstructor.Visible = curentCustomer.CustomerRole == Role.Administrator || Demo.IsDemoEnabled || Trial.IsTrialEnabled;   
        aLogin.Visible = aRegister.Visible = !curentCustomer.RegistredUser;
        aRegister.HRef = UrlService.GetAbsoluteLink("registration.aspx");
        aLogin.HRef = UrlService.GetAbsoluteLink("login.aspx");
        lbLogOut.Visible = aMyAccount.Visible = curentCustomer.RegistredUser;
        aMyAccount.HRef = UrlService.GetAbsoluteLink("myaccount.aspx");
        pnlAdmin.Visible = (curentCustomer.CustomerRole == Role.Administrator || curentCustomer.CustomerRole == Role.Moderator) && !Demo.IsDemoEnabled && !Trial.IsTrialEnabled;
        pnlAdmin.HRef = UrlService.GetAbsoluteLink("admin/default.aspx");

        int wishCount = ShoppingCartService.CurrentWishlist.Count;
        wishlistCount = string.Format("{0} {1}", wishCount == 0 ? "" : wishCount.ToString(CultureInfo.InvariantCulture),
                                 Strings.Numerals(wishCount, Resources.Resource.Client_MasterPage_WishList_Empty,
                                                  Resources.Resource.Client_MasterPage_WishList_1Product,
                                                  Resources.Resource.Client_MasterPage_WishList_2Products,
                                                  Resources.Resource.Client_MasterPage_WishList_5Products));
       
        pnlCurrency.Visible = SettingsDesign.CurrencyVisibility;
        foreach (Currency row in CurrencyService.GetAllCurrencies(true))
        {
            ddlCurrency.Items.Add(new ListItem { Text = row.Name, Value = row.Iso3 });
        }

        ddlCurrency.SelectedValue = CurrencyService.CurrentCurrency.Iso3;
    }

    public void btnLogout_Click(object sender, EventArgs e)
    {
        CustomerSession.CreateAnonymousCustomerGuid();
        AuthorizeService.DeleteCookie();
        Response.Redirect("~/");
    }

    protected void lbLoginAsAdmin_Click(object sender, EventArgs e)
    {
        if (Demo.IsDemoEnabled || Trial.IsTrialEnabled)
        {
            CustomerSession.CreateAnonymousCustomerGuid();
            AuthorizeService.DeleteCookie();
            var customer = CustomerService.GetCustomerByEmail("admin");
            if (customer != null)
            {
                AuthorizeService.AuthorizeTheUser(customer.EMail, customer.Password, true);
                Response.Redirect("~/admin/default.aspx");
            }
        }
    }
}