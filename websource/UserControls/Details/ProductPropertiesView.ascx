﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductPropertiesView.ascx.cs"
    Inherits="UserControls_ProductPropertiesView" %>
    
<asp:ListView runat="server" ID="lvProperties" ItemPlaceholderID="liPlaceHolder">
    <LayoutTemplate>
        <ul class="properties">
            <li runat="server" id="liPlaceHolder" />
        </ul>
    </LayoutTemplate>
    <ItemTemplate>
        <li><span class="param-name"><%#Eval("Property.Name")%></span> <span class="param-value"><%#Eval("Value")%></span>
    </li>
    </ItemTemplate>
</asp:ListView>