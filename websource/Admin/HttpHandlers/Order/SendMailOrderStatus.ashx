<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.Order.SendMailOrderStatus" %>

using System;
using System.Web;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Mails;
using AdvantShop.Orders;
using Resources;

namespace Admin.HttpHandlers.Order
{
    public class SendMailOrderStatus : AdminHandler, IHttpHandler
    {
        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
                return;

            var orderId = 0;

            if (!Int32.TryParse(context.Request["orderid"], out orderId))
            {
                ReturnResult(context, "error");
            }

            var order = OrderService.GetOrder(orderId);
            if (order != null)
            {
                SendMail.SendMailNow(
                    order.OrderCustomer.Email,
                    Resource.Admin_ViewOrder_OrderhasbeenChanged,
                    SendMail.BuildMail(new ClsMailParamOnChangeOrderStatus
                    {
                        OrderID = orderId.ToString(),
                        OrderStatus = order.OrderStatus.StatusName,
                        StatusComment = order.StatusComment.Replace("\r\n", "<br />"),
                        Number = order.Number
                    }),
                    true);

                ReturnResult(context, string.Empty);
            }

            ReturnResult(context, "error");
        }

        private static void ReturnResult(HttpContext context, string result)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { result }));
            context.Response.End();
        }

    }
}