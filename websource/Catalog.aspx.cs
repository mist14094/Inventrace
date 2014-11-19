//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.CMS;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using Newtonsoft.Json;
using Resources;

public partial class Catalog_Page : AdvantShopClientPage
{
    private int _categoryId;
    protected Category Category;
    protected int ProductsCount;
    protected bool Indepth;

    private SqlPaging _paging;

    protected void Page_Load(object sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(Request["categoryid"]) || !Int32.TryParse(Request["categoryid"], out _categoryId))
        {
            Error404();
        }

        Category = CategoryService.GetCategory(_categoryId);
        if (Category == null || Category.Enabled == false || Category.ParentsEnabled == false)
        {
            Error404();
            return;
        }

        Indepth = Request["indepth"] == "1" || Category.DisplayChildProducts;

        _paging = new SqlPaging
            {
                TableName =
                    "[Catalog].[Product] LEFT JOIN [Catalog].[Offer] ON [Product].[ProductID] = [Offer].[ProductID] inner join Catalog.ProductCategories on ProductCategories.ProductId = [Product].[ProductID] Left JOIN [Catalog].[ProductPropertyValue] ON [Product].[ProductID] = [ProductPropertyValue].[ProductID] LEFT JOIN [Catalog].[ShoppingCart] ON [Catalog].[ShoppingCart].[OfferID] = [Catalog].[Offer].[OfferID] AND [Catalog].[ShoppingCart].[ShoppingCartType] = 3 AND [ShoppingCart].[CustomerID] = @CustomerId Left JOIN [Catalog].[Ratio] on Product.ProductId= Ratio.ProductID and Ratio.CustomerId=@CustomerId"
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
                    new Field {Name = "(CASE WHEN Price=0 THEN 0 ELSE 1 END) as TempSort", Sorting=SortDirection.Descending},
                    new Field {Name = "Recomended"},
                    new Field {Name = "Bestseller"},
                    new Field {Name = "New"},
                    new Field {Name = "OnSale"},
                    new Field {Name = "Discount"},
                    new Field {Name = "Offer.Main", NotInQuery=true},
                    new Field {Name = "Offer.OfferID"},
                    new Field {Name = "(Select Max(Offer.Amount) from catalog.Offer Where ProductId=[Product].[ProductID]) as Amount"},
                    new Field {Name = "(CASE WHEN (Select Max(Offer.Amount) from catalog.Offer Where ProductId=[Product].[ProductID])  <= 0 OR (Select Max(Offer.Amount) from catalog.Offer Where ProductId=[Product].[ProductID]) < IsNull(MinAmount,0) THEN 0 ELSE 1 END) as TempAmountSort", Sorting=SortDirection.Descending},
                    new Field {Name = "MinAmount"},
                    new Field {Name = "MaxAmount"},
                    new Field {Name = "Enabled"},
                    new Field {Name = "AllowPreOrder"},
                    new Field {Name = "Ratio"},
                    new Field {Name = "RatioID"},
                    new Field {Name = "DateModified"},
                    new Field {Name = "ShoppingCartItemId"},
                    new Field {Name = "UrlPath"},
                    new Field {Name = !Indepth ? "[ProductCategories].[SortOrder]" : "0 as SortOrder"},
                    new Field {Name = "[ShoppingCart].[CustomerID]", NotInQuery=true},
                    new Field {Name = "BrandID", NotInQuery=true},
                    new Field {Name = "Offer.ProductID as Size_ProductID", NotInQuery=true},
                    new Field {Name = "Offer.ProductID as Color_ProductID", NotInQuery=true},
                    new Field {Name = "Offer.ProductID as Price_ProductID", NotInQuery=true},
                    new Field {Name = "Offer.ColorID"},
                    new Field {Name = "CategoryEnabled", NotInQuery=true},
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
        
        ProductsCount = Indepth ? Category.TotalProductsCount : Category.GetProductCount();

        categoryView.CategoryID = _categoryId;
        categoryView.Visible = Category.DisplayStyle == "True" || ProductsCount == 0;
        pnlSort.Visible = ProductsCount > 0;
        productView.Visible = ProductsCount > 0;
        catalogView.CategoryID = _categoryId;

        filterProperty.CategoryId = _categoryId;

        filterBrand.CategoryId = _categoryId;
        filterBrand.InDepth = Indepth;

        filterSize.CategoryId = _categoryId;
        filterSize.InDepth = Indepth;

        filterColor.CategoryId = _categoryId;
        filterColor.InDepth = Indepth;

        filterPrice.CategoryId = _categoryId;
        filterPrice.InDepth = Indepth;


        lblCategoryName.Text = _categoryId != 0 ? Category.Name : Resource.Client_MasterPage_Catalog;

        breadCrumbs.Items =
            CategoryService.GetParentCategories(_categoryId).Select(parent => new BreadCrumbs
                                                                                  {
                                                                                      Name = parent.Name,
                                                                                      Url = UrlService.GetLink(ParamType.Category, parent.UrlPath, parent.CategoryId)
                                                                                  }).Reverse().ToList();
        breadCrumbs.Items.Insert(0, new BreadCrumbs
                                        {
                                            Name = Resource.Client_MasterPage_MainPage,
                                            Url = UrlService.GetAbsoluteLink("/")
                                        });

        var categoryMeta = SetMeta(Category.Meta, Category.Name, page: paging.CurrentPage);
        lblCategoryName.Text = categoryMeta.H1;

        if (Category.DisplayChildProducts || Indepth)
        {
            var cfilter = new InChildCategoriesFieldFilter
                              {
                                  CategoryId = _categoryId.ToString(),
                                  ParamName = "@CategoryID"
                              };
            _paging.Fields["[ProductCategories].[CategoryID]"].Filter = cfilter;
        }
        else
        {
            var cfilter = new EqualFieldFilter { Value = _categoryId.ToString(), ParamName = "@catalog" };
            _paging.Fields["[ProductCategories].[CategoryID]"].Filter = cfilter;
        }

        _paging.Fields["Enabled"].Filter = new EqualFieldFilter { Value = "1", ParamName = "@enabled" };
        _paging.Fields["CategoryEnabled"].Filter = new EqualFieldFilter { Value = "1", ParamName = "@CategoryEnabled" };
        
        var logicalFilter = new LogicalFilter{ ParamName = "@Main", HideInCustomData = true };
        logicalFilter.AddFilter(new EqualFieldFilter {Value = "1", ParamName = "@Main1", HideInCustomData = true});
        logicalFilter.AddLogicalOperation("OR");
        logicalFilter.AddFilter(new NullFieldFilter { Null = true, ParamName = "@Main2", HideInCustomData = true });
        _paging.Fields["Offer.Main"].Filter = logicalFilter;

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

        foreach (SortOrder enumItem in Enum.GetValues(typeof(SortOrder)))
        {
            if (!SettingsCatalog.EnableProductRating && (enumItem == SortOrder.DescByRatio || enumItem == SortOrder.AscByRatio))
            {
                continue;
            }

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
                if(!Indepth)
                    _paging.Fields["[ProductCategories].[SortOrder]"].Sorting = SortDirection.Ascending;
                else
                {
                    _paging.Fields["SortOrder"].Sorting = SortDirection.Ascending;
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

            _paging.Fields["Price_ProductID"].Filter = new PriceFieldFilter
                                                           {
                                                               ParamName = "@priceRange",
                                                               From = pricefrom * CurrencyService.CurrentCurrency.Value,
                                                               To = priceto * CurrencyService.CurrentCurrency.Value,
                                                               CategoryId = _categoryId,
                                                               GetSubCategoryes = Indepth
                                                           };
        }
        else
        {
            filterPrice.CurValMin = 0;
            filterPrice.CurValMax = int.MaxValue;
        }

        if (!string.IsNullOrEmpty(Request["prop"]))
        {
            var valIDs = new Dictionary<int, List<int>>();
            var selectedPropertyIDs = new List<int>();
            var filterCollection = Request["prop"].Split('-');
            int key = 1;
            foreach (var val in filterCollection)
            {
                var tempListIds = new List<int>();
                foreach (int id in val.Split(',').Select(item => item.TryParseInt()).Where(id => id != 0))
                {
                    tempListIds.Add(id);
                    selectedPropertyIDs.Add(id);
                }
                valIDs.Add(key, tempListIds);
                key++;
            }

            if (valIDs.Count != 0)
            {
                var lfilter = new PropertyFieldFilter { ListFilter = valIDs, ParamName = "@prop", CategoryId = Category.CategoryId, GetSubCategoryes = Category.DisplayChildProducts };
                _paging.Fields["[Product].[ProductID]"].Filter = lfilter;
            }
            filterProperty.SelectedPropertyIDs = selectedPropertyIDs;
        }
        else
        {
            filterProperty.SelectedPropertyIDs = new List<int>();
        }

        if (!string.IsNullOrEmpty(Request["brand"]))
        {
            var brandIds = Request["brand"].Split(',').Select(item => item.TryParseInt()).Where(id => id != 0).ToList();
            filterBrand.SelectedBrandIDs = brandIds;
            var isf = new InSetFieldFilter
            {
                IncludeValues = true,
                ParamName = "@brands",
                Values = brandIds.Select(brandid => brandid.ToString()).ToArray()
            };
            _paging.Fields["BrandID"].Filter = isf;
        }
        else
        {
            filterBrand.SelectedBrandIDs = new List<int>();
        }

        if (!string.IsNullOrEmpty(Request["size"]))
        {
            var sizeIds = Request["size"].Split(',').Select(item => item.TryParseInt()).Where(id => id != 0).ToList();
            filterSize.SelectedSizesIDs = sizeIds;
            _paging.Fields["Size_ProductID"].Filter = new SizeFieldFilter
                {
                    CategoryId = _categoryId,
                    ParamName = "@size",
                    ListFilter = sizeIds,
                    GetSubCategoryes = Indepth
                };
        }
        else
        {
            filterSize.SelectedSizesIDs = new List<int>();
        }

        if (!string.IsNullOrEmpty(Request["color"]))
        {
            var colorIds = Request["color"].Split(',').Select(item => item.TryParseInt()).Where(id => id != 0).ToList();
            filterColor.SelectedColorsIDs = colorIds;
            _paging.Fields["Color_ProductID"].Filter = new ColorFieldFilter
                {
                    CategoryId = _categoryId,
                    ParamName = "@color",
                    ListFilter = colorIds,
                    GetSubCategoryes = Indepth
                };

            if (SettingsCatalog.ComplexFilter)
            {
                _paging.AddField(new Field
                    {
                        Name =
                            string.Format(
                                "(select Top 1 PhotoName from catalog.Photo inner join catalog.offer on Photo.objid=offer.productid and Type='product'" +
                                " where offer.productid=product.productid and Photo.ColorID in({0}) order by Photo.PhotoSortOrder, Photo.Main)" +
                                " as AdditionalPhoto",
                                colorIds.AggregateString(','))
                    });
            }
            else
            {
                _paging.AddField(new Field
                {
                    Name = "null as AdditionalPhoto"
                });
            }

        }
        else
        {
            filterColor.SelectedColorsIDs = new List<int>();
            _paging.AddField(new Field
            {
                Name = "null as AdditionalPhoto"
            });
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {

        _paging.ItemsPerPage = paging.CurrentPage != 0 ? SettingsCatalog.ProductsPerPage : int.MaxValue;
        _paging.CurrentPageIndex = paging.CurrentPage != 0 ? paging.CurrentPage : 1;

        var totalCount = _paging.TotalRowsCount;

        productView.ViewMode = productViewChanger.CatalogViewMode;
        lTotalItems.Text = string.Format(Resource.Client_Catalog_ItemsFound, totalCount);

        paging.TotalPages = _paging.PageCount;

        if ((paging.TotalPages < paging.CurrentPage && paging.CurrentPage > 1) || paging.CurrentPage < 0)
        {
            Error404();
            return;
        }

        
            
        


        // if we get request from ajax filter
        if (Request["ajax"] == "1")
        {
            Response.Clear();
            Response.ContentType = "application/json";

            var prices =
                _paging.GetCustomData(
                    "min(Price - Price * discount/100) as PriceFrom, max(Price - Price * discount/100) as PriceTo",
                    string.Empty,
                    reader => new
                    {
                        From = SQLDataHelper.GetFloat(reader, "PriceFrom"),
                        To = SQLDataHelper.GetFloat(reader, "PriceTo"),
                    }
                    ).First();

            var res = JsonConvert.SerializeObject(new
            {
                ProductsCount = totalCount,
                AvalibleProperties = filterProperty.AvaliblePropertyIDs,
                AvalibleBrands = filterBrand.AvalibleBrandIDs,
                AvaliblePriceFrom = Math.Floor(prices.From / CurrencyService.CurrentCurrency.Value),
                AvaliblePriceTo = Math.Ceiling(prices.To / CurrencyService.CurrentCurrency.Value),
            });
            Response.Write(res);
            Response.End();
            return;
        }

        productView.DataSource = _paging.PageItems;
        productView.DataBind();


        bool exluderingFilters = SettingsCatalog.ExluderingFilters;

        filterProperty.AvaliblePropertyIDs = exluderingFilters
                                                 ? _paging.GetCustomData("PropertyValueID",
                                                                         " AND PropertyValueID is not null",
                                                                         reader => SQLDataHelper.GetInt(reader, "PropertyValueID"))
                                                 : null;
        filterBrand.AvalibleBrandIDs = exluderingFilters ? _paging.GetCustomData("BrandID",
                                                                         " AND BrandID is not null",
                                                                         reader => SQLDataHelper.GetInt(reader, "BrandID"))
                                                 : null;
        filterSize.AvalibleSizesIDs = exluderingFilters ? _paging.GetCustomData("SizeID",
                                                                         " AND SizeID is not null",
                                                                         reader => SQLDataHelper.GetInt(reader, "SizeID")) : null;

        filterColor.AvalibleColorsIDs = exluderingFilters ? _paging.GetCustomData("Offer.ColorID",
                                                                         " AND Offer.ColorID is not null",
                                                                         reader => SQLDataHelper.GetInt(reader, "ColorID")) : null;

    }
}