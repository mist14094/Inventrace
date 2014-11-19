<%@ WebHandler Language="C#" Class="ColorSizePrice" %>

using System.Web;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using Newtonsoft.Json;

public class ColorSizePrice : IHttpHandler 
{
    public void ProcessRequest (HttpContext context) 
    {        
        context.Response.ContentType = "application/json";

        var price = context.Request["price"].Replace(".", ",").TryParseFloat();
        var discount = context.Request["discount"].Replace(".", ",").TryParseFloat();
        var attributesXml = HttpUtility.UrlDecode(context.Request["AttributesXml"]);

        float discountByTime = DiscountByTimeService.GetDiscountByTime();
        
        var priceHtml = CatalogService.RenderPrice(price, (discount == 0 ? discountByTime : discount), 
                                                     true, CustomerSession.CurrentCustomer.CustomerGroup, attributesXml);

        context.Response.Write(JsonConvert.SerializeObject(new { Price = priceHtml }));
    }
 
    public bool IsReusable 
    {
        get { return false; }
    }
}