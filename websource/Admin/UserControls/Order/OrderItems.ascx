<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OrderItems.ascx.cs" Inherits="Admin.UserControls.Order.OrderItemsControl" %>
<%@ Register Src="~/Admin/UserControls/PopupTreeView.ascx" TagName="PopupTree" TagPrefix="adv" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<adv:PopupTree runat="server" ID="pTreeProduct" HeaderText="<%$ Resources:Resource, Admin_CatalogLinks_ParentCategory %>"
    Type="CategoryProduct" ExceptId="0" OnTreeNodeSelected="pTreeProduct_NodeSelected"
    OnHiding="pTreeProduct_Hiding" />
<asp:Label runat="server" ID="lblError" Visible="false"></asp:Label>
<table style="width: 100%">
    <tr>
        <td>
            <asp:SqlDataSource runat="server" ID="sdsCurrs" OnInit="sds_Init" SelectCommand="SELECT * FROM [Catalog].[Currency]">
            </asp:SqlDataSource>
            <span>
                <% = Resources.Resource.Admin_ViewOrder_ChoosingCurrency%>: </span>
            <asp:DropDownList ID="ddlCurrs" runat="server" DataSourceID="sdsCurrs" DataTextField="Name"
                DataValueField="CurrencyIso3" AutoPostBack="true" CausesValidation="false" OnSelectedIndexChanged="ddlCurrs_SelectedChanged" Visible="false">
            </asp:DropDownList>
            <asp:Label runat="server" ID="lcurrency" />
            <asp:HiddenField runat="server" ID="hfOldCurrencyValue" />
        </td>
        <td>
            <asp:Label runat="server" ID="lDiscount" Text="<%$ Resources: Resource, Admin_EditOrder_Discount%>"></asp:Label>:
            <asp:TextBox runat="server" ID="txtDiscount" Width="30px"></asp:TextBox>
            %
        </td>
        <td>
            <div style="float: right;">
                <div style="display: inline-block; vertical-align: top;padding: 3px 0 0 0;">
                    <asp:TextBox runat="server" ID="txtArtNo" CssClass="autocompleteSearch" Width="230px" />
                    <input type="hidden" id="hfOffer" runat="server" class="acsearchhf" />
                </div>
                <div style="display: inline-block;">
                    <adv:OrangeRoundedButton CausesValidation="false" ID="btnAddProductByArtNo" runat="server"
                        OnClick="btnAddProductByArtNo_Click" Text="<%$ Resources: Resource, Admin_OrderSearch_AddProductByArtNoOrName%>" />
                </div>
            </div>
        </td>
        <td>
            <div style="float: right;">
                <adv:OrangeRoundedButton CausesValidation="false" ID="btnAddProduct" runat="server"
                    OnClick="btnAddProduct_Click" Text="<%$ Resources: Resource, Admin_OrderSearch_AddProduct%>" />
            </div>
        </td>
    </tr>
    <tr class="formheaderfooter">
        <td colspan="2">
        </td>
    </tr>
</table>
<div style="text-align: center;">
    <asp:Repeater ID="DataListOrderItems" runat="server" OnItemCommand="dlItems_ItemCommand"
        OnItemDataBound="dlItems_ItemDataBound">
        <HeaderTemplate>
            <table width="100%" border="0" cellspacing="0" cellpadding="3" class="grid-main">
                <tr class="header">
                    <td>
                        &nbsp;
                    </td>
                    <td>
                        <b>
                            <%= Resources.Resource.Admin_ViewOrder_ArtNo %></b>
                    </td>
                    <td>
                        <b>
                            <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemName %>"></asp:Label></b>
                    </td>
                    <td>
                        <b>
                            <asp:Label ID="Label14" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_CustomOptions %>"></asp:Label></b>
                    </td>
                    <td style="width: 100px; text-align: center;">
                        <b>
                            <asp:Label ID="Label2" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Size %>"></asp:Label>
                        </b>
                    </td>
                    <td style="width: 100px; text-align: center;">
                        <b>
                            <asp:Label ID="Label3" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Color %>"></asp:Label>
                        </b>
                    </td>
                    <td style="width: 150px; text-align: center;">
                        <b>
                            <asp:Label ID="Label13" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Price %>"></asp:Label>
                        </b>
                    </td>
                    <td style="width: 100px; text-align: center;">
                        <b>
                            <asp:Label ID="Label6" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemAmount %>"></asp:Label>
                        </b>
                    </td>
                    <% if (AdvantShop.Configuration.SettingsOrderConfirmation.AmountLimitation)
                       {%>
                    <td class="OrderTableHead" style="width: 100px;">
                        <asp:Localize ID="Localize_Client_ShoppingCart_Available" runat="server" Text="<%$ Resources:Resource, Client_ShoppingCart_AvailableHeader %>"></asp:Localize>
                    </td>
                    <%
                       }%>
                    <td style="width: 150px; text-align: center;">
                        <b>
                            <asp:Label ID="Label7" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemCost %>"></asp:Label>
                        </b>
                    </td>
                    <td style="width: 30px">
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class="row1" style="height: 35px;">
                <td>
                    <%# Eval("ProductID") != null ? RenderPicture((int)Eval("ProductID"), SQLDataHelper.GetInt(Eval("PhotoID"))) : ""%>
                </td>
                <td>
                    <asp:Literal runat="server" ID="ltArtNo" Text='<%#Eval("ArtNo")%>'></asp:Literal>
                </td>
                <td>
                    <%# Eval("Name")%>
                </td>
                <td>
                    <%#RenderSelectedOptions((IList<EvaluatedCustomOptions>)Eval("SelectedOptions"))%>
                </td>
                <td>
                    <%# Eval("Size")%>
                </td>
                <td>
                    <%# Eval("Color")%>
                </td>
                <td style="text-align: center;">
                    <%#  CatalogService.GetStringPrice((float)Eval("Price"), 1, CurrencyCode, CurrencyValue)%>
                </td>
                <td style="text-align: center;">
                    <asp:TextBox ID="txtQuantity" runat="server" Text='<%# Eval("Amount") %>' Width="25"></asp:TextBox>
                    <asp:ImageButton CausesValidation="false" ID="btnQuantUp" ImageUrl="~/Admin/images/refresh.png"
                        runat="server" CommandArgument='<%# Eval("OrderItemID") %>' CommandName="SaveQuantity"/>
                </td>
                <% if (AdvantShop.Configuration.SettingsOrderConfirmation.AmountLimitation)
                   {%>
                <td class="OrderTable_td" style="width: 100px;">
                    <asp:Label ID="lbMaxCount" runat="server" Text="Label" ForeColor="Red" CssClass="lbMaxCount" Font-Bold="true"/>
                </td>
                <%
                   }%>
                <td style="text-align: center;">
                    <%# CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Price")), SQLDataHelper.GetFloat(Eval("Amount")), CurrencyCode, CurrencyValue)%>
                </td>
                <td>
                    <asp:ImageButton runat="server" ID="btnDelete" CausesValidation="false" CommandArgument='<%#  Eval("OrderItemID") %>'
                        CommandName="Delete" ImageUrl="~/Admin/images/deletebtn.png" />
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</div>
