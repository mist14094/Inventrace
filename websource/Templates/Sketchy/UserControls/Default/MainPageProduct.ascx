<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MainPageProduct.ascx.cs"
    Inherits="Templates.Sketchy.UserControls.Default.MainPageProduct" %>
<%@ Register TagPrefix="adv" TagName="Rating" Src="~/UserControls/Rating.ascx" %>
<%@ Register TagPrefix="adv" TagName="SizeColorPickerCatalog" Src="~/UserControls/Catalog/SizeColorPickerCatalog.ascx" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<!--container_special-->
<asp:MultiView runat="server" ID="mvMainPageProduct">
    <Views>
        <asp:View runat="server" ID="viewDefault">
            <table class="container-special">
                <tr>
                    <td class="block" runat="server" id="liBestsellers">
                        <div class="best-title">
                            <a href="productlist.aspx?type=bestseller"><%= Resources.Resource.Client_Default_BestSellers %></a></div>
                        <asp:ListView runat="server" ID="lvBestSellers" ItemPlaceholderID="liItemPlaceholder">
                            <LayoutTemplate>
                                <ul class="p-list scp">
                                    <li runat="server" id="liItemPlaceholder"></li>
                                </ul>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <li class="p-block scp-item" data-productid="<%# Eval("productId")%>">
                                    <table class="p-table">
                                        <tr>
                                            <td class="img-middle" style="height:<%= ImageMaxHeight%>px">
                                                <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("productId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")))%>
                                                <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="pv-div-link">
                                                    <a href="<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                        class="link-pv-name">
                                                        <%# Eval("Name") %></a>
                                                    <%# string.Format("<div class=\"sku\">{0}</div>", Eval("ArtNo")) %>
                                                </div>
                                                <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>'/>
                                                <adv:Rating runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                    ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                    ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                                <div class="price-container">
                                                    <div class="price">
                                                        <%# RenderPriceTag(SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                    </div>
                                                </div>
                                                <%--<adv:Button data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="Small" Type="Add"
                                                    Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                    Text='<%$ Resources:Resource, Client_Catalog_Add %>' Visible='<%# SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />--%>
                                                <adv:Button runat="server" Type="Action" Size="Small" CssClass="pv-btn" Text='<%# PreOrderButtonText %>'
                                                    Href='<%# "sendrequestonproduct.aspx?offerid=" + Eval("offerid") %>' Visible='<%# DisplayPreOrderButton && (SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetInt(Eval("Amount")) <= 0) && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>' />
                                               <%-- <adv:Button runat="server" Size="XSmall" Type="Buy" Text='<%$Resources:Resource, Client_More %>'
                                                    Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>' />--%>
                                            </td>
                                        </tr>
                                    </table>
                                </li>
                            </ItemTemplate>
                        </asp:ListView>
                    </td>
                    <td class="block" runat="server" id="liNew">
                        <div class="new-title">
                            <a href="productlist.aspx?type=new"><%= Resources.Resource.Client_Default_New %></a></div>
                        <asp:ListView runat="server" ID="lvNew" ItemPlaceholderID="liItemPlaceholder">
                            <LayoutTemplate>
                                <ul class="p-list scp">
                                    <li runat="server" id="liItemPlaceholder"></li>
                                </ul>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <li class="p-block scp-item" data-productid="<%# Eval("productId")%>">
                                    <table class="p-table">
                                        <tr>
                                            <td class="img-middle" style="height:<%= ImageMaxHeight%>px">
                                                <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("productId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")))%>
                                                <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="pv-div-link">
                                                    <a href="<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                        class="link-pv-name">
                                                        <%# Eval("Name") %></a>
                                                    <%# string.Format("<div class=\"sku\">{0}</div>", Eval("ArtNo")) %>
                                                </div>
                                                <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>'/>
                                                <adv:Rating runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                    ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                    ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                                <div class="price-container">
                                                    <div class="price">
                                                        <%# RenderPriceTag(SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                    </div>
                                                </div>
                                                <adv:Button CssClass="pv-btn" data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="Small" Type="Add"
                                                    Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                    Text='<%# BuyButtonText %>' Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />
                                                <adv:Button CssClass="pv-btn" runat="server" Type="Action" Size="Small" Text='<%# PreOrderButtonText %>'
                                                    Href='<%# "sendrequestonproduct.aspx?offerid=" + Eval("offerid") %>' Visible='<%# DisplayPreOrderButton && (SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetInt(Eval("Amount")) <= 0) && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>' />
                                                <adv:Button CssClass="pv-btn" runat="server" Size="XSmall" Type="Buy" Text='<%# MoreButtonText %>'
                                                    Href='<%# UrlService.GetLink(ParamType.Product,Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>' Visible="<%# DisplayMoreButton %>" />
                                            </td>
                                        </tr>
                                    </table>
                                </li>
                            </ItemTemplate>
                        </asp:ListView>
                    </td>
                    <td class="block" runat="server" id="liDiscount">
                        <div class="discount-title">
                            <a href="productlist.aspx?type=discount"><%= Resources.Resource.Client_Default_Discount %></a></div>
                        <asp:ListView runat="server" ID="lvDiscount" ItemPlaceholderID="liItemPlaceholder">
                            <LayoutTemplate>
                                <ul class="p-list scp">
                                    <li runat="server" id="liItemPlaceholder"></li>
                                </ul>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <li class="p-block scp-item" data-productid="<%# Eval("productId")%>">
                                    <table class="p-table">
                                        <tr>
                                            <td class="img-middle">
                                                <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("productId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")))%>
                                                <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div class="pv-div-link">
                                                    <a href="<%# UrlService.GetLink(ParamType.Product,Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                        class="link-pv-name">
                                                        <%# Eval("Name") %></a>
                                                    <%# string.Format("<div class=\"sku\">{0}</div>", Eval("ArtNo")) %>
                                                </div>
                                                <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>'/>
                                                <adv:Rating runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                    ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                    ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                                <div class="price-container">
                                                    <div class="price">
                                                        <%# RenderPriceTag(SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                    </div>
                                                </div>
                                                <adv:Button CssClass="pv-btn" data-cart-add-productid='<%# Eval("ProductID") %>' data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="Small" Type="Add"
                                                    Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                    Text='<%# BuyButtonText %>' Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />
                                                <adv:Button CssClass="pv-btn" runat="server" Type="Action" Size="Small" Text='<%# PreOrderButtonText %>'
                                                    Href='<%# "sendrequestonproduct.aspx?offerid=" + Eval("offerid") %>' Visible='<%# DisplayPreOrderButton && (SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetInt(Eval("Amount")) <= 0) && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>' />
                                                <adv:Button CssClass="pv-btn" runat="server" Size="XSmall" Type="Buy" Text='<%# MoreButtonText %>'
                                                    Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>' Visible="<%# DisplayMoreButton %>" />
                                            </td>
                                        </tr>
                                    </table>
                                </li>
                            </ItemTemplate>
                        </asp:ListView>
                    </td>
                </tr>
            </table>
            <script type="text/javascript">
                $("td:last-child", ".container-special").addClass("block-last");
            </script>
        </asp:View>
        <asp:View runat="server" ID="viewAlternative">
            <div class="block-best" runat="server" id="pnlBest">
                <div class="best-title">
                    <a href="productlist.aspx?type=bestseller"><%= Resources.Resource.Client_Default_BestSellers %></a></div>
                <div class="pv-tile scp">
                    <asp:ListView runat="server" ID="lvBestSellersAltervative"  ItemPlaceholderID="liItemPlaceholder">
                        <ItemTemplate>
                            <div class="pv-item scp-item" data-productid="<%# Eval("productId") %>">
                                <table class="p-table">
                                    <tr>
                                        <td class="img-middle" style="height:<%= ImageMaxHeight %>px">
                                            <div class="pv-photo">
                                                <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("productId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")))%>
                                            </div>
                                            <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="pv-div-link">
                                                <a href="<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                    class="link-pv-name">
                                                    <%# Eval("Name") %></a>
                                                <%--<%# string.Format("<div class=\"sku\">{0}</div>", Eval("ArtNo")) %>--%>
                                            </div>
                                             <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>'/>
                                            <adv:Rating runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                            <div class="price-container">
                                                <div class="price">
                                                    <%# RenderPriceTag(SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                </div>
                                            </div>
                                            <div class="pv-btns">
                                                <adv:Button CssClass="pv-btn" data-cart-add-productid='<%# Eval("ProductID") %>'  data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="Small" Type="Add" 
                                                    Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                    Text='<%# BuyButtonText %>' Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />
                                                <adv:Button CssClass="pv-btn" runat="server" Type="Action" Size="Small" Text='<%# PreOrderButtonText %>'
                                                    Href='<%# "sendrequestonproduct.aspx?offerid=" + Eval("offerid") %>' Visible='<%# DisplayPreOrderButton && (SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetInt(Eval("Amount")) <= 0) && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>' />
                                                <%--<adv:Button runat="server" Size="XSmall" Type="Buy" Text='<%$Resources:Resource, Client_More %>'
                                                    Href='<%# UrlService.GetLink(ParamType.Product,Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>' />--%>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </div>
            <div class="block-best" runat="server" id="pnlNew">
                <div class="best-title">
                    <a href="productlist.aspx?type=new"><%= Resources.Resource.Client_Default_New %></a></div>
                <div class="pv-tile scp">
                    <asp:ListView runat="server" ID="lvNewAlternative" ItemPlaceholderID="liItemPlaceholder">
                        <ItemTemplate>
                            <div class="pv-item scp-item" data-productid="<%# Eval("productId") %>">
                                <table class="p-table">
                                    <tr>
                                        <td class="img-middle" style="height:<%= ImageMaxHeight %>px">
                                            <div class="pv-photo">
                                                <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("productId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")))%>
                                            </div>
                                            <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="pv-div-link">
                                                <a href="<%# UrlService.GetLink(ParamType.Product,Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                    class="link-pv-name">
                                                    <%# Eval("Name") %></a>
                                                <%--<%# string.Format("<div class=\"sku\">{0}</div>", Eval("ArtNo")) %>--%>
                                            </div>
                                            <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>'/>
                                            <adv:Rating runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                            <div class="price-container">
                                                <div class="price">
                                                    <%# RenderPriceTag(SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                </div>
                                            </div>
                                            <div class="pv-btns">
                                                <adv:Button CssClass="pv-btn" data-cart-add-productid='<%# Eval("ProductID") %>'  data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="Small" Type="Add" 
                                                    Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                    Text='<%# BuyButtonText %>' Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />
                                                <adv:Button CssClass="pv-btn" runat="server" Type="Action" Size="Small" Text='<%# PreOrderButtonText %>'
                                                    Href='<%# "sendrequestonproduct.aspx?offerid=" + Eval("offerid") %>' Visible='<%# DisplayPreOrderButton && (SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetInt(Eval("Amount")) <= 0) && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>' />
                                               <%-- <adv:Button runat="server" Size="XSmall" Type="Buy" Text='<%$Resources:Resource, Client_More %>'
                                                    Href='<%# UrlService.GetLink(ParamType.Product,Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>' />--%>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </div>
            <div class="block-best" runat="server" id="pnlDiscount">
                <div class="discount-title">
                    <a href="productlist.aspx?type=discount"><%= Resources.Resource.Client_Default_Discount %></a></div>
                <div class="pv-tile scp">
                    <asp:ListView runat="server" ID="lvDiscountAlternative" ItemPlaceholderID="liItemPlaceholder">
                        <ItemTemplate>
                            <div class="pv-item scp-item" data-productid="<%# Eval("productId") %>">
                                <table class="p-table">
                                    <tr>
                                        <td class="img-middle" style="height:<%= ImageMaxHeight %>px">
                                            <div class="pv-photo">
                                                <%# RenderPictureTag(SQLDataHelper.GetInt(Eval("productId")), SQLDataHelper.GetString(Eval("Photo")), SQLDataHelper.GetString(Eval("UrlPath")), SQLDataHelper.GetString(Eval("PhotoDesc")))%>
                                            </div>
                                            <%# CatalogService.RenderLabels(SQLDataHelper.GetBoolean(Eval("Recomended")), SQLDataHelper.GetBoolean(Eval("OnSale")), SQLDataHelper.GetBoolean(Eval("Bestseller")), SQLDataHelper.GetBoolean(Eval("New")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <div class="pv-div-link">
                                                <a href="<%# UrlService.GetLink(ParamType.Product,Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>"
                                                    class="link-pv-name">
                                                    <%# Eval("Name") %></a>
                                                <%--<%# string.Format("<div class=\"sku\">{0}</div>", Eval("ArtNo")) %>--%>
                                            </div>
                                            <adv:SizeColorPickerCatalog ID="SizeColorPicker" runat="server" ProductId='<%# Eval("ProductId") %>' Colors='<%# Eval("Colors") %>' DefaultColorID='<%# SQLDataHelper.GetInt(Eval("ColorID")) %>'/>
                                            <adv:Rating runat="server" ProductId='<%# SQLDataHelper.GetInt(Eval("ProductID")) %>'
                                                ShowRating='<%# EnableRating %>' Rating='<%# SQLDataHelper.GetDouble(Eval("Ratio")) %>'
                                                ReadOnly='<%# Eval("RatioID") != DBNull.Value %>' />
                                            <div class="price-container">
                                                <div class="price">
                                                    <%# RenderPriceTag(SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Discount")))%>
                                                </div>
                                            </div>
                                            <div class="pv-btns">
                                                <adv:Button CssClass="pv-btn" data-cart-add-productid='<%# Eval("ProductID") %>'  data-cart-amount='<%# Eval("MinAmount") %>' runat="server" Size="Small" Type="Add" 
                                                    Href='<%# UrlService.GetLink(ParamType.Product, Eval("UrlPath").ToString(),SQLDataHelper.GetInt(Eval("ProductID"))) %>'
                                                    Text='<%# BuyButtonText %>' Visible='<%# DisplayBuyButton && SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetFloat(Eval("Amount")) > 0 %>' />
                                                <adv:Button CssClass="pv-btn" runat="server" Type="Action" Size="Small" Text='<%# PreOrderButtonText %>'
                                                    Href='<%# "sendrequestonproduct.aspx?offerid=" + Eval("offerid") %>' Visible='<%# DisplayPreOrderButton && (SQLDataHelper.GetFloat(Eval("Price")) > 0 && SQLDataHelper.GetInt(Eval("Amount")) <= 0) && SQLDataHelper.GetBoolean(Eval("AllowPreorder")) %>' />
                                                <%--<adv:Button runat="server" Size="XSmall" Type="Buy" Text='<%$Resources:Resource, Client_More %>'
                                                    Href='<%# UrlService.GetLink(ParamType.Product,Eval("UrlPath").ToString(), SQLDataHelper.GetInt(Eval("ProductID"))) %>' />--%>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </ItemTemplate>
                    </asp:ListView>
                </div>
            </div>
        </asp:View>
    </Views>
</asp:MultiView>
<div class="clear">
</div>
<!--end_container_special-->
