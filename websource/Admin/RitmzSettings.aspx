<%@ Page Language="C#" MasterPageFile="~/Admin/MasterPageAdmin.master" AutoEventWireup="true" CodeFile="RitmzSettings.aspx.cs"
    Inherits="Admin_RitmzSettings" %>
<%@ Import Namespace="Resources" %>

<%@ Register Src="~/Admin/UserControls/Modules/RitmzSettings.ascx" TagName="RitmzSettings" TagPrefix="adv" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="content-top">
        <menu class="neighbor-menu neighbor-catalog">
            <li class="neighbor-menu-item"><a href="Discount_PriceRange.aspx">
                <%= Resource.Admin_MasterPageAdmin_DiscountMethods%></a></li>
            <li class="neighbor-menu-item"><a href="Coupons.aspx">
                <%= Resource.Admin_MasterPageAdmin_Coupons%></a></li>
            <li class="neighbor-menu-item"><a href="Certificates.aspx">
                <%= Resource.Admin_MasterPageAdmin_Certificate%></a></li>
            <li class="neighbor-menu-item"><a href="SendMessage.aspx">
                <%= Resource.Admin_MasterPageAdmin_SendMessage%></a></li>
            <li class="neighbor-menu-item"><a href="Voting.aspx">
                <%= Resource.Admin_MasterPageAdmin_Voting%></a></li>
            <li class="neighbor-menu-item"><a href="SiteMapGenerate.aspx">
                <%= Resource.Admin_SiteMapGenerate_Header%></a></li>
            <li class="neighbor-menu-item"><a href="SiteMapGenerateXML.aspx">
                <%= Resource.Admin_SiteMapGenerateXML_Header%></a></li>
        </menu>
    </div>
    <div class="content-own" style="text-align: center;">
        <asp:Label ID="lblAdminHead" runat="server" CssClass="AdminHead" Text="<%$ Resources:Resource, Admin_RitmzSettings_Header %>"></asp:Label><br />
        <asp:Label ID="lblAdminSubHead" runat="server" CssClass="AdminSubHead" Text="<%$ Resources:Resource, Admin_RitmzSettings_SubHeader %>"></asp:Label>
        <br />
        <br />
        <br />
        <adv:RitmzSettings runat="server" ID="RitmzSettings" />
        <br />
        <asp:Button runat="server" ID="btnSave" OnClick="btnSave_Click" Text="<%$ Resources:Resource, Admin_RitmzSettings_Save %>" />
    </div>
</asp:Content>
