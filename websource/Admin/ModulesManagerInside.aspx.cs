﻿using AdvantShop;
using AdvantShop.Controls;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;
using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Admin
{
    public partial class ModulesManagerInside : AdvantShopAdminPage
    {
    
        private const int ItemsPerPage = 6;

        protected override void InitializeCulture()
        {
            AdvantShop.Localization.Culture.InitializeCulture();
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
        
            if (!string.IsNullOrEmpty(Request["installModule"]))
            {
                ModulesService.InstallModule(SQLDataHelper.GetString(Request["installModule"].ToLower()), Request["version"]);
                Response.Redirect(UrlService.GetAdminAbsoluteLink("modulesmanagerinside.aspx"));
            }

            LoadData();
        }

        protected void lvModules_ItemCommand(object source, ListViewCommandEventArgs e)
        {

            var moduleVersion = ((HiddenField)e.Item.FindControl("hfLastVersion")).Value;
            var moduleIdOnRemoteServer = ((HiddenField)e.Item.FindControl("hfId")).Value;

            if (e.CommandName == "InstallLastVersion")
            {
                var message = ModulesService.GetModuleArchiveFromRemoteServer(moduleIdOnRemoteServer);

                if (message.IsNullOrEmpty())
                {
                    //ModulesService.InstallModule(SQLDataHelper.GetString(e.CommandArgument));
                    HttpRuntime.UnloadAppDomain();

                    Context.ApplicationInstance.CompleteRequest();
                    Response.Redirect(
                        UrlService.GetAdminAbsoluteLink("modulesmanagerinside.aspx?installModule=" + e.CommandArgument + "&version=" +
                                                        moduleVersion), false);
                }
                else
                {
                    //вывести message
                }
            }
            if (e.CommandName == "Install")
            {
                var moduleInst = AttachedModules.GetModules(AttachedModules.EModuleType.All).FirstOrDefault(
                    item =>
                    ((IModule)Activator.CreateInstance(item, null)).ModuleStringId.ToLower() == SQLDataHelper.GetString(e.CommandArgument).ToLower());

                if (moduleInst != null)
                {
                    ModulesService.InstallModule(SQLDataHelper.GetString(e.CommandArgument), moduleVersion);
                    Response.Redirect(UrlService.GetAdminAbsoluteLink("modulesmanagerinside.aspx"));
                }
                else
                {
                    var message = ModulesService.GetModuleArchiveFromRemoteServer(moduleIdOnRemoteServer);
                    if (message.IsNullOrEmpty())
                    {
                        //ModulesService.InstallModule(SQLDataHelper.GetString(e.CommandArgument));
                        HttpRuntime.UnloadAppDomain();

                        Context.ApplicationInstance.CompleteRequest();
                        Response.Redirect(
                            UrlService.GetAdminAbsoluteLink("modulesmanagerinside.aspx?installModule=" + e.CommandArgument + "&version=" +
                                                            moduleVersion), false);
                    }
                }
            }
            if (e.CommandName == "Uninstall")
            {
                ModulesService.UninstallModule(SQLDataHelper.GetString(e.CommandArgument));
                HttpRuntime.UnloadAppDomain();
                Response.Redirect(Request.Url.AbsoluteUri);
            }
        }

        protected void LoadData()
        {
            var modulesBox = ModulesService.GetModules();
            //if (modulesBox.Message.IsNullOrEmpty())
            {
                //lvModules.DataSource = modulesBox.Items.OrderBy(t => t.Name);
                //lvModules.DataBind();

                paging.TotalPages = (int)Math.Ceiling((double)modulesBox.Items.Count / ItemsPerPage);

                lvModulesManager.DataSource = modulesBox.Items.Skip((paging.CurrentPage - 1) * ItemsPerPage).Take(ItemsPerPage);
                lvModulesManager.DataBind();


            }
        }
    }
}