//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Orders
{
    /// <summary>
    /// Summary description for StatisticsService
    /// </summary>
    public class OrderStatisticsService
    {
        private static float? _salesPlan;
        private static float? _profitPlan;

        public static float SalesPlan
        {
            get
            {
                if (_salesPlan != null)
                    return _salesPlan.Value;

                GetProfitPlan();

                return _salesPlan != null ? _salesPlan.Value : 0;
            }
            set { _salesPlan = value; }
        }

        public static float ProfitPlan
        {
            get
            {
                if (_profitPlan != null)
                    return _profitPlan.Value;
                GetProfitPlan();
                return _profitPlan != null ? _profitPlan.Value : 0;
            }
            set { _profitPlan = value; }
        }
        public static Dictionary<DateTime, float> GetOrdersSumByPeriod(DateTime minDate, DateTime maxDate)
        {
            var sums = new Dictionary<DateTime, float>();
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Order].[sp_GetSumByMonths]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cmd.Parameters.AddWithValue("@MinDate", minDate);
                db.cmd.Parameters.AddWithValue("@MaxDate", maxDate);
                db.cnOpen();
                using (SqlDataReader reader = db.cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        sums.Add(
                            new DateTime(SQLDataHelper.GetInt(reader, "Year"), SQLDataHelper.GetInt(reader, "Month"), 1),
                            SQLDataHelper.GetFloat(reader, "Sum"));
                    }
                db.cnClose();
                return sums;
            }
        }

        public static Dictionary<DateTime, float> GetOrdersProfitByPeriod(DateTime minDate, DateTime maxDate)
        {
            var sums = new Dictionary<DateTime, float>();
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Order].[sp_GetProfitByMonths]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cmd.Parameters.AddWithValue("@MinDate", minDate);
                db.cmd.Parameters.AddWithValue("@MaxDate", maxDate);
                db.cnOpen();
                using (SqlDataReader reader = db.cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        sums.Add(
                            new DateTime(SQLDataHelper.GetInt(reader, "Year"), SQLDataHelper.GetInt(reader, "Month"), 1),
                            SQLDataHelper.GetFloat(reader, "Profit"));
                    }
                db.cnClose();
                return sums;
            }
        }

        public static Dictionary<DateTime, float> GetOrdersSumByDays(DateTime minDate, DateTime maxDate)
        {
            var sums = new Dictionary<DateTime, float>();
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Order].[sp_GetSumByDays]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cmd.Parameters.AddWithValue("@MinDate", minDate);
                db.cmd.Parameters.AddWithValue("@MaxDate", maxDate);
                db.cnOpen();
                using (SqlDataReader reader = db.cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        sums.Add(SQLDataHelper.GetDateTime(reader, "Date"), SQLDataHelper.GetFloat(reader, "Sum"));
                    }
                db.cnClose();
                return sums;
            }
        }

        public static Dictionary<DateTime, float> GetOrdersProfitByDays(DateTime minDate, DateTime maxDate)
        {
            var sums = new Dictionary<DateTime, float>();
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Order].[sp_GetProfitByDays]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cmd.Parameters.AddWithValue("@MinDate", minDate);
                db.cmd.Parameters.AddWithValue("@MaxDate", maxDate);
                db.cnOpen();
                using (SqlDataReader reader = db.cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        sums.Add(SQLDataHelper.GetDateTime(reader, "Date"), SQLDataHelper.GetFloat(reader, "Profit"));
                    }
                db.cnClose();
                return sums;
            }
        }

        public static Dictionary<DateTime, int> GetOrdersCountByPeriod(DateTime minDate, DateTime maxDate)
        {
            var sums = new Dictionary<DateTime, int>();
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Order].[sp_GetCountByMonths]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cmd.Parameters.AddWithValue("@MinDate", minDate);
                db.cmd.Parameters.AddWithValue("@MaxDate", maxDate);
                db.cnOpen();
                using (SqlDataReader reader = db.cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        sums.Add(
                            new DateTime(SQLDataHelper.GetInt(reader, "Year"), SQLDataHelper.GetInt(reader, "Month"), 1),
                            SQLDataHelper.GetInt(reader, "Count"));
                    }
                db.cnClose();
                return sums;
            }
        }


        public static List<KeyValuePair<string, int>> GetShippingMethodRating()
        {
            var result = new List<KeyValuePair<string, int>>();

            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Order].[sp_GetShippingRating]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cnOpen();
                using (SqlDataReader reader = db.cmd.ExecuteReader())
                    while (reader.Read())
                    {

                        result.Add( new KeyValuePair<string,int>( SQLDataHelper.GetString(reader, "ShippingMethod"),
                                   SQLDataHelper.GetInt(reader, "Rating")));
                    }
                db.cnClose();
            }
            return result;
        }


        //TODO vladimir, �������� ����� �������� ���������� ������ ������ � ��������
        public static List<KeyValuePair<string, int>> GetPaymentTypeRating()
        {
            var result = new List<KeyValuePair<string, int>>();

            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Order].[sp_GetPaymentRating]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cnOpen();
                using (SqlDataReader reader = db.cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new KeyValuePair<string, int>(SQLDataHelper.GetString(reader, "PaymentMethod"),
                                   SQLDataHelper.GetInt(reader, "Rating")));
                    }
                    reader.Close();
                }
                db.cnClose();
            }
            return result;
        }
        public static KeyValuePair<float, float> GetMonthProgress()
        {
            KeyValuePair<float, float> res;
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Order].[sp_GetOrdersMonthProgress]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cnOpen();

                using (SqlDataReader reader = db.cmd.ExecuteReader())
                {
                    reader.Read();
                    res = new KeyValuePair<float, float>(SQLDataHelper.GetFloat(reader, "Sum"),
                                                             SQLDataHelper.GetFloat(reader, "Profit"));
                    reader.Close();
                }

                db.cnClose();
            }

            return res;
        }

        public static void GetProfitPlan()
        {
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Settings].[sp_GetLastProfitPlan]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cnOpen();
                using (SqlDataReader reader = db.cmd.ExecuteReader())
                {
                    reader.Read();
                    SalesPlan = SQLDataHelper.GetFloat(reader, "SalesPlan");
                    ProfitPlan = SQLDataHelper.GetFloat(reader, "ProfitPlan");
                }
                db.cnClose();
            }

        }

        public static void SetProfitPlan(float sales, float profit)
        {
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = "[Settings].[sp_SetPlan]";
                db.cmd.CommandType = CommandType.StoredProcedure;
                db.cmd.Parameters.Clear();
                db.cmd.Parameters.AddWithValue("@SalesPlan", sales);
                db.cmd.Parameters.AddWithValue("@ProfitPlan", profit);
                db.cnOpen();
                db.cmd.ExecuteNonQuery();
                db.cnClose();
                GetProfitPlan();
            }
        }
    }
}