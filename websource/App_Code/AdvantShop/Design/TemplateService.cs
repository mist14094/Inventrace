using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using AdvantShop.Configuration;
using System.Net;
using Newtonsoft.Json;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Design
{
    public class TemplateService
    {
        private const string RequestUrlGetTemplates = "http://modules.advantshop.net/template/gettemplates/?lickey={0}";
        private const string RequestUrlGetTemplateArchive = "http://modules.advantshop.net/template/gettemplate?lickey={0}&templateId={1}";
        public const string DefaultTemplateId = "_default";

        public static TemplateBox GetTemplates()
        {
            var templates = GetTemplatesFromRemoteServer();

            templates.Items.Add(new Template()
                                {
                                    StringId = DefaultTemplateId,
                                    Name = Resources.Resource.Admin_Templates_DefaultTemplate,
                                    IsInstall = true,
                                    PreviewImage = "../images/design/preview.png",
                                    Active = true
                                });

            foreach (var templateFolder in Directory.GetDirectories(SettingsGeneral.AbsolutePath + "Templates"))
            {
                if (File.Exists(templateFolder + "\\MasterPage.master"))
                {
                    var stringId = templateFolder.Split('\\').Last();
                    var curTemplate = templates.Items.Find(t => t.StringId == stringId.ToLower());

                    if (curTemplate != null)
                    {
                        curTemplate.IsInstall = true;
                        curTemplate.Active = true;
                        curTemplate.PreviewImage = File.Exists(templateFolder + "\\images\\design\\preview.png")
                                                                        ? "../templates/" + stringId + "/images/design/preview.png" : string.Empty;
                    }
                    else
                    {
                        templates.Items.Add(new Template
                                                {
                                                    StringId = stringId,
                                                    Name = stringId,
                                                    IsInstall = true,
                                                    Active = true,
                                                    PreviewImage = File.Exists(templateFolder + "\\images\\design\\preview.png")
                                                                        ? "../templates/" + stringId + "/images/design/preview.png" : string.Empty
                                                });
                    }
                }
            }

            templates.Items = templates.Items.OrderBy(t => t.Name).OrderByDescending(t => t.IsInstall).ToList();

            var resultTemplateBox = new TemplateBox() 
                                    { 
                                        Message = templates.Message, 
                                        Items = new List<Template>() 
                                    };

            resultTemplateBox.Items.Add(templates.Items.FirstOrDefault(t => t.StringId == SettingsDesign.Template));
            resultTemplateBox.Items.AddRange(templates.Items.Where(t => t.StringId != SettingsDesign.Template));

            return resultTemplateBox;
        }

        private static TemplateBox GetTemplatesFromRemoteServer()
        {
            var templateBox = new TemplateBox() { Items = new List<Template>() };

            try
            {
                var request = WebRequest.Create(string.Format(RequestUrlGetTemplates, SettingsLic.LicKey));
                request.Method = "GET";

                using (var dataStream = request.GetResponse().GetResponseStream())
                    using (var reader = new StreamReader(dataStream))
                    {
                        var responseFromServer = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(responseFromServer))
                        {
                            templateBox = JsonConvert.DeserializeObject<TemplateBox>(responseFromServer);
                        }
                    }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }

            return templateBox;
        }

        public static string GetTemplateArchiveFromRemoteServer(string templateId)
        {
            var zipFileName = HttpContext.Current.Server.MapPath("~/App_Data/" + Guid.NewGuid() + ".Zip");
            try
            {
                using (var wc = new WebClient())
                {
                    wc.DownloadFile(string.Format(RequestUrlGetTemplateArchive, SettingsLic.LicKey, templateId),
                                                    zipFileName);
                }

                if (!FileHelpers.UnZipFile(zipFileName, HttpContext.Current.Server.MapPath("~/Templates/")))
                {
                    return "error on UnZipFile";
                }

                FileHelpers.DeleteFile(zipFileName);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return "error on download or unzip";
            }

            return string.Empty;
        }
    }
}