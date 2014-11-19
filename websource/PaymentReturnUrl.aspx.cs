//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Controls;
using AdvantShop.Payment;

namespace ClientPages
{
    public partial class PaymentReturnUrl : AdvantShopClientPage
    {
        protected int PaymentMethodID
        {
            get
            {
                int id;
                return int.TryParse(Request["paymentmethodid"], out id) ? id : 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PaymentMethodID < 1)
                return;

            var method = PaymentService.GetPaymentMethod(PaymentMethodID);
            if (method != null && (method.NotificationType & NotificationType.ReturnUrl) == NotificationType.ReturnUrl)
                lblResult.Text = @"Статус оплаты: " + method.ProcessResponse(Context);
        }
    }
}