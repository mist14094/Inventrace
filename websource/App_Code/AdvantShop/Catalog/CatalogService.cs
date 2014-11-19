//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using AdvantShop.Repository.Currencies;
using Resources;
using AdvantShop.Customers;

namespace AdvantShop.Catalog
{
    public enum SortOrder
    {
        NoSorting,
        AscByName,
        DescByName,
        AscByPrice,
        DescByPrice,
        AscByRatio,
        DescByRatio,
    }

    public class CatalogService
    {
        #region  GetStringPrice

        public static string GetStringPrice(float price)
        {
            return GetStringPrice(price, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Symbol, 0, 1, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, null, false);
        }

        public static string GetStringPrice(float price, bool isWrap)
        {
            return GetStringPrice(price, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Symbol, 0, 1, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, null, isWrap);
        }

        public static string GetStringPrice(float price, float currentCurrencyRate, string currentCurrencyIso3)
        {
            return GetStringPrice(price, 1, currentCurrencyIso3, currentCurrencyRate);
        }

        public static string GetStringPrice(float price, Currency currency)
        {
            return GetStringPrice(price, currency.Value, currency.Symbol, 0, 1, currency.IsCodeBefore, currency.PriceFormat, null, false);
        }

        public static string GetStringPrice(float price, float discount)
        {
            return GetStringPrice(price, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Symbol, discount, 1, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, null, false);
        }

        public static string GetStringPrice(float price, float discount, float amount)
        {
            return GetStringPrice(price, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Symbol, discount, amount, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, null, false);
        }

        public static string GetStringPrice(float price, float discount, float amount, string zeroPriceMsg)
        {
            return GetStringPrice(price, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Symbol, discount, amount, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, zeroPriceMsg, false);
        }

        public static string GetStringPrice(float price, float qty, string currencyCode, float currencyRate)
        {
            Currency cur = CurrencyService.Currency(currencyCode);
            if (cur == null)
                return GetStringPrice(price, currencyRate, currencyCode, 0, qty, false, CurrencyService.DefaultPriceFormat, null, false);
            return GetStringPrice(price, currencyRate, cur.Symbol, 0, qty, cur.IsCodeBefore, cur.PriceFormat, null, false);
        }

        private static string GetStringPrice(float price, float currentCurrencyRate, string currentCurrencyCode, float discount, float amount, bool isCodeBefore, string priceFormat, string zeroPriceMsg, bool isWrap)
        {
            if ((price == 0 || amount == 0) && !String.IsNullOrEmpty(zeroPriceMsg))
            {
                return zeroPriceMsg;
            }

            string strPriceRes;
            if (discount == 0)
            {
                strPriceRes = String.IsNullOrEmpty(priceFormat) ? Math.Round((price * amount) / currentCurrencyRate, 2).ToString() : String.Format("{0:" + priceFormat + "}", Math.Round((price * amount) / currentCurrencyRate, 2));
            }
            else
            {
                float dblTemp = (price * amount) / currentCurrencyRate;
                strPriceRes = String.IsNullOrEmpty(priceFormat) ? Math.Round(dblTemp - ((dblTemp / 100) * discount), 2).ToString() : String.Format("{0:" + priceFormat + "}", Math.Round(dblTemp - ((dblTemp / 100) * discount), 2));
            }

            string strCurrencyFormat;

            if (isWrap)
            {
                strCurrencyFormat = isCodeBefore ? "<span class=\"curr\">{1}</span> <span class=\"price-num\">{0}</span>" : "<span class=\"price-num\">{0}</span> <span class=\"curr\">{1}</span>";
            }
            else
            {
                strCurrencyFormat = isCodeBefore ? "{1} {0}" : "{0} {1}";
            }

            return String.Format(strCurrencyFormat, strPriceRes, currentCurrencyCode);
        }


        public static string GetStringDiscountPercent(float price, float discount, bool boolAddMinus)
        {
            return GetStringDiscountPercent(price, discount, CurrencyService.CurrentCurrency.Value, CurrencyService.CurrentCurrency.Symbol, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, boolAddMinus);
        }

        public static string GetStringDiscountPercent(float price, float discount, float currentCurrencyRate, string currentCurrencyCode, bool boolAddMinus)
        {
            return GetStringDiscountPercent(price, discount, currentCurrencyRate, currentCurrencyCode, CurrencyService.CurrentCurrency.IsCodeBefore, CurrencyService.CurrentCurrency.PriceFormat, boolAddMinus);
        }

        public static string GetStringDiscountPercent(float price, float discount, float currentCurrencyRate, string currentCurrencyCode, bool isCodeBefore, string priceFormat, bool boolAddMinus)
        {
            var strFormat = String.Empty;
            var dblRes = Math.Round(((price / 100) * discount) / currentCurrencyRate, 2);

            string strFormatedPrice = priceFormat == "" ? dblRes.ToString() : String.Format("{0:" + priceFormat + "}", dblRes);

            if (boolAddMinus)
            {
                strFormat = "-";
            }

            if (isCodeBefore)
            {
                strFormat += "{1}{0} ({2}%)";
            }
            else
            {
                strFormat += "{0}{1} ({2}%)";
            }

            return String.Format(strFormat, strFormatedPrice, currentCurrencyCode, discount);
        }

