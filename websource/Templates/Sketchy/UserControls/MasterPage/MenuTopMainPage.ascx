<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MenuTopMainPage.ascx.cs"
    Inherits="Templates.Sketchy.UserControls.MasterPage.MenuTopMainPage" EnableViewState="false" %>
<%--<%@ Register TagPrefix="adv" TagName="MenuCatalogAlternative" Src="~/UserControls/MasterPage/MenuCatalogAlternative.ascx" %>--%>
<%@ Register TagPrefix="adv" TagName="MenuTopAlternative" Src="MenuTopAlternative.ascx" %>
<%@ Register Src="~/UserControls/LogoImage.ascx" TagName="Logo" TagPrefix="adv" %>
<div class="table-emul">
    <div class="col-left">
        <%--<menu class="catalog-menu-root">
            <li><a href="catalog" class="categories-root pie"><%= Resources.Resource.Client_Catalog_Categories %></a>
                <adv:MenuCatalogAlternative ID="menuCatalogAlternative" runat="server" />
            </li>
        </menu>--%>
        <adv:Logo ID="Logo" ImgHref='/' runat="server" CssClassImage="logoFloat" />
    </div>
    <div class="col-right">
        <adv:MenuTopAlternative ID="menuTopAlternative" runat="server" />
    </div>
</div>
