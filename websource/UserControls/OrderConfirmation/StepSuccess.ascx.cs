using System;
using AdvantShop;

public partial class UserControls_OrderConfirmation_FifthStep : System.Web.UI.UserControl
{
    public int OrderID { get; set; }

    public string Number { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        BindData();
    }

    public void BindData()
    {
        lnkToDefault.Href = AdvantShop.Core.UrlRewriter.UrlService.GetAbsoluteLink("");
        btnPrintOrder.OnClientClick =
            string.Format("javascript:open_printable_version('PrintOrder.aspx?OrderNumber={0}'); return false;", Number);

        if (OrderID != 0)
        {
            Trial.TrackEvent(Trial.TrialEvents.CheckoutOrder, OrderID.ToString());
        }
    }
}