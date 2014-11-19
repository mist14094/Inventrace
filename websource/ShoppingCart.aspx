<%@ Page Language="C#" MasterPageFile="MasterPage.master" CodeFile="ShoppingCart.aspx.cs" Inherits="ClientPages.ShoppingCart_Page" %>

<%@ Register TagPrefix="adv" TagName="BuyInOneClick" Src="~/UserControls/BuyInOneClick.ascx" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>
<%@ MasterType VirtualPath="MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="content-owner">
            <h1>
                <%= Resources.Resource.Client_ShoppingCart_ShoppingCart %></h1>
            <asp:Panel ID="pnlTopContent" runat="server">
            </asp:Panel>
            <asp:Literal ID="ltrlTopContent" runat="server"></asp:Literal>
            <div id="cartWrapper" class="cart-wrapper">
                <div id="dvOrderMerged" runat="server" visible="false" class="ShoppingCart_MergedOrder">
                    <asp:Localize ID="Localize_Client_ShoppingCart_ProductsInBasket" runat="server" Text="<%$ Resources:Resource, Client_ShoppingCart_ProductsInBasket %>"></asp:Localize>
                </div>
                <div data-plugin="cart">
                </div>
            </div>
            <div class="btn-cart-confirm">
                <adv:BuyInOneClick ID="BuyInOneClick" runat="server" />
                <adv:Button ID="aCheckOut" runat="server" Type="Confirm" Size="Big" Text="<%$ Resources:Resource, Client_ShoppingCart_DrawUp %>" DisableValidation="True"
                    OnClick="btnConfirmOrder_Click" />
            </div>
            <asp:Label ID="lDemoWarning" runat="server" CssClass="warn" Text="<%$ Resources:Resource, Client_ShoppingCart_FakeShop %>" />
            <adv:StaticBlock runat="server" SourceKey="shoppingcart" />
            <asp:Panel ID="pnlBottomContent" runat="server">
            </asp:Panel>
            <asp:Literal ID="ltrlBottomContent" runat="server"></asp:Literal>
        </div>
    </div>
    <asp:Label ID="lblEmpty" runat="server" Text="" Visible="False" />
</asp:Content>
