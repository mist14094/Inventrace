//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Payment
{
    public class PaymentService
    {
        public enum PageWithPaymentButton
        {
            myaccount,
            orderconfirmation
        }

        public static IEnumerable<PaymentMethod> GetAllPaymentMethods(bool onlyEnabled)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<PaymentMethod>(
                onlyEnabled
                    ? "SELECT * FROM [Order].[PaymentMethod] left join Catalog.Photo on Photo.ObjId=PaymentMethod.PaymentMethodID and Type=@Type where Enabled=1 ORDER BY [SortOrder]"
                    : "SELECT * FROM [Order].[PaymentMethod] left join Catalog.Photo on Photo.ObjId=PaymentMethod.PaymentMethodID and Type=@Type ORDER BY [SortOrder]",
                CommandType.Text,
                reader => GetPaymentMethodFromReader(reader, true), new SqlParameter("@Type", PhotoType.Payment.ToString()));
        }

        public static int GetCountPaymentMethods()
        {
            return SQLDataAccess.ExecuteScalar<int>("SELECT count(*) FROM [Order].[PaymentMethod] ", CommandType.Text);
        }

        public static IEnumerable<int> GetAllPaymentMethodIDs()
        {
            return SQLDataAccess.ExecuteReadColumnIEnumerable<int>("SELECT [PaymentMethodID] FROM [Order].[PaymentMethod]", CommandType.Text, "PaymentMethodID");
        }

        public static void DeletePaymentMethod(int paymentMethodId)
        {
            PhotoService.DeletePhotos(paymentMethodId, PhotoType.Payment);
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Order].[PaymentMethod] WHERE [PaymentMethodID] = @PaymentMethodID", CommandType.Text, new SqlParameter("@PaymentMethodID", paymentMethodId));
        }

        public static PaymentMethod GetPaymentMethod(int paymentMethodId)
        {
            return SQLDataAccess.ExecuteReadOne<PaymentMethod>(
                "SELECT * FROM [Order].[PaymentMethod] WHERE [PaymentMethodID] = @PaymentMethodID", CommandType.Text, reader => GetPaymentMethodFromReader(reader),
                new SqlParameter("@PaymentMethodID", paymentMethodId));
        }

        public static PaymentMethod GetPaymentMethodByName(string name)
        {
            return SQLDataAccess.ExecuteReadOne<PaymentMethod>(
                "SELECT top(1) * FROM [Order].[PaymentMethod] WHERE [Name] = @Name",
                CommandType.Text, reader => GetPaymentMethodFromReader(reader), new SqlParameter("@Name", name));
        }

        public static PaymentMethod GetPaymentMethodByType(PaymentType type)
        {
            return SQLDataAccess.ExecuteReadOne<PaymentMethod>(
                "SELECT top(1) * FROM [Order].[PaymentMethod] WHERE [PaymentType] = @PaymentType",
                CommandType.Text, reader => GetPaymentMethodFromReader(reader), new SqlParameter("@PaymentType", (int)type));
        }

        public static PaymentMethod GetPaymentMethodFromReader(SqlDataReader reader, bool loadPic = false)
        {
            PaymentMethod method = PaymentMethod.Create((PaymentType)SQLDataHelper.GetInt(reader, "PaymentType"));
            method.PaymentMethodID = SQLDataHelper.GetInt(reader, "PaymentMethodID");
            method.Name = SQLDataHelper.GetString(reader, "Name");
            method.Enabled = SQLDataHelper.GetBoolean(reader, "Enabled");
            method.Description = SQLDataHelper.GetString(reader, "Description");
            method.SortOrder = SQLDataHelper.GetInt(reader, "SortOrder");
            method.ExtrachargeType = (ExtrachargeType)SQLDataHelper.GetInt(reader, "ExtrachargeType");
            method.Extracharge = SQLDataHelper.GetFloat(reader, "Extracharge");

            method.Parameters = GetPaymentMethodParameters(method.PaymentMethodID);
            if (loadPic)
                method.IconFileName = new Photo(SQLDataHelper.GetInt(reader, "PhotoId"), SQLDataHelper.GetInt(reader, "ObjId"), PhotoType.Payment) { PhotoName = SQLDataHelper.GetString(reader, "PhotoName") };
            return method;
        }

        private static Dictionary<string, string> GetPaymentMethodParameters(int paymentMethodId)
        {
            return SQLDataAccess.ExecuteReadDictionary<string, string>("SELECT Name, Value FROM [Order].[PaymentParam] WHERE [PaymentMethodID] = @PaymentMethodID",
                CommandType.Text, "Name", "Value", new SqlParameter("@PaymentMethodID", paymentMethodId));
        }

        public static int AddPaymentMethod(PaymentMethod method)
        {
            var id = SQLDataAccess.ExecuteScalar<int>(
                    "INSERT INTO [Order].[PaymentMethod] ([PaymentType],[Name], [Enabled], [Description], [SortOrder],Extracharge,ExtrachargeType) VALUES " +
                    "(@PaymentType,@Name, @Enabled, @Description, @SortOrder,@Extracharge,@ExtrachargeType); SELECT scope_identity();",
                    CommandType.Text,
                    new SqlParameter("@PaymentType", method.Type),
                    new SqlParameter("@Name", method.Name ?? string.Empty),
                    new SqlParameter("@Enabled", method.Enabled),
                    new SqlParameter("@Description", method.Description ?? string.Empty),
                    new SqlParameter("@SortOrder", method.SortOrder),
                    new SqlParameter("@Extracharge", method.Extracharge),
                    new SqlParameter("@ExtrachargeType", (int)method.ExtrachargeType)
                    );
            AddPaymentMethodParameters(id, method.Parameters);
            return id;

        }

        private static void AddPaymentMethodParameters(int paymentMethodId, Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters.Where(parameter => parameter.Value.IsNotEmpty()))
            {
                SQLDataAccess.ExecuteNonQuery(
                    "INSERT INTO [Order].[PaymentParam] (PaymentMethodID, Name, Value) VALUES (@PaymentMethodID, @Name, @Value)",
                    CommandType.Text,
                    new SqlParameter("@PaymentMethodID", paymentMethodId),
                    new SqlParameter("@Name", parameter.Key),
                    new SqlParameter("@Value", parameter.Value));
            }
        }

        public static void UpdatePaymentMethod(PaymentMethod paymentMethod)
        {
            SQLDataAccess.ExecuteNonQuery(
                @"UPDATE [Order].[PaymentMethod] SET [Name] = @Name,[Enabled] = @Enabled,[SortOrder] = @SortOrder,[Description] = @Description,[PaymentType] = @PaymentType, " +
                "Extracharge=@Extracharge, ExtrachargeType=@ExtrachargeType WHERE [PaymentMethodID] = @PaymentMethodID",
                CommandType.Text,
                new SqlParameter("@PaymentMethodID", paymentMethod.PaymentMethodID),
                new SqlParameter("@Name", paymentMethod.Name),
                new SqlParameter("@Enabled", paymentMethod.Enabled),
                new SqlParameter("@SortOrder", paymentMethod.SortOrder),
                new SqlParameter("@Description", paymentMethod.Description),
                new SqlParameter("@PaymentType", (int)paymentMethod.Type),
                new SqlParameter("@Extracharge", paymentMethod.Extracharge),
                new SqlParameter("@ExtrachargeType", (int)paymentMethod.ExtrachargeType)
                );

            UpdatePaymentParams(paymentMethod.PaymentMethodID, paymentMethod.Parameters);
        }

        public static void UpdatePaymentParams(int paymentMethodId, Dictionary<string, string> parameters)
        {
            foreach (var kvp in parameters)
            {
                SQLDataAccess.ExecuteNonQuery("[Order].[sp_UpdatePaymentParam]", CommandType.StoredProcedure,
                    new SqlParameter("@PaymentMethodID", paymentMethodId), new SqlParameter("@Name", kvp.Key), new SqlParameter("@Value", kvp.Value));
            }
        }

        public static List<PaymentMethod> GetCertificatePaymentMethods()
        {
            return SQLDataAccess.ExecuteReadList<PaymentMethod>(
                "SELECT * FROM [Settings].[GiftCertificatePayments] INNER JOIN [Order].[PaymentMethod] ON [PaymentMethod].[PaymentMethodID] = [GiftCertificatePayments].[PaymentID] WHERE [Enabled] = 1",
                CommandType.Text, reader => GetPaymentMethodFromReader(reader));
        }

       public static void SaveOrderpaymentInfo(int orderId, int paymentId, string name, string value)
        {
            SQLDataAccess.ExecuteNonQuery(
                    "INSERT INTO [Order].[OrderPaymentInfo] (OrderID, PaymentMethodID, Name, Value) VALUES (@OrderID, @PaymentMethodID, @Name, @Value)",
                    CommandType.Text,
                    new SqlParameter("@OrderID", orderId),
                    new SqlParameter("@PaymentMethodID", paymentId),
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Value", value));
        }

public static PaymentAdditionalInfo GetOrderIdByMaymentIdAndCode(int paymentMethodId, string response_code)
        {
            return SQLDataAccess.ExecuteReadOne<PaymentAdditionalInfo>(
                "Select * From [Order].[OrderPaymentInfo] Where PaymentMethodID = @PaymentMethodID AND Value = @Code",
                CommandType.Text,
                (reader) => new PaymentAdditionalInfo
                    {
                        PaymentMethodId = SQLDataHelper.GetInt(reader, "PaymentMethodID"),
                        OrderId = SQLDataHelper.GetInt(reader, "OrderID"),
                        Name = SQLDataHelper.GetString(reader, "Name"),
                        Value = SQLDataHelper.GetString(reader, "Value")
                    },
                new SqlParameter("@PaymentMethodID", paymentMethodId),
                new SqlParameter("@Code", response_code));
        }
    }
}