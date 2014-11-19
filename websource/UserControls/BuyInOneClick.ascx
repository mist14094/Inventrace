<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BuyInOneClick.ascx.cs"
    Inherits="UserControls_BuyInOneClick" EnableViewState="false" %>
<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="Resources" %>
<%@ Import Namespace="AdvantShop.Orders" %>
<div id="divBuyInOneClick" class="<%= pageEnum == OrderConfirmationService.BuyInOneclickPage.details ? "buy-one-click-wrap" : "buy-one-click-wrap-alt" %>">
    <a id="lBtnBuyInOneClick" class="<%= pageEnum == OrderConfirmationService.BuyInOneclickPage.details ? "btn-buy-one-click" : "btn btn-add btn-big" %>"
        href="javascript:void(0)" data-page="<%= pageEnum.ToString() %>">
        <%= Resource.Client_BuyInOneclick_Button %></a></div>
<div style="display: none;">
    <div id="modalBuyInOneClick" class="modal-buy-in-one">
        <asp:HiddenField ID="hfProductId" runat="server" />
        <ul class="form form-vr" id="modalBuyInOneClickForm">
            <li>
                <p class="headtext">
                    <%= SettingsOrderConfirmation.BuyInOneClick_FirstText%></p>
            </li>
            <li>
                <div class="param-name one-click-label">
                    <label for="txtName">
                        <%=Resource.Client_BuyInOneClick_Name%></label></div>
                <div class="param-value">
                    <adv:AdvTextBox ID="txtName" CssClassWrap="one-click-input-wrap" ValidationType="Required"
                        runat="server" ValidationGroup="buyInOneClick" DefaultButtonID="btnAddChangeContact">
                    </adv:AdvTextBox>
                </div>
            </li>
            <li>
                <div class="param-name one-click-label">
                    <label for="txtPhone">
                        <%=Resource.Client_BuyInOneClick_Phone%></label></div>
                <div class="param-value">
                    <adv:AdvTextBox ID="txtPhone" CssClassWrap="one-click-input-wrap" ValidationType="Required"
                        runat="server" ValidationGroup="buyInOneClick" DefaultButtonID="btnAddChangeContact">
                    </adv:AdvTextBox>
                </div>
            </li>
            <li>
                <div class="param-name one-click-label">
                    <label for="txtComment">
                        <%=Resource.Client_BuyInOneClick_Comment%></label></div>
                <div class="param-value">
                    <adv:AdvTextBox ID="txtComment" CssClassWrap="one-click-textarea-wrap" TextMode="Multiline"
                        runat="server" ValidationGroup="buyInOneClick"></adv:AdvTextBox>
                </div>
            </li>
            <li>
                <span style="color:red" id="errorOneClick"></span>
            </li>
            <li class="form-footer">
                <div class="param-name one-click-label">
                </div>
                <div class="param-value">
                    <adv:Button ID="btnBuyInOneClick" Size="Big" Type="Submit" CssClass="btn-save-pass-my"
                        runat="server" Text=' <%$Resources: Resource,Client_BuyInOneClick_WaitForTheCall%>'
                        ValidationGroup="buyInOneClick" />
                </div>
            </li>
        </ul>
        <div style="display: none;" id="modalBuyInOneClickFinal">
            <div class="finaltext">
                <%= SettingsOrderConfirmation.BuyInOneClick_FinalText %></div>
            <div class="btn-final-one-click-wrap">
                <adv:Button ID="btnBuyInOneClickOk" Size="Big" Type="Submit" runat="server" Text=' Ok'
                    ValidationGroup="buyInOneClick" />
            </div>
        </div>
    </div>
</div>