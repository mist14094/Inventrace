using System.Collections.Generic;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace Admin.UserControls.PaymentMethods
{
    public partial class MasterBankControl : ParametersControl
    {
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return _valid ||
                       ValidateFormData(new[] { txtTerminal }, null, null)
                           ? new Dictionary<string, string>
                               {
                                   {MasterBankTemplate.Terminal, txtTerminal.Text}
                               }
                           : null;
            }
            set
            {
                txtTerminal.Text = value.ElementOrDefault(MasterBankTemplate.Terminal);

            }
        }

    }
}