﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MasterControl.ascx.cs"
    Inherits="Admin.UserControls.ShippingMethods.MasterControl" %>
<%@ Import Namespace="AdvantShop" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<input type="hidden" id="hfShipping<%= Method == null ? "" : Method.ShippingMethodId.ToString()  %>" />
<table border="0" cellpadding="2" cellspacing="0" style="width: 99%; margin-left: 5px;
    margin-top: 5px;">
    <tr class="rowPost">
        <td colspan="2" style="height: 34px;">
            <h4 style="display: inline; font-size: 12pt;">
                <asp:Localize ID="Localize13" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethods_HeadGeneral%>"></asp:Localize></h4>
            <hr color="#C2C2C4" size="1px" />
        </td>
    </tr>
    <tr>
        <td style="width: 250px; height: 34px; text-align: left; vertical-align: top;">
            <asp:Localize ID="Localize14" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_Name %>"></asp:Localize><span
                class="required">*</span>
        </td>
        <td style="vertical-align: top">
            <asp:TextBox runat="server" ID="txtName" Width="250" MaxLength="50"></asp:TextBox>
            <asp:Label runat="server" ID="msgName" Visible="false"></asp:Label>
        </td>
    </tr>
    <tr>
        <td style="width: 250px; height: 34px; text-align: left; vertical-align: top;">
            <asp:Localize ID="Localize3" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_MethodAvailibleFor %>"></asp:Localize>
        </td>
        <td style="vertical-align: top">
            <div id="GEO">
            </div>
        </td>
    </tr>
    <tr>
        <td style="width: 250px; height: 34px; text-align: left; vertical-align: top;">
        </td>
        <td style="vertical-align: top">
            <div style="width: 70px; float: left;">
                <%= Resources.Resource.Admin_ShippingMethod_Countries%>:</div>
            <asp:TextBox runat="server" ID="txtCountry" Width="177"></asp:TextBox>
            <asp:LinkButton ID="btnAddCountry" runat="server" ValidationGroup="1" Text="<%$ Resources:Resource, Admin_ShippingMethod_AddCountry%>"
                OnClick="btnAddCountry_Click"></asp:LinkButton>
            <asp:Label runat="server" ID="msgCountry" Visible="false"></asp:Label>
        </td>
    </tr>
    <tr>
        <td style="width: 250px; height: 34px; text-align: left; vertical-align: top;">
        </td>
        <td style="vertical-align: top">
            <div style="width: 70px; float: left;">
                <%= Resources.Resource.Admin_ShippingMethod_Cities%>:</div>
            <asp:TextBox runat="server" ID="txtCity" Width="177"></asp:TextBox>
            <asp:LinkButton ID="btnAddCity" runat="server" ValidationGroup="1" Text="<%$ Resources:Resource, Admin_ShippingMethod_AddCity%>"
                OnClick="btnAddCity_Click"></asp:LinkButton>
        </td>
    </tr>
    <tr>
        <td style="width: 250px; height: 34px; text-align: left; vertical-align: top;">
            <asp:Localize ID="Localize1" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_Type %>"></asp:Localize>
        </td>
        <td style="vertical-align: top">
            <%= string.Format("{0} ({1})", ShippingType.GetLocalizedName(), (int)ShippingType)%>
        </td>
    </tr>
    <tr>
        <td style="width: 250px; height: 34px; text-align: left; vertical-align: top;">
            <asp:Localize ID="Localize15" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_Description %>"></asp:Localize>
        </td>
        <td style="vertical-align: top">
            <asp:TextBox runat="server" ID="txtDescription" Width="250" TextMode="MultiLine"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td style="width: 250px; height: 34px; text-align: left; vertical-align: top;">
            <asp:Localize ID="Localize16" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_SortOrder %>"></asp:Localize>
        </td>
        <td style="vertical-align: top">
            <asp:TextBox runat="server" ID="txtSortOrder" Width="250"></asp:TextBox>
            <asp:Label runat="server" ID="msgSortOrder" Visible="false"></asp:Label>
        </td>
    </tr>
    <tr>
        <td style="width: 250px; height: 34px; text-align: left; vertical-align: top;">
            <asp:Localize ID="Localize17" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_Enabled %>"></asp:Localize>
        </td>
        <td style="vertical-align: top">
            <asp:CheckBox runat="server" ID="chkEnabled" />
        </td>
    </tr>
    <tr>
        <td style="width: 250px; height: 34px; text-align: left; vertical-align: top;">
            <asp:Localize ID="Localize6" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_Icon %>"></asp:Localize>
        </td>
        <td style="vertical-align: top">
            <table>
                <tr>
                    <td>
                        <asp:Image runat="server" ID="imgIcon" />
                    </td>
                    <td>
                        <asp:FileUpload runat="server" ID="fuIcon" />
                        <asp:Button runat="server" ID="btnUploadIcon" Text="<%$ Resources:Resource, Admin_ShippingMethod_UploadIcon %>"
                            OnClick="btnUploadIcon_Click" CausesValidation="false" />
                        <asp:Button runat="server" ID="btnDeleteIcon" Text="<%$ Resources:Resource, Admin_ShippingMethod_DeleteIcon %>"
                            OnClick="btnDeleteIcon_Click" CausesValidation="false" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td style="width: 250px; height: 34px; text-align: left; vertical-align: top;">
            <asp:Localize ID="Localize2" runat="server" Text="<%$ Resources:Resource, Admin_ShippingMethod_ActiveForPayments %>"></asp:Localize>
        </td>
        <td style="vertical-align: top">
            <asp:Repeater ID="rptrPayments" runat="server">
                <ItemTemplate>
                    <asp:HiddenField ID="hfPaymentId" runat="server" Value='<%# Eval("PaymentMethodID")%>' />
                    <asp:CheckBox ID="ckbUsePayment" runat="server" Text='<%#Eval("Name") %>' Checked='<%# SQLDataHelper.GetInt(Eval("Use")) == 0 %>' /><br />
                </ItemTemplate>
            </asp:Repeater>
        </td>
    </tr>
