<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CheckOrder.ascx.cs"
    Inherits="Templates.Sketchy.UserControls.Default.CheckOrder" %>
<!--noindex-->
<article class="block-uc" data-plugin="expander">
    <h3 class="title checkorder"><%=Resources.Resource.Client_UserControls_StatusComment_Head%></h3>
    <div class="content" id="check-status">
        <div class="status-input">
            <div class="check-wrapper">
                <div class="check-txt"><%= Resources.Resource.Client_UserControls_StatusComment_Number%>:</div>
                <adv:AdvTextBox ID="txtNumber" runat="server" MaxLength="20" DefaultButtonID="btncheckorder" />
            </div>
            
            <div id="orderStatus" class="checkorder"></div>
        </div>
        <div class="btn-status-check">
            <adv:Button runat="server" ID="btncheckorder" Size="Middle" Type="Confirm" Text='<%$ Resources:Resource, Client_UserControls_StatusComment_Check%>' />
        </div>
    </div>
</article>
<!--/noindex-->
