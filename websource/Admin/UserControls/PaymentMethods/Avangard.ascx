﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Avangard.ascx.cs" Inherits="Admin.UserControls.PaymentMethods.AvangardControl" %>

<table border="0" cellpadding="2" cellspacing="0" style="width: 99%; margin-left: 5px;
    margin-top: 5px;">
    <tr class="rowPost">
        <td colspan="3" style="height: 34px;">
            <h4 style="display: inline; font-size: 12pt;">
                <asp:Localize ID="Localize13" runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethods_HeadSettings %>"></asp:Localize></h4>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Robocassa_MerchantLogin %>"></asp:Localize><span
                class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtShopId" Width="250"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Image runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="<%$ Resources:Resource, Admin_PaymentMethod_Robocassa_MerchantLogin_Description %>" /><asp:Label
                    runat="server" ID="msgShopId" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="columnName">
            <asp:Localize runat="server" Text="<%$ Resources:Resource, Admin_PaymentMethod_Robocassa_Password %>"></asp:Localize><span
                class="required">&nbsp;*</span>
        </td>
        <td class="columnVal">
            <asp:TextBox runat="server" ID="txtShopPassword" Width="250"></asp:TextBox>
        </td>
        <td class="columnDescr">
            <asp:Image runat="server" Width="18" Height="18" ImageUrl="~/Admin/images/messagebox_info.png"
                ToolTip="<%$ Resources:Resource, Admin_PaymentMethod_Robocassa_Password_Description %>" /><asp:Label
                    runat="server" ID="msgShopPassword" Visible="false" ForeColor="Red"></asp:Label>
        </td>
    </tr>
</table>
