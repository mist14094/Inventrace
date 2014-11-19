<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RelatedProducts.ascx.cs"
    Inherits="UserControls_RelatedProducts" %>
<%@ Register TagPrefix="adv" TagName="Rating" Src="~/UserControls/Rating.ascx" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Customers" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<div class="pv-tile carousel-default">
    <asp:ListView runat="server" ID="lvRelatedProducts" ItemPlaceholderID="liPlaceHolder">
        <LayoutTemplate>
            <ul class="jcarousel">
                <li runat="server" id="liPlaceHolder"></li>
            </ul>
        </LayoutTemplate>
        <ItemTemplate>
            <li style="width:<%= ImageMaxWidth%>px;">
                <table class="p-table">
                    <tr>
                        <td class="img-middle" style="height:<%= ImageMaxHeight%>px;">
                             <%--UrlService.GetAbsoluteLink for IE--%>
                            <div class="pv-photo" onclick="location.href='<%# UrlService.GetAbsoluteLink(UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductId"))))  %>'">
                                <%# RenderPictureTag(SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("Name")), SQLDataHelper.GetString(Eval("PhotoDesc")))%>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div>
                                <a href="<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductId"))) %>"
                                    class="link-pv-name">
                                    <%# Eval("Name") %></a>
                            </div>
                            <div class="sku"><%# Eval("ArtNo") %></div>
                            <adv:Rating ID="Rating1" runat="server" />
                            <div class="price-container">
                                    <%# CatalogService.RenderPrice(SQLDataHelper.GetFloat(Eval("MainPrice")), SQLDataHelper.GetFloat(Eval("Discount")), true, customerGroup)%>
                            </div>
                            <div class="pv-btns">
                                <adv:Button ID="btnAdd"  data-cart-add-productid='<%# Eval("ProductID") %>' runat="server" Type="Add" Size="Small" Text='<%# BuyButtonText  %>'
                                    Href='<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("UrlPath")), Convert.ToInt32(Eval("ProductId"))) %>'
                                    Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("MainPrice")) > 0 && SQLDataHelper.GetFloat(Eval("TotalAmount")) > 0 %>' />
                                <adv:Button ID="btnAction" runat="server" Type="Action" Size="Small" Text='<%# PreOrderButtonText %>'
                                    Href='<%# "sendrequestonproduct.aspx?productid=" + Eval("productId") %>' 
                                    Visible='<%# DisplayPreOrderButton && (!(SQLDataHelper.GetFloat(Eval("MainPrice")) > 0 && SQLDataHelper.GetInt(Eval("TotalAmount")) > 0) && SQLDataHelper.GetBoolean(Eval("AllowPreOrder"))) %>' />
                                <adv:Button ID="btnBuy" runat="server" Type="Buy" Size="Small" Text='<%# MoreButtonText %>'
                                    Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductId"))) %>'
                                    Visible='<%# DisplayMoreButton %>' />
                            </div>
                        </td>
                    </tr>
                </table>
            </li>
        </ItemTemplate>
    </asp:ListView>
</div>
