using System;
using System.Linq;
using AdvantShop.Catalog;

public partial class UserControls_Details_SizeColorPicker : System.Web.UI.UserControl
{
    public Product Product { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        Visible = Product.Offers.Any(o => o.ColorID != null || o.SizeID != null);
    }
}