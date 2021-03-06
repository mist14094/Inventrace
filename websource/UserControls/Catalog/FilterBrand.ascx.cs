using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using AdvantShop.Catalog;

//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

public partial class UserControls_FilterBrand : UserControl
{
    public List<int> AvalibleBrandIDs { set; get; }
    public List<int> SelectedBrandIDs { set; get; }
    public int CategoryId { get; set; }
    public bool InDepth { get; set; }
    public ProductOnMain.TypeFlag WorkType { set; get; }

    public UserControls_FilterBrand()
    {
        WorkType = ProductOnMain.TypeFlag.None;
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        List<Brand> brands = WorkType == ProductOnMain.TypeFlag.None ? BrandService.GetBrandsByCategoryID(CategoryId, InDepth) : BrandService.GetBrandsByProductOnMain(WorkType);

        if (brands.Any())
        {
            lvBrands.DataSource = brands;
            lvBrands.DataBind();
        }
        else
        {
            this.Visible = false;
        }
    }
}
