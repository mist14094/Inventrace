<%@ Page Language="C#" MasterPageFile="MasterPage.master" CodeFile="ProductList.aspx.cs"
    Inherits="ClientPages.ProductList_Page" %>

<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Orders" %>
<%@ Register TagPrefix="adv" TagName="ProductView" Src="~/UserControls/Catalog/ProductView.ascx" %>
<%@ Register TagPrefix="adv" TagName="FilterPrice" Src="~/UserControls/Catalog/FilterPrice.ascx" %>
<%@ Register TagPrefix="adv" TagName="FilterBrand" Src="~/UserControls/Catalog/FilterBrand.ascx" %>
<%@ Register TagPrefix="adv" TagName="RecentlyView" Src="~/UserControls/Catalog/RecentlyView.ascx" %>
<%@ Register TagPrefix="adv" TagName="BreadCrumbs" Src="~/UserControls/BreadCrumbs.ascx" %>
<%@ Register TagPrefix="adv" TagName="ProductViewChanger" Src="~/UserControls/Catalog/ProductViewChanger.ascx" %>
<%@ MasterType VirtualPath="MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="col-left">
            <div class="block-uc expander">
                <span class="title expand ex-control">
                    <%= Resources.Resource.Client_ProductList_Sections%><span class="control"></span></span>
                <div class="content list-link-marker ex-content">
                    <a href="productlist.aspx?type=bestseller">
                        <%= _typeFlag== ProductOnMain.TypeFlag.Bestseller ? "<span class='bold'>" +  Resources.Resource.Client_ProductList_AllBestSellers + "</span>" : Resources.Resource.Client_ProductList_AllBestSellers %></a>
                    <a href="productlist.aspx?type=new">
                        <%= _typeFlag== ProductOnMain.TypeFlag.New ? "<span class='bold'>" +  Resources.Resource.Client_ProductList_AllNew + "</span>" : Resources.Resource.Client_ProductList_AllNew %></a>
                    <a href="productlist.aspx?type=discount">
                        <%= _typeFlag == ProductOnMain.TypeFlag.Discount ? "<span class='bold'>" + Resources.Resource.Client_ProductList_AllDiscount + "</span>" : Resources.Resource.Client_ProductList_AllDiscount%></a>
                </div>
            </div>
            <%if ((filterBrand.Visible || filterPrice.Visible) && ProductsCount > 1)
              {%>
            <div class="expander block-uc" id="filter">
                <span class="title expand ex-control"><span class="control"></span>
                    <%= Resources.Resource.Client_Catalog_Filter %>
                </span>
                <div class="content expand ex-content">
                    <adv:FilterPrice runat="server" ID="filterPrice" />
                    <adv:FilterBrand runat="server" ID="filterBrand" />
                    <div class="aplly-price">
                        <adv:Button runat="server" Size="Small" Type="Action" Text="<%$ Resources:Resource, Client_Catalog_ResetFilter%>"
                            OnClientClick="ClearFilter();"></adv:Button>
                        <adv:Button runat="server" Size="Small" Type="Confirm" Text="<%$ Resources:Resource, Client_Catalog_FilterApply %>"
                            OnClientClick="ApplyFilter(null, true, false);"></adv:Button>
                    </div>
                </div>
            </div>
            <% } %>
            <adv:RecentlyView runat="server" />
        </div>
        <div class="col-right">
            <h1>
                <%= PageName %></h1>
            <adv:BreadCrumbs ID="breadCrumbs" runat="server" />
            <% if (productView.HasProducts)
               {%>
            <div class="str-sort" runat="server" id="pnlSort">
                <div class="count-search">
                    <asp:Literal runat="server" ID="lTotalItems" /></div>
                <adv:ProductViewChanger runat="server" ID="productViewChanger" CurrentPage="Catalog" />
                  <% if (SettingsCatalog.EnableCompareProducts)
                   { %>
                <div class="cp">
                    <a href="compareproducts.aspx" target="_blank" id="compareBasket">
                        <%=Resources.Resource.Client_Catalog_CompareCart%>
                        (<span id="compareCount"><%=ShoppingCartService.CurrentCompare.Count() %></span>)</a>
                </div>
                <% } %>
                <div class="sort-variant">
                    <%=Resources.Resource.Client_Catalog_SortBy%>
                    <asp:DropDownList ID="ddlSort" runat="server" onChange="ApplyFilter(null, true, false);" />
                </div>
                <br class="clear" />
            </div>
            <% } %>
            <adv:ProductView ID="productView" runat="server" />
            <adv:AdvPaging runat="server" ID="paging" DisplayShowAll="True" />
        </div>
        <br class="clear" />
    </div>
</asp:Content>
