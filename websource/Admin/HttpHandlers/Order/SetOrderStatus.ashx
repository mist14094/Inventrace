<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.Order.SetOrderStatus" %>

using System;
using System.Web;
using AdvantShop;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Modules;
using AdvantShop.Orders;

namespace Admin.HttpHandlers.Order
{
    public class SetOrderStatus : AdminHandler, IHttpHandler
    {
        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
                return;

            var statusId = 0;
            var orderId = 0;

            if (!Int32.TryParse(context.Request["statusid"], out statusId) ||
                !Int32.TryParse(context.Request["orderid"], out orderId))
            {
                ReturnResult(context, "error");
            }
            var status = OrderService.GetOrderStatus(statusId);
            if (status != null)
            {
                OrderService.ChangeOrderStatus(orderId, statusId);
                ModulesRenderer.OrderChangeStatus(orderId);
                
                Trial.TrackEvent(Trial.TrialEvents.ChangeOrderStatus, "");
                ReturnResult(context, status.Color);
            }

            ReturnResult(context, "error");
        }

        private static void ReturnResult(HttpContext context, string result)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new {result}));
            context.Response.End();
        }

    }
}
