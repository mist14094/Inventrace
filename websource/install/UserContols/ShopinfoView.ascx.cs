using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using AdvantShop;
using AdvantShop.Configuration;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Permission;
using AdvantShop.Repository;
using AdvantShop.SaasData;

public partial class install_UserContols_ShopinfoView : AdvantShop.Controls.InstallerStep
{
    public bool ActiveLic;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Demo.IsDemoEnabled || Trial.IsTrialEnabled)
        {
            divKey.Visible = false;
        }
    }

    protected void sds_Init(object sender, EventArgs e)
    {
        ((SqlDataSource)sender).ConnectionString = Connection.GetConnectionString();
    }

    public new void LoadData()
    {
        if (!string.IsNullOrWhiteSpace(SettingsMain.LogoImageName))
            imgLogo.ImageUrl = "../" + FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, true);
        else
            imgLogo.Visible = false;

        if (!string.IsNullOrWhiteSpace(SettingsMain.FaviconImageName))
            imgFavicon.ImageUrl = "../" + FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.FaviconImageName, true);
        else
            imgFavicon.Visible = false;

        if (!(Demo.IsDemoEnabled || Trial.IsTrialEnabled))
        {
            txtKey.Text = SettingsLic.LicKey;
        }

        txtShopName.Text = SettingsMain.ShopName;
        txtUrl.Text = SettingsMain.SiteUrl;
        //txtTitle.Text = SettingsSEO.DefaultMetaTitle;
        //txtMetadescription.Text = SettingsSEO.DefaultMetaDescription;
        //txtMetakeywords.Text = SettingsSEO.DefaultMetaKeywords;

        ddlCountry.DataBind();
        ddlCountry.SelectedValue = SettingsMain.SalerCountryId.ToString();
        txtCity.Text = SettingsMain.City;

        int value = SettingsMain.SalerRegionId;

        if (SettingsMain.SalerRegionId != 0)
        {
            var region = RegionService.GetRegion(SettingsMain.SalerRegionId);
            if (region != null)
                txtRegion.Text = region.Name;
        }

        txtPhone.Text = SettingsMain.Phone;
        txtDirector.Text = SettingsBank.Director;
        txtAccountant.Text = SettingsBank.Accountant;
        txtManager.Text = SettingsBank.Manager;
    }

    public new void SaveData()
    {
        if (!(Demo.IsDemoEnabled || Trial.IsTrialEnabled))
        {
            SettingsLic.LicKey = txtKey.Text;
        }


        SettingsMain.ShopName = txtShopName.Text;
        SettingsMain.SiteUrl = txtUrl.Text;
        SettingsMain.LogoImageAlt = txtShopName.Text;
        if (fuLogo.HasFile)
        {
            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.Pictures));
            FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.LogoImageName));
            SettingsMain.LogoImageName = fuLogo.FileName;
            fuLogo.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.Pictures, fuLogo.FileName));
        }

        if (fuFavicon.HasFile)
        {
            FileHelpers.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.Pictures));
            const string extensions = ".ico.png.gif";

            if (extensions.Contains(Path.GetExtension(fuFavicon.FileName)))
            {
                FileHelpers.DeleteFile(FoldersHelper.GetPathAbsolut(FolderType.Pictures, SettingsMain.FaviconImageName));
                SettingsMain.FaviconImageName = fuFavicon.FileName;
                fuLogo.SaveAs(FoldersHelper.GetPathAbsolut(FolderType.Pictures, fuFavicon.FileName));
            }
        }

        //SettingsSEO.DefaultMetaTitle = txtTitle.Text;
        //SettingsSEO.DefaultMetaDescription = txtMetadescription.Text;
        //SettingsSEO.DefaultMetaKeywords = txtMetakeywords.Text;
        var countryId = 0;
        int.TryParse(ddlCountry.SelectedValue, out countryId);
        SettingsMain.SalerCountryId = countryId;

        var regionId = RegionService.GetRegionIdByName(txtRegion.Text);
        SettingsMain.SalerRegionId = regionId;

        SettingsMain.City = txtCity.Text;
        SettingsMain.Phone = txtPhone.Text;
        SettingsBank.Director = txtDirector.Text;
        SettingsBank.Accountant = txtAccountant.Text;
        SettingsBank.Manager = txtManager.Text;
    }

    public new bool Validate()
    {
        if (!(Demo.IsDemoEnabled || Trial.IsTrialEnabled || SaasDataService.IsSaasEnabled))
        {
            try
            {
                ActiveLic = PermissionAccsess.ActiveLic(txtKey.Text, txtUrl.Text, txtShopName.Text,
                                                        SettingsGeneral.SiteVersion, SettingsGeneral.SiteVersionDev);
                SettingsLic.ActiveLic = ActiveLic;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex, "Error at license check at installer");
            }
            if (!ActiveLic)
            {
                lblError.Text = Resources.Resource.Install_UserContols_ShopinfoView_Err_WrongKey;
            }
        }

        var validList = new List<ValidElement>
                            {
                                new ValidElement()
                                    {
                                        Control = txtShopName,
                                        ErrContent = ErrContent,
                                        ValidType = ValidType.Required,
                                        Message = Resources.Resource.Install_UserContols_ShopinfoView_Err_NeedName
                                    },
                                new ValidElement()
                                    {
                                        Control = txtUrl,
                                        ErrContent = ErrContent,
                                        ValidType = ValidType.Required,
                                        Message = Resources.Resource.Install_UserContols_ShopinfoView_Err_NeedUrl
                                    }
                            };

        return ValidationHelper.Validate(validList);
    }
}