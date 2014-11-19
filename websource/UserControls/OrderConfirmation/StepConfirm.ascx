<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StepConfirm.ascx.cs" Inherits="UserControls_FourthStep" %>
<%@ Register TagPrefix="adv" TagName="captchacontrol" Src="~/UserControls/Captcha.ascx" %>
<%@ Import Namespace="AdvantShop.Catalog" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<%@ Import Namespace="AdvantShop.Orders" %>
<%@ Import Namespace="AdvantShop.Repository.Currencies" %>
<%@ Import Namespace="Resources" %>
<asp:Label runat="server" ID="lCouponMessage" Visible="false" Text="<%$ Resources: Resource,Client_OrderConfirmation_CouponNotice %>"></asp:Label>
<ul class="order-info">
    <li>
        <div class="title">
            <% =Resource.Client_OrderConfirmation_ShippingAddress%></div>
        <div class="str">
            <span class="param-name">
                <% =Resource.Client_OrderConfirmation_Country%>:</span>
            <asp:Label class="param-value" runat="server" ID="lblShippingCountry"></asp:Label>
        </div>
        <div class="str">
            <span class="param-name">
                <% =Resource.Client_OrderConfirmation_Region%>:</span>
            <asp:Label class="param-value" runat="server" ID="lblShippingRegion"></asp:Label>
        </div>
        <div class="str">
            <span class="param-name">
                <% =Resource.Client_OrderConfirmationt_City%>:</span>
            <asp:Label class="param-value" runat="server" ID="lblShippingCity">
            </asp:Label>
        </div>
        <div class="str">
            <span class="param-name">
                <% =Resource.Client_OrderConfirmation_Zip%>:</span>
            <asp:Label class="param-value" runat="server" ID="lblShippingZip"></asp:Label>
        </div>
        <div class="str">
            <span class="param-name">
                <% =Resource.Client_OrderConfirmation_Address%>:</span>
            <asp:Label class="param-value" runat="server" ID="lblShippingAddress">
            </asp:Label>
        </div>
    </li>
    <li>
        <div class="title">
            <% =Resource.Client_OrderConfirmation_BillingAddress%></div>
        <div class="str">
            <span class="param-name">
                <% =Resource.Client_OrderConfirmation_Country%>:</span>
            <asp:Label class="param-value" runat="server" ID="lblBillingCountry"></asp:Label>
        </div>
        <div class="str">
            <span class="param-name">
                <% =Resource.Client_OrderConfirmation_Region%>:</span>
            <asp:Label class="param-value" runat="server" ID="lblBillingRegion"></asp:Label>
        </div>
        <div class="str">
            <span class="param-name">
                <% =Resource.Client_OrderConfirmationt_City%>:</span>
            <asp:Label class="param-value" runat="server" ID="lblBillingCity">
            </asp:Label>
        </div>
        <div class="str">
            <span class="param-name">
                <% =Resource.Client_OrderConfirmation_Zip%>:</span>
            <asp:Label class="param-value" runat="server" ID="lblBillingZip"></asp:Label>
        </div>
        <div class="str">
            <span class="param-name">
                <% =Resource.Client_OrderConfirmation_Address%>:</span>
            <asp:Label class="param-value" runat="server" ID="lblBillingAddress">
            </asp:Label>
        </div>
    </li>
    <li>
        <div class="shipping-info">
            <div class="title">
                <% =Resource.Client_OrderConfirmation_ShippingMethod%></div>
            <div class="str">
                <asp:Label ID="lblShippingMethod" runat="server"></asp:Label>
            </div>
        </div>
        <div>
            <div class="title">
                <% =Resource.Client_OrderConfirmation_PayMethod %></div>
            <div class="str">
                <asp:Label ID="lblPaymentType" runat="server"></asp:Label>
            </div>
        </div>
    </li>
