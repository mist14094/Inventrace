using System;
using System.Linq;
using System.Web.UI;
using AdvantShop.Catalog;

namespace UserControls.Details
{
    public partial class ProductPhotoView : UserControl
    {
        public Product Product { set; get; }
        protected Photo MainPhoto { set; get; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Product.ProductPhotos.Any())
            {
                var mainOffer = Product.Offers.FirstOrDefault(item => item.Main);
                if (mainOffer != null )
                {
                    if (mainOffer.ColorID != null)
                    {
                       MainPhoto = Product.ProductPhotos.FirstOrDefault(item => item.ColorID == mainOffer.ColorID) ??
                                 Product.ProductPhotos.OrderByDescending(item => item.Main).ThenByDescending(item => item.PhotoSortOrder)
                                           .FirstOrDefault() ?? new Photo(0, Product.ProductId, PhotoType.Product);
                    }
                    else
                    {
                    MainPhoto = Product.ProductPhotos.OrderByDescending(item => item.Main).ThenBy(item => item.PhotoSortOrder).FirstOrDefault(item => item.Main) ?? new Photo(0, Product.ProductId, PhotoType.Product);
                    }
                }else{
					MainPhoto = Product.ProductPhotos.OrderByDescending(item => item.Main).ThenBy(item => item.PhotoSortOrder).FirstOrDefault(item => item.Main) ?? new Photo(0, Product.ProductId, PhotoType.Product);
				}

                lvPhotos.DataSource = Product.ProductPhotos;
                lvPhotos.DataBind();

                carouselDetails.Visible = lvPhotos.Items.Count > 1 || Product.Offers.Count > 1;
                pnlPhoto.Visible = true;
                pnlNoPhoto.Visible = false;
            }
            else
            {
                pnlPhoto.Visible = false;
                pnlNoPhoto.Visible = true;
            }
        }
    }
}