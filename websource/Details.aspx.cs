//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using AdvantShop.Orders;
using AdvantShop.Security;
using AdvantShop.SEO;
using Resources;
using AdvantShop.Payment;

namespace ClientPages
{
    public partial class Details : AdvantShopClientPage
    {
        protected Product CurrentProduct;
        protected Offer CurrentOffer;
        protected Brand CurrentBrand;

        protected bool ExistInWishlist;
        protected int ProductId
        {
            get { return Request["productid"].TryParseInt(); }

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ProductId == 0)
            {
                Error404();
                return;
            }

            //if not have category
            if (ProductService.GetCountOfCategoriesByProductId(ProductId) == 0)
            {
                Error404();
                return;
            }

            // --- Check product exist ------------------------
            CurrentProduct = ProductService.GetProduct(ProductId);
            if (CurrentProduct == null || CurrentProduct.Enabled == false || CurrentProduct.CategoryEnabled == false)
            {
                Error404();
                return;
            }

            int? colorid = Request["color"].TryParseInt(true);
            int? sizeId = Request["size"].TryParseInt(true);


            CurrentOffer = CurrentProduct.Offers.FirstOrDefault(o => o.ColorID == colorid && o.SizeID == sizeId) ?? CurrentProduct.Offers.FirstOrDefault(o => o.Main) ?? CurrentProduct.Offers.FirstOrDefault();

            btnAdd.Text = SettingsCatalog.BuyButtonText;
            btnOrderByRequest.Text = SettingsCatalog.PreOrderButtonText;

            BuyInOneClick.ProductId = CurrentProduct.ProductId;
            BuyInOneClick.SelectedOptions = productCustomOptions.SelectedOptions;
            BuyInOneClick.CustomOptions = productCustomOptions.CustomOptions;

            if (CurrentProduct.TotalAmount <= 0 || CurrentProduct.MainPrice == 0)
            {
                divAmount.Visible = false;
            }

            CompareControl.Visible = SettingsCatalog.EnableCompareProducts;
            CompareControl.ProductId = ProductId;


            if (CurrentOffer != null)
            {
                CompareControl.OfferId = CurrentOffer.OfferId;
                CompareControl.IsSelected =
                    ShoppingCartService.CurrentCompare.Any(p => p.Offer.OfferId == CurrentOffer.OfferId);
            }
            else
            {
                CompareControl.Visible = false;
                pnlPrice.Visible = false;
            }

            sizeColorPicker.ProductId = ProductId;

            divUnit.Visible = CurrentProduct.Unit.IsNotEmpty();

            sbShareButtons.Visible = SettingsDesign.EnableSocialShareButtons;

            rating.ProductId = CurrentProduct.ID;
            rating.Rating = CurrentProduct.Ratio;
            rating.ShowRating = SettingsCatalog.EnableProductRating;
            rating.ReadOnly = RatingService.DoesUserVote(ProductId, CustomerSession.CustomerId);

            pnlWishlist.Visible = SettingsDesign.WishListVisibility && CurrentOffer != null;

            pnlSize.Visible = !string.IsNullOrEmpty(CurrentProduct.Size) && (CurrentProduct.Size != "0|0|0");
            pnlWeight.Visible = CurrentProduct.Weight != 0;

            CurrentBrand = CurrentProduct.Brand;
            pnlBrand.Visible = CurrentBrand != null && CurrentBrand.Enabled;
            pnlBrnadLogo.Visible = CurrentBrand != null && CurrentBrand.Enabled && CurrentBrand.BrandLogo != null;

            productPropertiesView.ProductId = ProductId;
            productPhotoView.Product = CurrentProduct;
            ProductVideoView.ProductID = ProductId;
            relatedProducts.ProductIds.Add(ProductId);
            alternativeProducts.ProductIds.Add(ProductId);
            breadCrumbs.Items =
                CategoryService.GetParentCategories(CurrentProduct.CategoryID).Reverse().Select(cat => new BreadCrumbs
                    {
                        Name = cat.Name,
                        Url = UrlService.GetLink(ParamType.Category, cat.UrlPath, cat.ID)
                    }).ToList();
            breadCrumbs.Items.Insert(0, new BreadCrumbs
                {
                    Name = Resource.Client_MasterPage_MainPage,
                    Url = UrlService.GetAbsoluteLink("/")
                });

