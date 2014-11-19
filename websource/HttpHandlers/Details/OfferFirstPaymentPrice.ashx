<%@ WebHandler Language="C#" Class="OfferFirstPaymentPrice" %>

using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using Newtonsoft.Json;

public class OfferFirstPaymentPrice : IHttpHandler 
{
    public void ProcessRequest (HttpContext context) 
    {        
        context.Response.ContentType = "application/json";

        var price = context.Request["price"].Replace(".", ",").TryParseFloat();
        var discount = context.Request["discount"].Replace(".", ",").TryParseFloat();
        var attributesXml = HttpUtility.UrlDecode(context.Request["AttributesXml"]);
        var firstPaymentPercent = context.Request["firstPaymentPercent"].Replace(".", ",").TryParseFloat();

        float discountByTime = DiscountByTimeService.GetDiscountByTime();

        var priceHtml = CatalogService.RenderPrice(price * firstPaymentPercent / 100, (discount == 0 ? discountByTime : discount), 
                                                     true, CustomerSession.CurrentCustomer.CustomerGroup, attributesXml);

        context.Response.Write(JsonConvert.SerializeObject(new { Price = priceHtml }));
    }
 
    public bool IsReusable 
    {
        get { return false; }
    }
}