using System;
using System.Linq;

public partial class UserControls_Catalog_SizeColorPicker : System.Web.UI.UserControl
{
    public int ProductId { get; set; }
    public string Colors { get; set; }
    public int DefaultColorID { get; set; }

    public int ImageHeight { get; set; }
    public int ImageWidth { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        
    }
}