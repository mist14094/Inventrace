<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PrintOrder.aspx.cs" Inherits="ClientPages.PrintOrder" %>

<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Orders" %>
<%@ Import Namespace="AdvantShop.Repository.Currencies" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="Admin/css/AdminStyle.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function position_this_window() {
            var x = (screen.availWidth - 770) / 2;
            window.resizeTo(762, 662);
            window.moveTo(Math.floor(x), 50);
        }
    </script>
    <title></title>
    <%if (mapType == "yandexmap") { %>
    <script src="http://api-maps.yandex.ru/2.0-stable/?load=package.standard&lang=ru-RU" type="text/javascript"> </script>
    <%} %>
	    <style>
          .grid-top {
            font-weight: bold;
            padding: 0 10px;
        }
    </style>
</head>
<body onload="position_this_window();window.print();">
    <form id="form1" runat="server">
    <div style="font-family: Arial; text-align: center; padding: 15px;">
        <table style="width: 100%;">
            <tr>
                <td>
                    <center>
                        <asp:Label ID="lblOrderID" CssClass="AdminHead" runat="server"></asp:Label><br />
                        <% if (showStatusInfo) { %>
                            (<%= order.OrderStatus.StatusName%>)<br />
                        <%} %>
                        <br />
                    </center>
                    <b>
                        <%=Resources.Resource.Admin_ViewOrder_Date%>
                    </b>
                    <asp:Label ID="lOrderDate" runat="server"></asp:Label>
                    <br />
                    <% if (showStatusInfo) { %>
                    <b>
                        <%=Resources.Resource.Admin_ViewOrder_Number%></b>
                        <%= order.Number%>
                    <br />                    
                    <b>
                        <%=Resources.Resource.Admin_ViewOrder_StatusComment%></b>
                        <%= order.StatusComment%>
                    <br />
                    <%} %>
                    <b>
                        <%=Resources.Resource.Admin_ViewOrder_Email%></b>
                        <%= order.OrderCustomer.Email%>
                    <br />
                    <b>
                        <%=Resources.Resource.Admin_ViewOrder_Telephone%></b>
                        <%= order.OrderCustomer.MobilePhone%>
                    <br />
                    <br />
                    <br />
                    <table border="0" width="100%" cellspacing="0" cellpadding="0">
                        <tr>
                            <td style="width: 34%; vertical-align: top" runat="server" id="trBilling">
                                <asp:Label ID="Label1" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Billing %>"></asp:Label>
                                <br />
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lblBilling" runat="server"></asp:Label>
                            </td>
                            <td style="width: 33%; vertical-align: top" runat="server" id="trShipping">
                                <asp:Label ID="Label2" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Shipping %>"></asp:Label>
                                <br />
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lblShipping" runat="server"></asp:Label>
                            </td>
                            <td style="width: 32%; vertical-align: top">
                                <asp:Label ID="Label3" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ShippingMethod %>"></asp:Label>
                                <br />
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lblShippingMethod" runat="server"></asp:Label>
                                <br />
                                <asp:Label ID="Label4" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_PaymentType %>"></asp:Label>
                                <br />
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="lblPaymentType" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <% if (Request["order"] == "details" && showMap) {
                           if (mapType == "googlemap") { %>
                           <%= string.Format("<div id=\"printorder-map\"><iframe width=\"500\" height=\"300\" frameborder=\"0\" scrolling=\"no\" marginheight=\"0\" marginwidth=\"0\" "
                                             + "src=\"http://maps.google.com/maps?ie=UTF8&iwloc=near&hl=ru&t=m&z=15&mrt=loc&geocode=&q={0}&output=embed\"></iframe></div>",
                                                HttpUtility.UrlEncode(mapAdress))%>
                        <% } else { %>
                        <script type="text/javascript">
                            var myMap;
                            ymaps.ready(function () {
                                var coordinates;
                                var myGeocoder = ymaps.geocode("<%= mapAdress %>");
                                myGeocoder.then(
                                    function (res) {
                                        coordinates = res.geoObjects.get(0).geometry.getCoordinates();
                                        myMap = new ymaps.Map("printorder-map", {
                                            center: coordinates,
                                            zoom: 15,
                                            behaviors: ["default", "scrollZoom"]
                                        });
                                        var myPlacemark = new ymaps.Placemark(coordinates);
                                        myMap.geoObjects.add(myPlacemark);
                                        myMap.controls.add("mapTools").add("zoomControl").add("typeSelector").add("trafficControl");
                                    }
                                );
                            });
                        </script>
                        <div id="printorder-map" style="width: 450px; height: 350px;"></div>
                    <%  }
                    } %>
                    <br />
                    <asp:Label ID="Label5" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_OrderItem %>"></asp:Label>
                    <br />
                    <br />
                    <asp:ListView ID="lvOrderItems" runat="server" ItemPlaceholderID="itemPlaceholderID">
                        <LayoutTemplate>
                            <table width="100%" border="0" cellspacing="0" cellpadding="0" class="grid-main">
                                <thead>
                                    <tr class="GridView_HeaderStyle">
                                        <td class="grid-top">
                                            <b>
                                                <asp:Label ID="lblAdmin_ViewOrder_ItemName" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemName %>"></asp:Label></b>
                                        </td>
                                        <td class="grid-top" runat="server" id="tdOptions">
                                            <b>
                                                <asp:Label ID="lblAdmin_ViewOrder_CustomOptions" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_CustomOptions %>"></asp:Label></b>
                                        </td>
                                        <td class="grid-top" style="width: 90px; text-align: center;">
                                            <b>
                                                <asp:Label ID="lblAdmin_ViewOrder_Price" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Price %>"></asp:Label></b>
                                        </td>
                                        <td class="grid-top" style="width: 55px; text-align: center;">
                                            <b>
                                                <asp:Label ID="lblAdmin_ViewOrder_ItemAmount" runat="server" Text="<%$ Resources:Resource, Client_Bill2_Count %>"></asp:Label></b>
                                        </td>
                                        <td class="grid-top" style="width: 90px; text-align: center;">
                                            <b>
                                                <asp:Label ID="lblAdmin_ViewOrder_ItemCost" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemCost %>"></asp:Label></b>
                                        </td>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr runat="server" id="itemPlaceholderID">
                                    </tr>
                                </tbody>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="grid-left">
                                    <%# Eval("ArtNo") + ", " + Eval("Name")%>
                                </td>
                                <% if (ShowOptions)
                                   { %>
                                <td class="grid-left">
                                    <%# Eval("Color") != null ?  SettingsCatalog.ColorsHeader + ": " + Eval("Color") + "<br />" : ""%>
                                    <%# Eval("Size") != null ?  SettingsCatalog.SizesHeader + ": " + Eval("Size") + "<br />" : ""%>
                                    <%#RenderSelectedOptions((IList<EvaluatedCustomOptions>)Eval("SelectedOptions"))%>
                                </td>
                                <% } %>
                                <td class="grid-even" style="text-align: center;">
                                    <%#CatalogService.GetStringPrice((float)Eval("Price"), 1, ordCurrency.CurrencyCode, ordCurrency.CurrencyValue)%>
                                </td>
                                <td class="grid-even" style="text-align: center;">
                                    <%#Eval("Amount")%>
                                </td>
                                <td class="grid-even" style="text-align: center;">
                                    <%#CatalogService.GetStringPrice((float)Eval("Price"), (float)Eval("Amount"), ordCurrency.CurrencyCode, ordCurrency.CurrencyValue)%>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:ListView>
                    <asp:ListView ID="lvOrderGiftCertificates" runat="server" ItemPlaceholderID="itemPlaceholderID">
                        <LayoutTemplate>
                            <table width="100%" border="0" cellspacing="0" cellpadding="0" class="grid-main">
                                <thead>
                                    <tr class="GridView_HeaderStyle">
                                        <th class="grid-top">
                                        </th>
                                        <th class="grid-top">
                                            <b>
                                                <asp:Label runat="server" Text="<%$ Resources:Resource,Client_PrintOrder_GiftCertificateCode %>"></asp:Label>
                                            </b>
                                        </th>
                                        <th class="grid-top" style="text-align: center;">
                                            <b>
                                                <asp:Label runat="server" Text="<%$ Resources:Resource,Client_PrintOrder_GiftCertificateSum %>"></asp:Label>
                                            </b>
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr runat="server" id="itemPlaceholderID">
                                    </tr>
                                </tbody>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="grid-left">
                                    <asp:Label runat="server" Text="<%$ Resources:Resource,Client_PrintOrder_GiftCertificate %>"></asp:Label>
                                </td>
                                <td class="grid-left">
                                    <%# Eval("CertificateCode") %>
                                </td>
                                <td class="grid-even">
                                    <%#CatalogService.GetStringPrice((float)Eval("Sum"), 1, ordCurrency.CurrencyCode, ordCurrency.CurrencyValue)%>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <tr>
                                <td class="grid-left_alt">
                                    <asp:Label ID="Label6" runat="server" Text="<%$ Resources:Resource,Client_PrintOrder_GiftCertificate %>"></asp:Label>
                                </td>
                                <td class="grid-left_alt">
                                    <%# Eval("CertificateCode") %>
                                </td>
                                <td class="grid-left_alt">
                                    <%#CatalogService.GetStringPrice((float)Eval("Sum"), 1, ordCurrency.CurrencyCode, ordCurrency.CurrencyValue)%>
                                </td>
                            </tr>
                        </AlternatingItemTemplate>
                    </asp:ListView>
                    <asp:Panel ID="pnlSummary" runat="server">
                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td style="background-color: #FFFFFF; text-align: right">
                                    <asp:Label ID="Label8" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemCost2 %>"></asp:Label>&nbsp;
                                </td>
                                <td style="background-color: #FFFFFF; width: 150px">
                                    <asp:Label ID="lblProductPrice" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="trDiscount" runat="server" visible="false">
                                <td style="background-color: #FFFFFF; text-align: right">
                                    <asp:Label ID="Label9" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ItemDiscount %>"></asp:Label>&nbsp;
                                </td>
                                <td style="background-color: #FFFFFF; width: 150px">
                                    <asp:Label ID="lblOrderDiscount" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="trCertificate" runat="server" visible="false">
                                <td style="background-color: #FFFFFF; text-align: right">
                                    <asp:Label ID="Label12" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Certificate %>"></asp:Label>:&nbsp;
                                </td>
                                <td style="background-color: #FFFFFF; width: 150px">
                                    <asp:Label ID="lblCertificate" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="trCoupon" runat="server" visible="false">
                                <td style="background-color: #FFFFFF; text-align: right">
                                    <asp:Label ID="Label16" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_Coupon%>"></asp:Label>:&nbsp;
                                </td>
                                <td style="background-color: #FFFFFF; width: 150px">
                                    <asp:Label ID="lblCoupon" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td style="background-color: #FFFFFF; text-align: right">
                                    <asp:Label ID="Label10" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_ShippingPrice %>"></asp:Label>&nbsp;
                                </td>
                                <td style="background-color: #FFFFFF; width: 150px">
                                    <asp:Label ID="lblShippingPrice" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="PaymentRow" runat="server">
                                <td style="background-color: #FFFFFF; text-align: right">
                                    <asp:Label ID="Label6" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_PaymentExtracharge %>"></asp:Label>&nbsp;
                                </td>
                                <td style="background-color: #FFFFFF; width: 150px">
                                    <asp:Label ID="lblPaymentPrice" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <asp:Literal ID="literalTaxCost" runat="server"></asp:Literal>
                            <tr>
                                <td style="background-color: #FFFFFF; text-align: right">
                                    <b>
                                        <asp:Label ID="Label11" runat="server" Text="<%$ Resources:Resource, Admin_ViewOrder_TotalPrice %>"></asp:Label>&nbsp;</b>
                                </td>
                                <td style="background-color: #FFFFFF; width: 150px">
                                    <b>
                                        <asp:Label ID="lblTotalPrice" runat="server"></asp:Label></b>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Label ID="lblUserComments" runat="server" Text="<%$ Resources:Resource, Client_PrintOrder_YourComment %>"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
