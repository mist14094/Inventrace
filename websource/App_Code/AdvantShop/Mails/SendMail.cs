//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Threading;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Mails
{
    public class SendMail
    {

        // Old version
        //public static bool SendMailThread(string strTo, string strSubject, string strText, bool isBodyHtml, string smtpServer, string login, string password, int port, string emailFrom, bool ssl)
        //{
        //    try
        //    {
        //        using (var emailClient = new SmtpClient(smtpServer)
        //                                                           {
        //                                                               UseDefaultCredentials = false,
        //                                                               Credentials = new NetworkCredential(login, password),
        //                                                               DeliveryMethod = SmtpDeliveryMethod.Network,
        //                                                               Port = port,
        //                                                               EnableSsl = ssl
        //                                                           })
        //        {
        //            string[] strMails = strTo.Split(';');
        //            foreach (string strEmail in strMails)
        //            {
        //                string strE = strEmail.Trim();
        //                if (string.IsNullOrEmpty(strE)) continue;

        //                if (!ValidationHelper.IsValidEmail(strE)) continue;
        //                using (var message = new MailMessage(emailFrom, strE, strSubject, strText))
        //                {
        //                    message.IsBodyHtml = isBodyHtml;
        //                    emailClient.Send(message);
        //                }
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.LogError(ex);
        //    }

        //    return false;
        //}

        public static bool SendMailThread(string strTo, string strSubject, string strText, bool isBodyHtml, string smtpServer, string login, string password, int port, string emailFrom, bool ssl)
        {
            return (SendMailThreadStringResult(strTo, strSubject, strText, isBodyHtml, smtpServer, login, password, port, emailFrom, ssl) == "True");
        }

        public static string SendMailThreadStringResult(string strTo, string strSubject, string strText, bool isBodyHtml, string smtpServer, string login, string password, int port, string emailFrom, bool ssl)
        {
            string strResult = "True";

            if (strText == null)
            {
                return "False";
            }


            try
            {
                using (var emailClient = new SmtpClient(smtpServer)
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(login, password),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Port = port,
                    EnableSsl = ssl
                })
                {
                    string[] strMails = strTo.Split(';');
                    foreach (string strEmail in strMails)
                    {
                        string strE = strEmail.Trim();
                        if (string.IsNullOrEmpty(strE)) continue;

                        if (!ValidationHelper.IsValidEmail(strE)) continue;
                        using (var message = new MailMessage(emailFrom, strE, strSubject, strText))
                        {
                            message.IsBodyHtml = isBodyHtml;
                            emailClient.Send(message);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                strResult = ex.Message;
                Debug.LogError(ex, false);
            }

            return strResult;
        }

        public static bool SendMailNow(string strTo, string strSubject, string strText, bool isBodyHtml, string setSmtpServer, int setPort, string setLogin, string setPassword, string setEmailFrom, bool setSsl)
        {

            int workerThreads;
            int asyncIoThreads;
            ThreadPool.GetAvailableThreads(out workerThreads, out asyncIoThreads);
            if (workerThreads != 0)
                ThreadPool.QueueUserWorkItem(a => SendMailThread(strTo, strSubject, strText, isBodyHtml, setSmtpServer, setLogin, setPassword, setPort, setEmailFrom, setSsl));
            else
                new Thread(a => SendMailThread(strTo, strSubject, strText, isBodyHtml, setSmtpServer, setLogin, setPassword, setPort, setEmailFrom, setSsl)).Start();

            //ThreadPool.QueueUserWorkItem(a => SendMailThread(strTo, strSubject, strText, isBodyHtml, setSmtpServer, setLogin, setPassword, setPort, setEmailFrom, setSsl));
            return true;
        }

        public static bool SendMailNow(string strTo, string strSubject, string strText, bool isBodyHtml)
        {
            string smtp = SettingsMail.SMTP;
            string login = SettingsMail.Login;
            string password = SettingsMail.Password;
            int port = SettingsMail.Port;
            string email = SettingsMail.From;
            bool ssl = SettingsMail.SSL;
            return SendMailNow(strTo, strSubject, strText, isBodyHtml, smtp, port, login, password, email, ssl);
        }

        #region  BuildMail

        private static string GetMailFormatByType(MailType type)
        {
            return SQLDataAccess.ExecuteScalar<string>("[Settings].[sp_GetMailFormatByID]", CommandType.StoredProcedure, new SqlParameter("@FormatType", (int)type));
        }

        public static string BuildMail<T>(T clsParam) where T : ClsMailParam
        {
            var logo = SettingsMain.LogoImageName.IsNotEmpty()
                           ? String.Format("<img src=\"{0}\" alt=\"{1}\" title=\"{2}\" />",
                                           SettingsMain.SiteUrl.Trim('/') + '/' +
                                           FoldersHelper.GetPath(FolderType.Pictures, SettingsMain.LogoImageName, false),
                                           SettingsMain.ShopName, SettingsMain.ShopName)
                           : string.Empty;
            var format = GetMailFormatByType(clsParam.Type);
            if (format == null)
                return null;
            else
            {
                return clsParam.FormatString(format).Replace("#LOGO#", logo);    
            }
            
        }
        #endregion
    }
}