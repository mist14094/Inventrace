using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Catalog;

namespace Admin.UserControls.Product
{
    public partial class ProductOffers : System.Web.UI.UserControl
    {
        public int ProductID
        {
            set { ViewState["ProductID"] = value; }
            get { return (int)ViewState["ProductID"]; }
        }

        public string ArtNo
        {
            set { ViewState["ArtNo"] = value; }
            get { return (string)ViewState["ArtNo"]; }
        }


        public bool HasMultiOffer
        {
            set { ViewState["HasMultiOffer"] = value; }
            get { return (bool)ViewState["HasMultiOffer"]; }
        }

        public List<Offer> Offers
        {
            set { ViewState["Offers"] = value; }
            get { return (List<Offer>)ViewState["Offers"]; }
        }


        private bool valid = true;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
                RefreshOffers();
        }

        public bool RefreshOffers()
        {
            if (HasMultiOffer)
            {
                var oldOffers = new List<Offer>();
                oldOffers.AddRange(Offers);

                Offers.Clear();

                foreach (var lvItem in lvOffers.Items)
                {
                    var offer = oldOffers.FirstOrDefault(o => o.OfferId.ToString() == ((HiddenField)lvItem.FindControl("hfOfferID")).Value) ?? new Offer();

                    if (Offers.Any(o => o.ArtNo == ((TextBox)lvItem.FindControl("txtMultySKU")).Text && o.OfferId.ToString() != ((HiddenField)lvItem.FindControl("hfOfferID")).Value) ||
                        OfferService.IsArtNoExist(((TextBox)lvItem.FindControl("txtMultySKU")).Text, offer.OfferId))
                    {
                        lvItem.FindControl("lErrorSKU").Visible = true;
                        valid = false;
                        return false;
                    }
                    else
                    {
                        lvItem.FindControl("lErrorSKU").Visible = false;
                        offer.ArtNo = ((TextBox)lvItem.FindControl("txtMultySKU")).Text;
                    }

                    offer.ProductId = ProductID;
                    offer.Price = ((TextBox)lvItem.FindControl("txtMultiPrice")).Text.TryParseFloat();
                    offer.SupplyPrice = ((TextBox)lvItem.FindControl("txtMultiSupplyPrice")).Text.TryParseFloat();
                    offer.SizeID = ((DropDownList)lvItem.FindControl("ddlMultiSize")).SelectedValue.TryParseInt(true);
                    offer.ColorID = ((DropDownList)lvItem.FindControl("ddlMultiColor")).SelectedValue.TryParseInt(true);
                    offer.Amount = ((TextBox)lvItem.FindControl("txtMultiAmount")).Text.TryParseFloat();
                    offer.Main = ((RadioButton)lvItem.FindControl("cbMultiMain")).Checked;

                    Offers.AddRange(new[] { offer });
                }
            }
            else
            {
                if (Offers == null)
                {
                    Offers = new List<Offer>();
                }

                var singleOffer = Offers.Count > 0 ? Offers[0] : new Offer();
                                     
                singleOffer.ProductId = ProductID;
                singleOffer.Amount = txtAmount.Text.TryParseFloat();
                singleOffer.ArtNo = ArtNo;
                singleOffer.Price = txtPrice.Text.TryParseFloat();
                singleOffer.SupplyPrice = txtSupplyPrice.Text.TryParseFloat();
                singleOffer.Main = true;
                singleOffer.ColorID = null;
                singleOffer.SizeID = null;

                Offers.Clear();
                Offers.Add(singleOffer);

            }
            return true;
        }


        protected void lvOffers_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteOffer")
            {
                Offers.RemoveAll(item => item.OfferId == e.CommandArgument.ToString().TryParseInt());

                if (Offers.Count > 0 && Offers.All(item => !item.Main))
                {
                    Offers[0].Main = true;
                }
            }
        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!valid)
                return;

            mvOffers.SetActiveView(HasMultiOffer ? viewMultiOffer : viewSingleOffer);
            chkMultiOffer.Checked = HasMultiOffer;

            if (HasMultiOffer)
            {
                lvOffers.DataSource = Offers;
                lvOffers.DataBind();
            }
            else
            {

                var singleOffer = Offers != null && Offers.Count > 0 ? Offers.First() : new Offer { Amount = 1 };
                txtSupplyPrice.Text = singleOffer.SupplyPrice.ToString("#0.00");
                txtPrice.Text = singleOffer.Price.ToString("#0.00");
                txtAmount.Text = singleOffer.Amount.ToString();
            }
        }


        protected void lbNewOffer_Click(object sender, EventArgs e)
        {
            Offers.Add(new Offer()
                {
                    OfferId = -1 * (Offers.Count + 1),
                    ProductId = ProductID,
                    Amount = 1,
                    ArtNo = ArtNo + "-" + (Offers.Count + 1),
                    Price = txtPrice.Text.TryParseFloat(),
                    SupplyPrice = txtSupplyPrice.Text.TryParseFloat(),
                    ColorID = null,
                    SizeID = null,
                    Main = Offers.Count(offer=> offer.Main) == 0
                });
        }

        protected void sds_Init(object sender, EventArgs e)
        {
            ((SqlDataSource)sender).ConnectionString = AdvantShop.Connection.GetConnectionString();
        }

        protected void chkMultiOffer_Click(object sender, EventArgs e)
        {
            HasMultiOffer = chkMultiOffer.Checked;
        }

    }
}