using System;
using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class DibsControl : ParametersControl
    {
        public void Page_Load(object sender, EventArgs e)
        {
        
            //ddlCurrencies.SelectedValue = Parameters[DibsTemplate.Currency];
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid ||
                       ValidateFormData(new[] { txtMerchant, txtCurrencyRate })
                           ? new Dictionary<string, string>
                               {
                                   {DibsTemplate.Merchant, txtMerchant.Text},
                                   //{DibsTemplate.Account, txtAccount.Text},
                                   {DibsTemplate.CurrencyRate, txtCurrencyRate.Text},
                                   {DibsTemplate.Currency, ddlCurrencies.SelectedValue},
                               }
                           : null;
            }
            set
            {
                txtMerchant.Text = value.ElementOrDefault(DibsTemplate.Merchant);
                //txtAccount.Text = value.ElementOrDefault(DibsTemplate.Account);
                txtCurrencyRate.Text = value.ElementOrDefault(DibsTemplate.CurrencyRate);

                ddlCurrencies.Items.Clear();
                ddlCurrencies.SelectedValue = null;

                var items = new List<object>();
                foreach (var currency in Dibs.GetCurrencies())
                {
                    items.Add(new
                        {
                            value = currency.Key,
                            text = currency.Value
                        });
                }

                ddlCurrencies.DataSource = items;
                ddlCurrencies.DataBind();

                ddlCurrencies.SelectedValue = value.ElementOrDefault(DibsTemplate.Currency);
            }
        }
    }
}