        #endregion

        //todo Vladimir точно нужна эта функция?
        public static float CalculatePrice(float price, float discount)
        {
            return (price - ((price / 100) * discount));
        }

        public static float CalculateProductPrice(float price, float productDiscount, CustomerGroup customerGroup, IList<EvaluatedCustomOptions> customOptions)
        {
            float customOptionPrice = 0;
            if (customOptions != null)
            {
                customOptionPrice = CustomOptionsService.GetCustomOptionPrice(price, customOptions);
            }

            float groupDiscount = customerGroup.CustomerGroupId == 0 ? 0 : customerGroup.GroupDiscount;

            float finalDiscount = Math.Max(productDiscount, groupDiscount);

            return price * (100 - finalDiscount) / 100 + customOptionPrice;
        }

        public static string FormatPriceInvariant(object price)
        {
            return String.Format("{0:##,##0.##}", price);
        }

        public static string RenderLabels(bool recomend, bool sales, bool best, bool news, float discount, int labelCount = 5)
        {
            var labels = new StringBuilder();
            labels.Append("<span class=\"label-p\">");

            if (discount > 0 && labelCount-- > 0)
                labels.AppendFormat("<span class=\"disc\">{0} {1}%</span>", Resource.Client_Catalog_Discount, FormatPriceInvariant(discount));
            if (recomend && labelCount-- > 0)
                labels.AppendFormat("<span class=\"recommend\">{0}</span>", Resource.Client_Catalog_LabelRecomend);
            if (sales && labelCount-- > 0)
                labels.AppendFormat("<span class=\"sales\">{0}</span>", Resource.Client_Catalog_LabelSales);
            if (best && labelCount-- > 0)
                labels.AppendFormat("<span class=\"best\">{0}</span>", Resource.Client_Catalog_LabelBest);
            if (news && labelCount > 0)
                labels.AppendFormat("<span class=\"new\">{0}</span>", Resource.Client_Catalog_LabelNew);

            labels.Append("</span>");

            return labels.ToString();
        }

        /// <summary>
        /// Render product price
        /// </summary>
        /// <param name="productPrice">product price</param>
        /// <param name="discount">total discount price</param>
        /// <param name="showDiscount">display discount</param>
        /// <param name="customerGroup">customer group</param>
        /// <param name="customOptions">custom options</param>
        /// <param name="isWrap">wrap</param>
        /// <returns></returns>
        public static string RenderPrice(float productPrice, float discount, bool showDiscount, CustomerGroup customerGroup, string customOptions = null, bool isWrap = false)
        {
            if (productPrice == 0)
            {
                return String.Format("<div class=\'price\'>{0}</div>", Resource.Client_Catalog_ContactWithUs);
            }
            string res;

            float price = CalculateProductPrice(productPrice, 0, customerGroup, CustomOptionsService.DeserializeFromXml(customOptions));
            float priceWithDiscount = CalculateProductPrice(productPrice, discount, customerGroup, CustomOptionsService.DeserializeFromXml(customOptions));


            float groupDiscount = customerGroup.CustomerGroupId == 0 ? 0 : customerGroup.GroupDiscount;

            float finalDiscount = Math.Max(discount, groupDiscount);


            if (price == priceWithDiscount || !showDiscount)
            {
                res = String.Format("<div class=\'price\'>{0}</div>", GetStringPrice(priceWithDiscount, isWrap));
            }
            else
            {
                res = String.Format("<div class=\"price-old\">{0}</div><div class=\"price\">{1}</div><div class=\"price-benefit\">{2} {3} {4} {5}% </div>",
                                    GetStringPrice(productPrice),
                                    GetStringPrice(priceWithDiscount),
                                    Resource.Client_Catalog_Discount_Benefit,
                                    GetStringPrice(productPrice - priceWithDiscount),
                                    Resource.Client_Catalog_Discount_Or,
                                    FormatPriceInvariant(finalDiscount));
            }

            return res;
        }

        public static string RenderSelectedOptions(string xml)
        {
            if (String.IsNullOrEmpty(xml))
            {
                return String.Empty;
            }
            var result = new StringBuilder("<div class=\"customoptions\">");

            foreach (var item in CustomOptionsService.DeserializeFromXml(xml))
            {
                result.Append(item.CustomOptionTitle);
                result.Append(": ");
                result.Append(item.OptionTitle);
                if (item.OptionPriceBc != 0)
                {
                    result.Append(" ");
                    if (item.OptionPriceBc > 0)
                    {
                        result.Append("+");
                    }
                    result.Append(GetStringPrice(item.OptionPriceBc));
                }
                result.Append("<br />");
            }

            result.Append("</div>");

            return result.ToString();
        }
    }
}