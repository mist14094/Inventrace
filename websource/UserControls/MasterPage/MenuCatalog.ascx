<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MenuCatalog.ascx.cs" EnableViewState="false"
    Inherits="UserControls_MenuCatalog" %>
<%@ Register Src="~/UserControls/MasterPage/Search.ascx" TagName="Search" TagPrefix="adv" %>

<div class="tree" id="tree">
    <div class="left">
        <div class="right">
            <div class="center">
                <nav class="tree-menu"><%=GetMenu()%></nav>
                <adv:Search runat="server" ID="searchBlock" />
            </div>
        </div>
    </div>
</div>