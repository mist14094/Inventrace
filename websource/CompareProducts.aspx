<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CompareProducts.aspx.cs"
    Inherits="ClientPages.CompareProducts" ValidateRequest="false" EnableViewState="false" %>

<%@ Import Namespace="AdvantShop.Configuration" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Design" %>
<%@ Import Namespace="AdvantShop.FilePath" %>
<%@ Import Namespace="Resources" %>

<%@ Register TagPrefix="adv" TagName="StaticBlock" Src="~/UserControls/StaticBlock.ascx" %>
<%@ Register Src="~/UserControls/LogoImage.ascx" TagName="Logo" TagPrefix="adv" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>
        <%=Resources.Resource.Client_CompareProducts_Header %></title>
    <link rel="stylesheet" type="text/css" href="css/compare.css" />
    <link rel="stylesheet" type="text/css" href="css/styles.css" />
    <link rel="stylesheet" type="text/css" href="css/styles-extra.css" />
    <link rel="stylesheet" type="text/css" href="<%= "design/colors/" + DesignService.GetDesign("colorscheme") + "/css/styles.css" %>"
        id="colorcss" />
    <script type="text/javascript" src="js/jq/jquery-1.7.1.min.js"></script>
    <!--[if lt IE 10]>
        <script type="text/javascript" src="js/fix/PIE.js"></script>
    <![endif]-->
    <script type="text/javascript" src="js/fix/PIEInit.js"></script>
    <script type="text/javascript">

        $(function () {
            PIELoad(".btn");
        });

        function buyProduct(id) {
            $('#<%=hiddenOfferID.ClientID%>').attr("value", id);
            document.getElementById('<%=btnBuyProduct.ClientID%>').click();
        }

        function deleteProduct(_offerId) {
            $(".compareLoader_" + _offerId).show();
            jQuery.ajax({
                url: "httphandlers/compareproducts/deleteproduct.ashx",
                dataType: "json",
                cache: false,
                data: { offerId: _offerId },
                success: function (successResult) {
                    if (successResult) {
                        $(".tdProduct_" + _offerId).remove();
                        var colspan = $(".cc").attr('colspan');
                        $(".cc").attr('colspan', colspan - 1);
                        deleteProperties();

                        if (colspan == 1) {
                            closeWindow();
                        }
                    }
                }
            });
        }

        function deleteProperties() {
            $(".nCell").each(function () {
                var clearProperties = true;
                $(this).find("td.compare_col").each(function () {
                    if ($.trim($(this).text()) != "-") {
                        clearProperties = false;
                        return;
                    }
                });

                if (clearProperties) {
                    $(this).remove();
                }
            });
        }

        function closeWindow() {
            if (window.opener) {
                window.opener.location.reload(false);
            }
            self.close();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div style="display: none">
                <input id="hiddenOfferID" runat="server" value="" type="hidden" />
                <asp:Button ID="btnBuyProduct" runat="server" OnClick="btnBuyProduct_Click" />
                <asp:Button ID="btnDeleteProduct" runat="server" OnClick="btnDeleteProduct_Click" />
            </div>
            <table class="comp" style="width: auto">
                <tr>
                    <td class="nc">
                        <span class="n">
                            <%=Resources.Resource.Client_CompareProducts_Header %></span>
                        <img src="images/compare/print.jpg" style="vertical-align: middle; cursor: pointer;"
                            onclick="window.print(); return false;" alt="" />
                    </td>
                    <td colspan="<%=ProductItems.Count%>" class="cc">
                        <img src="images/compare/close.jpg" onclick="closeWindow(); return false;" alt=""
                            style="cursor: pointer;" />
                    </td>
                </tr>
                <tr class="cCell">
                    <td rowspan="3" class="inf">
                        <adv:Logo ID="Logo" ImgHref='/' runat="server" />
                        <br />
                        <br />
                        <adv:StaticBlock ID="StaticBlockTop" runat="server" SourceKey="CompareProductsTop" />
                    </td>
                    <%foreach (var item in ProductItems)
                      {%>
                    <td class="nb tdProduct_<%=item.OfferId %>">
                        <div style="position: relative; padding: 0 10px;">
                            <a href="<%= UrlService.GetLink(ParamType.Product, item.UrlPath, item.ProductId) %>"><span class="pName">
                                <%=item.Name %></span></a>
                            <div style="margin-top: 5px; color: #a5a8af"><%= item.ArtNo %></div>
                            <img style="vertical-align: inherit; cursor: pointer; position: absolute; right: -10px; top: 0;" src="images/compare/remove.jpg" alt="<%=Resources.Resource.Client_CompareProducts_Delete %>" onclick="deleteProduct('<%=item.OfferId  %>'); return false;" />
                        </div>
                    </td>
                    <%}%>
                </tr>
                <tr class="cCell">
                    <%foreach (var item in ProductItems)
                      {%>
                    <td class="nb tdProduct_<%=item.OfferId%>">
                        <img src='<%=FoldersHelper.GetImageProductPath(ProductImageType.Small, item.Photo, false) %>'
                            alt="" />
                    </td>
                    <%}%>
                </tr>
                <tr class="cCell">
                    <%foreach (var item in ProductItems)
                      {%>
                    <td class="tdProduct_<%=item.OfferId%>">
                        <%= item.Price > 0 && item.Amount > 0 ? AdvantShop.Controls.Button.RenderHtml(SettingsCatalog.BuyButtonText, AdvantShop.Controls.Button.eType.Buy, AdvantShop.Controls.Button.eSize.Middle, onClientClick: "buyProduct('" + item.OfferId + "')") : string.Empty%>
                        <%= (item.Amount <= 0) && item.AllowPreorder ? AdvantShop.Controls.Button.RenderHtml(SettingsCatalog.PreOrderButtonText, AdvantShop.Controls.Button.eType.Action, AdvantShop.Controls.Button.eSize.Middle, href: "sendrequestonproduct.aspx?offerid=" + item.OfferId ) :string.Empty%>
                    </td>
                    <%}%>
                </tr>
                <tr class="nCell">
                    <td class="propN">
                        <%=Resources.Resource.Client_CompareProducts_Price %>
                    </td>
                    <%foreach (var item in ProductItems)
                      {%>
                    <td class="compare_col price tdProduct_<%=item.OfferId%>">
                        <%= RenderPriceTag(item.Price, item.Discount) %>
                    </td>
                    <%}%>
                </tr>
                <%foreach (var propertyName in PropertyNames)
                  {%>
                <tr class="nCell">
                    <td class="propN">
                        <%=propertyName%>
                    </td>
                    <%foreach (var item in ProductItems)
                      {%>
                    <td class="compare_col tdProduct_<%=item.OfferId%>">
                        <% int i = 0;
                           foreach (var itemval in item.Properties.Where(p => p.Name == propertyName))
                           { %>
                        <%= i != 0 ? ", " : string.Empty %><%= itemval.Value %><% i++;
                           } %>
                    </td>
                    <%}%>
                </tr>
                <%}%>
            </table>
            <asp:ListView runat="server"></asp:ListView>
        </div>
    </form>
</body>
</html>
