﻿<%@ WebHandler Language="C#" Class="ExportOrderExcel" %>

using System;
using System.Web;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport.Excel;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Orders;

public class ExportOrderExcel : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        if (context.Request["OrderID"] != null)
        {
            string strPath = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp);
            FileHelpers.CreateDirectory(strPath);
            try
            {
                var ord = OrderService.GetOrder(SQLDataHelper.GetInt(context.Request["OrderID"]));
                var filename = String.Format("Order{0}.xls", ord.Number);
                var wrt = new ExcelSingleOrderWriter();
                wrt.Generate(strPath + filename, ord);
                CommonHelper.WriteResponseXls(strPath + filename, filename);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                context.Response.ContentType = "text/plain";
                context.Response.Write("Error on creating xls document");
            }
        }
        else
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("Error on creating xls document");
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}