<%@ WebHandler Language="C#" Class="HttpHandlers.Details.BuyInOneClickHandler" %>

using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.Mails;
using AdvantShop.Modules;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace HttpHandlers.Details
{
    public class BuyInOneClickHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            var amount = 0;
            var productId = 0;
            var page = OrderConfirmationService.BuyInOneclickPage.details;

            var valid = true;
            valid &= context.Request["page"].IsNotEmpty() && Enum.TryParse(context.Request["page"], true, out page);
            valid &= context.Request["productId"].IsNotEmpty() &&
                     Int32.TryParse(context.Request["productId"], out productId);
            if (page == OrderConfirmationService.BuyInOneclickPage.details)
            {
                valid &= context.Request["amount"].IsNotEmpty() && Int32.TryParse(context.Request["amount"], out amount);
            }
            valid &= context.Request["name"].IsNotEmpty();
            valid &= context.Request["phone"].IsNotEmpty();

            if (!valid)
            {
                ReturnResult(context, "Неверно заполнены данные", true);
            }

            var OrderItems = new List<OrderItem>();
            float DiscountPercentOnTotalPrice = 0;
            float totalPrice = 0;

            OrderCertificate orderCertificate = null;
            OrderCoupon orderCoupon = null;

            if (page == OrderConfirmationService.BuyInOneclickPage.details)
            {
                Offer offer;

                if (context.Request["offerid"].IsNullOrEmpty())
                {
                    var p = ProductService.GetProduct(context.Request["productid"].TryParseInt());
                    if (p == null || p.Offers.Count == 0)
                    {
                        ReturnResult(context, Resources.Resource.Client_ShoppingCart_Error, true);
                        return;
                    }
                    offer = p.Offers.First();
                }
                else
                {
                    offer = OfferService.GetOffer(context.Request["offerid"].TryParseInt());
                }

                IList<EvaluatedCustomOptions> listOptions = null;
                string selectedOptions = HttpUtility.UrlDecode(context.Request["customOptions"]);
                float customOptionsPrice = 0;

                if (selectedOptions.IsNotEmpty())
                {
                    try
                    {
                        listOptions = CustomOptionsService.DeserializeFromXml(selectedOptions);
                    }
                    catch (Exception)
                    {
                        listOptions = null;
                    }

                    foreach (var listOption in listOptions)
                    {
                        if (listOption.OptionPriceType == OptionPriceType.Fixed)
                        {
                            customOptionsPrice += listOption.OptionPriceBc;
                        }
                        else if (listOption.OptionPriceType == OptionPriceType.Percent)
                        {
                            customOptionsPrice += offer.Price * listOption.OptionPriceBc / 100;
                        }
                    }
                }

                DiscountPercentOnTotalPrice = OrderService.GetDiscount((offer.Price + customOptionsPrice) * amount);
                totalPrice = (offer.Price - (offer.Price * offer.Product.Discount / 100) + customOptionsPrice) * amount;

                if (totalPrice < AdvantShop.Configuration.SettingsOrderConfirmation.MinimalOrderPrice)
                {
                    ReturnResult(context, string.Format(Resources.Resource.Client_ShoppingCart_MinimalOrderPrice,
                                                        CatalogService.GetStringPrice(
                                                            AdvantShop.Configuration.SettingsOrderConfirmation
                                                                      .MinimalOrderPrice),
                                                        CatalogService.GetStringPrice(
                                                            AdvantShop.Configuration.SettingsOrderConfirmation
                                                                      .MinimalOrderPrice - totalPrice)), true);
                    return;
                }
                OrderItems = new List<OrderItem>
                    {
                        new OrderItem
                            {
                                ProductID = offer.ProductId,
                                Name = offer.Product.Name,
                                ArtNo = offer.ArtNo,
                                Price = offer.Price - (offer.Price * offer.Product.CalculableDiscount / 100) + customOptionsPrice,
                                Amount = amount,
                                SupplyPrice = offer.SupplyPrice,
                                SelectedOptions = listOptions,
                                Weight = offer.Product.Weight,
                                Color = offer.Color != null ? offer.Color.ColorName : string.Empty,
                                Size = offer.Size != null ? offer.Size.SizeName : string.Empty,
                                PhotoID = offer.Photo != null ? offer.Photo.PhotoId : (int?) null
                            }
                    };
            }
            else if (page == OrderConfirmationService.BuyInOneclickPage.shoppingcart)
            {
                var shoppingCart = ShoppingCartService.CurrentShoppingCart;
                DiscountPercentOnTotalPrice = shoppingCart.DiscountPercentOnTotalPrice;
                totalPrice = shoppingCart.TotalPrice;

                if (totalPrice < AdvantShop.Configuration.SettingsOrderConfirmation.MinimalOrderPrice)
                {
                    ReturnResult(context, string.Format(Resources.Resource.Client_ShoppingCart_MinimalOrderPrice,
                                                        CatalogService.GetStringPrice(
                                                            AdvantShop.Configuration.SettingsOrderConfirmation
                                                                      .MinimalOrderPrice),
                                                        CatalogService.GetStringPrice(
                                                            AdvantShop.Configuration.SettingsOrderConfirmation
                                                                      .MinimalOrderPrice - totalPrice)), true);
                    return;
                }

                foreach (var orderItem in shoppingCart.Select(item => (OrderItem)item))
                {
                    OrderItems.Add(orderItem);
                }

                GiftCertificate certificate = shoppingCart.Certificate;
                Coupon coupon = shoppingCart.Coupon;

                if (certificate != null)
                {
                    orderCertificate = new OrderCertificate()
                        {
                            Code = certificate.CertificateCode,
                            Price = certificate.Sum
                        };
                }
                if (coupon != null && shoppingCart.TotalPrice >= coupon.MinimalOrderPrice)
                {
                    orderCoupon = new OrderCoupon()
                        {
                            Code = coupon.Code,
                            Type = coupon.Type,
                            Value = coupon.Value
                        };
                }

            }

            var orderContact = new OrderContact
                {
                    Address = string.Empty,
                    City = string.Empty,
                    Country = string.Empty,
                    Name = string.Empty,
                    Zip = string.Empty,
                    Zone = string.Empty
                };

            var customerGroup = CustomerSession.CurrentCustomer.CustomerGroup;
            var order = new Order
                {
                    CustomerComment = context.Request["comment"],
                    OrderDate = DateTime.Now,
                    OrderCurrency = new OrderCurrency
                        {
                            CurrencyCode = CurrencyService.CurrentCurrency.Iso3,
                            CurrencyNumCode = CurrencyService.CurrentCurrency.NumIso3,
                            CurrencySymbol = CurrencyService.CurrentCurrency.Symbol,
                            CurrencyValue = CurrencyService.CurrentCurrency.Value,
                            IsCodeBefore = CurrencyService.CurrentCurrency.IsCodeBefore
                        },
                    OrderCustomer = new OrderCustomer
                        {
                            CustomerID = CustomerSession.CurrentCustomer.Id,
                            Email = CustomerSession.CurrentCustomer.EMail,
                            FirstName = context.Request["name"],
                            LastName = string.Empty,
                            MobilePhone = context.Request["phone"],
                            CustomerIP = HttpContext.Current.Request.UserHostAddress
                        },

                    OrderStatusId = OrderService.DefaultOrderStatus,
                    AffiliateID = 0,
                    GroupName = customerGroup.GroupName,
                    GroupDiscount = customerGroup.GroupDiscount,


                    OrderItems = OrderItems,
                    OrderDiscount = DiscountPercentOnTotalPrice,
                    Number = OrderService.GenerateNumber(1),
                    ShippingContact = orderContact,
                    BillingContact = orderContact,
                    Certificate = orderCertificate,
                    Coupon = orderCoupon,
                    AdminOrderComment = Resources.Resource.Client_BuyInOneClick_Header
                };

            order.OrderID = OrderService.AddOrder(order);
            order.Number = OrderService.GenerateNumber(order.OrderID);
            OrderService.UpdateNumber(order.OrderID, order.Number);
            OrderService.ChangeOrderStatus(order.OrderID, OrderService.DefaultOrderStatus);

            if (order.OrderID != 0)
            {
                try
                {
                    string message = SendMail.BuildMail(new ClsMailParamOnBuyInOneClick
                        {
                            Name = HttpUtility.HtmlEncode(context.Request["name"]),
                            Phone = HttpUtility.HtmlEncode(context.Request["phone"]),
                            Comment = HttpUtility.HtmlEncode(context.Request["comment"]),
                            OrderTable = OrderService.GenerateHtmlOrderTable(
                                order.OrderItems,
                                CurrencyService.CurrentCurrency,
                                totalPrice,
                                DiscountPercentOnTotalPrice,
                                orderCoupon,
                                orderCertificate,
                                DiscountPercentOnTotalPrice > 0 ? DiscountPercentOnTotalPrice * totalPrice / 100 : 0,
                                0,
                                0,
                                0,
                                CustomerSession.CurrentCustomer.Contacts.Count > 0
                                    ? CustomerSession.CurrentCustomer.Contacts[0]
                                    : new CustomerContact(),
                                CustomerSession.CurrentCustomer.Contacts.Count > 0
                                    ? CustomerSession.CurrentCustomer.Contacts[0]
                                    : new CustomerContact()),
                        });

                    SendMail.SendMailNow(AdvantShop.Configuration.SettingsMail.EmailForOrders,
                                         AdvantShop.Configuration.SettingsMain.SiteUrl + " - " +
                                         Resources.Resource.Client_BuyInOneClick_Header, message, true);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }

            ModulesRenderer.OrderAdded(order.OrderID);

            if (order.OrderID != 0 && page == OrderConfirmationService.BuyInOneclickPage.shoppingcart)
            {
                ShoppingCartService.ClearShoppingCart(ShoppingCartType.ShoppingCart, CustomerSession.CustomerId);
                ReturnResult(context, "reload", false);
                return;
            }

        }

        private static void ReturnResult(HttpContext context, string result, bool error)
        {
            context.Response.ContentType = "application/json";
            if (error)
            {
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { type = "error", result }));
            }
            else
            {
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new { result }));
            }
            context.Response.End();
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }

}