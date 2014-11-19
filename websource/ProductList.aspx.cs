//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using Newtonsoft.Json;
using Resources;

namespace ClientPages
{
    public partial class ProductList_Page : AdvantShopClientPage
    {
        protected ProductOnMain.TypeFlag _typeFlag = ProductOnMain.TypeFlag.None;
        protected string PageName;
        protected int ProductsCount;

        private SqlPaging _paging;

        protected void Page_Load(object sender, EventArgs e)
        {
            _paging = new SqlPaging
                {
                    TableName =
                        "[Catalog].[Product] LEFT JOIN [Catalog].[Offer] ON [Product].[ProductID] = [Offer].[ProductID] and [Offer].Main = 1 LEFT JOIN [Catalog].[ShoppingCart] ON [Catalog].[ShoppingCart].[OfferID] = [Catalog].[Offer].[OfferID] AND [Catalog].[ShoppingCart].[ShoppingCartType] = 3 AND [ShoppingCart].[CustomerID] = @CustomerId  Left JOIN [Catalog].[Ratio] on Product.ProductId= Ratio.ProductID and Ratio.CustomerId=@CustomerId"
                };

            _paging.AddFieldsRange(
                new List<Field>()
                    {
                        new Field {Name = "[Product].[ProductID] as ProductID", IsDistinct = true},
                        new Field {Name = "null AS AdditionalPhoto"},
                        new Field {Name = "(CASE WHEN Offer.ColorID is not null THEN (Select Count(PhotoName) From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type) ELSE (Select Count(PhotoName) From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type) END)  AS CountPhoto"},
                        new Field {Name = "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) PhotoName From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type Order By main desc, [Photo].[PhotoSortOrder]) ELSE (Select TOP(1) PhotoName From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type AND [Photo].[Main] = 1) END)  AS Photo"},
                       new Field {Name = "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) [Photo].[Description] From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type Order By main desc, [Photo].[PhotoSortOrder]) ELSE (Select TOP(1) [Photo].[Description] From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type AND [Photo].[Main] = 1) END)  AS PhotoDesc"},
                        new Field {Name = "BriefDescription"},
                        new Field {Name = "Product.ArtNo"},
                        new Field {Name = "Name"},
                        new Field {Name = "Price"},
                        new Field {Name = "Price - Price*discount/100 as discountPrice", NotInQuery = true},
                        new Field {Name = "0 as MultiPrices"},
                        new Field {Name = "Recomended"},
                        new Field {Name = "New"},
                        new Field {Name = "Bestseller"},
                        new Field {Name = "OnSale"},
                        new Field {Name = "Discount"},
                        new Field {Name = "UrlPath"},
                        new Field {Name = "[Offer].Amount"},
                        new Field {Name = "Offer.Main", NotInQuery = true},
                        new Field {Name = "Offer.OfferID"},
                        new Field {Name = "MinAmount"},
                        new Field {Name = "Enabled"},
                        new Field {Name = "ShoppingCartItemId"},
                        new Field {Name = "AllowPreOrder"},
                        new Field {Name = "Ratio"},
                        new Field {Name = "RatioID"},
                        new Field {Name = "DateModified"},
                        new Field {Name = "[ShoppingCart].[CustomerID]", NotInQuery = true},
                        new Field {Name = "BrandID", NotInQuery = true},
                        new Field {Name = "CategoryEnabled", NotInQuery = true},
                        new Field {Name = "Offer.ColorID as ColorID"},
                        new Field
                            {
                                Name = "(select [Settings].[ProductColorsToString]([Product].[ProductID])) as Colors"
                            }
                    });

            _paging.AddParam(new SqlParam { ParameterName = "@CustomerId", Value = CustomerSession.CustomerId.ToString() });
            _paging.AddParam(new SqlParam { ParameterName = "@Type", Value = PhotoType.Product.ToString() });

            if (string.IsNullOrEmpty(Request["type"]) || !Enum.TryParse(Request["type"], true, out _typeFlag) ||
                _typeFlag == ProductOnMain.TypeFlag.None)
            {
                Error404();
            }

            ProductsCount = ProductOnMain.GetProductCountByType(_typeFlag);
            pnlSort.Visible = ProductsCount > 0;
            productView.Visible = ProductsCount > 0;

            filterBrand.CategoryId = 0;
            filterBrand.InDepth = true;
            filterBrand.WorkType = _typeFlag;

            filterPrice.CategoryId = 0;
            filterPrice.InDepth = true;

            _paging.Fields["CategoryEnabled"].Filter = new EqualFieldFilter
                {
                    Value = "1",
                    ParamName = "@CategoryEnabled"
                };
            _paging.Fields["Enabled"].Filter = new EqualFieldFilter {Value = "1", ParamName = "@Enabled"};

            if (_typeFlag == ProductOnMain.TypeFlag.Bestseller)
            {
                var filterB = new EqualFieldFilter {ParamName = "@Bestseller", Value = "1"};
                _paging.Fields["Bestseller"].Filter = filterB;
                _paging.Fields.Add(new KeyValuePair<string, Field>("SortBestseller",
                                                                   new Field {Name = "SortBestseller as Sort"}));
                PageName = Resource.Client_ProductList_AllBestSellers;

                SetMeta(
                    new AdvantShop.SEO.MetaInfo(SettingsMain.ShopName + " - " + Resource.Client_Bestsellers_Header),
                    string.Empty);
            }
            if (_typeFlag == ProductOnMain.TypeFlag.New)
            {
                var filterN = new EqualFieldFilter {ParamName = "@New", Value = "1"};
                _paging.Fields["New"].Filter = filterN;
                _paging.Fields.Add(new KeyValuePair<string, Field>("SortNew", new Field {Name = "SortNew as Sort"}));
                PageName = Resource.Client_ProductList_AllNew;

                SetMeta(new AdvantShop.SEO.MetaInfo(SettingsMain.ShopName + " - " + Resource.Client_New_Header),
                        string.Empty);
            }
            if (_typeFlag == ProductOnMain.TypeFlag.Discount)
            {
                var filterN = new NotEqualFieldFilter() {ParamName = "@Discount", Value = "0"};
                _paging.Fields["Discount"].Filter = filterN;
                _paging.Fields.Add(new KeyValuePair<string, Field>("SortDiscount",
                                                                   new Field {Name = "SortDiscount as Sort"}));
                PageName = Resource.Client_ProductList_AllDiscount;

                SetMeta(new AdvantShop.SEO.MetaInfo(SettingsMain.ShopName + " - " + Resource.Client_Discount_Header),
                        string.Empty);
            }

            breadCrumbs.Items.AddRange(new BreadCrumbs[]
                {
                    new BreadCrumbs()
                        {
                            Name = Resource.Client_MasterPage_MainPage,
                            Url = UrlService.GetAbsoluteLink("/")
                        },
                    new BreadCrumbs() {Name = PageName, Url = null}
                });

            BuildSorting();
            BuildFilter();
        }

