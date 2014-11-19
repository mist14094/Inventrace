<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductVideoView.ascx.cs" Inherits="UserControls_ProductVideoView" %>
<% if (hasVideos)
   { %>
   <div data-plugin="videos" data-productId="<%=ProductID %>"></div>
<% } %>