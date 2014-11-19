<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdminSearch.ascx.cs" Inherits="Admin.UserControls.MasterPage.AdminSearch" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="Resources" %>
<div class="justify-item search-wrap">
    <div class="search">
        <div class="search-cell">
            <input type="text" value="<%= searchRequest%>" id="txtAdminSearch" placeholder="<%= Resource.Admin_MasterPageAdmin_Search %>"
                class="search-input" onkeyup="defaultButtonClick('btnAdminSearch', event)" />
        </div>
        <div class="search-cell search-right">
            <div id="searchSubmenuContainer" class="search-category dropdown-arrow-dark">
                <span class="search-cat" id="searchHeader" data-href="<%=SettingsMain.SearchPage %>">
                    <%= SettingsMain.SearchArea ?? Resource.Admin_MasterPageAdmin_SearchInProducts%></span>
                <div id="searchSubmenu" class="dropdown-menu-wrap dropdown-menu-invert">
                    <ul class="dropdown-menu" id="liSearchItems">
                        <li class="dropdown-menu-item"><a href="Catalog.aspx?CategoryID=AllProducts">
                            <%= Resource.Admin_MasterPageAdmin_SearchInProducts %></a></li>
                        <li class="dropdown-menu-item"><a href="OrderSearch.aspx">
                            <%= Resource.Admin_MasterPageAdmin_SearchInOrders %></a></li>
                        <li class="dropdown-menu-item"><a href="CustomerSearch.aspx">
                            <%= Resource.Admin_MasterPageAdmin_SearchInCustomers %></a></li>
                    </ul>
                </div>
            </div>
            <a href="javascript:void(0);" class="search-btn" id="btnAdminSearch">&nbsp;</a>
        </div>
    </div>
</div>
