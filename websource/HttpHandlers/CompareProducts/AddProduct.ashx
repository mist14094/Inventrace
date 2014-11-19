<%@ WebHandler Language="C#" Class="AddProduct" %>

using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Orders;
using Newtonsoft.Json;

public class AddProduct : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        if (string.IsNullOrEmpty(context.Request["offerid"]))
        {
            ReturnValue(context, false);
        }

        int offerId = context.Request["offerid"].TryParseInt();
        var offer = OfferService.GetOffer(offerId);
        if (offer != null)
        {
            
            ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
                {
                    OfferId = offer.OfferId,
                    Amount = 0,
                    ShoppingCartType = ShoppingCartType.Compare
                });

            ReturnValue(context, true);
            return;
        }

        ReturnValue(context, false);
    }

    private static void ReturnValue(HttpContext context, bool result)
    {
        context.Response.ContentType = "application/json";
        context.Response.Write(result ? JsonConvert.True : JsonConvert.False);
        context.Response.End();
    }

    public bool IsReusable
    {
        get { return true; }
    }
}