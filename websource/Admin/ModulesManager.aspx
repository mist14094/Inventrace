<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ModulesManager.aspx.cs" Inherits="Admin.ModulesManager"
    MasterPageFile="MasterPageAdmin.master" %>

<asp:Content ID="contentMain" runat="server" ContentPlaceHolderID="cphMain">
    <div id="inprogress" style="display: none">
        <div id="curtain" class="opacitybackground">
            &nbsp;
        </div>
        <div class="loader">
            <table width="100%" style="font-weight: bold; text-align: center;">
                <tbody>
                    <tr>
                        <td align="center">
                            <img src="images/ajax-loader.gif" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center" style="color: #0D76B8;">
                            <asp:Localize ID="Localize_Admin_Properties_PleaseWait" runat="server" Text="<%$ Resources:Resource, Admin_Properties_PleaseWait %>"></asp:Localize>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div class="content-own">
        <table cellpadding="0" cellspacing="0" width="100%">
            <tbody>
                <tr>
                    <td style="width: 72px;">
                        <img src="images/orders_ico.gif" alt="" />
                    </td>
                    <td>
                        <asp:Label ID="lblHead" CssClass="AdminHead" runat="server" Text="<%$ Resources:Resource, Admin_ModuleManager_Header %>"></asp:Label><br />
                        <asp:Label ID="lblSubHead" CssClass="AdminSubHead" runat="server" Text="<%$ Resources:Resource, Admin_ModuleManager_SubHeader %>"></asp:Label>
                        <br />
                        <br />
                        <asp:Label ID="lTrialMode" runat="server" Text="<%$ Resources:Resource, Admin_Module_TrialMode %>" ForeColor="Red"></asp:Label>
                        <br />
                        <br />
                    </td>
                    <td style="vertical-align: bottom; padding-right: 10px">
                        <div style="float: right; padding-right: 10px">
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
        <br>
        <script>
            function progressShow() {
                document.getElementById('inprogress').style.display = 'block';
            }
            function progressHide() {
                document.getElementById('inprogress').style.display = 'none';
            }
        </script>

        <iframe src="ModulesManagerInside.aspx" frameborder="0" height="690" width="100%" hspace="0" scrolling="auto"></iframe>

    </div>
</asp:Content>
