﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ModulesManagerInside.aspx.cs"
    Inherits="Admin.ModulesManagerInside" %>

<%@ Import Namespace="AdvantShop" %>
<%@ Import Namespace="Resources" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="../css/normalize.css" />
    <link rel="stylesheet" href="css/new_admin/buttons.css" />
    <link rel="stylesheet" href="css/new_admin/pagenumber.css" />
    <link rel="stylesheet" href="css/new_admin/modules.css" />
    <script src="../js/jq/jquery-1.7.1.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="Server" EnablePartialRendering="true" ID="ScriptManager1"
        ScriptMode="Release">
    </asp:ScriptManager>
    <asp:ListView runat="server" ID="lvModulesManager" ItemPlaceholderID="pl" OnItemCommand="lvModules_ItemCommand"
        Visible="True">
        <LayoutTemplate>
            <div>
                <ul class="modules-list">
                    <asp:PlaceHolder runat="server" ID="pl" />
                </ul>
            </div>
        </LayoutTemplate>
        <ItemTemplate>
            <li class="modules-item-item">
                <div class="modules-item-pic-cell">
                    <div class="modules-item-pic-wrap">
                        <a class="modules-item-pic-lnk inset-shadow" href="javascript:void(0);" onclick="<%# !string.IsNullOrEmpty(Convert.ToString(Eval("DetailsLink"))) ? "parent.open('"+ Eval("DetailsLink") +"', 'AdvantshopDetails')"  : "return false;" %>">
                            <img alt="<%# Eval("Name") %>" src="<%# !string.IsNullOrEmpty(Convert.ToString(Eval("Icon"))) ? Eval("Icon").ToString() : "images/new_admin/modules/nophoto.jpg" %>"
                                class="modules-item-pic">
                        </a>
                    </div>
                    <div runat="server" visible='<%# !string.IsNullOrEmpty(Convert.ToString(Eval("DetailsLink"))) %>'
                        class="modules-item-more">
                        <a href="javascript:void(0);" onclick="<%#!string.IsNullOrEmpty(Convert.ToString(Eval("DetailsLink"))) ? "parent.open('"+ Eval("DetailsLink") +"', 'AdvantshopDetails')" : "return false;" %>"
                            class="modules-item-more-lnk">
                            <%= Resource.Admin_Module_More %></a></div>
                </div>
                <div class="modules-item-info">
                    <div class="modules-item-title">
                        <a class="modules-item-title-lnk" target="_blank" href="<%# !string.IsNullOrEmpty(Convert.ToString(Eval("DetailsLink"))) ? Eval("DetailsLink") : "javascript:void(0);" %>">
                            <%# Eval("Name") %>
                        </a>
                        <asp:HiddenField ID="hfLastVersion" runat="server" Value='<%# Eval("Version") %>' />
                        <asp:HiddenField ID="hfId" runat="server" Value='<%# Eval("Id") %>' />
                    </div>
                    <div class="modules-item-descr">
                        <%# Eval("BriefDescription") %>
                    </div>
                   
                    <div class="justify">
                        <div class="justify-item">
                            <div runat="server" visible='<%# !Convert.ToBoolean(Eval("IsInstall")) %>' class="modules-item-module-price">
                                <%#  Convert.ToDecimal(Eval("Price")) != 0 ? String.Format("{0:##,##0.##}", Eval("Price")) + " " + Eval("Currency") : Resource.Admin_Modules_FreeCost%></div>
                            <a class="btn btn-middle btn-action" onclick='<%# "parent.location.assign(\"" + "Module.aspx?module=" + Eval("StringId") + "\")" %>'
                                runat="server" href="javascript:void(0)" visible='<%# Convert.ToBoolean(Eval("IsInstall")) %>'>
                                <%= Resource.Admin_ModulesManager_Settings %></a>
                        </div>
                         <% if (!Trial.IsTrialEnabled)
                       { %>
                        <div class="justify-item">
                            <asp:Button OnClientClick="parent.progressShow();" runat="server" ID="btnInstall"
                                CausesValidation="false" CommandArgument='<%# Eval("StringId") %>' CommandName="Install"
                                CssClass="btn btn-middle btn-submit" Text='<%$ Resources:Resource, Admin_ModulesManager_Install %>'
                                Visible='<%# !Convert.ToBoolean(Eval("IsInstall")) && Convert.ToBoolean(Eval("Active")) %>' />
                            <a runat="server" visible='<%# Convert.ToBoolean(Eval("IsInstall")) && !(Convert.ToBoolean(Eval("Active")) && !Convert.ToString(Eval("Version")).Equals(Convert.ToString(Eval("CurrentVersion")))) %>'
                                class="btn btn-middle btn-disabled" href="javascript:void(0);">
                                <%= Resource.Admin_ModulesManager_Installed %></a>
                            <asp:Button OnClientClick="parent.progressShow();" ID="btnInstallLastVersion" runat="server"
                                CssClass="btn btn-middle btn-update" CommandArgument='<%# Eval("StringId") %>'
                                CommandName="InstallLastVersion" Text='<%$ Resources : Resource , Admin_Modules_Update%>'
                                Visible='<%# Convert.ToBoolean(Eval("IsInstall")) && Convert.ToBoolean(Eval("Active")) && !Convert.ToString(Eval("Version")).Equals(Convert.ToString(Eval("CurrentVersion"))) %>' />
                            <a id="A2" class="btn btn-middle btn-action" runat="server" href='<%# Eval("DetailsLink") %>'
                                visible='<%# !Convert.ToBoolean(Eval("IsInstall")) && !Convert.ToBoolean(Eval("Active")) %>' target="_blank"> 
                            Купить</a> 
                        </div>
                         <% } %>
                    </div>
                    <asp:LinkButton CssClass="module-delete" runat="server" ID="btnDelete" CausesValidation="false"
                        CommandArgument='<%# Eval("StringId") %>' CommandName="Uninstall" Visible='<%# !Trial.IsTrialEnabled && Convert.ToBoolean(Eval("IsInstall")) %>'>
                            <img src="images/deletebtn.png" onclick="if(confirm('<%= Resource.Admin_ThemesSettings_Confirmation %>')){ parent.progressShow(); }else{ return false;}" alt='<%= Resource.Admin_ModulesManager_Delete %>' />
                    </asp:LinkButton>
                   
                </div>
            </li>
        </ItemTemplate>
    </asp:ListView>
    <div class="modules-paging">
        <adv:AdvPaging runat="server" ID="paging" DisplayArrows="false" DisplayPrevNext="false"
            DisplayShowAll="false" />
    </div>
    </form>
    <script>
        $(function () {

            $(document).on('keydown.pagenumber', function (e) {
                //37 - left arrow
                //39 - right arrow
                if (e.ctrlKey === true && e.keyCode === 37) {
                    if ($("#paging-prev").length)
                        document.location = $("#paging-prev").attr("href");
                } else if (e.ctrlKey === true && e.keyCode === 39) {
                    if ($("#paging-next").length)
                        document.location = $("#paging-next").attr("href");
                }
            });
        });

        $(window).load(function () {
            parent.progressHide();
        });
    </script>
</body>
</html>
