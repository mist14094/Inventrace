using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class AlfabankUaControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid ||
                       ValidateFormData(new[] { txtPartnerId }, null, null)
                           ? new Dictionary<string, string>
                               {
                                   {AlfabankUaTemplate.PartnerId, txtPartnerId.Text}                                 
                               }
                           : null;
            }
            set
            {
                txtPartnerId.Text = value.ElementOrDefault(AlfabankUaTemplate.PartnerId);
            }
        }
    }
}