</table>
<asp:Panel ID="pnlSpecific" Width="100%" runat="server">
</asp:Panel>
<div style="width: 340px; height: 40px;">
    <adv:RoundedButton Type="Orange" runat="server" ID="btnDelete" Width="170px" Text="<%$ Resources: Resource, Admin_ShippingMethod_Delete %>"
        OnClick="btnDelete_Click" CausesValidation="false" />
    <ajaxToolkit:ConfirmButtonExtender runat="server" TargetControlID="btnDelete" ID="cbeDelete" />
    <adv:RoundedButton Type="Orange" runat="server" ID="btnSave" Width="150px" Text="<%$ Resources: Resource, Admin_ShippingMethod_Save %>"
        OnClick="btnSave_Click" ValidationGroup="5"/>
</div>
<script type="text/javascript">

    function DelCity(cityID, methodId) {
        DelCityHandler(cityID, methodId, '<% = UrlService.GetAbsoluteLink( "HttpHandlers/DeleteCity.ashx") %>');
    }

    function DelCityHandler(cityID, methodId, url) {
        $.ajax({
            data: { cityID: cityID, methodId: methodId, subject: 'shipping' },
            url: url,
            dataType: "html",
            cache: false,
            success: function (result) {
                GetResult('<% = UrlService.GetAbsoluteLink( "HttpHandlers/GetAdminCountryCity.ashx?methodId=" + ShippingMethodID) %>');
            },
            error: function (result) {
                alert(result);
            }
        });
    }

    function DelCountry(countryId, methodId) {
        DelCountryHandler(countryId, methodId, '<% = UrlService.GetAbsoluteLink( "HttpHandlers/DeleteCountry.ashx") %>');
    }

    function DelCountryHandler(countryId, methodId, url) {
        $.ajax({
            data: { countryId: countryId, methodId: methodId, subject: 'shipping' },
            url: url,
            dataType: "html",
            cache: false,
            success: function (result) {
                GetResult('<% = UrlService.GetAbsoluteLink( "HttpHandlers/GetAdminCountryCity.ashx?methodId=" + ShippingMethodID) %>');
            },
            error: function (result) {
                alert(result);
            }
        });
    }

    function GetResult(url) {
        $.ajax({
            url: url,
            data: { subject: 'shipping' },
            dataType: "html",
            async: false,
            cache: false,
            success: function (result) {
                $("#GEO").html(result);
            },
            error: function (result) {
                alert(result);
            }
        });
    }

    $(document).ready(function () {
        GetResult('<% = UrlService.GetAbsoluteLink( "HttpHandlers/GetAdminCountryCity.ashx?methodId=" + ShippingMethodID) %>');
    });
    
</script>
<script type="text/javascript">
    $(function () {
        window.Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function () {

            GetResult('<% = UrlService.GetAbsoluteLink( "HttpHandlers/GetAdminCountryCity.ashx?methodId=" + ShippingMethodID) %>');

            $("#<%=txtCountry.ClientID%>").autocomplete('<%=UrlService.GetAbsoluteLink("/HttpHandlers/GetCountries.ashx") %>', {
                delay: 10,
                minChars: 1,
                matchSubset: 1,
                autoFill: true,
                matchContains: 1,
                cacheLength: 10,
                selectFirst: true,
                //formatItem: liFormat,
                maxItemsToShow: 10
            });

            $("#<%=txtCity.ClientID%>").autocomplete('<%=UrlService.GetAbsoluteLink("/HttpHandlers/GetCities.ashx") %>', {
                delay: 10,
                minChars: 1,
                matchSubset: 1,
                autoFill: true,
                matchContains: 1,
                cacheLength: 10,
                selectFirst: true,
                //formatItem: liFormat,
                maxItemsToShow: 10
            });

            $("img[src='images/messagebox_info.png']").hide().filter("[title]").show();
        });
    });
    function ConfirmDelete() {
        return;
    }
</script>
