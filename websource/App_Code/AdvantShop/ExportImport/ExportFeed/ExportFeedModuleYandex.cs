//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Statistic;

namespace AdvantShop.ExportImport
{
    public class ExportFeedModuleYandex : ExportFeedModule
    {
        private string _currency;
        private string _description;
        private string _salesNotes;
        private bool _delivery;
        protected override string ModuleName
        {
            get { return "YandexMarket"; }
        }

        public static List<string> AvailableCurrencies = new List<string> { "RUB", "UAH", "BYR", "USD", "EUR" };
        public override void GetExportFeedString(string filenameAndPath)
        {
            try
            {
                _currency = ExportFeed.GetModuleSetting(ModuleName, "Currency");
                _description = ExportFeed.GetModuleSetting(ModuleName, "DescriptionSelection");
                _salesNotes = ExportFeed.GetModuleSetting(ModuleName, "SalesNotes");
                _delivery = ExportFeed.GetModuleSetting(ModuleName, "Delivery").TryParseBool();

                var shopName = ExportFeed.GetModuleSetting(ModuleName, "ShopName").Replace("#STORE_NAME#", SettingsMain.ShopName);
                var companyName = ExportFeed.GetModuleSetting(ModuleName, "CompanyName").Replace("#STORE_NAME#", SettingsMain.ShopName);
                FileHelpers.DeleteFile(filenameAndPath);
                using (var outputFile = new FileStream(filenameAndPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    var settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
                    using (var writer = XmlWriter.Create(outputFile, settings))
                    {
                        writer.WriteStartDocument();
                        writer.WriteDocType("yml_catalog", null, "shops.dtd", null);
                        writer.WriteStartElement("yml_catalog");
                        writer.WriteAttributeString("date", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        writer.WriteStartElement("shop");

                        writer.WriteStartElement("name");
                        writer.WriteString(shopName);
                        writer.WriteEndElement();

                        writer.WriteStartElement("company");
                        writer.WriteString(companyName);
                        writer.WriteEndElement();

                        writer.WriteStartElement("url");
                        writer.WriteString(ShopUrl);
                        writer.WriteEndElement();

                        writer.WriteStartElement("currencies");
                        var currencies = GetCurrencies().Where(item => AvailableCurrencies.Contains(item.Iso3)).ToList();
                        ProcessCurrency(currencies, _currency, writer);
                        writer.WriteEndElement();

                        CommonStatistic.TotalRow = GetCategoriesCount(ModuleName) + GetProdutsCount(ModuleName);
                        writer.WriteStartElement("categories");
                        foreach (var categoryRow in GetCategories(ModuleName))
                        {
                            ProcessCategoryRow(categoryRow, writer);
                            CommonStatistic.RowPosition++;
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("offers");

                        foreach (var offerRow in GetProduts(ModuleName))
                        {
                            ProcessProductRow(offerRow, writer);
                            CommonStatistic.RowPosition++;
                        }
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                        writer.WriteEndElement();
                        writer.WriteEndDocument();

                        writer.Flush();
                        writer.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        private static void ProcessCurrency(List<Currency> currencies, string currency, XmlWriter writer)
        {
            if (currencies == null) return;
            var defaultCurrency = currencies.FirstOrDefault(item => item.Iso3 == currency);
            if (defaultCurrency == null) return;
            ProcessCurrencyRow(new Currency
                {
                    CurrencyID = defaultCurrency.CurrencyID,
                    Value = 1,
                    Iso3 = defaultCurrency.Iso3
                }, writer);

            foreach (var curRow in currencies.Where(item => item.Iso3 != currency))
            {
                curRow.Value = curRow.Value / defaultCurrency.Value;
                ProcessCurrencyRow(curRow, writer);
            }
        }

        private static void ProcessCurrencyRow(Currency currency, XmlWriter writer)
        {
            writer.WriteStartElement("currency");
            writer.WriteAttributeString("id", currency.Iso3 == "RUB" ? "RUR" : currency.Iso3);

            var nfi = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            nfi.NumberDecimalSeparator = ".";
            writer.WriteAttributeString("rate", Math.Round(currency.Value, 2).ToString(nfi));
            writer.WriteEndElement();
        }

        private static void ProcessCategoryRow(ExportFeedCategories row, XmlWriter writer)
        {
            writer.WriteStartElement("category");
            writer.WriteAttributeString("id", row.Id.ToString(CultureInfo.InvariantCulture));
            if (row.ParentCategory != 0)
            {
                writer.WriteAttributeString("parentId", row.ParentCategory.ToString(CultureInfo.InvariantCulture));
            }
            writer.WriteString(row.Name);
            writer.WriteEndElement();
        }

        private void ProcessProductRow(ExportFeedProduts row, XmlWriter writer)
        {
            if (string.IsNullOrWhiteSpace(row.BrandName)) ProcessSimpleModel(row, writer);
            else ProcessVendorModel(row, writer);
        }

        private string CreateLink(ExportFeedProduts row)
        {
            var sufix = string.Empty;
            if (!row.Main)
            {
                if (row.ColorId != 0)
                    sufix = "color=" + row.ColorId;
                if (row.SizeId != 0)
                {
                    if (string.IsNullOrEmpty(sufix))
                        sufix = "size=" + row.SizeId;
                    else
                        sufix += "&size=" + row.SizeId;
                }
                sufix = !string.IsNullOrEmpty(sufix) ? "?" + sufix : sufix;
            }
            return ShopUrl.TrimEnd('/') + "/" + UrlService.GetLink(ParamType.Product, row.UrlPath, row.ProductId) + sufix;
        }

        private void ProcessSimpleModel(ExportFeedProduts row, XmlWriter writer)
        {
            //var tempUrl = (_shopUrl.EndsWith("/") ? _shopUrl.TrimEnd('/') : _shopUrl);
            writer.WriteStartElement("offer");
            writer.WriteAttributeString("id", row.OfferId.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("available", (row.Amount > 0).ToString().ToLower());

            writer.WriteStartElement("url");
            writer.WriteString(CreateLink(row));
            writer.WriteEndElement();


            writer.WriteStartElement("price");
            var nfi = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            nfi.NumberDecimalSeparator = ".";
            writer.WriteString(Math.Round(CatalogService.CalculatePrice(row.Price, row.Discount)).ToString(nfi));
            writer.WriteEndElement();

            writer.WriteStartElement("currencyId");
            writer.WriteString(_currency == "RUB" ? "RUR" : _currency);
            writer.WriteEndElement();

            writer.WriteStartElement("categoryId");
            writer.WriteString(row.ParentCategory.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            if (!string.IsNullOrEmpty(row.Photos))
            {
                var temp = row.Photos.Split(',');
                foreach (var item in temp.Where(item => !string.IsNullOrWhiteSpace(item)))
                {
                    writer.WriteStartElement("picture");
                    writer.WriteString(GetImageProductPath(item));
                    writer.WriteEndElement();
                }
            }

            writer.WriteStartElement("name");
            writer.WriteString(row.Name);
            writer.WriteEndElement();

            writer.WriteStartElement("description");
            string desc = SQLDataHelper.GetString(_description == "full" ? row.Description : row.BriefDescription);

            writer.WriteString(desc.XmlEncode().RemoveInvalidXmlChars());

            writer.WriteEndElement();

            if (!string.IsNullOrWhiteSpace(row.SalesNote))
            {
                writer.WriteStartElement("sales_notes");
                writer.WriteString(row.SalesNote);
                writer.WriteEndElement();
            }
            else if (!string.IsNullOrWhiteSpace(_salesNotes))
            {
                writer.WriteStartElement("sales_notes");
                writer.WriteString(_salesNotes);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void ProcessVendorModel(ExportFeedProduts row, XmlWriter writer)
        {
            writer.WriteStartElement("offer");
            writer.WriteAttributeString("id", row.OfferId.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("available", (row.Amount > 0).ToString().ToLower());

            writer.WriteAttributeString("type", "vendor.model");

            writer.WriteStartElement("url");
            writer.WriteString(CreateLink(row));
            writer.WriteEndElement();


            writer.WriteStartElement("price");
            var nfi = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            nfi.NumberDecimalSeparator = ".";
            writer.WriteString(Math.Round(CatalogService.CalculatePrice(row.Price, row.Discount)).ToString(nfi));
            writer.WriteEndElement();

            writer.WriteStartElement("currencyId");
            writer.WriteString(_currency == "RUB" ? "RUR" : _currency);
            writer.WriteEndElement();

            writer.WriteStartElement("categoryId");
            writer.WriteString(row.ParentCategory.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();


            if (!string.IsNullOrEmpty(row.Photos))
            {
                var temp = row.Photos.Split(',');
                foreach (var item in temp.Where(item => !string.IsNullOrWhiteSpace(item)))
                {
                    writer.WriteStartElement("picture");
                    writer.WriteString(GetImageProductPath(item));
                    writer.WriteEndElement();
                }
            }


            writer.WriteStartElement("delivery");
            writer.WriteString(_delivery.ToString().ToLower());
            writer.WriteEndElement();

            writer.WriteStartElement("vendor");
            writer.WriteString(row.BrandName);
            writer.WriteEndElement();

            writer.WriteStartElement("vendorCode");
            writer.WriteString(row.ArtNo);
            writer.WriteEndElement();

            writer.WriteStartElement("model");
            writer.WriteString(row.Name + (!string.IsNullOrWhiteSpace(row.SizeName) ? " " + row.SizeName : string.Empty) + (!string.IsNullOrWhiteSpace(row.ColorName) ? " " + row.ColorName : string.Empty));
            writer.WriteEndElement();

            writer.WriteStartElement("description");
            string desc = SQLDataHelper.GetString(_description == "full" ? row.Description : row.BriefDescription);

            writer.WriteString(desc.XmlEncode().RemoveInvalidXmlChars());

            writer.WriteEndElement();

            if (!string.IsNullOrWhiteSpace(row.SalesNote))
            {
                writer.WriteStartElement("sales_notes");
                writer.WriteString(row.SalesNote);
                writer.WriteEndElement();
            }
            else if (!string.IsNullOrWhiteSpace(_salesNotes))
            {
                writer.WriteStartElement("sales_notes");
                writer.WriteString(_salesNotes);
                writer.WriteEndElement();
            }

            if (!string.IsNullOrWhiteSpace(row.ColorName))
            {
                writer.WriteStartElement("param");
                writer.WriteAttributeString("name", "Цвет");
                writer.WriteString(row.ColorName);
                writer.WriteEndElement();
            }

            if (!string.IsNullOrWhiteSpace(row.SizeName))
            {
                writer.WriteStartElement("param");
                writer.WriteAttributeString("name", "Размер");
                writer.WriteString(row.SizeName);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}