<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SizeColorPicker.ascx.cs" Inherits="UserControls_Details_SizeColorPicker" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<div data-productId="<%= ProductId %>" 
     data-part="sizeColorPicker" 
     data-sizeHeader="<%= SettingsCatalog.SizesHeader %>" 
     data-colorHeader="<%= SettingsCatalog.ColorsHeader %>"
     data-sizeColorPicker-options ="{type: '<%= Type.ToString() %>'}"
    >
</div>
