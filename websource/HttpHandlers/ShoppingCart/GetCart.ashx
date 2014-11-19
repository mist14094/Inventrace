<%@ WebHandler Language="C#" Class="GetCart" %>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Orders;
using Newtonsoft.Json;


public class GetCart : IHttpHandler, IRequiresSessionState
{
    protected float TotalPrice = 0;
    protected float DiscountOnTotalPrice = 0;
    protected float TotalDiscount = 0;
    protected float TotalItems = 0;
    
    public void ProcessRequest(HttpContext context)
    {
        AdvantShop.Localization.Culture.InitializeCulture();
        context.Response.ContentType = "application/json";
        var shpCart = ShoppingCartService.CurrentShoppingCart;

        var cartProducts = (from item in shpCart
                            select new
                            {
                                Price = CatalogService.GetStringPrice(item.Price),
                                item.Amount,
                                   SKU = item.Offer.ArtNo,
                                Photo = item.Offer.Photo != null
                                     ? String.Format("<img src='{0}' alt='{1}' class='img-cart' />", FoldersHelper.GetImageProductPath(ProductImageType.XSmall, item.Offer.Photo.PhotoName, false), item.Offer.Product.Name)
                                     : "<img src='images/nophoto_xsmall.jpg' alt='' class='img-cart' />",
                                item.Offer.Product.Name,
                                //TODO vladimir: разделить получение ссылки для social
                                Link = context.Request.UrlReferrer != null && context.Request.UrlReferrer.ToString().Contains("/social/")
                                    ? "social/detailssocial.aspx?productid=" + item.Offer.Product.ID
                                    : UrlService.GetLink(ParamType.Product, item.Offer.Product.UrlPath, item.Offer.Product.ID),
                                Cost = CatalogService.GetStringPrice(item.Price * item.Amount),
                                item.ShoppingCartItemId,
                                SelectedOptions = CatalogService.RenderSelectedOptions(item.AttributesXml),
                                ColorName = item.Offer.Color != null ? item.Offer.Color.ColorName : null,
                                SizeName = item.Offer.Size != null ? item.Offer.Size.SizeName : null,
                                Avalible = GetAvalible(item),
                                item.Offer.CanOrderByRequest,
                                MinAmount = item.Offer.Product.MinAmount ?? 1,
                                MaxAmount = item.Offer.Product.MaxAmount ?? Int32.MaxValue,
                                item.Offer.Product.Multiplicity
                            }).ToList();
                                    
        TotalPrice = shpCart.TotalPrice;
        DiscountOnTotalPrice = shpCart.DiscountPercentOnTotalPrice;
        TotalDiscount = shpCart.TotalDiscount;
        TotalItems = shpCart.TotalItems;
        
        var count = ItemsCount(shpCart);

        object objects = new
        {
            CartProducts = cartProducts,
            ColorHeader = SettingsCatalog.ColorsHeader,
            SizeHeader = SettingsCatalog.SizesHeader,
            TotalPrice = TotalPrice,
            TotalProductPrice = TotalPrice,
            DiscountOnTotalPrice = DiscountOnTotalPrice,
            TotalDiscount = TotalDiscount,
            Summary = GetSummary(shpCart),
            Count = count,
            CountNumber = TotalItems,
            Valid = Valid(context, shpCart),
            CouponInputVisible = shpCart.HasItems && shpCart.Coupon == null && shpCart.Certificate == null
                && CustomerSession.CurrentCustomer.CustomerGroup.CustomerGroupId == CustomerGroupService.DefaultCustomerGroup
        };

        context.Response.Write(JsonConvert.SerializeObject(objects));
    }

    public string Valid(HttpContext context, ShoppingCart shpCart)
    {
        var errorMessage = string.Empty;
        var itemsCount = TotalItems;

        if (itemsCount == 0)
        {
            errorMessage = Resources.Resource.Client_ShoppingCart_NoProducts;
        }

        if (TotalPrice < SettingsOrderConfirmation.MinimalOrderPrice)
        {
            errorMessage = string.Format(Resources.Resource.Client_ShoppingCart_MinimalOrderPrice,
                                         CatalogService.GetStringPrice(SettingsOrderConfirmation.MinimalOrderPrice),
                                         CatalogService.GetStringPrice(SettingsOrderConfirmation.MinimalOrderPrice - TotalPrice));
        }
        else if (shpCart.Any(item => GetAvalible(item).IsNotEmpty()))
        {
            errorMessage = string.Format(Resources.Resource.Client_ShoppingCart_NotAvailableProducts);
        }

        return errorMessage;
    }

    private string ItemsCount(ShoppingCart shpCart)
    {
        return string.Format("{0} {1}", TotalItems == 0 ? "" : TotalItems.ToString(CultureInfo.InvariantCulture), Strings.Numerals(TotalItems, Resources.Resource.Client_UserControls_ShoppingCart_Empty,
                                                  Resources.Resource.Client_UserControls_ShoppingCart_1Product,
                                                  Resources.Resource.Client_UserControls_ShoppingCart_2Products,
                                                  Resources.Resource.Client_UserControls_ShoppingCart_5Products));
    }

