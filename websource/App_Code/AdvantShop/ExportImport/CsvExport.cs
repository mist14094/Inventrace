//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Helpers.CsvHelper;
using AdvantShop.SEO;
using AdvantShop.Statistic;
using CsvHelper;
using Resources;

namespace AdvantShop.ExportImport
{
    public class CsvExport
    {
        private const int MaxCellLength = 60000;

        public static void SaveProductsToCsv(string path, Encodings.EncodingsEnum encodeType, Separators.SeparatorsEnum delimetr, List<ProductFields.Fields> fieldMapping)
        {
            using (var writer = new CsvWriter(new StreamWriter(path, false, Encodings.GetEncoding(encodeType))))
            {
                writer.Configuration.Delimiter = Separators.GetCharSeparator(delimetr);
                foreach (var item in fieldMapping)
                    writer.WriteField(ProductFields.GetStringNameByEnum(item));
                writer.NextRecord();
                var products = ProductService.GetAllProducts();
                if (products == null) return;

                foreach (var product in products)
                {
                    if (!CommonStatistic.IsRun) return;

                    if (fieldMapping.Contains(ProductFields.Fields.Description) && product.Description.Length > MaxCellLength)
                    {
                        CommonStatistic.WriteLog(string.Format(Resource.Admin_ExportCsv_TooLargeDescription, product.Name, product.ArtNo));
                        CommonStatistic.TotalErrorRow++;
                        continue;
                    }

                    if (fieldMapping.Contains(ProductFields.Fields.BriefDescription) && product.BriefDescription.Length > MaxCellLength)
                    {
                        CommonStatistic.WriteLog(string.Format(Resource.Admin_ExportCsv_TooLargeBriefDescription, product.Name, product.ArtNo));
                        CommonStatistic.TotalErrorRow++;
                        continue;
                    }

                    var meta = MetaInfoService.GetMetaInfo(product.ID, MetaType.Product) ??
                        new MetaInfo(0, 0, MetaType.Product, string.Empty, string.Empty, string.Empty, string.Empty);
                    for (int i = 0; i < fieldMapping.Count; i++)
                    {
                        var item = fieldMapping[i];
                        if (item == ProductFields.Fields.Sku)
                            writer.WriteField(product.ArtNo);
                        if (item == ProductFields.Fields.Name)
                            writer.WriteField(product.Name);

                        if (item == ProductFields.Fields.ParamSynonym)
                            writer.WriteField(product.UrlPath);

                        if (item == ProductFields.Fields.Category)
                            writer.WriteField((CategoryService.GetCategoryStringByProductId(product.ProductId)));

                        if (item == ProductFields.Fields.Sorting)
                            writer.WriteField((CategoryService.GetCategoryStringByProductId(product.ProductId, true)));

                        if (item == ProductFields.Fields.Enabled)
                            writer.WriteField(product.Enabled ? "+" : "-");

                        if (!product.HasMultiOffer)
                        {
                            var offer = product.Offers.FirstOrDefault() ?? new Offer();
                            if (item == ProductFields.Fields.Price)
                                writer.WriteField(offer.Price.ToString("F2"));
                            if (item == ProductFields.Fields.PurchasePrice)
                                writer.WriteField(offer.SupplyPrice.ToString("F2"));
                            if (item == ProductFields.Fields.Amount)
                                writer.WriteField(offer.Amount.ToString());

                            if (item == ProductFields.Fields.MultiOffer)
                                writer.WriteField(string.Empty);
                        }
                        else
                        {
                            if (item == ProductFields.Fields.Price)
                                writer.WriteField(string.Empty);
                            if (item == ProductFields.Fields.PurchasePrice)
                                writer.WriteField(string.Empty);
                            if (item == ProductFields.Fields.Amount)
                                writer.WriteField(string.Empty);
                            if (item == ProductFields.Fields.MultiOffer)
                                writer.WriteField(OfferService.OffersToString(product.Offers));
                        }

                        if (item == ProductFields.Fields.Unit)
                            writer.WriteField(product.Unit);
                        if (item == ProductFields.Fields.ShippingPrice)
                            writer.WriteField(product.ShippingPrice.ToString("F2"));
                        if (item == ProductFields.Fields.Discount)
                            writer.WriteField(product.Discount.ToString("F2"));
                        if (item == ProductFields.Fields.Weight)
                            writer.WriteField(product.Weight.ToString("F2"));
                        if (item == ProductFields.Fields.Size)
                            writer.WriteField(product.Size.Replace("|", " x "));

                        if (item == ProductFields.Fields.BriefDescription)
                            writer.WriteField(product.BriefDescription);
                        if (item == ProductFields.Fields.Description)
                            writer.WriteField(product.Description);

                        if (item == ProductFields.Fields.Title)
                            writer.WriteField(meta.Title);
                        if (item == ProductFields.Fields.H1)
                            writer.WriteField(meta.H1);
                        if (item == ProductFields.Fields.MetaKeywords)
                            writer.WriteField(meta.MetaKeywords);
                        if (item == ProductFields.Fields.MetaDescription)
                            writer.WriteField(meta.MetaDescription);
                        if (item == ProductFields.Fields.Markers)
                            writer.WriteField(ProductService.MarkersToString(product));
                        if (item == ProductFields.Fields.Photos)
                            writer.WriteField(PhotoService.PhotoToString(product.ProductPhotos));
                        if (item == ProductFields.Fields.Properties)
                            writer.WriteField(PropertyService.PropertiesToString(product.ProductPropertyValues));

                        if (item == ProductFields.Fields.Producer)
                            writer.WriteField(BrandService.BrandToString(product.BrandId));
                        
                        if (item == ProductFields.Fields.OrderByRequest)
                            writer.WriteField(product.AllowPreOrder ? "+" : "-");
                        if (item == ProductFields.Fields.SalesNote)
                            writer.WriteField(product.SalesNote);

                        if (item == ProductFields.Fields.Related)
                            writer.WriteField(ProductService.LinkedProductToString(product.ProductId, RelatedType.Related));

                        if (item == ProductFields.Fields.Alternative)
                            writer.WriteField(ProductService.LinkedProductToString(product.ProductId, RelatedType.Alternative));

                        if (item == ProductFields.Fields.CustomOption)
                            writer.WriteField(CustomOptionsService.CustomOptionsToString(CustomOptionsService.GetCustomOptionsByProductId(product.ProductId)));
                    }
                    writer.NextRecord();
                    CommonStatistic.RowPosition++;
                }
            }
        }
    }
}