<%@ Page Language="C#" MasterPageFile="MasterPage.master" EnableViewState="false"
    CodeFile="Details.aspx.cs" Inherits="ClientPages.Details" %>

<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.FilePath" %>
<%@ Import Namespace="AdvantShop.Customers" %>
<%@ Import Namespace="Resources" %>
<%@ MasterType VirtualPath="MasterPage.master" %>
<%@ Register Src="~/UserControls/Details/CustomOptions.ascx" TagName="CustomOptions"
    TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Details/ProductPhotoView.ascx" TagName="ProductPhotoView"
    TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Details/ProductVideoView.ascx" TagName="ProductVideoView"
    TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Details/ProductPropertiesView.ascx" TagName="ProductPropertiesView"
    TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Details/RelatedProducts.ascx" TagName="RelatedProducts"
    TagPrefix="adv" %>
<%@ Register Src="~/UserControls/Details/ProductReviews.ascx" TagName="ProductReviews"
    TagPrefix="adv" %>
<%@ Register TagPrefix="adv" TagName="BuyInOneClick" Src="~/UserControls/BuyInOneClick.ascx" %>
<%@ Register TagPrefix="adv" TagName="BreadCrumbs" Src="~/UserControls/BreadCrumbs.ascx" %>
<%@ Register TagPrefix="adv" TagName="Rating" Src="~/UserControls/Rating.ascx" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>
<%@ Register TagPrefix="adv" TagName="CompareControl" Src="~/UserControls/Catalog/CompareControl.ascx" %>
<%@ Register TagPrefix="adv" TagName="SizeColorPicker" Src="~/UserControls/Details/SizeColorPickerDetails.ascx" %>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolderHeader" runat="Server">
    <script type="text/javascript" src="http://vk.com/js/api/share.js?11" charset="windows-1251"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolderBottom" runat="Server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="crumbs-thin">
            <adv:BreadCrumbs runat="server" ID="breadCrumbs" />
        </div>
        <div class="block-d">
            <adv:ProductPhotoView runat="server" ID="productPhotoView" />
            <div class="d-info hproduct">
                <h1 class="product-name fn">
                    <%= CurrentProduct.Meta.H1 %></h1>
                <div id="hrefAdmin" runat="server" visible="false">
                    <a target="_blank" class="details-admin" href="<%= "admin/product.aspx?productid=" + Request["productid"] %>">
                        <% = Resource.Client_Details_Link_ShowInClient %></a>
                </div>
                <adv:Rating ID="rating" runat="server" />
                <div class="prop">
                    <div class="prop-str">
                        <span class="param-name">
                            <%= Resource.Client_Details_SKU %>:</span><span class="param-value" id="skuValue"><%= CurrentProduct.ArtNo %></span>
                    </div>
                    <div class="prop-str" id="pnlBrand" runat="server">
                        <span class="param-name">
                            <%= Resource.Client_Details_Brand %>:</span><span class="param-value"> <a href="<%= UrlService.GetLink(ParamType.Brand, CurrentProduct.Brand.UrlPath, CurrentProduct.BrandId) %>" class="brand">
                                <%= CurrentProduct.Brand.Name %></a></span>
                    </div>
                    <div class="category">
                        <%= CurrentProduct.ProductCategories.Count > 0 ? CurrentProduct.ProductCategories[0].Name : string.Empty %>
                    </div>
                    <div class="prop-str" id="pnlSize" runat="server">
                        <span class="param-name">
                            <%= Resource.Client_Details_Size%>:</span><span class="param-value"><%= CurrentProduct.Size.Replace("|", " x ") %>
                            </span>
                    </div>
                    <div class="prop-str" id="pnlWeight" runat="server">
                        <span class="param-name">
                            <%= Resource.Client_Details_Weight%>:</span><span class="param-value"><%= CurrentProduct.Weight %>
                                <%= Resource.Client_Details_KG %></span>
                    </div>
                    <div class="prop-str">
                        <span class="param-name">
                            <%= Resource.Client_Details_Availability %>:</span> <span class="param-value">
                                <asp:Literal ID="lAvailiableAmount" runat="server" /></span>
                    </div>
                    <div class="prop-str" runat="server" id="divAmount">
                        <span class="param-name param-name-txt">
                            <%= Resource.Client_Details_Amount %>:</span> <span class="param-value"><span class="input-wrap">
                                <%=RenderSpinBox()%></span></span>
                    </div>
                    <div class="prop-str" runat="server" id="divUnit">
                        <span class="param-name">
                            <%= Resource.Client_Details_Unit %>:</span> <span class="param-value">
                                <%= CurrentProduct.Unit %></span>
                    </div>
                    <adv:SizeColorPicker runat="server" ID="sizeColorPicker" />
                    <adv:CustomOptions ID="productCustomOptions" runat="server" />
                </div>
                <div id="pnlPrice" runat="server">
                    <div class="price-c">
                        <div id="priceWrap">
                            <%= CatalogService.RenderPrice(CurrentOffer.Price, CurrentProduct.CalculableDiscount, true, CustomerSession.CurrentCustomer.CustomerGroup, CustomOptionsService.SerializeToXml(productCustomOptions.CustomOptions, productCustomOptions.SelectedOptions)) %>
                        </div>
                        <div class="fpayment">
                            <asp:Label ID="lblFirstPayment" runat="server" Visible="False" CssClass="price"></asp:Label>
                        </div>
                        <asp:HiddenField ID="hfFirstPaymentPercent" runat="server" />
                    </div>
                    <div class="btns-d">
                        <adv:Button ID="btnAdd" runat="server" Type="Buy" Size="Big" Text='<%$ Resources:Resource, Client_Details_Add %>'
                            ValidationGroup="cOptions" />
                        <adv:Button ID="btnAddCredit" runat="server" Visible="false" Type="Buy" Size="Big"
                            CssClass="btn-credit" Text='<%$ Resources:Resource, Client_Details_AddCredit %>'
                            ValidationGroup="cOptions" />
                        <asp:Label ID="lblFirstPaymentNote" runat="server" Style="text-transform: none; font-size: 10px;
                            color: gray;" Visible="False">*Первый взнос</asp:Label>
                        <adv:Button ID="btnOrderByRequest" runat="server" Size="Big" Type="Action" Text='<%$ Resources:Resource, Client_Catalog_OrderByRequest %>'
                            DisableValidation="true" OnClientClick="window.location.href=$('base').attr('href') + 'sendrequestonproduct.aspx?offerid='+$(this).attr('data-offerid') +'\&amount=' +$('#txtAmount').val();" />
                        <adv:BuyInOneClick ID="BuyInOneClick" runat="server" />
                    </div>
                    <br class="clear" />
                </div>
                <div class="features">
                    <adv:CompareControl ID="CompareControl" runat="server" />
                    <div id="pnlWishlist" runat="server">
                        <a id="addToWishlist" href="<%= ExistInWishlist ? "wishlist.aspx" : "javascript:void(0);" %>" class="wishlist-link" data-offerid="<%= CurrentOffer != null ? CurrentOffer.OfferId : 0 %>">
                            <%= ExistInWishlist ? Resource.Client_Details_AlreadyInWishList : Resource.Client_Details_AddToWishList %>
                        </a>
                    </div>
                </div>
            </div>
            <div class="sb_details">
                <adv:StaticBlock ID="sbBannerDetails" runat="server" SourceKey="bannerDetails" />
            </div>
            <div class="d-social">
                <div id="pnlBrnadLogo" runat="server">
                    <a href="<%= UrlService.GetLink(ParamType.Brand, CurrentBrand.UrlPath, CurrentBrand.BrandId)%>"
                        title="<%= HttpUtility.HtmlEncode(CurrentBrand.Name) %>">
                        <img src="<%= !String.IsNullOrEmpty(CurrentBrand.BrandLogo.PhotoName) ? FoldersHelper.GetPath(FolderType.BrandLogo, CurrentBrand.BrandLogo.PhotoName, false) : UrlService.GetAbsoluteLink("images/nophoto_xsmall.jpg") %>"
                            alt="<%= HttpUtility.HtmlEncode(CurrentBrand.Name) %>" />
                    </a>
                </div>
                <div class="d-social-block">
                    <adv:StaticBlock Visible="False" ID="sbShareButtons" runat="server" SourceKey="socialShareDetails" />
                </div>
                <div id="tabs-link" data-tabs-links="true">
                </div>
                <div class="d-qr">
                    <asp:Literal ID="ltrlRightColumnModules" runat="server"></asp:Literal>
                </div>
            </div>
            <br class="clear" />
        </div>
        <adv:StaticBlock runat="server" ID="details2" SourceKey="details-2" />
        <div class="tabs tabs-hr" data-plugin="tabs">
            <div class="tabs-headers">
                <div data-tabs-header="true" class="tab-header <%= string.IsNullOrEmpty(CurrentProduct.Description) ? "tab-hidden" : ""%>"
                    id="tab-descr">
                    <span class="tab-inside">
                        <%= Resource.Client_Details_Description %></span>
                </div>
                <div data-tabs-header="true" class="tab-header <%= !productPropertiesView.HasProperties ? "tab-hidden" : ""%>"
                    id="tab-property">
                    <span class="tab-inside">
                        <%= Resource.Client_Details_Properties %></span>
                </div>
                <div data-tabs-header="true" class="tab-header" id="tab-video">
                    <span class="tab-inside">
                        <%= Resource.Client_Details_Video %></span>
                </div>
                <asp:ListView ID="lvTabsTitles" runat="server" ItemPlaceholderID="itemPlaceholderID">
                    <LayoutTemplate>
                        <div runat="server" id="itemPlaceholderID">
                        </div>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <div data-tabs-header="true" class="tab-header" id="<%# "tab-" + Eval("TabTitleId") %>">
                            <span class="tab-inside">
                                <%# Eval("Title")%></span>
                        </div>
                    </ItemTemplate>
                </asp:ListView>
                <div class="tabs-border">
                </div>
            </div>
            <div class="tabs-contents">
                <div data-tabs-content="true" class="tab-content description">
                    <% = CurrentProduct.Description %>
                    <asp:Literal runat="server" ID="liAdditionalDescription" />
                </div>
                <div data-tabs-content="true" class="tab-content">
                    <adv:ProductPropertiesView ID="productPropertiesView" runat="server" />
                </div>
                <div data-tabs-content="true" class="tab-content">
                    <adv:ProductVideoView ID="ProductVideoView" runat="server" />
                </div>
                <asp:ListView ID="lvTabsBodies" runat="server" ItemPlaceholderID="itemPlaceholderID">
                    <LayoutTemplate>
                        <div runat="server" id="itemPlaceholderID">
                        </div>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <div data-tabs-content="true" class="tab-content">
                            <%#Eval("Body") %>
                        </div>
                    </ItemTemplate>
                </asp:ListView>
            </div>
        </div>
        <div class="tabs tabs-hr" data-plugin="tabs">
            <div class="tabs-headers">
                <div data-tabs-header="true" class="tab-header <%= !alternativeProducts.HasProducts ? "tab-hidden" : ""%>"
                    id="tab-alt">
                    <span class="tab-inside">
                        <%= SettingsCatalog.AlternativeProductName %></span>
                </div>
                <div class="tabs-border">
                </div>
            </div>
            <div class="tabs-contents">
                <div class="tab-content" data-tabs-content="true">
                    <adv:RelatedProducts runat="server" ID="alternativeProducts" RelatedType="Alternative" />
                </div>
            </div>
        </div>
        <div class="tabs tabs-hr" data-plugin="tabs">
            <div class="tabs-headers">
                <div data-tabs-header="true" class="tab-header <%= !relatedProducts.HasProducts ? "tab-hidden" : ""%>"
                    id="tab-related">
                    <span class="tab-inside">
                        <%= SettingsCatalog.RelatedProductName %></span>
                </div>
                <div class="tabs-border">
                </div>
            </div>
            <div class="tabs-contents">
                <div class="tab-content" data-tabs-content="true">
                    <adv:RelatedProducts runat="server" ID="relatedProducts" RelatedType="Related" />
                </div>
            </div>
        </div>
        <% if (SettingsCatalog.AllowReviews)
           { %>
        <div class="tabs tabs-hr" data-plugin="tabs">
            <div class="tabs-headers">
                <div data-tabs-header="true" class="tab-header selected" id="tab-review">
                    <span class="tab-inside">
                        <%= Resource.Client_Details_Reviews %>
                        <asp:Literal ID="lReviewsCount" runat="server" /></span>
                </div>
                <div class="tabs-border">
                </div>
            </div>
            <div class="tabs-contents">
                <div data-tabs-content="true" class="tab-content selected">
                    <adv:ProductReviews ID="productReviews" runat="server" />
                </div>
            </div>
        </div>
        <% } %>
    </div>
    <input type="hidden" data-page="details" id="hfProductId" name="hfProductId" value="<%= ProductId %>" />
</asp:Content>
