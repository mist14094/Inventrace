//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.SaasData;
using AdvantShop.Statistic;

namespace AdvantShop.ExportImport
{
    public class ImportProduct
    {
        private static bool useMultiThreadImport = false;

        public static void UpdateInsertProduct(Dictionary<ProductFields.Fields, string> productInStrings)
        {
            if (useMultiThreadImport)
            {
                var added = false;
                while (!added)
                {
                    int workerThreads;
                    int asyncIoThreads;
                    ThreadPool.GetAvailableThreads(out workerThreads, out asyncIoThreads);
                    if (workerThreads != 0)
                    {
                        ThreadPool.QueueUserWorkItem(UpdateInsertProductWorker, productInStrings);
                        added = true;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            else
            {
                UpdateInsertProductWorker(productInStrings);
            }
        }

        private static void UpdateInsertProductWorker(object o)
        {
            //return;

            var productInStrings = (Dictionary<ProductFields.Fields, string>)o;
            try
            {
                bool addingNew;
                Product product = null;
                if (productInStrings.ContainsKey(ProductFields.Fields.Sku) && productInStrings[ProductFields.Fields.Sku].IsNullOrEmpty())
                    throw new Exception("SKU can not be empty");

                var artNo = productInStrings.ContainsKey(ProductFields.Fields.Sku) ? productInStrings[ProductFields.Fields.Sku] : string.Empty;
                if (string.IsNullOrEmpty(artNo))
                {
                    addingNew = true;
                }
                else
                {
                    product = ProductService.GetProduct(artNo);
                    addingNew = product == null;
                }

                if (addingNew)
                {
                    product = new Product { ArtNo = string.IsNullOrEmpty(artNo) ? null : artNo, Multiplicity=1};
                }

                if (productInStrings.ContainsKey(ProductFields.Fields.Name))
                    product.Name = productInStrings[ProductFields.Fields.Name];
                else
                    product.Name = product.Name ?? string.Empty;

                if (productInStrings.ContainsKey(ProductFields.Fields.Enabled))
                {
                    product.Enabled = productInStrings[ProductFields.Fields.Enabled].Trim().Equals("+");
                }
                else
                {
                    product.Enabled = true;
                }

                if (productInStrings.ContainsKey(ProductFields.Fields.OrderByRequest))
                    product.AllowPreOrder = productInStrings[ProductFields.Fields.OrderByRequest].Trim().Equals("+");

                if (productInStrings.ContainsKey(ProductFields.Fields.Discount))
                    product.Discount = SQLDataHelper.GetFloat(productInStrings[ProductFields.Fields.Discount]);

                if (productInStrings.ContainsKey(ProductFields.Fields.Weight))
                    product.Weight = SQLDataHelper.GetFloat(productInStrings[ProductFields.Fields.Weight]);

                if (productInStrings.ContainsKey(ProductFields.Fields.Size))
                    product.Size = GetSizeForBdFormat(productInStrings[ProductFields.Fields.Size]);

                if (productInStrings.ContainsKey(ProductFields.Fields.BriefDescription))
                    product.BriefDescription = productInStrings[ProductFields.Fields.BriefDescription];

                if (productInStrings.ContainsKey(ProductFields.Fields.Description))
                    product.Description = productInStrings[ProductFields.Fields.Description];

                if (productInStrings.ContainsKey(ProductFields.Fields.SalesNote))
                    product.SalesNote = productInStrings[ProductFields.Fields.SalesNote];
                if (productInStrings.ContainsKey(ProductFields.Fields.ShippingPrice))
                    product.ShippingPrice = SQLDataHelper.GetFloat(productInStrings[ProductFields.Fields.ShippingPrice]);

                if (productInStrings.ContainsKey(ProductFields.Fields.Unit))
                    product.Unit = productInStrings[ProductFields.Fields.Unit];


                if (productInStrings.ContainsKey(ProductFields.Fields.MultiOffer))
                {
                    OfferService.OffersFromString(product, productInStrings[ProductFields.Fields.MultiOffer]);
                }
                else
                {
                    OfferService.OfferFromStrings(product,
                                                  productInStrings.ContainsKey(ProductFields.Fields.Price)
                                                      ? productInStrings[ProductFields.Fields.Price]
                                                      : string.Empty,
                                                  productInStrings.ContainsKey(ProductFields.Fields.PurchasePrice)
                                                      ? productInStrings[ProductFields.Fields.PurchasePrice]
                                                      : string.Empty,
                                                  productInStrings.ContainsKey(ProductFields.Fields.Amount)
                                                      ? productInStrings[ProductFields.Fields.Amount]
                                                      : string.Empty);
                }

                if (productInStrings.ContainsKey(ProductFields.Fields.ParamSynonym))
                {
                    var prodUrl = productInStrings[ProductFields.Fields.ParamSynonym].IsNotEmpty()
                                      ? productInStrings[ProductFields.Fields.ParamSynonym]
                                      : product.ArtNo;
                    product.UrlPath = UrlService.GetEvalibleValidUrl(product.ID, ParamType.Product, prodUrl);
                }
                else
                {
                    product.UrlPath = product.UrlPath ??
                                      UrlService.GetEvalibleValidUrl(product.ID, ParamType.Product,
                                      product.ArtNo ?? product.Name.Substring(0, product.Name.Length-1 < 50 ? product.Name.Length-1: 50));

                }

                product.Meta.ObjId = product.ProductId;

                if (productInStrings.ContainsKey(ProductFields.Fields.Title))
                    product.Meta.Title = productInStrings[ProductFields.Fields.Title];
                else
                    product.Meta.Title = product.Meta.Title ?? SettingsSEO.ProductMetaTitle;

                if (productInStrings.ContainsKey(ProductFields.Fields.H1))
                    product.Meta.H1 = productInStrings[ProductFields.Fields.H1];
                else
                    product.Meta.H1 = product.Meta.H1 ?? SettingsSEO.ProductMetaH1;

                if (productInStrings.ContainsKey(ProductFields.Fields.MetaKeywords))
                    product.Meta.MetaKeywords = productInStrings[ProductFields.Fields.MetaKeywords];
                else
                    product.Meta.MetaKeywords = product.Meta.MetaKeywords ?? SettingsSEO.ProductMetaKeywords;

                if (productInStrings.ContainsKey(ProductFields.Fields.MetaDescription))
                    product.Meta.MetaDescription = productInStrings[ProductFields.Fields.MetaDescription];
                else
                    product.Meta.MetaDescription = product.Meta.MetaDescription ?? SettingsSEO.ProductMetaDescription;

                if (productInStrings.ContainsKey(ProductFields.Fields.Markers))
                    ProductService.MarkersFromString(product, productInStrings[ProductFields.Fields.Markers]);

                if (productInStrings.ContainsKey(ProductFields.Fields.Producer))
                    product.BrandId = BrandService.BrandFromString(productInStrings[ProductFields.Fields.Producer]);
                
                if (!addingNew)
                {
                    ProductService.UpdateProduct(product, false);
                    //Log(string.Format(Resource.Admin_Import1C_Updated, product.Name, product.ArtNo));
                    CommonStatistic.TotalUpdateRow++;
                }
                else
                {
                    if (!(SaasDataService.IsSaasEnabled && ProductService.GetProductsCount() >= SaasDataService.CurrentSaasData.ProductsCount))
                    {
                        ProductService.AddProduct(product, false);
                        //Log(string.Format(Resource.Admin_Import1C_Added, product.Name, product.ArtNo));
                        CommonStatistic.TotalAddRow++;
                    }
                }

                if (product.ProductId > 0)
                    OtherFields(productInStrings, product.ProductId);
            }
            catch (Exception e)
            {
                CommonStatistic.TotalErrorRow++;
                Log(CommonStatistic.RowPosition + ": " + e.Message);
            }

            productInStrings.Clear();
            CommonStatistic.RowPosition++;
        }


        private static void OtherFields(Dictionary<ProductFields.Fields, string> fields, int productId)
        {
            //Category
            if (fields.ContainsKey(ProductFields.Fields.Category))
            {
                string sorting = string.Empty;
                if (fields.ContainsKey(ProductFields.Fields.Sorting))
                {
                    sorting = fields[ProductFields.Fields.Sorting];
                }
                var parentCategory = fields[ProductFields.Fields.Category];
                CategoryService.SubParseAndCreateCategory(parentCategory, productId, sorting);
            }

            //photo
            if (fields.ContainsKey(ProductFields.Fields.Photos))
            {
                string photos = fields[ProductFields.Fields.Photos];
                if (!string.IsNullOrEmpty(photos))
                    PhotoService.PhotoFromString(productId, photos);
            }

            //Properties
            if (fields.ContainsKey(ProductFields.Fields.Properties))
            {
                string properties = fields[ProductFields.Fields.Properties];
                PropertyService.PropertiesFromString(productId, properties);
            }

            if (fields.ContainsKey(ProductFields.Fields.CustomOption ))
            {
                string customOption = fields[ProductFields.Fields.CustomOption];
                CustomOptionsService.CustomOptionsFromString(productId, customOption);
            }
        }

        private static void Log(string message)
        {
            CommonStatistic.WriteLog(message);
        }

        private static string GetSizeForBdFormat(string str)
        {
            if (string.IsNullOrEmpty(str)) return "0|0|0";

            var listSymb = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',', '.' };
            string res = string.Empty;
            var list = new List<string>();
            foreach (char t in str)
            {
                if (listSymb.Contains(t))
                {
                    res += t;
                }
                else
                {
                    if (!string.IsNullOrEmpty(res))
                    {
                        list.Add(res.Trim());
                        res = string.Empty;
                    }
                }
            }
            if (!string.IsNullOrEmpty(res))
                list.Add(res.Trim());

            res = list.AggregateString('|');

            return res;
        }

        public static void PostProcess(Dictionary<ProductFields.Fields, string> productInStrings)
        {
            if (productInStrings.ContainsKey(ProductFields.Fields.Sku))
            {
                var artNo = productInStrings[ProductFields.Fields.Sku];
                int productId = ProductService.GetProductId(artNo);

                //relations
                if (productInStrings.ContainsKey(ProductFields.Fields.Related))
                {
                    string linkproducts = productInStrings[ProductFields.Fields.Related];
                    ProductService.LinkedProductFromString(productId, linkproducts, RelatedType.Related);
                }

                //relations
                if (productInStrings.ContainsKey(ProductFields.Fields.Alternative))
                {
                    string linkproducts = productInStrings[ProductFields.Fields.Alternative];
                    ProductService.LinkedProductFromString(productId, linkproducts, RelatedType.Alternative);
                }
            }
            CommonStatistic.RowPosition++;
        }
    }
}