<%@ WebHandler Language="C#" Class="BuyInOneClick" %>

//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

// Removed from patch 4.0.5

using System;
using System.Web;
using System.Web.SessionState;

public class BuyInOneClick : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        // nothing here...       
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
