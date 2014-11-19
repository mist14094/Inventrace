<%@ WebHandler Language="C#" Class="ChangeOrderStatus" %>
using System.Web;
using System.Web.SessionState;
using AdvantShop.Orders;
using Newtonsoft.Json;
using AdvantShop;

public class ChangeOrderStatus : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        
        if (!AdvantShop.Customers.CustomerSession.CurrentCustomer.RegistredUser || context.Request["ordernumber"].IsNullOrEmpty())
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }

        var order = OrderService.GetOrderByNumber(context.Request["ordernumber"]);
        var customer = OrderService.GetOrderCustomer(context.Request["ordernumber"]);

        if (order == null || customer == null || customer.CustomerID != AdvantShop.Customers.CustomerSession.CurrentCustomer.Id)
        {
            context.Response.Write(JsonConvert.SerializeObject(string.Empty));
            return;
        }
        
        OrderService.CancelOrder(order.OrderID);
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
