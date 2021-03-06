using System;
using System.IO;
using System.Web;

public partial class install_UserContols_TrialSelectView : System.Web.UI.UserControl
{
    public bool HasWriteAccess = true;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!HasWriteAccess)
        {
            divExpress.Visible = false;
            divNoWriteAccess.Visible = true;
            lblHead.Text = Resources.Resource.Install_UserContols_TrialSelectView_h1_NoWriteAccess;
        }
        else
        {
            lblHead.Text = Resources.Resource.Install_UserContols_TrialSelectView_h1;
        }
    }

    public void LoadData()
    {
    }

    public void SaveData()
    {
    }

    public bool IsExpressInstall()
    {
        return rbExpressInstall.Checked;
    }
}