        private void BuildSorting()
        {
            var sort = SortOrder.NoSorting;
            if (!string.IsNullOrEmpty(Request["sort"]))
            {
                Enum.TryParse(Request["sort"], true, out sort);
            }

            foreach (SortOrder enumItem in Enum.GetValues(typeof (SortOrder)))
            {
                ddlSort.Items.Add(new ListItem
                    {
                        Text = enumItem.GetLocalizedName(),
                        Value = enumItem.ToString(),
                        Selected = sort == enumItem
                    });
            }

            switch (sort)
            {
                case SortOrder.AscByName:
                    _paging.Fields["Name"].Sorting = SortDirection.Ascending;
                    break;

                case SortOrder.DescByName:
                    _paging.Fields["Name"].Sorting = SortDirection.Descending;
                    break;

                case SortOrder.AscByPrice:
                    _paging.ExtendedSorting = "Price - Price * Discount / 100";
                    _paging.ExtendedSortingDirection = SortDirection.Ascending;
                    break;

                case SortOrder.DescByPrice:
                    _paging.ExtendedSorting = "Price - Price * Discount / 100";
                    _paging.ExtendedSortingDirection = SortDirection.Descending;
                    break;

                case SortOrder.AscByRatio:
                    _paging.Fields["Ratio"].Sorting = SortDirection.Ascending;
                    break;

                case SortOrder.DescByRatio:
                    _paging.Fields["Ratio"].Sorting = SortDirection.Descending;
                    break;

                default:
                    switch (_typeFlag)
                    {
                        case ProductOnMain.TypeFlag.Bestseller:
                            _paging.Fields["SortBestseller"].Sorting = SortDirection.Ascending;
                            break;
                        case ProductOnMain.TypeFlag.New:
                            _paging.Fields["SortNew"].Sorting = SortDirection.Ascending;
                            break;
                        case ProductOnMain.TypeFlag.Discount:
                            _paging.Fields["SortDiscount"].Sorting = SortDirection.Ascending;
                            break;
                    }
                    break;
            }
        }

