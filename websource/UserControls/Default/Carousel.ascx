﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Carousel.ascx.cs" Inherits="UserControls_Default_Carousel" %>
<%@ Import Namespace="AdvantShop" %>
<%@ Import Namespace="AdvantShop.CMS" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Core" %>
<%@ Import Namespace="AdvantShop.FilePath" %>
<%@ Import Namespace="System.IO" %>
<!--slider-->
<div class="flexslider<%= CssSlider.IsNotEmpty() ? " " + CssSlider : "" %>" data-plugin="flexslider" data-flexslider-options="<%= "{animation:'" + SettingsDesign.CarouselAnimation + "', animationSpeed:" + SettingsDesign.CarouselAnimationSpeed + ", slideshowSpeed:" + SettingsDesign.CarouselAnimationDelay + "}" %>">
    <ul class="slides">
        <% foreach (var item in CarouselService.GetAllCarouselsMainPage())
           {%>
        <li><a href="<%=item.URL %>">
            <img class="pie" src="<%= File.Exists(FoldersHelper.GetPathAbsolut(FolderType.Carousel, item.Picture.PhotoName)) 
                                    ? FoldersHelper.GetPath(FolderType.Carousel, item.Picture.PhotoName , false  )
                                    : "images/nophoto_carousel.jpg" %>" alt="" /></a></li>
        <%}%>
    </ul>
</div>
<!--end_slider-->
