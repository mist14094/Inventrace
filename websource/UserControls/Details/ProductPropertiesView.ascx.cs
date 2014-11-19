using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using System.Linq;


public partial class UserControls_ProductPropertiesView : System.Web.UI.UserControl
{
    public bool HasProperties { get; private set; }
    public int ProductId { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        var listProperties = PropertyService.GetPropertyValuesByProductId(ProductId);
        var productProperties = new List<PropertyValue>();

        foreach (var itemProperty in listProperties)
        {
            var prop = productProperties.FirstOrDefault(p => p.PropertyId == itemProperty.PropertyId);
            if (prop != null)
                prop.Value += string.Format(", {0}", itemProperty.Value);
            else
                productProperties.Add(itemProperty);
        }

        if (productProperties.Any())
        {
            lvProperties.DataSource = productProperties;
            lvProperties.DataBind();
            HasProperties = true;
        }
        else
        {
            HasProperties = false;
            this.Visible = false;
        }
    }
}