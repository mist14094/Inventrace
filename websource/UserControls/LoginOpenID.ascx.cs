using System;
using System.Web.UI;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Security.OpenAuth;

public partial class UserControls_LoginOpenID : UserControl
{
    public string PageToRedirect = string.Empty;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (SettingsOAuth.GoogleActive || SettingsOAuth.YandexActive || SettingsOAuth.TwitterActive ||
            SettingsOAuth.VkontakteActive || SettingsOAuth.FacebookActive || SettingsOAuth.MailActive || SettingsOAuth.OdnoklassnikiActive)
        {
            var rootUrlPath = /*Request.Url.AbsoluteUri.Contains("localhost") ? "~/" : */StringHelper.MakeASCIIUrl(SettingsMain.SiteUrl);
            var strRedirectUrl = PageToRedirect.IsNotEmpty() ? rootUrlPath + "/" + PageToRedirect : rootUrlPath;

            if (!string.IsNullOrEmpty(Request["code"]) && Request["auth"] == "vk")
            {
                VkontakteOauth.VkontakteAuth(Request["code"], string.Empty);
                Response.Redirect(strRedirectUrl, false);
            }
            if (!string.IsNullOrEmpty(Request["code"]) && Request["auth"] == "od")
            {
                OdnoklassnikiOauth.OdnoklassnikiLogin(Request["code"]);
                Response.Redirect(strRedirectUrl, false);
            }
            //else if (!string.IsNullOrEmpty(Request["oauth_token"]))
            //{
            //    TwitterOAuth.TwitterGetUser(Request["oauth_token"], string.Empty);
            //    Response.Redirect(strRedirectUrl, false);
            //}
            else if (SettingsOAuth.GoogleActive && !string.IsNullOrEmpty(Request["code"]) && string.Equals(Request["auth"], "google"))
            {
                GoogleOauth.ExchangeCodeForAccessToken(Request["code"]);
                Response.Redirect(strRedirectUrl, false);
            }
            else if (!string.IsNullOrEmpty(Request["code"]))
            {
                FacebookOauth.SendFacebookRequest(Request["code"], SettingsMain.SiteUrl + "/" + PageToRedirect);
                Response.Redirect(strRedirectUrl, false);
            }
            else if (OAuthResponce.OAuthUser(Request))
            {
                Response.Redirect(strRedirectUrl, false);
            }
        }
        else
        {
            divSocial.Visible = false;
        }
    }

    protected void lnkbtnVkClick(object sender, EventArgs e)
    {
        VkontakteOauth.VkontakteAuthDialog();
    }

    protected void lnkbtnFacebookClick(object sender, EventArgs e)
    {
        FacebookOauth.ShowAuthDialog(SettingsMain.SiteUrl + "/" + PageToRedirect);
    }

    protected void lnkbtnMailClick(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtOauthUserId.Text))
            return;
        var userId = txtOauthUserId.Text;
            
            var userIdAndDomainPair = txtOauthUserId.Text.Split(new[] { '@' });
            if (userIdAndDomainPair.Length != 2)
            {
                return;
            }

        var oAuthRequest = new OAuthRequest { UserId = userId, Provider = OAuthRequest.Providers.Mail };
        oAuthRequest.CreateRequest(new ClaimParameters(), true);
    }

    //protected void lnkbtnTwitterClick(object sender, EventArgs e)
    //{
    //    TwitterOAuth.TwitterOpenAuth();
    //}

    protected void lnkbtnGoogleClick(object sender, EventArgs e)
    {
            GoogleOauth.SendAuthenticationRequest();
    }

    protected void lnkbtnYandexClick(object sender, EventArgs e)
    {
        var oAuthRequest = new OAuthRequest { Provider = OAuthRequest.Providers.Yandex };
        oAuthRequest.CreateRequest(new ClaimParameters(), false);
    }

    protected void lnkbtnOdnoklassnikiClick(object sender, EventArgs e)
    {
        OdnoklassnikiOauth.OdnoklassnikiAuthDialog();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        lnkbtnGoogle.Visible = SettingsOAuth.GoogleActive;
        lnkbtnYandex.Visible = SettingsOAuth.YandexActive;
        //lnkbtnTwitter.Visible = SettingsOAuth.TwitterActive;
        lnkbtnVk.Visible = SettingsOAuth.VkontakteActive;
        lnkbtnFacebook.Visible = SettingsOAuth.FacebookActive;
        lnkbtnMail.Visible = pnlMail.Visible = SettingsOAuth.MailActive;
        lnkbtnOdnoklassniki.Visible = SettingsOAuth.OdnoklassnikiActive;
    }
}