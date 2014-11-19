//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using Advantshop_Tools;

namespace Tools.core
{
    public partial class UpdaterFromFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!(ckbUpdate.Checked && ckbUpdate1.Checked))
            {
                return;
            }

            //1. �������� �� ����������� ����������
            //var compareCodeInf = UpdaterService.CompareCodeVersions();
            //if (!string.IsNullOrEmpty(compareCodeInf))
            //{
            //    // ������� �������
            //    return;
            //}
            //var compareBaseInf = UpdaterService.CompareBaseVersions();
            //if (!string.IsNullOrEmpty(compareBaseInf))
            //{
            //    // ������� �������
            //    return;
            //}
        
            //2. �������� �������
            UpdaterService.CreateBaseBackup();

            UpdaterService.CreateCodeBackup();
        
            //3. �������� ��������� ������ ����� � ���������
            UpdaterService.UpdateAvantshop();
        }
    }
}