</ul>
<div class="order-list">
    <div class="title">
        <%= Resource.Client_OrderConfirmation_OrderContent %></div>
    <div>
        <table class="fullcart">
            <asp:ListView ID="lvOrderList" runat="server" ItemPlaceholderID="itemPlaceHolder">
                <LayoutTemplate>
                    <thead>
                        <tr>
                            <th class="fullcart-name" colspan="2">
                                <asp:Literal runat="server" Text="<%$ Resources:Resource, Client_OrderConfirmation_Product %>" />
                            </th>
                            <th class="fullcart-price">
                                <asp:Literal ID="Literal1" runat="server" Text="<%$ Resources:Resource, Client_OrderConfirmation_PriceForUnit %>" />
                            </th>
                            <th class="fullcart-count">
                                <asp:Literal ID="Literal2" runat="server" Text="<%$ Resources:Resource, Client_OrderConfirmation_Amount %>" />
                            </th>
                            <th class="fullcart-cost">
                                <asp:Literal ID="Literal3" runat="server" Text="<%$ Resources:Resource, Client_OrderConfirmation_Cost %>" />
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr id="itemPlaceHolder" runat="server">
                        </tr>
                    </tbody>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="fullcart-photo-data">
                            <a href="<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("Offer.Product.UrlPath")), SQLDataHelper.GetInt( Eval("Offer.ProductId"))) %>">
                                <%# RenderPhoto(SQLDataHelper.GetString(Eval("Offer.Photo.PhotoName")), SQLDataHelper.GetString(Eval("Offer.Product.Name")))%>
                            </a>
                        </td>
                        <td class="fullcart-name-data">
                            <div>
                                <a href="<%# UrlService.GetLink(ParamType.Product, SQLDataHelper.GetString(Eval("Offer.Product.UrlPath")), SQLDataHelper.GetInt( Eval("Offer.ProductId"))) %>"
                                    class="link-pv-name">
                                    <%# SQLDataHelper.GetString(Eval("Offer.Product.Name")) %></a></div>
                            <div class="sku">
                                <%#SQLDataHelper.GetString(Eval("Offer.ArtNo")) %></div>
                            <%# Eval("Offer.Color") != null ? SettingsCatalog.ColorsHeader + ": " + SQLDataHelper.GetString(Eval("Offer.Color.ColorName")) + "<br />" : "" %>
                            <%# Eval("Offer.Size") != null ? SettingsCatalog.SizesHeader + ": " + SQLDataHelper.GetString(Eval("Offer.Size.SizeName")) + "<br />" : ""%>
                            <%# RenderSelectedOptions(SQLDataHelper.GetString(Eval("AttributesXml")))%>
                        </td>
                        <td class="fullcart-price-data">
                            <span class="price">
                                <%# CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Price")), CurrencyService.CurrentCurrency.Value,
                                                          CurrencyService.CurrentCurrency.Iso3)%></span>
                        </td>
                        <td class="fullcart-count-data">
                            <%# SQLDataHelper.GetFloat(Eval("Amount")) %>
                        </td>
                        <td class="fullcart-cost-data">
                            <span class="price">
                                <%# CatalogService.GetStringPrice(SQLDataHelper.GetFloat(Eval("Price")), 0F, SQLDataHelper.GetFloat(Eval("Amount")))%></span>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
            <tfoot>
                <tr>
                    <td class="fullcart-summary" colspan="5">
                        <asp:Panel ID="pnlSummary" runat="server">
                            <span class="fullcart-summary-text">
                                <%= Resource.Client_OrderConfirmation_OrderCost %>:</span>
                            <asp:Label ID="lblTotalOrderPrice" runat="server" CssClass="price"></asp:Label><br />
                            <div id="certificateRow" runat="server" visible="false">
                                <span class="fullcart-summary-text">
                                    <% =Resource.Client_OrderConfirmation_Certificate%>:</span>
                                <asp:Label ID="lblCertificatePrice" runat="server" CssClass="price"></asp:Label>
                            </div>
                            <div id="couponRow" runat="server" visible="false">
                                <span class="fullcart-summary-text">
                                    <% =Resource.Client_OrderConfirmation_Coupon%>:</span>
                                <asp:Label ID="lblCouponPrice" runat="server" CssClass="price"></asp:Label>
                            </div>
                            <div id="discountRow" runat="server" visible="false">
                                <span class="fullcart-summary-text">
                                    <%= Resource.Client_OrderConfirmation_Discount %>:</span>
                                <asp:Label ID="lblOrderDiscount" runat="server"></asp:Label>
                            </div>
                            <div>
                                <span class="fullcart-summary-text">
                                    <%= Resource.Client_OrderConfirmation_DeliveryCost%>:</span>
                                <asp:Label ID="lblShippingPrice" runat="server" CssClass="price"></asp:Label>
                            </div>
                            <div id="paymentExtraChargeRow" runat="server">
                                <span class="fullcart-summary-text">
                                    <asp:Literal runat="server" ID="lPaymentCost"></asp:Literal></span>
                                <asp:Label ID="lblPaymentExtraCharge" runat="server" CssClass="price"></asp:Label>
                            </div>

                            <asp:Literal ID="literalTaxCost" runat="server"></asp:Literal>
                        </asp:Panel>
                    </td>
                </tr>
                <tr class="footer" id="pnlInfoForSberBank" runat="server" visible="False">
                    <td colspan="5">
                        <div class="fullcart-comment-title">
                            <label for="txtINN2">
                                <%=Resource.Client_OrderConfirmation_INN%></label>
                        </div>
                        <div class="param-name">
                            <adv:AdvTextBox CssClassWrap="input-company" ValidationType="None" ID="txtINN2" runat="server" />
                        </div>
                    </td>
                </tr>
                <tr class="footer" id="pnlInfoForBill2CompanyName" runat="server" visible="False">
                    <td colspan="5">
                        <div class="fullcart-comment-title">
                            <label for="txtCompanyName">
                                <%=Resource.Client_OrderConfirmation_OrganizationName%></label>
                        </div>
                        <div class="param-name">
                            <adv:AdvTextBox CssClassWrap="input-company" ValidationType="None" ID="txtCompanyName"
                                runat="server" />
                        </div>
                    </td>
                </tr>
                <tr class="footer" id="pnlInfoForBill2Inn" runat="server" visible="False">
                    <td colspan="5">
                        <div class="fullcart-comment-title">
                            <label for="txtINN">
                                <%=Resource.Client_OrderConfirmation_INN%></label>
                        </div>
                        <div class="param-name">
                            <adv:AdvTextBox CssClassWrap="input-company" ValidationType="None" ID="txtINN" runat="server" />
                        </div>
                    </td>
                </tr>
                <tr class="footer" id="pnlPhoneForQiwi" runat="server" visible="False">
                    <td colspan="5">
                        <div class="fullcart-comment-title">
                            <label for="txtINN">
                                <%=Resource.Client_OrderConfirmation_Phone%>:</label>
                        </div>
                        <div class="param-name" style="position: relative">
                            +<adv:AdvTextBox CssClassWrap="input-company" ValidationType="Phone" ID="txtPhone" runat="server" /><br/>
                            <br /><%=Resource.Client_OrderConfirmation_PhoneComment%>
                        </div>
                    </td>
                </tr>
                <tr class="footer">
                    <td colspan="4">
                        <div class="fullcart-comment-title">
                            <%=Resource.Client_OrderConfirmation_Comment%></div>
                        <div class="param-name">
                            <adv:AdvTextBox ID="txtComments" runat="server" CssClassWrap="order-comment" TextMode="MultiLine"
                                DefaultButtonID="btnRegUserConfirm"></adv:AdvTextBox>
                        </div>
                    </td>
                    <td class="fullcart-summary fullcart-summary-alg">
                        <div class="fullcart-summary-bg pie">
                            <span class="fullcart-summary-text">
                                <%=Resource.Client_OrderConfirmation_Total%></span>
                            <asp:Label ID="lblTotalPrice" runat="server" CssClass="price"></asp:Label>
                        </div>
                    </td>
                </tr>
                <tr class="footer" id="trCaptcha" runat="server" visible="true">
                    <td colspan="5">
                        <div class="fullcart-code-title">
                            <%=Resource.Client_Registration_ConfCode%></div>
                        <div class="param-name">
                            <adv:captchacontrol ID="dnfValid" runat="server" DefaultButtonID="btnRegUserConfirm" />
                        </div>
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>
</div>
<div class="oc-panel-wr">
    <div class="oc-panel pie">
        <div class="oc-back">
            <adv:Button ID="btnBackToPaymentType" runat="server" Size="Big" Type="Confirm" CssClass="btn-back"
                Text="<%$ Resources:Resource, Client_OrderConfirmation_Back %>" OnClick="btnBackToPaymentType_Click"
                DisableValidation="True" />
        </div>
        <div class="oc-continue">
            <adv:Button ID="btnRegUserConfirm" runat="server" Size="Big" Type="Confirm" CssClass="btn-continue"
                Text="<%$ Resources:Resource, Client_OrderConfirmation_DrawUp %>" OnClick="btnRegUserConfirm_Click" />
        </div>
        <br class="clear" />
    </div>
</div>
