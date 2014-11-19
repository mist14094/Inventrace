using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class AvangardControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid ||
                       ValidateFormData(new[] { txtShopId, txtShopPassword })
                           ? new Dictionary<string, string>
                               {
                                   {AvangardTemplate.ShopId, txtShopId.Text},
                                   {AvangardTemplate.ShopPassword, txtShopPassword.Text}
                               }
                           : null;
            }
            set
            {
                txtShopId.Text = value.ElementOrDefault(AvangardTemplate.ShopId);
                txtShopPassword.Text = value.ElementOrDefault(AvangardTemplate.ShopPassword);
            }
        }
    }
}