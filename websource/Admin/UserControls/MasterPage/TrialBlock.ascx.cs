using System;
using System.Web.UI;
using AdvantShop;


namespace Admin.UserControls.MasterPage
{
    public partial class TrialBlock : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Trial.IsTrialEnabled)
            {
                this.Visible = false;
                return;
            }

            lDate.Text = Trial.TrialPeriodTill.ToShortDateString();
        }
    }
}