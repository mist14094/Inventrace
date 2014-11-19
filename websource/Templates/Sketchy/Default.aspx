<%@ Page Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true"
    CodeFile="Default.aspx.cs" Inherits="Templates.Sketchy.DefaultPage" EnableViewState="false" %>

<%@ Register Src="~/UserControls/StaticBlock.ascx" TagName="StaticBlock" TagPrefix="adv" %>
<%@ Register Src="UserControls/Default/MainPageProduct.ascx" TagName="MainPageProduct" TagPrefix="adv" %>
<%@ Register TagPrefix="adv" TagName="News" Src="UserControls/Default/News.ascx" %>
<%@ Register TagPrefix="adv" TagName="Carousel" Src="~/UserControls/Default/Carousel.ascx" %>
<%@ Register TagPrefix="adv" TagName="CheckOrder" Src="UserControls/Default/CheckOrder.ascx" %>
<%@ Register TagPrefix="adv" TagName="Voting" Src="UserControls/Default/VotingUC.ascx" %>
<%@ Register TagPrefix="adv" TagName="MenuCatalogAlternative" Src="~/UserControls/MasterPage/MenuCatalogAlternative.ascx" %>
<%@ Register TagPrefix="adv" TagName="GiftCertificate" Src="~/UserControls/Default/GiftCertificate.ascx" %>
<%@ MasterType VirtualPath="MasterPage.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
<adv:Carousel ID="carousel" runat="server" CssSlider="flexslider-main"/>
    <div class="col-left-metro">

        <adv:StaticBlock ID="StaticBlock10" runat="server" SourceKey="TextOnMain" />
        <adv:MainPageProduct ID="mainPageProduct" runat="server" Mode="TwoColumns" />
        <adv:StaticBlock ID="StaticBlock11" runat="server" SourceKey="TextOnMain2" />
        <br class="clear" />
    </div>
    <div class="col-right-metro expander-enable">
        <adv:StaticBlock SourceKey="RightImage" ID="RightImage" runat="server"/>
        <adv:News runat="server" ID="news" />
        <adv:CheckOrder runat="server" ID="checkOrder" />
            <adv:GiftCertificate runat="server" ID="giftCertificate" />
        <adv:Voting runat="server" ID="voting" />
        <br class="clear" />
        <div class="block-uc social-big">
            <adv:StaticBlock ID="staticBlock2" runat="server" SourceKey="Vk" />
        </div>
        <br class="clear" />
    </div>
    <br class="clear" />
</asp:Content>
