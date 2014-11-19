<%@ WebHandler Language="C#" Class="DownloadLog" %>

using System.Web;
using AdvantShop.Helpers;

public class DownloadLog : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        //AdvantShop.Helpers.CommonHelper.WriteResponseTxt(AdvantShop.Statistic.CommonStatistic.FileLog, AdvantShop.Statistic.CommonStatistic.VirtualFileLogPath);
        if (!System.IO.File.Exists(AdvantShop.Statistic.CommonStatistic.FileLog))
        {
            FileHelpers.CreateFile(AdvantShop.Statistic.CommonStatistic.FileLog);
        }
        CommonHelper.WriteResponseFile(AdvantShop.Statistic.CommonStatistic.FileLog, AdvantShop.Statistic.CommonStatistic.VirtualFileLogPath);
        //    HttpContext.Current.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}