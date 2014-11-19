<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TopPanel.ascx.cs" Inherits="UserControls_MasterPage_TopPanel" %>
<%@ Import Namespace="Resources" %>
<%@ Register Src="~/UserControls/MasterPage/ShoppingCart.ascx" TagName="ShoppingCart" TagPrefix="adv" %>
<!--top_panel-->
<div class="top-panel">
    <div class="top-panel-content">
        <div class="top-panel-login" runat="server" id="pnlLogin">
            <a runat="server" id="aLogin" rel="nofollow" class="tpl tpl-signin"><%= Resources.Resource.Client_MasterPage_Authorization %></a>
            <a runat="server" id="aMyAccount" class="tpl tpl-signin"><%= Resources.Resource.Client_MasterPage_MyAccount %></a>
            <%--Admin link --%>
            <a id="pnlAdmin" runat="server" class="tpl tpl-admin"><%= Resource.Client_MasterPage_Administration%></a>
            <a id="aCreateTrial" href="javascript:void(0);" runat="server" class="tpl tpl-admin trialAdmin">
                <%= Resource.Client_MasterPage_Administration%></a>
            <asp:LinkButton ID="lbLoginAsAdmin" OnClick="lbLoginAsAdmin_Click" runat="server"
                Text="<%$ Resources:Resource, Client_MasterPage_Administration %>" CssClass="tpl tpl-admin" />
            <%--/Admin link --%>
             <a runat="server" id="aRegister" rel="nofollow" class="tpl tpl-reg"><%= Resources.Resource.Client_MasterPage_Registration %></a>
            <asp:LinkButton ID="lbLogOut" CssClass="tpl tpl-reg" runat="server" 
                OnClick="btnLogout_Click"><%= Resources.Resource.Client_MasterPage_LogOut %></asp:LinkButton>
        </div>
        <div class="top-panel-constructor" runat="server" id="pnlConstructor">
            <span><%= Resources.Resource.Client_MasterPage_AdminPanel_DesignConstructor %></span>
        </div>
        <adv:ShoppingCart ID="shoppingCart" runat="server" />
        <div class="top-panel-wishlist" runat="server" id="pnlWishList">
            <%=Resources.Resource.Client_MasterPage_WishList%>: <a href="wishlist.aspx" class="link-purple"><%= wishlistCount %></a>
        </div>
        <div class="top-panel-currency" runat="server" id="pnlCurrency">
                <asp:DropDownList ID="ddlCurrency" runat="server" onchange="ChangeCurrency(this);" />
            </div>
        <br class="clear" />
    </div>
</div>
<!--end_top_panel-->
