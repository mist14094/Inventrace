<%@ WebHandler Language="C#" Class="AddToWishlist" %>

//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Orders;


public class AddToWishlist : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        var offerId = 0;

        var valid = true;
        valid &= context.Request["offerid"].IsNotEmpty() && Int32.TryParse(context.Request["offerid"], out offerId);

        if (!valid)
        {
            ReturnResult(context, false);
        }

        var offer = OfferService.GetOffer(offerId);
        if (offer != null)
        {
            ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
                {
                    OfferId = offer.OfferId,
                    ShoppingCartType = ShoppingCartType.Wishlist,
                    AttributesXml = context.Request["customOptions"],
                });
        }
        else
        {
            ReturnResult(context, false);
        }
        
        ReturnResult(context, ShoppingCartService.CurrentWishlist.Any(item => item.Offer.OfferId == offerId &&
                                                        item.AttributesXml == context.Request["customOptions"]));

    }

    private static void ReturnResult(HttpContext context, bool result)
    {
        context.Response.ContentType = "application/json";
        context.Response.Write(result ? Newtonsoft.Json.JsonConvert.True : Newtonsoft.Json.JsonConvert.False);
        context.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
