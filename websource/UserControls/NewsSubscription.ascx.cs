//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

public partial class UserControls_NewsSubscription : System.Web.UI.UserControl
{
    protected void btnSubmit_Click(object sender, System.EventArgs e)
    {
        Page.Response.Redirect("~/subscribe.aspx?emailtosubscribe=" + txtEmail.Text);
    }
}
