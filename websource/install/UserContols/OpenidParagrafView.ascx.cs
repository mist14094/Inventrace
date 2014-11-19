using System;
using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Helpers;

public partial class install_UserContols_OpenidParagrafView : AdvantShop.Controls.InstallerStep
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var providers = AdvantShop.Core.AdvantshopConfigService.GetActivityAuthProviders();
        fieldsetFacebook.Visible = !providers.ContainsKey("facebook") || providers["facebook"];
        fieldsetTwitter.Visible = !providers.ContainsKey("twitter") || providers["twitter"];
        fieldsetVk.Visible = !providers.ContainsKey("vkontakte") || providers["vkontakte"];
        fieldsetMailru.Visible = !providers.ContainsKey("mail.ru") || providers["mail.ru"];
        fieldsetGoogle.Visible = !providers.ContainsKey("google") || providers["google"];
        fieldsetYandex.Visible = !providers.ContainsKey("yandex") || providers["yandex"];
    }

    public new void LoadData()
    {
        String pass = Session["adminPass"] != null ? Session["adminPass"].ToString() : "";

        if (!string.IsNullOrEmpty(pass))
        {
            txtPass.Attributes.Add("value", pass);
            txtPassAgain.Attributes.Add("value", pass);
        }

        chbGoogle.Checked = fieldsetGoogle.Visible && SettingsOAuth.GoogleActive;
        chbMailru.Checked = fieldsetMailru.Visible && SettingsOAuth.MailActive;
        chbYandex.Checked = fieldsetYandex.Visible && SettingsOAuth.YandexActive;
        chbVk.Checked = fieldsetVk.Visible && SettingsOAuth.VkontakteActive;
        chbFacebook.Checked = fieldsetFacebook.Visible && SettingsOAuth.FacebookActive;
        chbTwitter.Checked = fieldsetTwitter.Visible && SettingsOAuth.TwitterActive;

        if (fieldsetVk.Visible)
        {
            txtVKAppId.Text = SettingsOAuth.VkontakeClientId;
            txtVKSecret.Text = SettingsOAuth.VkontakeSecret;
        }

        if (fieldsetFacebook.Visible)
        {
            txtFacebookClientId.Text = SettingsOAuth.FacebookClientId;
            txtFacebookApplicationSecret.Text = SettingsOAuth.FacebookApplicationSecret;
        }

        if (fieldsetTwitter.Visible)
        {
            txtTwitterConsumerKey.Text = SettingsOAuth.TwitterConsumerKey;
            txtTwitterConsumerSecret.Text = SettingsOAuth.TwitterConsumerSecret;
            txtTwitterAccessToken.Text = SettingsOAuth.TwitterAccessToken;
            txtTwitterAccessTokenSecret.Text = SettingsOAuth.TwitterAccessTokenSecret;
        }
    }

    public new bool Validate()
    {
        if (string.IsNullOrEmpty(txtPass.Text) && string.IsNullOrEmpty(txtPassAgain.Text))
        {
            lblError.Text = Resources.Resource.Install_UserContols_OpenidParagrafView_NeedPass;
            return false;
        }
        if (txtPass.Text != txtPassAgain.Text)
        {
            lblError.Text = Resources.Resource.Install_UserContols_OpenidParagrafView_WrongPass;
            return false;
        }

        var validList = new List<ValidElement>();
        if (chbFacebook.Checked)
        {
            validList.Add(new ValidElement { Control = txtFacebookApplicationSecret, ErrContent = ErrContent, ValidType = ValidType.Required, Message = "Поле \"Application Secret\" обязательно для заполнения" });
            validList.Add(new ValidElement { Control = txtFacebookClientId, ErrContent = ErrContent, ValidType = ValidType.Required, Message = "Поле \"Client Id\" обязательно для заполнения" });
        }
        if (chbVk.Checked)
        {
            validList.Add(new ValidElement { Control = txtVKAppId, ErrContent = ErrContent, ValidType = ValidType.Required, Message = "Поле \"App Id\" обязательно для заполнения" });
            validList.Add(new ValidElement { Control = txtVKSecret, ErrContent = ErrContent, ValidType = ValidType.Required, Message = "Поле \"Secret\" обязательно для заполнения" });
        }
        if (chbTwitter.Checked)
        {
            validList.Add(new ValidElement { Control = txtTwitterAccessToken, ErrContent = ErrContent, ValidType = ValidType.Required, Message = "Поле \"Access Token\" обязательно для заполнения" });
            validList.Add(new ValidElement { Control = txtTwitterAccessTokenSecret, ErrContent = ErrContent, ValidType = ValidType.Required, Message = "Поле \"Access Token Secret\" обязательно для заполнения" });
            validList.Add(new ValidElement { Control = txtTwitterConsumerKey, ErrContent = ErrContent, ValidType = ValidType.Required, Message = "Поле \"Consumer Key\" обязательно для заполнения" });
            validList.Add(new ValidElement { Control = txtTwitterConsumerSecret, ErrContent = ErrContent, ValidType = ValidType.Required, Message = "Поле \"Consumer Secret\" обязательно для заполнения" });
        }
        return ValidationHelper.Validate(validList);
    }

    public new void SaveData()
    {
        Session["adminPass"] = txtPass.Text.Trim();
        SettingsOAuth.GoogleActive = chbGoogle.Checked;
        SettingsOAuth.MailActive = chbMailru.Checked;
        SettingsOAuth.YandexActive = chbYandex.Checked;
        SettingsOAuth.VkontakteActive = chbVk.Checked;
        SettingsOAuth.FacebookActive = chbFacebook.Checked;
        SettingsOAuth.TwitterActive = chbTwitter.Checked;

        SettingsOAuth.VkontakeClientId = txtVKAppId.Text;
        SettingsOAuth.VkontakeSecret = txtVKSecret.Text;

        SettingsOAuth.FacebookClientId = txtFacebookClientId.Text;
        SettingsOAuth.FacebookApplicationSecret = txtFacebookApplicationSecret.Text;

        SettingsOAuth.TwitterConsumerKey = txtTwitterConsumerKey.Text;
        SettingsOAuth.TwitterConsumerSecret = txtTwitterConsumerSecret.Text;
        SettingsOAuth.TwitterAccessToken = txtTwitterAccessToken.Text;
        SettingsOAuth.TwitterAccessTokenSecret = txtTwitterAccessTokenSecret.Text;

        if (!string.IsNullOrEmpty(txtPass.Text) && !string.IsNullOrEmpty(txtPassAgain.Text))
        {
            if (txtPass.Text == txtPassAgain.Text)
            {
                CustomerService.ChangePassword("835d82df-aaa6-4d54-a870-8d353e541af9", txtPass.Text, false);
            }
        }
    }
}