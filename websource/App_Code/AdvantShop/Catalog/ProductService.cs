//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using AdvantShop.Core;
using AdvantShop.Core.Caching;
using AdvantShop.Core.SQL;
using AdvantShop.Customers;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.FullSearch;
using AdvantShop.Helpers;
using AdvantShop.SaasData;
using AdvantShop.SEO;
using Resources;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Catalog
{
    public class ProductService
    {
        #region Categories

        public static void SetProductHierarchicallyEnabled(int productId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[SetProductHierarchicallyEnabled]", CommandType.StoredProcedure,
                                            new SqlParameter("@ProductId", productId));
        }

        /// <summary>
        /// get first categoryId by productId(сделал инклуд индекс)
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static int GetFirstCategoryIdByProductId(int productId)
        {
            int categoryId = SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar("[Catalog].[sp_GetCategoryIDByProductID]", CommandType.StoredProcedure, new SqlParameter("@ProductID", productId)), CategoryService.DefaultNonCategoryId);
            return categoryId;
        }

        /// <summary>
        /// get categoryIds by productId
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static IEnumerable<int> GetCategoriesIDsByProductId(int productId)
        {
            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>("[Catalog].[sp_GetCategoriesIDsByProductId]",
                                                                   CommandType.StoredProcedure,
                                                                   "CategoryID",
                                                                   new SqlParameter("@ProductID", productId));
        }

        /// <summary>
        /// get categories by productId
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static List<Category> GetCategoriesByProductId(int productId)
        {
            var res = SQLDataAccess.ExecuteReadList("[Catalog].[sp_GetProductCategories]", CommandType.StoredProcedure, CategoryService.GetCategoryFromReader, new SqlParameter("@ProductID", productId));
            return res;
        }

        /// <summary>
        /// create html for tooltip
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static string CreateTooltipContent(int productId)
        {
            var res = new StringBuilder();
            var content = new StringBuilder();
            int categoryCounter = 0;
            try
            {
                using (var db = new SQLDataAccess())
                {
                    db.cmd.CommandText = "[Catalog].[sp_GetCategoriesPathesByProductID]";
                    db.cmd.CommandType = CommandType.StoredProcedure;
                    db.cmd.Parameters.Clear();
                    db.cmd.Parameters.AddWithValue("@ProductID", productId);

                    db.cnOpen();
                    using (SqlDataReader reader = db.cmd.ExecuteReader())

                        while (reader.Read())
                        {
                            content.Append("<br/>&nbsp;&nbsp;&nbsp;" + SQLDataHelper.GetString(reader, "CategoryPath"));
                            categoryCounter++;
                        }

                    db.cnClose();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            if (categoryCounter > 0)
            {
                var strHead = new StringBuilder();
                strHead.Append("<div class=\'tooltipDiv\'><span class=\'tooltipBold\'>");
                strHead.Append(string.Format(Resource.Admin_CategoriesService_ProductInCategories, categoryCounter));
                strHead.Append("<br/><div style=\'height:5px;width:0px;\' />");
                strHead.Append(Resource.Admin_CategoriesService_Categories);

                res.Append(strHead);
                res.Append(content);
                res.Append("</span></div>");
            }
            else
            {
                res.Append("");
            }

            return res.ToString();
        }

        /// <summary>
        /// get count of categories by productId
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static int GetCountOfCategoriesByProductId(int productId)
        {
            var count = SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetCOUNTOfCategoriesByProductID]", CommandType.StoredProcedure, new SqlParameter("@ProductID", productId));
            return count;
        }

        #endregion

        #region Related Products

        public static string LinkedProductToString(int productId, RelatedType related)
        {
            var temp = SQLDataAccess.ExecuteReadList("Select ArtNo from Catalog.Product inner join Catalog.RelatedProducts on " +
                                                    "RelatedProducts.LinkedProductID=Product.ProductId where RelatedType=@type and RelatedProducts.ProductID=@productId",
                                                    CommandType.Text,
                                                    reader => SQLDataHelper.GetString(reader, "ArtNo"),
                                                    new SqlParameter("@productId", productId),
                                                    new SqlParameter("@type", (int)related));
            return temp.AggregateString(',');
        }

        public static void LinkedProductFromString(int productId, string linkproducts, RelatedType type)
        {
            ClearRelatedProducts(productId, type);

            if (!string.IsNullOrEmpty(linkproducts))
            {
                var arrArt = linkproducts.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string t in arrArt)
                {
                    var artNo = t.Trim();
                    if (string.IsNullOrWhiteSpace(artNo)) continue;
                    int linkProductId = GetProductId(artNo);
                    if (linkProductId != 0)
                        AddRelatedProduct(productId, linkProductId, type);
                }
            }
        }

        /// <summary>
        /// Add related product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="relatedProductId"></param>
        /// <param name="relatedType"></param>
        public static void AddRelatedProduct(int productId, int relatedProductId, RelatedType relatedType)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_AddRelatedProduct]", CommandType.StoredProcedure,
                                                new SqlParameter("@ProductID", productId),
                                                new SqlParameter("@RelatedProductID", relatedProductId),
                                                new SqlParameter("@RelatedType", (int)relatedType));
        }

        /// <summary>
        /// delete ralated product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="relatedProductId"></param>
        /// <param name="relatedType"></param>
        public static void DeleteRelatedProduct(int productId, int relatedProductId, RelatedType relatedType)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeleteRelatedProduct]", CommandType.StoredProcedure,
                                            new SqlParameter("@ProductID", productId),
                                            new SqlParameter("@RelatedProductID", relatedProductId),
                                            new SqlParameter("@RelatedType", (int)relatedType));
        }

        public static void DeleteRelatedProducts(int productId, RelatedType relatedType)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from catalog.RelatedProducts Where ProductId=@ProductID Or LinkedProductID=@ProductID and RelatedType=@RelatedType",
                                            CommandType.Text,
                                            new SqlParameter("@ProductID", productId),
                                            new SqlParameter("@RelatedType", (int)relatedType));
        }

        public static void ClearRelatedProducts(int productId, RelatedType relatedType)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from catalog.RelatedProducts Where ProductId=@ProductID and RelatedType=@RelatedType",
                                            CommandType.Text,
                                            new SqlParameter("@ProductID", productId),
                                            new SqlParameter("@RelatedType", (int)relatedType));
        }

        /// <summary>
        /// Get related products
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="relatedType"></param>
        /// <returns></returns>
        public static List<Product> GetRelatedProducts(int productId, RelatedType relatedType)
        {
            List<Product> res = SQLDataAccess.ExecuteReadList<Product>("[Catalog].[sp_GetRelatedProducts]", CommandType.StoredProcedure,
                                                              reader => new Product
                                                                            {
                                                                                ProductId = SQLDataHelper.GetInt(reader, "ProductId"),
                                                                                ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                                                                                Photo = SQLDataHelper.GetString(reader, "Photo"),
                                                                                PhotoDesc = SQLDataHelper.GetString(reader, "PhotoDesc"),
                                                                                Name = SQLDataHelper.GetString(reader, "Name"),
                                                                                BriefDescription = SQLDataHelper.GetString(reader, "BriefDescription"),
                                                                                Discount = SQLDataHelper.GetFloat(reader, "Discount"),
                                                                                UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                                                                            },
                                                                            new SqlParameter("@ProductID", productId),
                                                                            new SqlParameter("@RelatedType", (int)relatedType),
                                                                            new SqlParameter("@Type", PhotoType.Product.ToString())
                                                                        );
            return res;
        }
        #endregion

        #region Get Add Update Delete

        /// <summary>
        /// delete product by productId
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="sentToLuceneIndex"></param>
        /// <returns></returns>
        public static bool DeleteProduct(int productId, bool sentToLuceneIndex)
        {
            PhotoService.DeletePhotos(productId, PhotoType.Product);
            DeleteRelatedProducts(productId, RelatedType.Related);
            DeleteRelatedProducts(productId, RelatedType.Alternative);
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DeleteProduct]", CommandType.StoredProcedure, new SqlParameter("@ProductID", productId));
            CategoryService.ClearCategoryCache();
            if (sentToLuceneIndex)
                LuceneSearch.ClearLuceneIndexRecord(productId);
            return true;
        }

        /// <summary>
        /// add product
        /// </summary>
        /// <param name="product"></param>
        /// <param name="sentToLuceneIndex"></param>
        /// <returns></returns>
        public static int AddProduct(Product product, bool sentToLuceneIndex)
        {
            if (SaasDataService.IsSaasEnabled && GetProductsCount() >= SaasDataService.CurrentSaasData.ProductsCount)
            {
                return 0;
            }

            product.ProductId = SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar("[Catalog].[sp_AddProduct]",
                CommandType.StoredProcedure,
                new SqlParameter("@ArtNo", product.ArtNo),
                new SqlParameter("@Name", product.Name),
                new SqlParameter("@Ratio", product.Ratio),
                new SqlParameter("@Discount", product.Discount),
                new SqlParameter("@Weight", product.Weight),
                new SqlParameter("@Size", product.Size ?? (object)DBNull.Value),
                new SqlParameter("@BriefDescription", product.BriefDescription ?? (object)DBNull.Value),
                new SqlParameter("@Description", product.Description ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", product.Enabled),
                new SqlParameter("@Recomended", product.Recomended),
                new SqlParameter("@New", product.New),
                new SqlParameter("@BestSeller", product.BestSeller),
                new SqlParameter("@OnSale", product.OnSale),
                new SqlParameter("@AllowPreOrder", product.AllowPreOrder),
                new SqlParameter("@BrandID", product.BrandId != 0 ? product.BrandId : (object)DBNull.Value),
                new SqlParameter("@UrlPath", product.UrlPath),
                new SqlParameter("@Unit", product.Unit ?? (object)DBNull.Value),
                new SqlParameter("@ShippingPrice", product.ShippingPrice),
                new SqlParameter("@MinAmount", product.MinAmount ?? (object)DBNull.Value),
                new SqlParameter("@MaxAmount", product.MaxAmount ?? (object)DBNull.Value),
                new SqlParameter("@Multiplicity", product.Multiplicity),
                new SqlParameter("@SalesNote", product.SalesNote ?? (object)DBNull.Value),
                new SqlParameter("@HasMultiOffer", product.HasMultiOffer)
                ));
            if (product.ProductId == 0)
                return 0;
            //by default in bd set ID if artNo is Null
            if (string.IsNullOrEmpty(product.ArtNo))
            {
                product.ArtNo = GetProductArtNoByProductID(product.ProductId);
            }

            SetProductHierarchicallyEnabled(product.ProductId);

            // ---- Offers
            if (product.Offers != null && product.Offers.Count != 0)
            {
                foreach (var offer in product.Offers)
                {
                    if (offer.ArtNo.IsNullOrEmpty())
                        offer.ArtNo = product.ArtNo;

                    offer.ProductId = product.ProductId;
                    OfferService.AddOffer(offer);
                }
            }
            // ---- Meta
            if (product.Meta != null)
            {
                if (!product.Meta.Title.IsNullOrEmpty() || !product.Meta.MetaKeywords.IsNullOrEmpty() || !product.Meta.MetaDescription.IsNullOrEmpty() || !product.Meta.H1.IsNullOrEmpty())
                {
                    product.Meta.ObjId = product.ProductId;
                    MetaInfoService.SetMeta(product.Meta);
                }
            }

            if (sentToLuceneIndex)
            {
                if (product.Enabled && product.CategoryEnabled)
                {
                    LuceneSearch.AddUpdateLuceneIndex(product.Offers != null
                                                          ? new SampleData(product.ProductId,
                                                                           product.ArtNo + " " +
                                                                           product.Offers.Select(o => o.ArtNo)
                                                                                  .AggregateString(' '), product.Name)
                                                          : new SampleData(product.ProductId, product.ArtNo,
                                                                           product.Name));
                }
                else
                {
                    LuceneSearch.ClearLuceneIndexRecord(product.ProductId);
                }
            }

            return product.ProductId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productId"></param>
        public static string GetProductArtNoByProductID(int productId)
        {
            return SQLDataAccess.ExecuteScalar<string>("SELECT [ArtNo] FROM [Catalog].[Product] WHERE [ProductID] = @ProductID",
                                                                   CommandType.Text, new SqlParameter("@ProductID", productId));
        }

        /// <summary>
        /// update product by artno
        /// </summary>
        /// <param name="product"></param>
        /// <param name="sentToLuceneIndex"></param>
        /// <returns></returns>
        public static void UpdateProductByArtNo(Product product, bool sentToLuceneIndex)
        {
            product.ProductId = SQLDataAccess.ExecuteScalar<int>("SELECT [ProductID] FROM [Catalog].[Product] WHERE [ArtNo] = @ArtNo",
                                                                   CommandType.Text, new SqlParameter("@ArtNo", product.ArtNo));
            if (product.ProductId > 0)
                UpdateProduct(product, sentToLuceneIndex);
        }

        /// <summary>
        /// update product
        /// </summary>
        /// <param name="product"></param>
        /// <param name="sentToLuceneIndex"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static void UpdateProduct(Product product, bool sentToLuceneIndex)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateProductById]", CommandType.StoredProcedure,
                                          new SqlParameter("@ArtNo", product.ArtNo),
                                          new SqlParameter("@Name", product.Name),
                                          new SqlParameter("@ProductID", product.ProductId),
                                          new SqlParameter("@Ratio", product.Ratio),
                                          new SqlParameter("@Discount", product.Discount),
                                          new SqlParameter("@Weight", product.Weight),
                                          new SqlParameter("@Size", (product.Size ?? (object)DBNull.Value)),
                                          new SqlParameter("@BriefDescription", product.BriefDescription ?? (object)DBNull.Value),
                                          new SqlParameter("@Description", product.Description ?? (object)DBNull.Value),
                                          new SqlParameter("@Enabled", product.Enabled),
                                          new SqlParameter("@AllowPreOrder", product.AllowPreOrder),
                                          new SqlParameter("@Recomended", product.Recomended),
                                          new SqlParameter("@New", product.New),
                                          new SqlParameter("@BestSeller", product.BestSeller),
                                          new SqlParameter("@OnSale", product.OnSale),
                                          new SqlParameter("@BrandID", product.BrandId != 0 ? product.BrandId : (object)DBNull.Value),
                                          new SqlParameter("@UrlPath", product.UrlPath),
                                          new SqlParameter("@Unit", product.Unit ?? (object)DBNull.Value),
                                          new SqlParameter("@ShippingPrice", product.ShippingPrice),
                                          new SqlParameter("@MinAmount", product.MinAmount ?? (object)DBNull.Value),
                                          new SqlParameter("@MaxAmount", product.MaxAmount ?? (object)DBNull.Value),
                                          new SqlParameter("@Multiplicity", product.Multiplicity),
                                          new SqlParameter("@SalesNote", product.SalesNote ?? (object)DBNull.Value),
                                          new SqlParameter("@HasMultiOffer", product.HasMultiOffer)
                                          );

            SetProductHierarchicallyEnabled(product.ProductId);

            OfferService.DeleteOldOffers(product.ProductId, product.Offers);

            if (product.Offers != null)
            {
                foreach (var offer in product.Offers)
                {
                    if (offer.OfferId <= 0)
                    {
                        OfferService.AddOffer(offer);
                    }
                    else
                    {
                        OfferService.UpdateOffer(offer);
                    }
                }
            }

            if (product.Meta != null)
            {
                if (product.Meta.Title.IsNullOrEmpty() && product.Meta.MetaKeywords.IsNullOrEmpty() && product.Meta.MetaDescription.IsNullOrEmpty() && product.Meta.H1.IsNullOrEmpty())
                {
                    if (MetaInfoService.IsMetaExist(product.ProductId, MetaType.Product))
                        MetaInfoService.DeleteMetaInfo(product.ProductId, MetaType.Product);
                }
                else
                    MetaInfoService.SetMeta(product.Meta);
            }

            if (sentToLuceneIndex)
            {
                if (product.Enabled && product.CategoryEnabled)
                {
                    LuceneSearch.AddUpdateLuceneIndex(product.Offers != null
                                                          ? new SampleData(product.ProductId,
                                                                           product.ArtNo + " " +
                                                                           product.Offers.Select(o => o.ArtNo)
                                                                                  .AggregateString(' '), product.Name)
                                                          : new SampleData(product.ProductId, product.ArtNo,
                                                                           product.Name));
                }
                else
                {
                    LuceneSearch.ClearLuceneIndexRecord(product.ProductId);
                }
            }
        }

        public static Product GetProductFromReader(SqlDataReader reader)
        {
            return new Product
                       {
                           ProductId = SQLDataHelper.GetInt(reader, "ProductId"),
                           ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                           Name = SQLDataHelper.GetString(reader, "Name"),
                           BriefDescription = SQLDataHelper.GetString(reader, "BriefDescription", string.Empty),
                           Description = SQLDataHelper.GetString(reader, "Description", string.Empty),
                           Photo = SQLDataHelper.GetString(reader, "PhotoName"),
                           Discount = SQLDataHelper.GetFloat(reader, "Discount"),
                           Size = SQLDataHelper.GetString(reader, "Size"),
                           Weight = SQLDataHelper.GetFloat(reader, "Weight"),
                           Ratio = SQLDataHelper.GetDouble(reader, "Ratio"),
                           Enabled = SQLDataHelper.GetBoolean(reader, "Enabled", true),
                           AllowPreOrder = SQLDataHelper.GetBoolean(reader, "AllowPreOrder"),
                           Recomended = SQLDataHelper.GetBoolean(reader, "Recomended"),
                           New = SQLDataHelper.GetBoolean(reader, "New"),
                           BestSeller = SQLDataHelper.GetBoolean(reader, "Bestseller"),
                           OnSale = SQLDataHelper.GetBoolean(reader, "OnSale"),
                           BrandId = SQLDataHelper.GetInt(reader, "BrandID", 0),
                           UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                           CategoryEnabled = SQLDataHelper.GetBoolean(reader, "CategoryEnabled"),
                           Unit = SQLDataHelper.GetString(reader, "Unit"),
                           ShippingPrice = SQLDataHelper.GetFloat(reader, "ShippingPrice"),
                           Multiplicity = SQLDataHelper.GetFloat(reader, "Multiplicity"),
                           MinAmount = SQLDataHelper.GetNullableFloat(reader, "MinAmount"),
                           MaxAmount = SQLDataHelper.GetNullableFloat(reader, "MaxAmount"),
                           SalesNote = SQLDataHelper.GetString(reader, "SalesNote"),
                           HasMultiOffer = SQLDataHelper.GetBoolean(reader, "HasMultiOffer")
                       };
        }

        /// <summary>
        /// get product by productId
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static Product GetProduct(int productId)
        {
            return SQLDataAccess.ExecuteReadOne("[Catalog].[sp_GetProductById]", CommandType.StoredProcedure, GetProductFromReader,
                                                new SqlParameter("@ProductID", productId), new SqlParameter("@Type", PhotoType.Product.ToString()));
        }

        public static int GetProductId(string artNo)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Top(1) ProductID FROM [Catalog].[Product] WHERE ArtNo=@artNo", CommandType.Text, new SqlParameter("@artNo", artNo));
        }

        public static int GetProductIdByName(string name)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Top(1) ProductID FROM [Catalog].[Product] WHERE Name=@Name", CommandType.Text, new SqlParameter("@Name", name));
        }

        public static Product GetFirstProduct()
        {
            var productId = SQLDataAccess.ExecuteScalar<int>("Select TOP(1) productId from Catalog.ProductCategories", CommandType.Text);
            return GetProduct(productId);
        }

        /// <summary>
        /// get product by artNo
        /// </summary>
        /// <param name="artNo"></param>
        /// <returns></returns>
        public static Product GetProduct(string artNo)
        {
            var productId = GetProductId(artNo);
            return productId > 0 ? GetProduct(productId) : null;
        }

        /// <summary>
        /// get product by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Product GetProductByName(string name)
        {
            var productId = GetProductIdByName(name);
            return productId > 0 ? GetProduct(productId) : null;
        }

        /// <summary>
        /// get product by offer artno
        /// </summary>
        /// <param name="artNo"></param>
        /// <returns></returns>
        public static Product GetProductByOfferArtNo(string artNo)
        {
            var productId = SQLDataAccess.ExecuteScalar<int>("Select Top(1) ProductID FROM [Catalog].[Offer] WHERE ArtNo=@artNo", CommandType.Text, new SqlParameter("@artNo", artNo));
            return productId > 0 ? GetProduct(productId) : null;
        }

        /// <summary>
        /// Get all products id
        /// </summary>
        /// <returns></returns>
        public static List<int> GetProductsIDs()
        {
            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>("SELECT [ProductID] FROM [Catalog].[Product]", CommandType.Text, "ProductID").ToList();
        }
        #endregion

        #region ProductLinks


        public static int DeleteAllProductLink(int productId)
        {
            var res = SQLDataAccess.ExecuteReadList<int>("Select [CategoryID] FROM [Catalog].[ProductCategories] WHERE [ProductID] =  @ProductId",
                                                        CommandType.Text,
                                                        reader => SQLDataHelper.GetInt(reader, "CategoryID"),
                                                        new SqlParameter("@ProductID", productId));
            foreach (var item in res)
            {
                CacheManager.Remove(CacheNames.GetCategoryCacheObjectName(item));
            }

            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Catalog].[ProductCategories] WHERE [ProductID] =  @ProductId", CommandType.Text, new SqlParameter("@ProductID", productId));

            return 0;
        }
        /// <summary>
        /// delete relationship between product and category
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="catId"></param>
        /// <returns></returns>
        public static int DeleteProductLink(int productId, int catId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_RemoveProductFromCategory]", CommandType.StoredProcedure, new SqlParameter("@ProductID", productId), new SqlParameter("@CategoryID", catId));
            CacheManager.Remove(CacheNames.GetCategoryCacheObjectName(catId));

            return 0;
        }

        /// <summary>
        /// enabled active trigger
        /// </summary>
        public static void EnableDynamicProductLinkRecalc()
        {
            SQLDataAccess.ExecuteNonQuery("ALTER TABLE [Catalog].[ProductCategories] ENABLE TRIGGER [InsertProductInCategory];" +
                                         " ALTER TABLE [Catalog].[ProductCategories] ENABLE TRIGGER [RemoveProductFromCategory];" +
                                         " ALTER TABLE [Catalog].[Product] ENABLE TRIGGER [EnabledChanged];", CommandType.Text);
        }

        /// <summary>
        /// disabled active trigger
        /// </summary>
        public static void DisableDynamicProductLinkRecalc()
        {
            SQLDataAccess.ExecuteNonQuery("ALTER TABLE [Catalog].[ProductCategories] DISABLE TRIGGER [InsertProductInCategory];" +
                                         " ALTER TABLE [Catalog].[ProductCategories] DISABLE TRIGGER [RemoveProductFromCategory];" +
                                         " ALTER TABLE [Catalog].[Product] DISABLE TRIGGER [EnabledChanged];",
                                            CommandType.Text);
        }

        /// <summary>
        /// add relationship between product and category
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="catId"></param>
        /// <returns></returns>
        public static int AddProductLink(int productId, int catId, int sortOrder = 0)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_AddProductToCategory]", CommandType.StoredProcedure,
                                            new SqlParameter("@ProductID", productId),
                                            new SqlParameter("@CategoryID", catId),
                                            new SqlParameter("@sortOrder", sortOrder));
            CategoryService.ClearCategoryCache();
            return 1;
        }

        /// <summary>
        /// Update relationship
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="sort"></param>
        /// <param name="cat"></param>
        /// <returns></returns>
        public static bool UpdateProductLinkSort(int productid, int sort, int cat)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_UpdateProductLinkSort]", CommandType.StoredProcedure,
                                            new SqlParameter { ParameterName = "@ProductID", Value = productid },
                                            new SqlParameter { ParameterName = "@CategoryID", Value = cat },
                                            new SqlParameter { ParameterName = "@SortOrder", Value = sort });

            return true;
        }

        #endregion

        #region Is Enabled
        /// <summary>
        /// Cheak if product enabled
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static bool IsProductEnabled(int productId)
        {
            var res = SQLDataAccess.ExecuteScalar<bool>("SELECT ([Enabled] & [CategoryEnabled]) as Enabled FROM [Catalog].[Product] WHERE [ProductID] = @id", CommandType.Text, new SqlParameter("@id", productId));

            return res;
        }

        /// <summary>
        /// disabled all products
        /// </summary>
        public static void DisableAllProducts()
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_DisableAllProducts]", CommandType.StoredProcedure);
            CategoryService.ClearCategoryCache();
        }

        #endregion

        #region Filtered Select
        /// <summary>
        /// get all products
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Product> GetAllProducts()
        {
            const string query = @"select * FROM [Catalog].[Product] LEFT JOIN [Catalog].[Photo] ON [Photo].[ObjId] = [Product].[ProductID] AND Type=@Type AND Photo.[Main] = 1";
            return SQLDataAccess.ExecuteReadIEnumerable<Product>(query, CommandType.Text, GetProductFromReader, new SqlParameter("@Type", PhotoType.Product.ToString()));
        }

        /// <summary>
        /// Get products without category
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<int> GetProductIDsWithoutCategory()
        {
            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>(
                    "SELECT [Product].[ProductID] FROM [Catalog].[Product] WHERE [Product].[ProductID] not in (select distinct [ProductID] from [Catalog].[ProductCategories])",
                    CommandType.Text,
                    "ProductID");
        }

        /// <summary>
        /// get products in categories
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<int> GetProductIDsInCategories()
        {
            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>(
                    "select distinct [ProductID] from [Catalog].[ProductCategories]",
                    CommandType.Text,
                    "ProductID");
        }



        #endregion

        #region Products Count and Existance
        /// <summary>
        /// get products count
        /// </summary>
        /// <returns></returns>
        public static int GetProductsCount(string condition = null, params SqlParameter[] parameters)
        {
            if (condition == null)
            {
                return SQLDataAccess.ExecuteScalar<int>("SELECT Count([ProductID]) FROM [Catalog].[Product]",
                                                           CommandType.Text);
            }
            else
            {
                return SQLDataAccess.ExecuteScalar<int>("SELECT Count([ProductID]) FROM [Catalog].[Product]" + " " + condition,
                                                           CommandType.Text, parameters);
            }

        }

        /// <summary>
        /// cheak exist product by productid
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static bool IsExists(int productId)
        {
            bool boolres = SQLDataAccess.ExecuteScalar<int>("[Catalog].[sp_GetProductCOUNTbyID]", CommandType.StoredProcedure, new SqlParameter("@ProductID", productId)) > 0;
            return boolres;
        }

        #endregion

        #region Offers
        #endregion

        #region Photos
        public static void AddProductPhotoByArtNo(string artNo, string fullfileName, string description, bool isMain, int? colorID)
        {
            AddProductPhotoByProductId(GetProductId(artNo), fullfileName, description, isMain, colorID);
        }

        public static void AddProductPhotoByProductId(int productId, string fullfilename, string description, bool isMain, int? colorID)
        {
            if (string.IsNullOrWhiteSpace(fullfilename) || (!IsExists(productId))) return;

            var tempName = PhotoService.AddPhoto(new Photo(0, productId, PhotoType.Product)
                                                   {
                                                       Description = description,
                                                       OriginName = Path.GetFileName(fullfilename),
                                                       PhotoSortOrder = 0,
                                                       ColorID = colorID
                                                   });

            if (string.IsNullOrWhiteSpace(tempName)) return;
            File.Copy(fullfilename, FoldersHelper.GetImageProductPathAbsolut(ProductImageType.Big, tempName));

            using (var image = Image.FromFile(fullfilename))
            {
                FileHelpers.SaveProductImageUseCompress(tempName, image);
            }
        }

        public static void UpdateProductPhotoByProductId(int productId, string fullfilename, string description, bool isMain, int? colorID)
        {
            if (string.IsNullOrWhiteSpace(fullfilename) || (!IsExists(productId))) return;

            var productPhoto = PhotoService.GetProductPhoto(productId, Path.GetFileName(fullfilename));
            if (productPhoto == null || !productPhoto.PhotoName.IsNotEmpty()) return;
            productPhoto.ColorID = colorID;
            PhotoService.UpdateProductPhoto(productPhoto);

            using (var image = Image.FromFile(fullfilename))
            {
                FileHelpers.SaveProductImageUseCompress(productPhoto.PhotoName, image);
            }
        }


        #endregion

        #region Product Price Change

        public static void IncrementAllProductsPrice(float value, bool percent, bool bySupply)
        {
            ChangeAllProductsPrice(value, percent, false, bySupply);
        }

        public static void DecrementAllProductsPrice(float value, bool percent, bool bySupply)
        {
            ChangeAllProductsPrice(value, percent, true, bySupply);
        }

        private static void ChangeAllProductsPrice(float value, bool percent, bool negative, bool bySupply)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_ChangeAllProductsPrice]", CommandType.StoredProcedure,
                                                new SqlParameter("@Value", value),
                                                new SqlParameter("@Percent", percent),
                                                new SqlParameter("@Negative", negative),
                                                new SqlParameter("@bySupply", bySupply)
                                                );

        }
        #endregion

        public static void SetMainLink(int productId, int categoryId)
        {
            SQLDataAccess.ExecuteNonQuery("[Catalog].[sp_SetMainCategoryLink]", CommandType.StoredProcedure, new SqlParameter("@ProductID", productId), new SqlParameter("@CategoryID", categoryId));
        }

        public static bool IsMainLink(int productId, int categoryId)
        {
            return
                SQLDataAccess.ExecuteReadColumn<bool>(
                    "SELECT Main FROM [Catalog].[ProductCategories] WHERE [ProductID] = @ProductID AND [CategoryID] = @CategoryID",
                    CommandType.Text, "Main", new SqlParameter("@ProductID", productId),
                    new SqlParameter("@CategoryID", categoryId)).FirstOrDefault();
        }

        public static float CalculateProductPrice(float price, float productDiscount, CustomerGroup customerGroup, IList<EvaluatedCustomOptions> customOptions, bool withProductDiscount)
        {
            float customOptionPrice = 0;
            if (customOptions != null)
            {
                customOptionPrice = CustomOptionsService.GetCustomOptionPrice(price, customOptions);
            }

            if (!withProductDiscount)
            {
                productDiscount = 0;
            }

            float groupDiscount = customerGroup.CustomerGroupId == 0 ? 0 : customerGroup.GroupDiscount;

            float finalDiscount = Math.Max(productDiscount, groupDiscount);

            return (price + customOptionPrice) * (100 - finalDiscount) / 100;
        }


        public static void SetActive(int productId, bool active)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Update [Catalog].[Product] Set Enabled = @Enabled Where ProductID = @ProductID",
                 CommandType.Text,
                 new SqlParameter("@ProductID", productId),
                 new SqlParameter("@Enabled", active));
        }


        public static void SetBrand(int productId, int brandId)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [Catalog].[Product] SET BrandID = @BrandID WHERE ProductID = @ProductID", CommandType.Text, new SqlParameter("@ProductID", productId), new SqlParameter("@BrandID", brandId));
        }

        public static void DeleteBrand(int productId)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [Catalog].[Product] SET BrandID = NULL Where ProductID = @ProductID", CommandType.Text, new SqlParameter("@ProductID", productId));
        }

        public static List<string> GetForAutoCompleteByIds(string productIds, bool social)
        {
            if (string.IsNullOrEmpty(productIds))
                return new List<string>();

            return
                SQLDataAccess.ExecuteReadList<string>(
                    "Select Product.ProductID, Product.Name, Product.ArtNo, Product.UrlPath from Catalog.Product "
                    +
                    " Inner Join (select item, sort from [Settings].[ParsingBySeperator](@productIds,'/') ) as dtt on Product.ProductId=convert(int, dtt.item) "
                    + " Where Enabled = 1 And CategoryEnabled = 1 "
                    + " order by dtt.sort",
                    CommandType.Text,
                    reader =>
                    string.Format("<a href=\"{2}\">{0}<span>({1})</span></a>", SQLDataHelper.GetString(reader, "Name"),
                                  SQLDataHelper.GetString(reader, "ArtNo"),
                                  social
                                      ? "social/detailssocial.aspx?productid=" + SQLDataHelper.GetInt(reader, "ProductID")
                                      : UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(reader, "UrlPath"), SQLDataHelper.GetInt(reader, "ProductID"))),
                    new SqlParameter("@productIds", productIds));
        }

        public static List<Product> GetForAutoCompleteProductsByIds(string productIds)
        {
            return SQLDataAccess.ExecuteReadList<Product>(
                "Select Product.ProductID, Product.Name, Product.ArtNo from Catalog.Product "
                + " Inner Join (select item, sort from [Settings].[ParsingBySeperator](@productIds,'/') ) as dtt on Product.ProductId=convert(int, dtt.item) "
                + " Where Enabled = 1 And CategoryEnabled = 1 "
                + " order by dtt.sort",
                CommandType.Text,
                reader => new Product()
                {
                    ProductId = SQLDataHelper.GetInt(reader, "ProductID"),
                    ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                    Name = SQLDataHelper.GetString(reader, "Name")
                },
                new SqlParameter("@productIds", productIds));
        }


        public static void MarkersFromString(Product product, string source)
        {
            // b,n,r,s
            if (!string.IsNullOrWhiteSpace(source))
            {
                var items = source.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                product.BestSeller = !string.IsNullOrEmpty(items.FirstOrDefault(item => item == "b"));
                product.New = !string.IsNullOrEmpty(items.FirstOrDefault(item => item == "n"));
                product.Recomended = !string.IsNullOrEmpty(items.FirstOrDefault(item => item == "r"));
                product.OnSale = !string.IsNullOrEmpty(items.FirstOrDefault(item => item == "s"));
            }
            else
            {
                product.BestSeller = product.New = product.Recomended = product.OnSale = false;
            }
        }

        public static string MarkersToString(Product product)
        {
            // b,n,r,s
            string res = string.Empty;
            res += product.BestSeller ? "b," : string.Empty;
            res += product.New ? "n," : string.Empty;
            res += product.Recomended ? "r," : string.Empty;
            res += product.OnSale ? "s," : string.Empty;
            if (res.Length > 0)
                res = res.Remove(res.Length - 1, 1);
            return res;
        }

        //public static void PreCalcProductParams(int productId)
        //{
        //    SQLDataAccess.ExecuteNonQuery("[Catalog].[PreCalcProductParams]", CommandType.StoredProcedure,
        //                        new SqlParameter("@ProductId", productId));
        //}

        //public static void PreCalcProductParamsMass()
        //{
        //    using (var db = new SQLDataAccess())
        //    {
        //        db.cmd.CommandText = "[Catalog].[PreCalcProductParamsMass]";
        //        db.cmd.CommandType = CommandType.StoredProcedure;
        //        db.cmd.CommandTimeout = 600;
        //        db.cnOpen();
        //        db.cmd.ExecuteNonQuery();
        //        db.cnClose();
        //    }

        //    SQLDataAccess.ExecuteNonQuery("[Catalog].[PreCalcProductParamsMass]", CommandType.StoredProcedure);
        //}

        //public static void PreCalcProductParamsMassInBackground()
        //{
        //    var worker = new System.Threading.Thread(PreCalcProductParamsMass) { IsBackground = true };
        //    worker.Start();
        //}

    }
}