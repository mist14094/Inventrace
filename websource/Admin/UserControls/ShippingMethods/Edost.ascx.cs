using System;
using System.Collections.Generic;
using System.Globalization;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Helpers;
using AdvantShop.Shipping;

namespace Admin.UserControls.ShippingMethods
{
    public partial class EdostControl : ParametersControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            rvRate.MinimumValue = float.Parse("0.0001", CultureInfo.InvariantCulture).ToString();
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid || ValidateFormData(new[] { txtShopId, txtPassword }, new[] { txtRate })
                           ? new Dictionary<string, string>
                               {
                                   {EdostTemplate.ShopId, txtShopId.Text},
                                   {EdostTemplate.Password, txtPassword.Text},
                                   {EdostTemplate.EnabledCOD, chbcreateCOD.Checked.ToString()  },
                                   {EdostTemplate.EnabledPickPoint , chbcreatePickPoint.Checked.ToString()  },
                                   {EdostTemplate.ShipIdCOD, hfCod.Value  },
                                   {EdostTemplate.ShipIdPickPoint, hfPickPoint.Value },
                                   {EdostTemplate.Rate, txtRate.Text }
                               }
                           : null;
            }
            set
            {
                txtShopId.Text = value.ElementOrDefault(EdostTemplate.ShopId);
                txtRate.Text = value.ElementOrDefault(EdostTemplate.Rate);
                txtPassword.Text = value.ElementOrDefault(EdostTemplate.Password);
                txtPassword.Visible = !(Demo.IsDemoEnabled || Trial.IsTrialEnabled);
                lPassword.Visible = Demo.IsDemoEnabled || Trial.IsTrialEnabled;

                chbcreateCOD.Checked = SQLDataHelper.GetBoolean(value.ElementOrDefault(EdostTemplate.EnabledCOD));
                chbcreatePickPoint.Checked = SQLDataHelper.GetBoolean(value.ElementOrDefault(EdostTemplate.EnabledPickPoint));
                hfCod.Value = value.ElementOrDefault(EdostTemplate.ShipIdCOD);
                hfPickPoint.Value = value.ElementOrDefault(EdostTemplate.ShipIdPickPoint);
            }
        }
    }
}