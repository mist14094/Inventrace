//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.itmcompany.ru
//--------------------------------------------------

using System;
using System.Web.UI;
using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Payment;
using Resources;

namespace ClientPages
{

    public partial class Check_SberBank : AdvantShopClientPage
    {

        private SberBank _bill;
        private Order _order;

        protected bool EmptyCheck
        {
            get { return OrderNumber == null || Order == null || Bill == null; }
        }

        protected string OrderNumber
        {
            get { return Request["ordernumber"]; }
        }

        protected Order Order
        {
            get { return _order ?? (_order = OrderService.GetOrderByNumber(OrderNumber)); }
        }

        //private int MethodID
        //{
        //    get
        //    {
        //        int id = 0;
        //        return int.TryParse(Request["methodid"], out id) ? id : 0;
        //    }
        //}

        protected SberBank Bill
        {
            get
            {
                if (_bill != null)
                    return _bill;

                //if (MethodID == 0)
                //    return null;
                //PaymentMethod method = PaymentService.GetPaymentMethod(Order.PaymentMethodId);
                if (!(Order.PaymentMethod is SberBank))
                    return null;
                _bill = (SberBank) Order.PaymentMethod;
                return _bill;
            }
        }

        protected override void InitializeCulture()
        {
            AdvantShop.Localization.Culture.InitializeCulture();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (EmptyCheck || Order == null)
                return;

            lPaymentDescr.Text = Resource.Client_OrderConfirmation_PayOrder + @" #" + Order.OrderID;
            lPaymentDescr2.Text = lPaymentDescr.Text;

            //var bill = new Bill_PaymentModule();

            lCompanyName.Text = Bill.CompanyName;
            lTransactAccount.Text = Bill.TransAccount;
            lBankName.Text = Bill.BankName;
            lINN.Text = Bill.INN;
            lKPP.Text = Bill.KPP;
            pnlKpp.Visible = Bill.KPP.IsNotEmpty();
            lBIK.Text = Bill.BIK;
            lCorrespondentAccount.Text = Bill.CorAccount;

            //CurVal = bill.CurRate;

            lCompanyName2.Text = Bill.CompanyName;
            lTransactAccount2.Text = Bill.TransAccount;
            lBankName2.Text = Bill.BankName;
            lINN2.Text = Bill.INN;
            lKPP2.Text = Bill.KPP;
            pnlKpp2.Visible = Bill.KPP.IsNotEmpty();
            lBIK2.Text = Bill.BIK;
            lCorrespondentAccount2.Text = Bill.CorAccount;

            if (!string.IsNullOrEmpty(Order.PaymentDetails.CompanyName))
            {
                lPayer.Text = Request["bill_companyname"];
            }
            else
            {
                lPayer.Text = Order.BillingContact.Name;
            }

            if (!string.IsNullOrEmpty(Order.PaymentDetails.INN))
            {
                lPayerINN.Text = Request["bill_inn"];
            }
            else
            {
                lPayerINN.Text = @"___________________";
            }

            lPayer2.Text = lPayer.Text;
            lPayerINN2.Text = lPayerINN.Text;

            lPayerAddress.Text += Order.BillingContact.Country + @", " + Order.BillingContact.Zone + @", " +
                                  Order.BillingContact.City;

            if (string.IsNullOrEmpty(Order.BillingContact.Zip))
            {
                lPayerAddress.Text += @", " + Order.BillingContact.Zip;
            }

            lPayerAddress.Text += @", " + Order.BillingContact.Address;
            lPayerAddress2.Text = lPayerAddress.Text;
            float priceInBaseCurrency = Order.Sum/Order.OrderCurrency.CurrencyValue;
            lWholeSum.Text = Math.Floor(priceInBaseCurrency).ToString();
            lWholeSum2.Text = lWholeSum.Text;
            lSumFractPart.Text =
                SQLDataHelper.GetInt(
                    Math.Round(priceInBaseCurrency - Math.Floor(priceInBaseCurrency), 2)*100).
                              ToString();
            lSumFractPart2.Text = lSumFractPart.Text;
        }
    }
}