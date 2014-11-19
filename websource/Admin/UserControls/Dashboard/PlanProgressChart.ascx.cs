using System;
using AdvantShop.Orders;

namespace Admin.UserControls.Dashboard
{
    public partial class PlanProgressChart : System.Web.UI.UserControl
    {
        protected double planPercent;
	protected float sales;

        protected void Page_Load(object sender, EventArgs e)
        {
            sales = (float)Math.Round(OrderStatisticsService.GetMonthProgress().Key);

            planPercent =( sales / OrderStatisticsService.SalesPlan) * 100;
        }
    }
}