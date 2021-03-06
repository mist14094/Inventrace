<%@ Control Language="C#" AutoEventWireup="true" CodeFile="News.ascx.cs" Inherits="Templates.Sketchy.UserControls.Default.News"
    EnableViewState="false" %>
<%@ Import Namespace="AdvantShop.Core.UrlRewriter" %>
<%@ Import Namespace="AdvantShop.Helpers" %>
<!--noindex-->
<article class="block-uc" data-plugin="expander">
    <h3 class="title news" ><%= Resources.Resource.Client_UserControls_News_News %></h3>
    <div class="content" id="expander-news">
        <asp:ListView runat="server" ID="lvNews">
            <ItemTemplate>
                <div class="news-item">
                    <div class="news-date">
                        <%# AdvantShop.Localization.Culture.ConvertShortDate((DateTime)Eval("AddingDate"))%>
                    </div>
                    <a href="<%# UrlService.GetLink(ParamType.News, Eval("UrlPath").ToString() ,SQLDataHelper.GetInt(Eval("NewsID"))) %>"
                        class="link-news-anno">
                        <%#Eval("Title")%></a>
                    
                    
                </div>
                
            </ItemTemplate>
        </asp:ListView>
        
            
        
    </div>
    <a class="all-news-show" href="news">
            <%= Resources.Resource.Client_UserControls_News_AllNews %></a>
        
</article>
<!--/noindex-->
