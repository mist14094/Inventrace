//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Controls;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using Resources;

namespace Admin
{
    public partial class SendMessage : AdvantShopAdminPage
    {
        private void MsgErr(bool clean)
        {
            if (clean)
            {
                lblError.Visible = false;
                lblError.Text = string.Empty;
            }
            else
            {
                lblError.Visible = false;
            }
        }

        private void MsgErr(string messageText)
        {
            lblError.Visible = true;
            lblError.Text = @"<br/>" + messageText + @"<br/>";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SetMeta(string.Format("{0} - {1}", SettingsMain.ShopName, Resource.Admin_SendMessage_Title));
            fckMailContent.Language = CultureInfo.CurrentCulture.ToString();
            MsgErr(true);
            lblInfo.Text = string.Empty;

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            var modules = AttachedModules.GetModules(AttachedModules.EModuleType.SendMails).Where(item => ((ISendMails)Activator.CreateInstance(item, null)).IsActive);
            if (!modules.Any())
            {
                mvSendingMessages.SetActiveView(vErrorForm);
            }
            else if (!IsPostBack)
            {
                mvSendingMessages.SetActiveView(vSendForm);
            }

        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (!IsValidData())
            {
                return;
            }

            var modules = AttachedModules.GetModules(AttachedModules.EModuleType.SendMails).Where(item => ((ISendMails)Activator.CreateInstance(item, null)).IsActive).ToArray();
            if (rbToAll.Checked)
            {
                foreach (var moduleType in modules)
                {
                    var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
                    moduleObject.SendMailsToAll(txtTitle.Text, fckMailContent.Text);
                }
            }
            if (rbToReg.Checked)
            {
                foreach (var moduleType in modules)
                {
                    var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
                    moduleObject.SendMailsToReg(txtTitle.Text, fckMailContent.Text);
                }
            }
            if (rbToUnReg.Checked)
            {
                foreach (var moduleType in modules)
                {
                    var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
                    moduleObject.SendMailsToNotReg(txtTitle.Text, fckMailContent.Text);
                }
            }
            mvSendingMessages.SetActiveView(vFinishForm);
        }

        private bool IsValidData()
        {
            if ((txtTitle.Text.IndexOf(">") != -1) || (txtTitle.Text.IndexOf("<") != -1))
            {
                MsgErr(Resource.Admin_SendMessage_HtmlNotSupported);
                return false;
            }

            if (string.IsNullOrEmpty(txtTitle.Text))
            {
                MsgErr(Resource.Admin_SendMessage_NoTitle);
                return false;
            }

            if (string.IsNullOrEmpty(fckMailContent.Text))
            {
                MsgErr(Resource.Admin_SendMessage_NoEmailText);
                return false;
            }

            return true;
        }
    }
}