    private List<object> GetSummary(ShoppingCart shpCart)
    {
        var summary = new List<object>();

        if (TotalDiscount != 0)
        {
            summary.Add(new { Key = Resources.Resource.Client_UserControls_ShoppingCart_Sum, Value = CatalogService.GetStringPrice(TotalPrice) });
        }

        if (DiscountOnTotalPrice > 0)
        {
            summary.Add(
                new
                    {
                        Key = Resources.Resource.Client_UserControls_ShoppingCart_Discount,
                        Value = string.Format("<span class=\"discount\">{0}</span>",
                                  CatalogService.GetStringDiscountPercent(TotalPrice, DiscountOnTotalPrice, true))
                    });
        }

        if (shpCart.Certificate != null)
        {
            summary.Add(new { Key = Resources.Resource.Client_UserControls_ShoppingCart_Certificate, Value = string.Format("-{0}<a class=\"cross\" data-cart-remove-cert=\"true\" title=\"{1}\"></a>", CatalogService.GetStringPrice(shpCart.Certificate.Sum), Resources.Resource.Client_ShoppingCart_DeleteCertificate) });
        }

        if (shpCart.Coupon != null)
        {
            if (TotalDiscount == 0)
            {
                summary.Add(
                    new
                        {
                            Key = Resources.Resource.Client_UserControls_ShoppingCart_Coupon,
                            Value = string.Format("-{0} ({1}) <a class=\"cross\"  data-cart-remove-cupon=\"true\" title=\"{2}\"></a>",
                                      CatalogService.GetStringPrice(0), shpCart.Coupon.Code,
                                      Resources.Resource.Client_ShoppingCart_DeleteCoupon)
                        });
            }
            else
            {
                switch (shpCart.Coupon.Type)
                {
                    case CouponType.Fixed:
                        summary.Add(new
                                        {
                                            Key = Resources.Resource.Client_UserControls_ShoppingCart_Coupon,
                                            Value = string.Format("-{0} ({1}) <a class=\"cross\" data-cart-remove-cupon=\"true\" title=\"{2}\"></a>",
                                                      CatalogService.GetStringPrice(TotalDiscount), shpCart.Coupon.Code,
                                                      Resources.Resource.Client_ShoppingCart_DeleteCoupon)
                                        });
                        break;
                    case CouponType.Percent:
                        summary.Add(new
                                        {
                                            Key = Resources.Resource.Client_UserControls_ShoppingCart_Coupon,
                                            Value = string.Format("-{0} ({1}%) ({2}) <a class=\"cross\"  data-cart-remove-cupon=\"true\" title=\"{3}\"></a>",
                                                        CatalogService.GetStringPrice(TotalDiscount),
                                                        CatalogService.FormatPriceInvariant(shpCart.Coupon.Value),
                                                        shpCart.Coupon.Code, Resources.Resource.Client_ShoppingCart_DeleteCoupon)
                                        });
                        break;
                }
            }

        }

        summary.Add(new
                        {
                            Key = string.Format("<span class=\"sum-result\">{0}</span>",
                                      Resources.Resource.Client_UserControls_ShoppingCart_Total),
                            Value = string.Format("<span class=\"sum-result\">{0}</span>",
                                      CatalogService.GetStringPrice(TotalPrice - TotalDiscount > 0
                                                                        ? TotalPrice - TotalDiscount
                                                                        : 0))
                        });
        return summary;
    }


    private static string GetAvalible(ShoppingCartItem item)
    {
        if (!item.Offer.Product.Enabled || !item.Offer.Product.CategoryEnabled)
        {
            return Resources.Resource.Client_ShoppingCart_NotAvailable + " 0 " + item.Offer.Product.Unit;
        }

        if (item.Offer.CanOrderByRequest)
            return string.Empty;

        if ((SettingsOrderConfirmation.AmountLimitation) && (item.Amount > item.Offer.Amount))
        {
            return Resources.Resource.Client_ShoppingCart_NotAvailable + " " + item.Offer.Amount + " " + item.Offer.Product.Unit;
        }

        if (item.Amount > item.Offer.Product.MaxAmount)
        {
            return Resources.Resource.Client_ShoppingCart_NotAvailable_MaximumOrder + " " + +item.Offer.Product.MaxAmount + " " + item.Offer.Product.Unit;
        }

        if (item.Amount < item.Offer.Product.MinAmount)
        {
            return Resources.Resource.Client_ShoppingCart_NotAvailable_MinimumOrder + " " + +item.Offer.Product.MinAmount + " " + item.Offer.Product.Unit;
        }

        return string.Empty;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
