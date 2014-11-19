<%@ Page Language="C#" MasterPageFile="MasterPage.master" CodeFile="Feedback.aspx.cs"
    Inherits="ClientPages.Feedback" %>

<%@ Register Src="UserControls/Captcha.ascx" TagName="CaptchaControl" TagPrefix="adv" %>
<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="Server">
    <div class="stroke">
        <div class="content-owner">
            <h1>
                <%=Resources.Resource.Client_Feedback_Header%></h1>
            <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
                <asp:View ID="ViewDataCollecting" runat="server">
                    <div class="form-c">
                        <ul class="form form-vr">
                            <li>
                                <div class="param-name">
                                    <label for="txtSenderName">
                                        <%=Resources.Resource.Client_Feedback_Name%></label></div>
                                <div class="param-value">
                                    <adv:AdvTextBox ValidationType="Required" ID="txtSenderName" runat="server" />
                                </div>
                            </li>
                            <li>
                                <div class="param-name">
                                    <label for="txtEmail">
                                        Email:</label></div>
                                <div class="param-value">
                                    <adv:AdvTextBox ValidationType="Email" ID="txtEmail" runat="server" />
                                </div>
                            </li>
                            <li>
                                <div class="param-name">
                                    <label for="txtPhone">
                                        <%=Resources.Resource.Client_Feedback_Phone%>:</label></div>
                                <div class="param-value">
                                    <adv:AdvTextBox ID="txtPhone" runat="server" ValidationType="Required" />
                                </div>
                            </li>
                            <li>
                                <div class="param-name">
                                    <label for="txtMessage">
                                        <%=Resources.Resource.Client_Feedback_MessageText%></label></div>
                                <div class="param-value-textarea  param-value">
                                    <adv:AdvTextBox ID="txtMessage" ValidationType="Required" TextMode="MultiLine" runat="server"></adv:AdvTextBox>
                                </div>
                            </li>
                            <li runat="server" id="liCaptcha">
                                <div class="param-name">
                                    <label>
                                        <%=Resources.Resource.Client_Feedback_Code%>:
                                    </label>
                                </div>
                                <div class="param-value">
                                    <adv:CaptchaControl ID="validShield" runat="server" />
                                </div>
                            </li>
                            <li>
                                <div class="param-name">
                                </div>
                                <div class="param-value">
                                    <adv:Button ID="btnSend" Type="Submit" Size="Middle" runat="server" Text="<%$ Resources:Resource, Client_Feedback_Send %>"
                                        OnClick="btnSend_Click"></adv:Button>
                                        <br/>
                                        <br/>
                                </div>
                            </li>
                        </ul>
                    </div>
                </asp:View>
                <asp:View ID="ViewEmailSend" runat="server">
                    <div class="form-c">
                        <%=Resources.Resource.Client_Feedback_MessageSent%>
                        <div class="form-padding">
                            <adv:Button ID="Button1" runat="server" Type="Action" Size="Middle" Text='<%$ Resources:Resource, Client_FogotPassword_ToMainPage %>'
                                Href="." />
                                <br/>
                                <br/>
                                <br/>
                        </div>
                    </div>
                </asp:View>
            </asp:MultiView>
            <div class="form-addon">
                <div class="form-addon-text">
                    <div class="title">
                        <adv:StaticBlock ID="staticBlock" runat="server" SourceKey="feedbackAddons" />
                    </div>
                    <p class="fb">
                        <%= AdvantShop.Configuration.SettingsMain.Phone %></p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