            breadCrumbs.Items.Add(new BreadCrumbs { Name = CurrentProduct.Name, Url = null });

            RecentlyViewService.SetRecentlyView(CustomerSession.CustomerId, ProductId);

            var listDetailsTabs = new System.Collections.Generic.List<ITab>();

            foreach (var detailsTabsModule in AttachedModules.GetModules(AttachedModules.EModuleType.ProductTabs))
            {
                var classInstance = (IProductTabs)Activator.CreateInstance(detailsTabsModule, null);
                if (ModulesRepository.IsInstallModule(classInstance.ModuleStringId) && classInstance.CheckAlive())
                {
                    listDetailsTabs.AddRange(classInstance.GetProductDetailsTabsCollection(CurrentProduct.ProductId));
                }
            }

            lvTabsBodies.DataSource = listDetailsTabs;
            lvTabsTitles.DataSource = listDetailsTabs;

            lvTabsBodies.DataBind();
            lvTabsTitles.DataBind();


            CurrentProduct.Meta = SetMeta(CurrentProduct.Meta, CurrentProduct.Name,
                    CategoryService.GetCategory(CurrentProduct.CategoryID).Name,
                    CurrentProduct.Brand != null ? CurrentProduct.Brand.Name : string.Empty);

            if (SettingsSEO.ProductAdditionalDescription.IsNotEmpty())
            {
                liAdditionalDescription.Text =
                    GlobalStringVariableService.TranslateExpression(
                        SettingsSEO.ProductAdditionalDescription, MetaType.Product, CurrentProduct.Name,
                        CategoryService.GetCategory(CurrentProduct.CategoryID).Name,
                        CurrentProduct.Brand != null ? CurrentProduct.Brand.Name : string.Empty);
            }
            productReviews.EntityType = EntityType.Product;
            productReviews.EntityId = ProductId;

            ltrlRightColumnModules.Text = ModulesRenderer.RenderDetailsModulesToRightColumn();

            int reviewsCount = SettingsCatalog.ModerateReviews
                                   ? ReviewService.GetCheckedReviewsCount(ProductId, EntityType.Product)
                                   : ReviewService.GetReviewsCount(ProductId, EntityType.Product);
            if (reviewsCount > 0)
            {
                lReviewsCount.Text = string.Format("({0})", reviewsCount);
            }
        }

