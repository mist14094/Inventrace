//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.FullSearch;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.SEO;
using AdvantShop.Statistic;
using Newtonsoft.Json;
using Resources;

namespace ClientPages
{
    public partial class Search : AdvantShopClientPage
    {
        protected string SearchTerm = string.Empty;
        private SortOrder _sort = SortOrder.NoSorting;

        private SqlPaging _paging;

        protected void Page_Load(object sender, EventArgs e)
        {
            _paging = new SqlPaging
                {
                    TableName =
                        "[Catalog].[Product] LEFT JOIN [Catalog].[Offer] ON [Product].[ProductID] = [Offer].[ProductID] AND [Offer].[Main] = 1 LEFT JOIN [Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId] and Type='product' and Photo.Main=1 inner join Catalog.ProductCategories on ProductCategories.ProductId = [Product].[ProductID] LEFT JOIN [Catalog].[ShoppingCart] ON [Catalog].[ShoppingCart].[OfferID] = [Catalog].[Offer].[OfferID] AND [Catalog].[ShoppingCart].[ShoppingCartType] = 3 AND [ShoppingCart].[CustomerID] = @CustomerId Left JOIN [Catalog].[Ratio] on Product.ProductId= Ratio.ProductID and Ratio.CustomerId=@CustomerId"
                };

            _paging.AddFieldsRange(
            new List<Field>
                {
                    new Field {Name = "[Product].[ProductID]", IsDistinct = true},
                    new Field {Name = "(CASE WHEN Offer.ColorID is not null THEN (Select Count(PhotoName) From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type) ELSE (Select Count(PhotoName) From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type) END)  AS CountPhoto"},
                    new Field {Name = "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) PhotoName From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type Order By main desc, [Photo].[PhotoSortOrder]) ELSE (Select TOP(1) PhotoName From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type AND [Photo].[Main] = 1) END)  AS Photo"},
                    new Field {Name = "(CASE WHEN Offer.ColorID is not null THEN (Select TOP(1) [Photo].[Description] From [Catalog].[Photo] WHERE ([Photo].ColorID = Offer.ColorID or [Photo].ColorID is null) and [Product].[ProductID] = [Photo].[ObjId] and Type=@Type Order By main desc, [Photo].[PhotoSortOrder]) ELSE (Select TOP(1) [Photo].[Description] From [Catalog].[Photo] WHERE [Product].[ProductID] = [Photo].[ObjId] and Type=@Type AND [Photo].[Main] = 1) END)  AS PhotoDesc"},
                    new Field {Name = "[ProductCategories].[CategoryID]", NotInQuery=true},
                    new Field {Name = "BriefDescription"},
                    new Field {Name = "Product.ArtNo"},
                    new Field {Name = "Name"},
                    
                    new Field {Name = "Recomended"},
                    new Field {Name = "Bestseller"},
                    new Field {Name = "New"},
                    new Field {Name = "OnSale"},
                    new Field {Name = "Discount"},
                    new Field {Name = "Offer.Main", NotInQuery=true},
                    new Field {Name = "Offer.OfferID"},
                    new Field {Name = "Offer.Amount"},
                    new Field {Name = "(CASE WHEN Offer.Amount=0 OR Offer.Amount < IsNull(MinAmount,0) THEN 0 ELSE 1 END) as TempAmountSort", Sorting=SortDirection.Descending},
                    new Field {Name = "MinAmount"},
                    new Field {Name = "MaxAmount"},
                    new Field {Name = "Enabled"},
                    new Field {Name = "AllowPreOrder"},
                    new Field {Name = "Ratio"},
                    new Field {Name = "RatioID"},
                    new Field {Name = "DateModified"},
                    new Field {Name = "ShoppingCartItemId"},
                    new Field {Name = "UrlPath"},

                    new Field {Name = "[ShoppingCart].[CustomerID]", NotInQuery=true},
                    new Field {Name = "BrandID", NotInQuery=true},
                    new Field {Name = "Offer.ProductID as Size_ProductID", NotInQuery=true},
                    new Field {Name = "Offer.ProductID as Color_ProductID", NotInQuery=true},
                    new Field {Name = "Offer.ProductID as Price_ProductID", NotInQuery=true},
                    new Field {Name = "Offer.ColorID"},
                    new Field {Name = "CategoryEnabled", NotInQuery=true},
                    new Field {Name = "null as AdditionalPhoto"}
                });

            if (SettingsCatalog.ComplexFilter)
            {
                _paging.AddFieldsRange(new List<Field>
                {
                    new Field {Name = "(select [Settings].[ProductColorsToString]([Product].[ProductID])) as Colors"},
                    new Field {Name = "(select max (price) - min (price) from catalog.offer where offer.productid=product.productid) as MultiPrices"},
                    new Field {Name = "(select min (price) from catalog.offer where offer.productid=product.productid) as Price"},
                });
            }
            else
            {
                _paging.AddFieldsRange(new List<Field>
                {
                    new Field {Name = "null as Colors"},
                    new Field {Name = "0 as MultiPrices"},
                    new Field {Name = "Price"},
                });
            }
            _paging.AddParam(new SqlParam { ParameterName = "@CustomerId", Value = CustomerSession.CustomerId.ToString() });
            _paging.AddParam(new SqlParam { ParameterName = "@Type", Value = PhotoType.Product.ToString() });

            BuildSorting();
            BuildFilter();

            var nmeta = new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Client_Search_AdvancedSearch));
            SetMeta(nmeta, string.Empty, page:paging.CurrentPage);
            txtName.Focus();
        }

        private void BuildSorting()
        {
            if (!string.IsNullOrEmpty(Request["sort"]))
            {
                Enum.TryParse(Request["sort"], true, out _sort);
            }


            foreach (SortOrder enumItem in Enum.GetValues(typeof (SortOrder)))
            {
                ddlSort.Items.Add(new ListItem
                    {
                        Text = enumItem.GetLocalizedName(),
                        Value = enumItem.ToString(),
                        Selected = _sort == enumItem
                    });
            }

            switch (_sort)
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
            }
        }

        private void BuildFilter()
        {

            foreach (Category c in (CategoryService.GetChildCategoriesByCategoryId(0, true).Where(p => p.Enabled)))
            {
                ddlCategory.Items.Add(new ListItem
                    {
                        Text = c.Name,
                        Value = c.CategoryId.ToString(),
                        Selected = Request["category"] == c.CategoryId.ToString()
                    });
            }

            _paging.Fields["[ProductCategories].[CategoryID]"].Filter = new InChildCategoriesFieldFilter
                {
                    CategoryId = ddlCategory.SelectedValue,
                    ParamName = "@CategoryID"
                };


            if (!string.IsNullOrEmpty(Page.Request["name"]))
            {
                var name = HttpUtility.UrlDecode(Page.Request["name"]).Trim();
                txtName.Text = name;
                var productIds = LuceneSearch.Search(txtName.Text).AggregateString('/');
                _paging.TableName +=
                    " inner join (select item, sort from [Settings].[ParsingBySeperator](@source,'/') ) as dtt on Product.ProductId=convert(int, dtt.item) ";
                _paging.AddParam(new SqlParam {ParameterName = "@source", Value = productIds});
                if (_sort == SortOrder.NoSorting)
                {
                    _paging.Fields.Add("sort", new Field("sort"));
                    _paging.Fields["sort"].Sorting = SortDirection.Ascending;
                }


                SearchTerm = HttpUtility.HtmlEncode(name);
            }

            filterPrice.CategoryId = 0;
            filterPrice.InDepth = true;

            if (!string.IsNullOrEmpty(Request["pricefrom"]) || !string.IsNullOrEmpty(Request["priceto"]))
            {
                int pricefrom = Request["pricefrom"].TryParseInt(0);
                int priceto = Request["priceto"].TryParseInt(int.MaxValue);

                filterPrice.CurValMin = pricefrom;
                filterPrice.CurValMax = priceto;

                _paging.Fields["Price_ProductID"].Filter =  new PriceFieldFilter
                                                           {
                                                               ParamName = "@priceRange",
                                                               From = pricefrom * CurrencyService.CurrentCurrency.Value,
                                                               To = priceto * CurrencyService.CurrentCurrency.Value,
                                                               CategoryId = 0,
                                                               GetSubCategoryes = true
                                                           };
            }
            else
            {
                filterPrice.CurValMin = 0;
                filterPrice.CurValMax = int.MaxValue;
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            _paging.CurrentPageIndex = paging.CurrentPage != 0 ? paging.CurrentPage : 1;
            _paging.ItemsPerPage = paging.CurrentPage != 0 ? SettingsCatalog.ProductsPerPage : int.MaxValue;
            var totalCount = _paging.TotalRowsCount;
            paging.TotalPages = _paging.PageCount;

            if (totalCount != 0 && paging.TotalPages < paging.CurrentPage || paging.CurrentPage < 0)
            {
                Error404();
                return;
            }

            // if we get request from ajax filter
            if (Request["ajax"] == "1")
            {
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

            if (!string.IsNullOrEmpty(Page.Request["name"]) && string.IsNullOrEmpty(Page.Request["ignorelog"]))
            {
                var url = Page.Request.Url.ToString();
                url = url.Substring(url.LastIndexOf("/"), url.Length - url.LastIndexOf("/"));
                StatisticService.AddSearchStatistic(
                    url,
                    Page.Request["name"],
                    string.Format(Resource.Client_Search_SearchIn,
                                  ddlCategory.SelectedItem.Text,
                                  filterPrice.CurValMin,
                                  string.IsNullOrEmpty(Request["priceto"]) ? "∞" : filterPrice.CurValMax.ToString()),
                    totalCount,
                    CustomerSession.CurrentCustomer.Id);
            }

            //filterPrice.Min = prices.Key / CurrencyService.CurrentCurrency.Value;
            //filterPrice.Max = prices.Value / CurrencyService.CurrentCurrency.Value;

            vProducts.DataSource = _paging.PageItems;
            vProducts.ViewMode = productViewChanger.SearchViewMode;
            vProducts.DataBind();

            int itemsCount = totalCount;
            lItemsCount.Text = string.Format("{0} {1}", itemsCount.ToString(),
                                             Strings.Numerals(itemsCount, Resource.Client_Searsh_NoResults,
                                                              Resource.Client_Searsh_1Result,
                                                              Resource.Client_Searsh_2Results,
                                                              Resource.Client_Searsh_5Results));
            pnlSort.Visible = itemsCount > 0;

        }
    }
}