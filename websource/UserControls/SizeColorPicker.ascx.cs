using System;
using System.Linq;
using AdvantShop.Catalog;

public partial class UserControls_Details_SizeColorPicker : System.Web.UI.UserControl
{
    public int ProductId { get; set; }
    public eType Type { get; set; }

    public enum eType { 
        Full = 0,
        Compact = 1
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        
    }
}