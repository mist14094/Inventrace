//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.IO;
using System.Net;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop
{
    public class Trial
    {
        public enum TrialEvents
        {
            // lvl 0

            VisitClientSide,
            VisitAdminSide,

            // lvl 1
            CreateShop,
            ChangeShopName,
            ChangePhoneNumber,
            ChangeTheme,
            ChangeColorScheme,
            ChangeBackGround,
            ChangeMainPageMode,
            ChangeLogo,
            AddCarousel,
            CheckoutOrder,
            ChangeOrderStatus,
            ShareInSocialNetwork,

            // lvl 2
            DeleteTestData,
            AddCategory,
            AddProduct,
            AddProductPhoto,
            AddProductProperty,
            AddShippingMethod,
            AddPaymentMethod,
            ChangeContactPage,
            ActivateModule,

            // lvl 3
            ChangeDomain,
            SendTestEmail,
            SetUpGoogleAnalytics,
            SetUpYandexMentrika,
            GetFirstThouthandVisitors,
            GetFirstOrder,
            ExportProductsToFeed,

            // lvl 2.5
            MakeCSVExport,
            MakeCSVImport
        }

        private const string UrlTrialInfo = "http://modules.advantshop.net/Trial/GetParams/{0}";
        private const string UrlTrialEvents = "http://cap.advantshop.net/Achivements/Event/LogEvent?licKey={0}&eventName={1}&eventParams={2}";

        private static DateTime _lastUpdate;

        private static DateTime _trialTillCached = DateTime.MinValue;

        public static bool IsTrialEnabled
        {
            get { return ModeConfigService.IsModeEnabled(ModeConfigService.Modes.TrialMode); }
        }


        public static DateTime TrialPeriodTill
        {
            get
            {
                if (DateTime.Now > _lastUpdate.AddHours(1))
                {
                    try
                    {
                        var request = WebRequest.Create(string.Format(UrlTrialInfo, SettingsLic.LicKey));
                        request.Method = "GET";

                        using (var dataStream = request.GetResponse().GetResponseStream())
                        {
                            using (var reader = new StreamReader(dataStream))
                            {
                                var responseFromServer = reader.ReadToEnd();
                                if (!string.IsNullOrEmpty(responseFromServer))
                                {
                                    _trialTillCached = JsonConvert.DeserializeObject<DateTime>(responseFromServer);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex);
                    }
                    _lastUpdate = DateTime.Now;
                }
                return _trialTillCached;
            }
        }

        public static void TrackEvent(TrialEvents trialEvent, string eventParams)
        {
            if (IsTrialEnabled)
            {
                new System.Threading.Tasks.Task(() =>
                    {
                        try
                        {
                            new WebClient().DownloadString(
                                string.Format(UrlTrialEvents, SettingsLic.LicKey, trialEvent.ToString(),
                                              HttpUtility.UrlEncode(eventParams)));
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex, false);
                        }
                    }).Start();
            }
        }
    }
}