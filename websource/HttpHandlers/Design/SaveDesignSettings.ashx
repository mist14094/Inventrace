﻿<%@ WebHandler Language="C#" Class="SaveDesignSettings" %>

using System;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using AdvantShop.Configuration;
using AdvantShop;
using AdvantShop.Core.Caching;
using AdvantShop.Customers;
using AdvantShop.Helpers;

public class SaveDesignSettings : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";

        if (CustomerSession.CurrentCustomer.CustomerRole != Role.Administrator && !Demo.IsDemoEnabled && !Trial.IsTrialEnabled ||
            context.Request["theme"].IsNullOrEmpty() || context.Request["colorscheme"].IsNullOrEmpty() || context.Request["background"].IsNullOrEmpty())
        {
            context.Response.Write("error");
            return;
        }

        try
        {
            if (Demo.IsDemoEnabled)
            {
                CommonHelper.SetCookie("theme", context.Request["theme"]);
                CommonHelper.SetCookie("colorscheme", context.Request["colorscheme"]);
                CommonHelper.SetCookie("background", context.Request["background"]);
                CommonHelper.SetCookie("structure", context.Request["structure"]);
            }
            else
            {
                SettingsDesign.Theme = context.Request["theme"];
                SettingsDesign.ColorScheme = context.Request["colorscheme"];
                SettingsDesign.BackGround = context.Request["background"];
                SettingsDesign.MainPageMode = (SettingsDesign.eMainPageMode)Enum.Parse(typeof(SettingsDesign.eMainPageMode), context.Request["structure"]);
            }

            if (Trial.IsTrialEnabled || Demo.IsDemoEnabled)
            {
                switch ((SettingsDesign.eMainPageMode)Enum.Parse(typeof(SettingsDesign.eMainPageMode), context.Request["structure"]))
                // не использовать свойство SettingsDesign.MainPageMode, оно еще не обновилось
                {
                    case SettingsDesign.eMainPageMode.Default:
                        SettingsDesign.CountProductInLine = 2;
                        break;
                    case SettingsDesign.eMainPageMode.TwoColumns:
                        SettingsDesign.CountProductInLine = 4;
                        break;
                    case SettingsDesign.eMainPageMode.ThreeColumns:
                        SettingsDesign.CountProductInLine = 3;
                        break;
                }
            }

            Trial.TrackEvent(Trial.TrialEvents.ChangeColorScheme, context.Request["colorscheme"]);
            Trial.TrackEvent(Trial.TrialEvents.ChangeTheme, context.Request["theme"]);
            Trial.TrackEvent(Trial.TrialEvents.ChangeBackGround, context.Request["background"]);
            Trial.TrackEvent(Trial.TrialEvents.ChangeMainPageMode, context.Request["structure"]);

            CacheManager.Clean();
        }
        catch (Exception e)
        {
            AdvantShop.Diagnostics.Debug.LogError(e);
            context.Response.Write("error");
        }
        context.Response.Write("success");
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}