        private void BuildFilter()
        {
            if (!string.IsNullOrEmpty(Request["pricefrom"]) || !string.IsNullOrEmpty(Request["priceto"]))
            {
                int pricefrom = Request["pricefrom"].TryParseInt(0);
                int priceto = Request["priceto"].TryParseInt(int.MaxValue);

                filterPrice.CurValMin = pricefrom;
                filterPrice.CurValMax = priceto;

                _paging.Fields["discountPrice"].Filter = new RangeFieldFilter
                    {
                        ParamName = "@priceRange",
                        From = pricefrom*CurrencyService.CurrentCurrency.Value,
                        To = priceto*CurrencyService.CurrentCurrency.Value
                    };

                _paging.Fields["Price"].Filter = new NotEqualFieldFilter {ParamName = "@price", Value = "0"};
            }
            else
            {
                filterPrice.CurValMin = 0;
                filterPrice.CurValMax = int.MaxValue;
            }

            if (!string.IsNullOrEmpty(Request["brand"]))
            {
                int id = 0;
                var brandIds = (from b in Request["brand"].Split(',') where int.TryParse(b, out id) select id).ToList();
                filterBrand.SelectedBrandIDs = brandIds;
                var isf = new InSetFieldFilter
                    {
                        IncludeValues = true,
                        ParamName = "@brands",
                        Values = brandIds.Select(brandid => brandid.ToString()).ToArray()
                    };
                _paging.Fields["BrandID"].Filter = isf;
                filterBrand.SelectedBrandIDs = brandIds;
            }
            else
            {
                filterBrand.SelectedBrandIDs = new List<int>();
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            _paging.CurrentPageIndex = paging.CurrentPage != 0 ? paging.CurrentPage : 1;
            _paging.ItemsPerPage = paging.CurrentPage != 0 ? SettingsCatalog.ProductsPerPage : int.MaxValue;
            var pageCount = _paging.PageCount;
            var totalCount = _paging.TotalRowsCount;
            paging.TotalPages = _paging.PageCount;

            if (totalCount != 0 && paging.TotalPages < paging.CurrentPage || paging.CurrentPage < 0)
            {
                Error404();
                return;
            }

            var prices =
                _paging.GetCustomData(
                    "min(Price - Price * discount/100) as PriceFrom, max(Price - Price * discount/100) as PriceTo",
                    string.Empty,
                    reader => new
                        {
                            From = SQLDataHelper.GetFloat(reader, "PriceFrom"),
                            To = SQLDataHelper.GetFloat(reader, "PriceTo")
                        }
                    ).First();

            if (Request["ajax"] == "1")
            {
                Response.Clear();
                Response.ContentType = "application/json";

                var res = JsonConvert.SerializeObject(new
                    {
                        ProductsCount = totalCount,
                        AvaliblePriceFrom = Math.Floor(prices.From/CurrencyService.CurrentCurrency.Value),
                        AvaliblePriceTo = Math.Ceiling(prices.To/CurrencyService.CurrentCurrency.Value),
                    });
                Response.Write(res);
                Response.End();
                return;
            }

            productView.DataSource = _paging.PageItems;
            productView.ViewMode = productViewChanger.CatalogViewMode;
            productView.DataBind();
            paging.TotalPages = pageCount;


            filterBrand.AvalibleBrandIDs = SettingsCatalog.ExluderingFilters
                                               ? _paging.GetCustomData("BrandID",
                                                                       " AND BrandID is not null",
                                                                       reader => SQLDataHelper.GetInt(reader, "BrandID"))
                                               : null;
        }
    }
}