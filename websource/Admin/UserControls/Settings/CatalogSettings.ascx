<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CatalogSettings.ascx.cs"
    Inherits="Admin.UserControls.Settings.CatalogSettings" %>
<table border="0" cellpadding="2" cellspacing="0">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <asp:Localize ID="Localize_Admin_CommonSettings_HeadCatalog" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_HeadCatalog %>"></asp:Localize></span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize_Admin_CommonSettings_ProductPerPage" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ProductPerPage %>"></asp:Localize>
            <br />
        </td>
        <td>&nbsp;<asp:TextBox ID="txtProdPerPage" runat="server" class="shortTextBoxClass"></asp:TextBox>
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize_Admin_CommonSettings_DefaultCurrency" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_DefaultCurrency %>"></asp:Localize>
            <br />
        </td>
        <td>&nbsp;<asp:DropDownList ID="ddlDefaultCurrency" runat="server" class="shortTextBoxClass">
        </asp:DropDownList>
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize9" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ShowProductsCount %>"></asp:Localize>
        </td>
        <td>
            <asp:CheckBox ID="cbShowProductsCount" runat="server" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_EnableProductRating %>"></asp:Localize>
        </td>
        <td>
            <asp:CheckBox ID="cbEnableProductRating" runat="server" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize4" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_EnableCompareProducts %>"></asp:Localize>
        </td>
        <td>
            <asp:CheckBox ID="cbEnableCompareProducts" runat="server" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize15" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ExluderingFilters%>"></asp:Localize>
        </td>
        <td>
            <asp:CheckBox ID="cbExluderingFilters" runat="server" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize23" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ShowStockAvailability %>"></asp:Localize>
            <br />
            <br />
        </td>
        <td>
            <asp:CheckBox ID="cbShowStockAvailability" runat="server" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <asp:Localize ID="Localize12" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_SizesAndColors %>"></asp:Localize></span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize13" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_SizesHeader %>"></asp:Localize>
        </td>
        <td>
            <asp:TextBox ID="txtSizesHeader" runat="server" Width="200px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize14" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ColorsHeader %>"></asp:Localize>
        </td>
        <td>
            <asp:TextBox ID="txtColorsHeader" runat="server" Width="200px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize24" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ColorsPictureWidth %>"></asp:Localize>
        </td>
        <td>
            <asp:Localize ID="Localize28" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ColorsPictureInCatalog %>"></asp:Localize>:
            <asp:TextBox ID="txtColorPictureWidthCatalog" runat="server" Width="50px" />
            <asp:Localize ID="Localize29" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ColorsPictureInDetails %>"></asp:Localize>:
            <asp:TextBox ID="txtColorPictureWidthDetails" runat="server" Width="50px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize25" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ColorsPictureHeight %>"></asp:Localize>
        </td>
        <td>
            <asp:Localize ID="Localize26" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ColorsPictureInCatalog %>"></asp:Localize>:
            <asp:TextBox ID="txtColorPictureHeightCatalog" runat="server" Width="50px" />
            <asp:Localize ID="Localize27" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ColorsPictureInDetails %>"></asp:Localize>:
            <asp:TextBox ID="txtColorPictureHeightDetails" runat="server" Width="50px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize11" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ComplexFilter%>"></asp:Localize>
            <br />
            <br />
        </td>
        <td>
            <asp:CheckBox ID="cbComplexFilter" runat="server" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <asp:Localize ID="Localize18" runat="server" Text="Кнопки у товаров"></asp:Localize></span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize19" runat="server" Text="Кнопка добавить в корзину"></asp:Localize>
        </td>
        <td>
            <asp:TextBox ID="txtBuyButtonText" runat="server" Width="200px" />
            <asp:CheckBox runat="server" ID="cbDisplayBuyButton" Text="<%$ Resources:Resource, Admin_CommonSettings_Display%>" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize20" runat="server" Text="Кнопка подробнее"></asp:Localize>
        </td>
        <td>
            <asp:TextBox ID="txtMoreButtonText" runat="server" Width="200px" />
            <asp:CheckBox ID="cbDisplayMoreButton" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_Display%>" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize21" runat="server" Text="Кнопка под заказ"></asp:Localize>
        </td>
        <td>
            <asp:TextBox ID="txtPreOrderButtonText" runat="server" Width="200px" />
            <asp:CheckBox ID="cbDisplayPreOrderButton" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_Display%>" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <asp:Localize ID="Localize_Admin_CommonSettings_AllowChangeCatalogView" runat="server"
                    Text="<%$ Resources:Resource, Admin_CommonSettings_AllowChangeCatalogView %>"></asp:Localize></span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize_Admin_CommonSettings_EnableCatalogViewChange" runat="server"
                Text="<%$ Resources:Resource, Admin_CommonSettings_EnableCatalogViewChange %>"></asp:Localize>
        </td>
        <td>
            <asp:CheckBox ID="cbEnableCatalogViewChange" runat="server" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize_Admin_CommonSettings_EnableSearchViewChange" runat="server"
                Text="<%$ Resources:Resource, Admin_CommonSettings_EnableSearchViewChange %>"></asp:Localize>
        </td>
        <td>
            <asp:CheckBox ID="cbEnableSearchViewChange" runat="server" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <asp:Localize ID="Localize_Admin_CommonSettings_Marketing" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_Marketing %>"></asp:Localize></span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_BlockOne %>"></asp:Localize>
        </td>
        <td>
            <asp:TextBox ID="txtBlockOne" runat="server" Width="200px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_BlockTwo %>"></asp:Localize>
        </td>
        <td>
            <asp:TextBox ID="txtBlockTwo" runat="server" Width="200px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <asp:Localize ID="Localize8" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_Search %>"></asp:Localize></span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; height: 34px; text-align: left; vertical-align: top;">
            <asp:Localize ID="Localize7" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_SearchIndex %>"></asp:Localize>
            <br />
        </td>
        <td style="vertical-align: top;">
            <asp:LinkButton runat="server" ID="btnDoindex" Text="<%$ Resources:Resource, Admin_CommonSettings_Generate %>"
                OnClick="btnDoindex_Click" />
            <asp:Label runat="server" ID="lbDone" Text="<%$ Resources:Resource, Admin_CommonSettings_RunningInBackGroung%>"
                Visible="False"></asp:Label>
        </td>
    </tr>
    <tr class="rowsPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <asp:Localize ID="Localize_Admin_CommonSettings_DefaultView" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_DefaultView %>"></asp:Localize></span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize_Admin_CommonSettings_DefaultCatalogView" runat="server"
                Text="<%$ Resources:Resource, Admin_CommonSettings_DefaultCatalogView %>"></asp:Localize>
        </td>
        <td>
            <asp:DropDownList ID="ddlCatalogView" runat="server">
                <asp:ListItem Text="<%$ Resources:Resource, Client_Catalog_Tiles %>" Value="0"></asp:ListItem>
                <asp:ListItem Text="<%$ Resources:Resource, Client_Catalog_List %>" Value="1"></asp:ListItem>
                <asp:ListItem Text="<%$ Resources:Resource, Client_Catalog_Table %>" Value="2"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize_Admin_CommonSettings_DefaultSearchView" runat="server"
                Text="<%$ Resources:Resource, Admin_CommonSettings_DefaultSearchView %>"></asp:Localize>
        </td>
        <td>
            <asp:DropDownList ID="ddlSearchView" runat="server">
                <asp:ListItem Text="<%$ Resources:Resource, Client_Catalog_Tiles %>" Value="0"></asp:ListItem>
                <asp:ListItem Text="<%$ Resources:Resource, Client_Catalog_List %>" Value="1"></asp:ListItem>
                <asp:ListItem Text="<%$ Resources:Resource, Client_Catalog_Table %>" Value="2"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize22" runat="server" Text="Показывать категории в нижнем меню"></asp:Localize>
        </td>
        <td>
            <asp:CheckBox ID="ckbShowCategoriesInBottomMenu" runat="server"></asp:CheckBox>
        </td>
    </tr>
    <tr class="rowsPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <asp:Localize ID="Localize_Admin_CommonSettings_ProductPhotos" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ProductPhotos %>"></asp:Localize></span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize17" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_CompressBigImage %>"></asp:Localize>
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkCompressBigImage" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <asp:Localize ID="Localize5" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_Reviews %>"></asp:Localize></span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize10" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_AllowReviews %>"></asp:Localize>
        </td>
        <td>
            <asp:CheckBox runat="server" ID="chkAllowReviews" />
        </td>
    </tr>
    <tr class="rowsPost">
        <td style="width: 250px; text-align: left;">
            <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ModerateReviews %>"></asp:Localize>
        </td>
        <td>
            <asp:CheckBox runat="server" ID="ckbModerateReviews" />
        </td>
    </tr>
    <tr>
        <td colspan="2" style="height: 34px;">
            <span class="spanSettCategory">
                <asp:Localize ID="Localize_Admin_CommonSettings_ZoomView" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_ZoomView %>"></asp:Localize></span>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr>
        <td style="width: 250px; height: 34px; text-align: left; vertical-align: top;">
            <asp:Localize ID="Localize16" runat="server" Text="<%$ Resources:Resource, Admin_CommonSettings_EnableZoom %>"></asp:Localize>
        </td>
        <td style="vertical-align: top;">&nbsp;<asp:CheckBox ID="chkEnableZoom" runat="server" />
        </td>
    </tr>
</table>
<asp:SqlDataSource ID="SqlDataSource2" runat="server" SelectCommand="SELECT [Name], [Code], [CurrencyIso3] FROM [Catalog].[Currency]"
    OnInit="SqlDataSource2_Init"></asp:SqlDataSource>
