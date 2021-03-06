﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="StepSuccess.ascx.cs" Inherits="UserControls_OrderConfirmation_FifthStep" %>
<%@ Import Namespace="AdvantShop.Orders" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>
<script type="text/javascript">
    $(function () {
        getPaymentButton('<%=OrderID%>', 'btnPaymentFunctionality');
    });
    
</script>
<div class="fifthStep">
    <br />
    <br />
    <div class="title">
        <%=Resources.Resource.Client_OrderConfirmation_OrderSuccessfullyConfirmed%></div>
    <br />
    <br />
    <asp:Label runat="server" Text="<%$ Resources:Resource, Client_OrderConfirmation_ManagerhvGotOrder %>"></asp:Label>
    <br />
    <br />
    <asp:Label runat="server" Text="<%$ Resources:Resource, Client_OrderConfirmation_MailSent %>"></asp:Label>
    <br />
    <br />
    <br />
    <adv:Button Type="Confirm" Size="Middle" runat="server" ID="lnkToDefault" Text="<%$ Resources:Resource, Client_OrderConfirmation_GotoMain %>" />
    &nbsp; <span id="btnPaymentFunctionality"></span>&nbsp;
    <adv:Button Type="Submit" Size="Middle" ID="btnPrintOrder" Text="<%$ Resources:Resource,Client_OrderConfirmation_PrintOrder %>"
        runat="server" />
    <br />
    <br />
    <adv:StaticBlock runat="server" SourceKey="ordersuccess" />
    <br />
    <%= AdvantShop.Modules.ModulesRenderer.RenderIntoOrderConfirmationFinalStep(OrderService.GetOrder(OrderID), OrderService.GetOrderItems(OrderID).Select(item=>(IOrderItem)item).ToList())%>
</div>
