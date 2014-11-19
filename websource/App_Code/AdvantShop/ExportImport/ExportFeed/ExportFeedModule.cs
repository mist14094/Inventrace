//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Configuration;
using AdvantShop.Core.SQL;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.ExportImport
{
    public abstract class ExportFeedModule
    {
        protected string ShopUrl;
        protected abstract string ModuleName { get; }
        public abstract void GetExportFeedString(string filenameAndPath);

        protected ExportFeedModule()
        {
            ShopUrl = SettingsMain.SiteUrl;
        }

        protected int GetCategoriesCount(string moduleName)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Settings].[sp_GetExportFeedCategories]",
                                                                               CommandType.StoredProcedure,
                                                                               new SqlParameter("@moduleName", moduleName),
                                                                               new SqlParameter("@onlyCount", true));
        }

        protected IEnumerable<ExportFeedCategories> GetCategories(string moduleName)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<ExportFeedCategories>("[Settings].[sp_GetExportFeedCategories]",
                                                                               CommandType.StoredProcedure,
                                                                               reader => new ExportFeedCategories
                                                                                             {
                                                                                                 Id = SQLDataHelper.GetInt(reader, "CategoryID"),
                                                                                                 ParentCategory = SQLDataHelper.GetInt(reader, "ParentCategory"),
                                                                                                 Name = SQLDataHelper.GetString(reader, "Name")
                                                                                             },
                                                                               new SqlParameter("@moduleName", moduleName),
                                                                               new SqlParameter("@onlyCount", false));
        }

        protected int GetProdutsCount(string moduleName)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Settings].[sp_GetExportFeedProducts]",
                                                                               CommandType.StoredProcedure,
                                                                               new SqlParameter("@moduleName", moduleName),
                                                                               new SqlParameter("@selectedCurrency", ExportFeed.GetModuleSetting(moduleName, "Currency")),
                                                                               new SqlParameter("@onlyCount", true));
        }
        protected IEnumerable<ExportFeedProduts> GetProduts(string moduleName)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<ExportFeedProduts>("[Settings].[sp_GetExportFeedProducts]",
                                                                               CommandType.StoredProcedure,
                                                                               reader => new ExportFeedProduts
                                                                               {
                                                                                   ProductId = SQLDataHelper.GetInt(reader, "ProductID"),
                                                                                   OfferId = SQLDataHelper.GetInt(reader, "OfferId"),
                                                                                   ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                                                                                   Amount = SQLDataHelper.GetInt(reader, "Amount"),
                                                                                   UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                                                                                   Price = SQLDataHelper.GetFloat(reader, "Price"),
                                                                                   Discount = SQLDataHelper.GetFloat(reader, "Discount"),
                                                                                   ParentCategory = SQLDataHelper.GetInt(reader, "ParentCategory"),
                                                                                   Name = SQLDataHelper.GetString(reader, "Name"),
                                                                                   Description = SQLDataHelper.GetString(reader, "Description"),
                                                                                   BriefDescription = SQLDataHelper.GetString(reader, "BriefDescription"),
                                                                                   Photos = SQLDataHelper.GetString(reader, "Photos"),
                                                                                   SalesNote = SQLDataHelper.GetString(reader, "SalesNote"),
                                                                                   ColorId = SQLDataHelper.GetInt(reader, "ColorId"),
                                                                                   ColorName = SQLDataHelper.GetString(reader, "ColorName"),
                                                                                   SizeId = SQLDataHelper.GetInt(reader, "SizeId"),
                                                                                   SizeName = SQLDataHelper.GetString(reader, "SizeName"),
                                                                                   BrandName = SQLDataHelper.GetString(reader, "BrandName"),
                                                                                   Main = SQLDataHelper.GetBoolean(reader, "Main"),
                                                                               },
                                                                               new SqlParameter("@moduleName", moduleName),
                                                                               new SqlParameter("@selectedCurrency", ExportFeed.GetModuleSetting(moduleName, "Currency")),
                                                                               new SqlParameter("@onlyCount", false));
        }

        protected List<Currency> GetCurrencies()
        {
            var dataTable = SQLDataAccess.ExecuteReadList("SELECT CurrencyID, CurrencyValue, CurrencyIso3 FROM [Catalog].[Currency];",
                                                                                           CommandType.Text,
                                                                                           reader =>
                                                                                           {
                                                                                               var currency = new Currency
                                                                                                   {
                                                                                                       CurrencyID = SQLDataHelper.GetInt(reader, "CurrencyID"),
                                                                                                       Value = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                                                                                                       Iso3 = SQLDataHelper.GetString(reader, "CurrencyIso3"),
                                                                                                   };

                                                                                               return currency;
                                                                                           });

            return dataTable;
        }

        protected string GetDefaultCurrencyISO3()
        {
            var result = SQLDataAccess.ExecuteScalar<string>("SELECT Value FROM [Settings].[Settings] WHERE [Name] = \'DefaultCurrencyISO3\';", CommandType.Text);
            return result;
        }

        protected string GetImageProductPath(string photoPath)
        {
            if (string.IsNullOrEmpty(photoPath))
                photoPath = "";

            photoPath = photoPath.Trim();

            if (photoPath.ToLower().Contains("http://"))
                return photoPath;
            return ShopUrl.TrimEnd('/') + "/" + FoldersHelper.GetImageProductPath(ProductImageType.Big, photoPath, false);
        }
    }
}