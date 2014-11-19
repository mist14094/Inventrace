//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;
using AdvantShop.SEO;
using Resources;

namespace ClientPages
{
    public partial class ShoppingCart_Page : AdvantShopClientPage
    {
        protected CustomerGroup customerGroup;

        protected void Page_Load(object sender, EventArgs e)
        {
            customerGroup = CustomerSession.CurrentCustomer.CustomerGroup;
            lDemoWarning.Visible = Demo.IsDemoEnabled || Trial.IsTrialEnabled;

            BuyInOneClick.Visible = SettingsOrderConfirmation.BuyInOneClick;

            if (!IsPostBack)
            {
                if (Request["productid"].IsNotEmpty())
                {
                    int productId = Request["productid"].TryParseInt();
                    int amount = Request["amount"].TryParseInt(1);
                    if (productId != 0 && ProductService.IsProductEnabled(productId))
                    {
                        IList<EvaluatedCustomOptions> listOptions = null;
                        string selectedOptions = HttpUtility.UrlDecode(Request["AttributesXml"]);
                        try
                        {
                            listOptions = CustomOptionsService.DeserializeFromXml(selectedOptions);
                        }
                        catch (Exception)
                        {
                            listOptions = null;
                        }

                        if (CustomOptionsService.DoesProductHaveRequiredCustomOptions(productId) && listOptions == null)
                        {
                            Response.Redirect(SettingsMain.SiteUrl + UrlService.GetLinkDB(ParamType.Product, productId));
                            return;
                        }

                        ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
                            {
                                OfferId = ProductService.GetProduct(productId).Offers[0].OfferId,
                                Amount = amount,
                                ShoppingCartType = ShoppingCartType.ShoppingCart,
                                AttributesXml = listOptions != null ? selectedOptions : string.Empty,
                            });

                        Response.Redirect("shoppingcart.aspx");
                    }
                }

                UpdateBasket();
                SetMeta(
                    new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName,
                                               Resource.Client_ShoppingCart_ShoppingCart)), string.Empty);
            }



            //подключение юзерконтрорла модуля
            foreach (var module in AttachedModules.GetModules(AttachedModules.EModuleType.RenderIntoShoppingCart).ToList())
            {
                var moduleObject = (IRenderIntoShoppingCart)Activator.CreateInstance(module, null);

                if (!ModulesRepository.IsInstallModule(moduleObject.ModuleStringId) || !moduleObject.Active())
                {
                    return;
                }

                ltrlBottomContent.Text = moduleObject.DoRenderToBottom();
                ltrlTopContent.Text = moduleObject.DoRenderToTop();

                if (!string.IsNullOrEmpty(moduleObject.ClientSideControlNameBottom))
                {
                    var userControl =
                        (this).LoadControl(moduleObject.ClientSideControlNameBottom);

                    if (userControl != null)
                    {
                        ((IUserControlinSc)userControl).ProductIds =
                            ShoppingCartService.CurrentShoppingCart.Select(p => p.Offer.ProductId).ToList();
                        pnlBottomContent.Controls.Add(userControl);
                    }
                }
                if (!string.IsNullOrEmpty(moduleObject.ClientSideControlNameTop))
                {
                    var userControl =
                        (this).LoadControl(moduleObject.ClientSideControlNameTop);

                    if (userControl != null)
                    {
                        ((IUserControlinSc)userControl).ProductIds =
                            ShoppingCartService.CurrentShoppingCart.Select(p => p.Offer.ProductId).ToList();
                        pnlTopContent.Controls.Add(userControl);
                    }
                }
            }
        }

        public void UpdateBasket()
        {
            var shpCart = ShoppingCartService.CurrentShoppingCart;

            if (shpCart.HasItems)
            {
                lblEmpty.Visible = false;
            }
            else
            {
                dvOrderMerged.Visible = false;
                BuyInOneClick.Visible = false;
                lblEmpty.Visible = true;
            }
        }


        protected void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            var shoppingCart = ShoppingCartService.CurrentShoppingCart;

            if (!shoppingCart.CanOrder)
            {
                UpdateBasket();
            }
            else
            {
                Response.Redirect("orderconfirmation.aspx");
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            UpdateBasket();
        }
    }
}