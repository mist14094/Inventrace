using System;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.FilePath;

public partial class Admin_Catalog_AdminCategoryView : System.Web.UI.UserControl
{
    public int CategoryID { get; set; }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        var categories = CategoryService.GetChildCategoriesByCategoryId(CategoryID, false);
        lvCategories.DataSource = categories;
        lvCategories.DataBind();

        lblCategories.Text = categories.Count.ToString();

        commandPanel.Visible = categories.Count != 0;
    }

    protected string RenderCategoryImage(string photoName, string categoryName)
    {
        return string.Format("<img src='{0}' alt='{1}' height='{2}' />", photoName.IsNotEmpty() ? FoldersHelper.GetImageCategoryPath(CategoryImageType.Small, photoName, true) :
                            "../images/nophoto_small.jpg", categoryName, SettingsPictureSize.SmallCategoryImageHeight);
    }

    protected void lvCategories_OnItemCommand(object sender, ListViewCommandEventArgs e)
    {
        if (string.Equals("DeleteCategory", e.CommandName))
        {
            CategoryService.DeleteCategoryAndPhotos(Convert.ToInt32(e.CommandArgument));
            CategoryService.RecalculateProductsCountManual();
        }
    }

    protected void lbDeleteSelectedCategories_Click(object sender, EventArgs e)
    {
        foreach (var item in lvCategories.Items)
        {
            var ckb = (CheckBox) item.FindControl("ckbSelectCategory");
            if (ckb != null && ckb.Checked)
            {
                int categoryId = 0;
                if (Int32.TryParse(ckb.Attributes["data-categorylist-categoryid"], out categoryId))
                {
                    CategoryService.DeleteCategoryAndPhotos(categoryId);
                }
            }
        }
        CategoryService.RecalculateProductsCountManual();
    }
}