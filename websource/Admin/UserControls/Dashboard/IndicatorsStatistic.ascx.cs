using System;
using AdvantShop.Statistic;

public partial class Admin_UserControls_IndicatorsStatistic : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        lblSale.Text = Convert.ToString(StatisticService.GetOrdersCountByDateRange(DateTime.Now.AddYears(-100), DateTime.Now));
        lblSaleMounth.Text = Convert.ToString(StatisticService.GetOrdersCountByDateRange(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now));

        lblSaleWeek.Text = Convert.ToString(StatisticService.GetOrdersCountByDateRange(DateTime.Now.AddDays(-Convert.ToInt32(DateTime.Now.DayOfWeek)), DateTime.Now));
        lblSaleToday.Text = Convert.ToString(StatisticService.GetOrdersCountByDate(DateTime.Now));
        lblSaleYesterday.Text = Convert.ToString(StatisticService.GetOrdersCountByDate(DateTime.Now.AddDays(-1)));
        //lblTotalOrders.Text = Convert.ToString(StatisticService.GetOrdersCount());
        lblTotalProducts.Text = Convert.ToString(StatisticService.GetProductsCount());
    }
}