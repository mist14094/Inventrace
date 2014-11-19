using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;

namespace ClientPages
{
    public partial class CompareProducts : AdvantShopClientPage
    {
        protected List<ProductItem> ProductItems = new List<ProductItem>();
        protected List<string> PropertyNames = new List<string>();

        protected CustomerGroup customerGroup = CustomerSession.CurrentCustomer.CustomerGroup;
        protected float DiscountByTime = DiscountByTimeService.GetDiscountByTime();

        protected void Page_Load(object sender, EventArgs e)
        {
            Logo.ImgSource = FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, false);

            var compareProducts = ShoppingCartService.CurrentCompare;
            if ((compareProducts == null) || (compareProducts.Count == 0))
            {
                Response.Redirect("~/");
                return;
            }

            var propertyNames = new List<string>();
            foreach (var item in compareProducts)
            {
                propertyNames.AddRange(
                    PropertyService.GetPropertyValuesByProductId(item.Offer.ProductId).Select(p => p.Property.Name));
            }

            PropertyNames = new List<string>();
            PropertyNames.AddRange(propertyNames.Distinct());

            ProductItems = new List<ProductItem>();
            foreach (ShoppingCartItem item in compareProducts)
            {
                Product product = ProductService.GetProduct(item.Offer.ProductId);
                if (product == null) continue;
                ProductItems.Add(new ProductItem(item.OfferId, product, PropertyNames));
            }
        }

        protected string RenderPictureTag(ProductItem item)
        {
            string strFormat = "";
            string strResult = "";

            if (string.IsNullOrEmpty(item.Photo))
            {
                strFormat =
                    "<div><div onclick=\"location='{0}'\" style=\"cursor:pointer;position:relative;height:90px;width:120px\" title=\"Фотография не доступна\"><img border=\"0\" src=\"images/nophoto_small.jpg\" alt=\"Фотография не доступна\" /></div></div><br/>";
                strResult = string.Format(strFormat, UrlService.GetLinkDB(ParamType.Product, item.ProductId));
            }
            else
            {
                strFormat =
                    "<div><div onclick=\"location='{4}'\" style=\"cursor:pointer;position:relative;height:90px;width:120px\" title=\"{0}\"><img class=\"imgPhoto\" border=\"0\" src=\"{1}\" alt=\"{0}\" /></div></div><br/>";
                strResult = string.Format(strFormat, Server.HtmlEncode(item.Name),
                                          FoldersHelper.GetImageProductPath(ProductImageType.Small, item.Photo, false),
                                          item.Photo, "", UrlService.GetLinkDB(ParamType.Product, item.ProductId));

            }
            return strResult;
        }

        protected string RenderPriceTag(float price, float discount)
        {
            float productDiscount = discount != 0 ? discount : DiscountByTime;
            return CatalogService.RenderPrice(price, productDiscount, false, customerGroup);
        }

        protected string GetProductLink(ProductItem item)
        {
            return UrlService.GetLinkDB(ParamType.Product, item.ProductId);
        }

        protected void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            int offerId = 0;
            Int32.TryParse(hiddenOfferID.Value, out offerId);

            if (offerId != 0)
            {
                ShoppingCart compareProducts = ShoppingCartService.CurrentCompare;
                ShoppingCartItem deleteItem = compareProducts.FirstOrDefault(p => p.OfferId == offerId);

                if (deleteItem != null)
                {
                    ShoppingCartService.DeleteShoppingCartItem(deleteItem.ShoppingCartItemId);
                    compareProducts.RemoveAll(p => p.OfferId == offerId);

                    if (compareProducts.Count == 0)
                    {
                        CommonHelper.RegCloseScript(this, string.Empty);
                    }
                }
            }
        }

        protected void btnBuyProduct_Click(object sender, EventArgs e)
        {
            try
            {
                int offerID = 0;
                Int32.TryParse(hiddenOfferID.Value, out offerID);

                var offer = OfferService.GetOffer(offerID);
                if (offer == null)
                    return;

                

                if (CustomOptionsService.DoesProductHaveRequiredCustomOptions(offer.ProductId))
                {
                    Response.Redirect(GetProductLink(ProductItems.First(p => p.ProductId == offer.ProductId)));
                }


                List<CustomOption> customOptions =
                    CustomOptionsService.GetCustomOptionsByProductId(SQLDataHelper.GetInt(offer.ProductId));
                if ((customOptions != null) && (customOptions.Count != 0))
                {
                    Page.Response.Redirect(UrlService.GetLinkDB(ParamType.Product, offer.ProductId));
                }

                ShoppingCartService.AddShoppingCartItem(new ShoppingCartItem
                    {
                        OfferId = offerID,
                        Amount = 1,
                        ShoppingCartType = ShoppingCartType.ShoppingCart,
                        AttributesXml = string.Empty,
                    });
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            Response.Redirect("~/shoppingcart.aspx");
        }
    }

    public class ProductItem
    {

         

        public ProductItem(int offerId, Product product, IEnumerable<string> propertyNames)
        {
            OfferId = offerId;
            ProductId = product.ProductId;
            CategoryId = ProductService.GetFirstCategoryIdByProductId(ProductId);
            Name = product.Name;
            ArtNo = product.ArtNo;
            Photo = product.Photo;
            UrlPath = product.UrlPath;
            var firstOrDefault = product.Offers.FirstOrDefault();
            if (firstOrDefault != null)
            {
                Price = firstOrDefault.Price;
                Amount = firstOrDefault.Amount;
            }
            Discount = product.Discount;
            AllowPreorder = product.AllowPreOrder;

            Properties = new List<ProductProperty>();

            var properties = PropertyService.GetPropertyValuesByProductId(product.ProductId);
            foreach (var propertyName in propertyNames)
            {
                var list = properties.Where(p => p.Property.Name == propertyName);
                if (list.Any())
                {
                    foreach (var propertyValue in list)
                    {
                        Properties.Add(new ProductProperty
                            {
                                PropertyId = propertyValue.PropertyId,
                                Name = propertyName,
                                Value = propertyValue.Value
                            });
                    }
                }
                else
                {
                    Properties.Add(new ProductProperty
                        {
                            Name = propertyName,
                            Value = " - "
                        });
                }
            }
        }


        public int OfferId { get; set; }
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string ArtNo { get; set; }
        public string Photo { get; set; }
        public float Price { get; set; }
        public float Discount { get; set; }
        public float Amount { get; set; }
        public bool AllowPreorder { get; set; }
        public string UrlPath { get; set; }
        public List<ProductProperty> Properties { get; set; }
    }
}