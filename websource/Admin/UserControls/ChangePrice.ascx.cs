using System;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.Repository.Currencies;

namespace Admin.UserControls
{
    public partial class ChangePrice : System.Web.UI.UserControl
    {

        enum eAction
        {
            Decrement = 0,
            Increment = 1,
            IncBySupply = 2
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ddlAction.Items.Add(new ListItem(Resources.Resource.Admin_ChangePrice_Increment, eAction.Increment.ToString()));
                ddlAction.Items.Add(new ListItem(Resources.Resource.Admin_ChangePrice_Decrement, eAction.Decrement.ToString()));
                ddlAction.Items.Add(new ListItem(Resources.Resource.Admin_ChangePrice_IncrementBySupply, eAction.IncBySupply.ToString()));
            }
        }
        protected void btnGo_Click(object sender, EventArgs e)
        {
            lblMessage.Visible = true;
            lblMessage.ForeColor = System.Drawing.Color.FromName("#0000ff");

            try
            {

                float d;
                if (!float.TryParse(txtValue.Text, out d))
                {
                    lblMessage.Text = Resources.Resource.Admin_ChangePrice_Error;
                    return;
                }

                if (ddlAction.SelectedValue == eAction.Decrement.ToString())
                {
                    ProductService.DecrementAllProductsPrice(d, Convert.ToBoolean(ddlPercent.SelectedValue), false);
                    lblMessage.Text = string.Format(Resources.Resource.Admin_ChangePrice_Decreased, d, Convert.ToBoolean(ddlPercent.SelectedValue) ? "%" : Resources.Resource.Admin_ChangePrice_InBasePrice);
                }

                if (ddlAction.SelectedValue == eAction.Increment.ToString())
                {
                    ProductService.IncrementAllProductsPrice(d, Convert.ToBoolean(ddlPercent.SelectedValue), false);
                    lblMessage.Text = string.Format(Resources.Resource.Admin_ChangePrice_Increased, d, Convert.ToBoolean(ddlPercent.SelectedValue) ? "%" : Resources.Resource.Admin_ChangePrice_InBasePrice);
                }

                if (ddlAction.SelectedValue == eAction.IncBySupply.ToString())
                {
                    ProductService.IncrementAllProductsPrice(d, Convert.ToBoolean(ddlPercent.SelectedValue), true);
                    lblMessage.Text = string.Format(Resources.Resource.Admin_ChangePrice_IncreasedBySupply, d, Convert.ToBoolean(ddlPercent.SelectedValue) ? "%" : Resources.Resource.Admin_ChangePrice_InBasePrice);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = ex.Message;
            }
        }

        protected void ddlPercent_Init(object sender, EventArgs e)
        {
            var baseCur = CurrencyService.GetAllCurrencies().FirstOrDefault(cur=> cur.Value == 1);
            if (baseCur != null)
            {
                ddlPercent.Items[0].Text = baseCur.Symbol;
            }
            
            ddlPercent.Items[1].Text = Resources.Resource.Admin_ChangePrice_Percent;
        }
    }
}