<%@ WebHandler Language="C#" Class="CommonStatisticData" %>

using System;
using System.Web;
using AdvantShop.Statistic;
using Newtonsoft.Json;

public class CommonStatisticData : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.Cache.SetLastModified(DateTime.UtcNow);

        context.Response.ContentType = "application/json";
        context.Response.Write(JsonConvert.SerializeObject(CommonStatistic.CurrentData));
        context.Response.End();
    }

    public bool IsReusable
    {
        get { return false; }
    }
}