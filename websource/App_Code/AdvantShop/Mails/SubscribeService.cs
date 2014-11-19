//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Modules;
using AdvantShop.Modules.Interfaces;

/// <summary>
/// Summary description for Subscribe
/// </summary>
public class SubscribeService
{
    //return true if  
    public static bool IsExistInSubscribeEmails(string email)
    {
        return SQLDataAccess.ExecuteScalar<int>("[dbo].[sp_SubscribeGetEmailCountByEmail]", CommandType.StoredProcedure, new SqlParameter("@Email", email)) > 0;
    }

    public static bool IsExistInCustomerEmails(string email)
    {
        return SQLDataAccess.ExecuteScalar<int>("[dbo].[sp_SubscribeGetRegEmailCountByEmail]", CommandType.StoredProcedure, new SqlParameter("@Email", email)) > 0;
    }

    public static void SubscribeInsertEmail(string email, string strActivateCode, string strDeactivateCode)
    {
        SQLDataAccess.ExecuteNonQuery("[dbo].[sp_SubscribeInsertEmail]",
                                        CommandType.StoredProcedure,
                                        new SqlParameter("@Email", email),
                                        new SqlParameter("@ActivateCode", strActivateCode),
                                        new SqlParameter("@DectivateCode", strDeactivateCode));
        
        var modules = AttachedModules.GetModules(AttachedModules.EModuleType.SendMails).Where(item => ((ISendMails)Activator.CreateInstance(item, null)).IsActive);
        foreach (var moduleType in modules)
        {
            var moduleObject = (ISendMails)Activator.CreateInstance(moduleType, null);
            moduleObject.SubscribeEmail(email);
        }
    }

    public static List<string> SubscribeGetRegCustomerEmails()
    {
        return SQLDataAccess.ExecuteReadColumn<string>(
            "SELECT Email FROM Customers.Customer WHERE Subscribed4News = 1",
            CommandType.Text,
            "Email");
    }

    public static List<string> SubscribeGetUnRegCustomerEmails()
    {
        return SQLDataAccess.ExecuteReadColumn<string>(
            "SELECT Email FROM dbo.Subscribe",
            CommandType.Text,
            "Email");
    }

    public static List<string> SubscribeGetAllCustomerEmails()
    {
        return SQLDataAccess.ExecuteReadColumn<string>(
            "SELECT [Email] FROM [Customers].[Customer] WHERE [Subscribed4News] = 1 union SELECT Email FROM Subscribe WHERE (Enable = 1)",
            CommandType.Text,
            "Email");
    }

    public static int SubscribeGetEmailCountByActivateCode(string activateCode)
    {
        return SQLDataAccess.ExecuteScalar<int>("[dbo].[sp_SubscribeGetEmailCountByActivateCode]", CommandType.StoredProcedure, new SqlParameter("@ActivateCode", activateCode));
    }

    public static void SubscribeUpdateEnableByActivateCode(string activateCode)
    {
        SQLDataAccess.ExecuteNonQuery("[dbo].[sp_SubscribeUpdateEnableByActivateCode]", CommandType.StoredProcedure, new SqlParameter("@ActivateCode", activateCode));
    }

    public static void SubscribeInsertDeactivateReason(string messageText)
    {
        SQLDataAccess.ExecuteNonQuery("[dbo].[sp_SubscribeInsertDeactivateReason]", CommandType.StoredProcedure, new SqlParameter("@MessageText", messageText));
    }

    public static string SubscribeGetDectivateCodeByEmail(string email)
    {
        return SQLDataAccess.ExecuteScalar<string>("[dbo].[sp_SubscribeGetDectivateCodeByEmail]", CommandType.StoredProcedure, new SqlParameter("@Email", email));
    }

    public static int SubscribeGetEmailCountByDeactivateCode(string dectivateCode)
    {
        return SQLDataAccess.ExecuteScalar<int>("[dbo].[sp_SubscribeGetEmailCountByDeactivateCode]", CommandType.StoredProcedure, new SqlParameter("@DectivateCode", dectivateCode));
    }

    public static string SubscribeDeleteEmail(string dectivateCode)
    {
        return SQLDataAccess.ExecuteScalar<string>("[dbo].[sp_SubscribeDeleteEmail]", CommandType.StoredProcedure, new SqlParameter("@DectivateCode", dectivateCode));
    }
}