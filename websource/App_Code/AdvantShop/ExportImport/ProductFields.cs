//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
using Resources;

namespace AdvantShop.ExportImport
{
    public class ProductFields
    {
        public enum Fields
        {
            None,
            Sku,
            Name, //required field
            ParamSynonym,
            Category, //required field
            Enabled, //required field
            Price, //required field
            PurchasePrice,
            Amount,
            MultiOffer,
            Unit,
            Discount,
            ShippingPrice,
            Weight,
            Size,
            BriefDescription,
            Description,
            Title,
            H1,
            MetaKeywords,
            MetaDescription,
            Photos,
            Markers,
            Properties,
            Producer,
            OrderByRequest,
            SalesNote,
            Sorting,
            Related,
            Alternative,
            CustomOption
        }

        public static string GetStringNameByEnum(Fields item)
        {
            switch (item)
            {
                case Fields.None:
                    return "none";
                case Fields.Sku:
                    return "sku";
                case Fields.Name:
                    return "name";
                case Fields.ParamSynonym:
                    return "paramsynonym";
                case Fields.Category:
                    return "category";
                case Fields.Enabled:
                    return "enabled";
                case Fields.Price:
                    return "price";
                case Fields.PurchasePrice:
                    return "purchaseprice";
                case Fields.Amount:
                    return "amount";
                case Fields.Unit:
                    return "unit";
                case Fields.Discount:
                    return "discount";
                case Fields.ShippingPrice:
                    return "shippingprice";
                case Fields.Weight:
                    return "weight";
                case Fields.Size:
                    return "size";
                case Fields.BriefDescription:
                    return "briefdescription";
                case Fields.Description:
                    return "description";
                case Fields.Title:
                    return "title";
                case Fields.H1:
                    return "h1";
                case Fields.MetaKeywords:
                    return "metakeywords";
                case Fields.MetaDescription:
                    return "metadescription";
                case Fields.Photos:
                    return "photos";
                case Fields.Markers:
                    return "markers";
                case Fields.Properties:
                    return "properties";
                case Fields.Producer:
                    return "producer";
                case Fields.OrderByRequest:
                    return "preorder";
                case Fields.SalesNote :
                    return "salesnote";
                case Fields.MultiOffer:
                    return "sku:size:color:price:purchaseprice:amount";
                case Fields.Sorting:
                    return "sorting";
                case Fields.Related:
                    return "related sku";
                case Fields.Alternative:
                    return "alternative sku";
                case Fields.CustomOption:
                    return "customoption";

            }
            return string.Empty;
        }

        public static string GetDisplayNameByEnum(Fields item)
        {
            switch (item)
            {
                case Fields.None:
                    return Resource.ProductFields_NotSelected;
                case Fields.Sku:
                    return Resource.ProductFields_Sku;
                case Fields.Name:
                    return Resource.ProductFields_Name;
                case Fields.ParamSynonym:
                    return Resource.ProductFields_Synonym;
                case Fields.Category:
                    return Resource.ProductFields_Categories;
                case Fields.Enabled:
                    return Resource.ProductFields_Enabled;
                case Fields.Price:
                    return Resource.ProductFields_Price;
                case Fields.PurchasePrice:
                    return Resource.ProductFields_PurchasePrice;
                case Fields.Amount:
                    return Resource.ProductFields_Amount;
                case Fields.Unit:
                    return Resource.ProductFields_Unit;
                case Fields.Discount:
                    return Resource.ProductFields_Discount;
                case Fields.ShippingPrice:
                    return Resource.ProductFields_ShippingPrice;
                case Fields.Weight:
                    return Resource.ProductFields_Weight;
                case Fields.Size:
                    return Resource.ProductFields_Size;
                case Fields.BriefDescription:
                    return Resource.ProductFields_BriefDescription;
                case Fields.Description:
                    return Resource.ProductFields_Description;
                case Fields.Title:
                    return Resource.ProductFields_SeoTitle;
                case Fields.H1:
                    return "H1";
                case Fields.MetaKeywords:
                    return Resource.ProductFields_MetaKeywords;
                case Fields.MetaDescription:
                    return Resource.ProductFields_MetaDescription;
                case Fields.Photos:
                    return Resource.ProductFields_Photos;
                case Fields.Markers:
                    return Resource.ProductFields_Markers;
                case Fields.Properties:
                    return Resource.ProductFields_Properties;
                case Fields.Producer:
                    return Resource.ProductFields_Producer;
                case Fields.OrderByRequest:
                    return Resource.ProductFields_PreOrder;
                case Fields.SalesNote:
                    return Resource.ProductFields_SalesNote;
                case Fields.MultiOffer:
                    return Resource.ProductFields_MultiOffer;
                case Fields.Sorting:
                    return Resource.ProductFields_Sorting;
                case Fields.Related:
                    return Resource.ProductFields_RelatedSKU;
                case Fields.Alternative:
                    return Resource.ProductFields_AlternativeSKU;
                case Fields.CustomOption:
                    return "CustomOption";
            }
            return string.Empty;
        }
    }
}