//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using System.Collections.Generic;
using AdvantShop.Orders;

public partial class UserControls_BuyInOneClick : System.Web.UI.UserControl
{
    public int ProductId;
    public List<CustomOption> CustomOptions;
    public List<OptionItem> SelectedOptions;
    public bool isShoppingCart = false;
    public OrderConfirmationService.BuyInOneclickPage pageEnum;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!isShoppingCart)
        {
            hfProductId.Value = ProductId.ToString();
            txtPhone.Text = CustomerSession.CurrentCustomer.Id != CustomerService.InternetUser.Id
                                ? CustomerSession.CurrentCustomer.Phone
                                : string.Empty;
            txtName.Text = CustomerSession.CurrentCustomer.Id != CustomerService.InternetUser.Id
                                ? CustomerSession.CurrentCustomer.FirstName + " " + CustomerSession.CurrentCustomer.LastName
                                : string.Empty;
        }

        if (Request.Url.AbsolutePath.ToLower().Contains("details"))
        {
            pageEnum = OrderConfirmationService.BuyInOneclickPage.details;
        }
        else if (Request.Url.AbsolutePath.ToLower().Contains("shoppingcart"))
        {
            pageEnum = OrderConfirmationService.BuyInOneclickPage.shoppingcart;
        }

    }
}