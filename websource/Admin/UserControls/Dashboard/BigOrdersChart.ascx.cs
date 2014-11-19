using System;
using System.Linq;
using AdvantShop;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace Admin.UserControls.Dashboard
{
    public partial class BigOrdersChart : System.Web.UI.UserControl
    {

        protected string RenderData(int days)
        {
            var profit = string.Empty;
            var orders = string.Empty;

            var listFrofit = OrderStatisticsService.GetOrdersProfitByDays(DateTime.Now.AddDays(-days).Date, DateTime.Now );
            var listSum = OrderStatisticsService.GetOrdersSumByDays(DateTime.Now.AddDays(-days).Date, DateTime.Now);

            var valCurrency = CurrencyService.CurrentCurrency.Value;
            profit = listFrofit.Select(item => string.Format("[{0},{1}]", GetTimestamp(item.Key), item.Value / valCurrency)).AggregateString(',');
            orders = listSum.Select(item => string.Format("[{0},{1}]", GetTimestamp(item.Key), item.Value / valCurrency)).AggregateString(',');

            return string.Format("[{{label: '{0}', data:[{1}]}}, {{label: '{2}', data:[{3}]}}]", Resources.Resource.Admin_Chart_Profit, profit, Resources.Resource.Admin_Default_Orders, orders);
        }

        public static long GetTimestamp(System.DateTime date)
        {

            TimeSpan span = (date - new DateTime(1970, 1, 1, 0, 0, 0, 0));

            return (long)(span.TotalSeconds * 1000);
        }


    }
}