        private void GetOffer()
        {
            if (CurrentOffer != null)
            {
               var isAvailable = CurrentOffer.Amount > 0;
                lAvailiableAmount.Text = string.Format(
                    "<span id='availability' class='{0}'>{1}{2}</span>",
                    isAvailable ? "available" : "not-available",
                    isAvailable ? Resource.Client_Details_Available : Resource.Client_Details_NotAvailable,
                    isAvailable && SettingsCatalog.ShowStockAvailability
                    ? string.Format(" <span>({0}{1})</span>", CurrentOffer.Amount, CurrentProduct.Unit.IsNotEmpty() ? " " + CurrentProduct.Unit : string.Empty)
                    : string.Empty);

                ExistInWishlist = ShoppingCartService.CurrentWishlist.Any(
                    item => item.OfferId == CurrentOffer.OfferId && 
                        item.AttributesXml == CustomOptionsService.SerializeToXml(productCustomOptions.CustomOptions,
                                productCustomOptions.SelectedOptions));

                btnOrderByRequest.Visible = (CurrentOffer.Amount <= 0 || CurrentOffer.Price == 0) && CurrentProduct.AllowPreOrder;
                btnOrderByRequest.Attributes["data-offerid"] = CurrentOffer.OfferId.ToString();
            }
            else
            {
                lAvailiableAmount.Text = Resource.Client_Details_NotAvailable;
            }


            if (CurrentProduct.Offers.Count == 1)
            {
                if (CurrentOffer != null)
                {
                    btnOrderByRequest.Visible = (CurrentOffer.Amount <= 0 || CurrentOffer.Price == 0) && CurrentProduct.AllowPreOrder;
                    btnOrderByRequest.Attributes["data-offerid"] = CurrentOffer.OfferId.ToString();
                }

                btnAdd.Visible = CurrentProduct.MainPrice > 0 && CurrentProduct.TotalAmount > 0;
                btnAdd.Attributes["data-cart-add-productid"] = ProductId.ToString();

                BuyInOneClick.Visible = SettingsOrderConfirmation.BuyInOneClick && CurrentProduct.MainPrice > 0 &&
                                        CurrentProduct.TotalAmount > 0;

                ShowCreditButtons();
            }
            else
            {
                btnOrderByRequest.Visible = CurrentProduct.AllowPreOrder;
                btnOrderByRequest.Attributes["style"] = "display:none;";


                btnAdd.Attributes["data-cart-add-productid"] = ProductId.ToString();
                btnAdd.Attributes["style"] = "display:none;";

                BuyInOneClick.Visible = SettingsOrderConfirmation.BuyInOneClick &&
                                        CurrentProduct.Offers.Any(offer => offer.Amount > 0);
                BuyInOneClick.Attributes["style"] = "display:none;";

                HideCreditButtons();
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (CustomerSession.CurrentCustomer.CustomerRole == Role.Administrator ||
                (CustomerSession.CurrentCustomer.CustomerRole == Role.Moderator &&
                 RoleAccess.Check(CustomerSession.CurrentCustomer, "product.aspx")))
            {
                hrefAdmin.Visible = true;
            }

            GetOffer();
        }


        protected string RenderSpinBox()
        {
            return
                string.Format(
                    "<input data-plugin=\"spinbox\" type=\"text\" id=\"txtAmount\" value=\"{0}\" data-spinbox-options=\"{{min:{0},max:{1},step:{2}}}\"/>",
                    CurrentProduct.MinAmount != null ? CurrentProduct.MinAmount.ToString().Replace(",", ".") : "1",
                    CurrentProduct.MaxAmount != null
                        ? CurrentProduct.MaxAmount.ToString().Replace(",", ".")
                        : Int16.MaxValue.ToString(),
                    CurrentProduct.Multiplicity.ToString().Replace(",", "."));
        }

        #region "Show/hide buttons"

        private void ShowCreditButtons()
        {
            if (CurrentProduct.MainPrice > 0 && CurrentProduct.TotalAmount > 0)
            {
                var payment = (Kupivkredit)PaymentService.GetPaymentMethodByType(PaymentType.Kupivkredit);
                var paymentRsbCredit = (RsbCredit)PaymentService.GetPaymentMethodByType(PaymentType.RsbCredit);
                var paymentAlfaBankCredit = (AlfabankUa)PaymentService.GetPaymentMethodByType(PaymentType.AlfabankUa);
                var paymentYesCredit = (YesCredit)PaymentService.GetPaymentMethodByType(PaymentType.YesCredit);

                var productPrice = CatalogService.CalculateProductPrice(CurrentOffer.Price,
                            CurrentProduct.CalculableDiscount,
                            CustomerSession.CurrentCustomer.CustomerGroup,
                            CustomOptionsService.DeserializeFromXml(
                                CustomOptionsService.SerializeToXml(productCustomOptions.CustomOptions, productCustomOptions.SelectedOptions)));

                if (payment != null && payment.Enabled && productPrice > payment.MinimumPrice)
                {
                    btnAddCredit.Visible = true;
                    lblFirstPaymentNote.Visible = true;
                    lblFirstPayment.Visible = true;
                    btnAddCredit.Attributes["data-cart-add-productid"] = ProductId.ToString();
                    btnAddCredit.Attributes["data-cart-payment"] = payment.PaymentMethodID.ToString();

                    hfFirstPaymentPercent.Value = payment.FirstPayment.ToString();
                    if (CurrentOffer != null)
                    {
                        lblFirstPayment.Text =
                            CatalogService.GetStringPrice(productPrice * payment.FirstPayment / 100) + @"*";
                    }
                }
                else if (paymentRsbCredit != null && paymentRsbCredit.Enabled &&
                         productPrice > paymentRsbCredit.MinimumPrice)
                {
                    btnAddCredit.Visible = true;
                    lblFirstPaymentNote.Visible = true;
                    lblFirstPayment.Visible = true;
                    btnAddCredit.Attributes["data-cart-add-productid"] = ProductId.ToString();
                    btnAddCredit.Attributes["data-cart-payment"] = paymentRsbCredit.PaymentMethodID.ToString();

                    hfFirstPaymentPercent.Value = paymentRsbCredit.FirstPayment.ToString();
                    if (CurrentOffer != null)
                    {
                        lblFirstPayment.Text =
                            CatalogService.GetStringPrice(productPrice * paymentRsbCredit.FirstPayment / 100) + @"*";
                    }
                }
                else if (paymentYesCredit != null && paymentYesCredit.Enabled &&
                     productPrice > paymentYesCredit.MinimumPrice)
                {
                    btnAddCredit.Visible = true;
                    lblFirstPaymentNote.Visible = true;
                    lblFirstPayment.Visible = true;
                    btnAddCredit.Attributes["data-cart-add-productid"] = ProductId.ToString();
                    btnAddCredit.Attributes["data-cart-payment"] = paymentYesCredit.PaymentMethodID.ToString();

                    hfFirstPaymentPercent.Value = paymentYesCredit.FirstPayment.ToString();
                    if (CurrentOffer != null)
                    {
                        lblFirstPayment.Text =
                            CatalogService.GetStringPrice(productPrice * paymentYesCredit.FirstPayment / 100) + @"*";
                    }
                }
                else if (paymentAlfaBankCredit != null && paymentAlfaBankCredit.Enabled)
                {
                    btnAddCredit.Visible = true;
                    lblFirstPaymentNote.Visible = false;
                    lblFirstPayment.Visible = false;
                    btnAddCredit.Attributes["data-cart-add-productid"] = ProductId.ToString();
                    btnAddCredit.Attributes["data-cart-payment"] = paymentAlfaBankCredit.PaymentMethodID.ToString();
                }
            }
        }

        private void HideCreditButtons()
        {
            var payment = (Kupivkredit)PaymentService.GetPaymentMethodByType(PaymentType.Kupivkredit);
            var paymentRsbCredit = (RsbCredit)PaymentService.GetPaymentMethodByType(PaymentType.RsbCredit);

            if (payment != null && payment.Enabled && CurrentProduct.MainPrice > payment.MinimumPrice)
            {
                btnAddCredit.Visible = true;
                lblFirstPaymentNote.Visible = true;
                lblFirstPayment.Visible = true;
                btnAddCredit.Attributes["data-cart-add-productid"] = ProductId.ToString();
                btnAddCredit.Attributes["data-cart-payment"] = payment.PaymentMethodID.ToString();
                btnAddCredit.Attributes["style"] = "display:none;";

                hfFirstPaymentPercent.Value = payment.FirstPayment.ToString();
                if (CurrentOffer != null)
                {
                    lblFirstPayment.Text = CatalogService.GetStringPrice(CurrentOffer.Price * payment.FirstPayment / 100) + @"*";
                }
            }
            else if (paymentRsbCredit != null && paymentRsbCredit.Enabled &&
                     CurrentProduct.MainPrice > paymentRsbCredit.MinimumPrice)
            {
                btnAddCredit.Visible = true;
                lblFirstPaymentNote.Visible = true;
                lblFirstPayment.Visible = true;
                btnAddCredit.Attributes["data-cart-add-productid"] = ProductId.ToString();
                btnAddCredit.Attributes["data-cart-payment"] = paymentRsbCredit.PaymentMethodID.ToString();
                btnAddCredit.Attributes["style"] = "display:none;";

                hfFirstPaymentPercent.Value = paymentRsbCredit.FirstPayment.ToString();
                if (CurrentOffer != null)
                {
                    lblFirstPayment.Text =
                        CatalogService.GetStringPrice(CurrentOffer.Price*paymentRsbCredit.FirstPayment/100) + @"*";
                }
            }
        }

        #endregion
    }
}