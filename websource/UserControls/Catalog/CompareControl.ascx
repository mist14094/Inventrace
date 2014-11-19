<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CompareControl.ascx.cs"
    Inherits="UserControls_Catalog_CompareControl" %>
<%@ Import Namespace="Resources" %>
<input class="compare-checkbox" data-plugin="compare" data-compare-animation-obj=".compare-<%=OfferId%>"
    type="checkbox" id="<%= "chk_" + OfferId %>" <%= IsSelected ? "checked=checked" : "" %>
    value="<%=OfferId %>" data-compare-options='<%= GetOptions() %>' />
<label class="compare-label" for="<%= "chk_" + OfferId %>">
    <%= IsSelected ? Resource.Client_Catalog_AlreadyCompare + " (<a href='compareproducts.aspx' target='_blank'>" + Resource.Client_Compare_View + "</a>)" : Resource.Client_Catalog_Compare%>
</label>