//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Mails;

namespace AdvantShop.Orders
{
    public class OrderByRequestService
    {
        public static OrderByRequest GetOrderByRequest(int orderByRequestId)
        {
            var orderByRequest = SQLDataAccess.ExecuteReadOne<OrderByRequest>("SELECT * FROM [Order].[OrderByRequest] WHERE OrderByRequestId = @OrderByRequestId ", CommandType.Text,
                                                                                         GetOrderByRequestFromReader, new SqlParameter("@OrderByRequestId", orderByRequestId));
            return orderByRequest;
        }

        public static OrderByRequest GetOrderByRequest(string code)
        {
            var orderByRequest = SQLDataAccess.ExecuteReadOne<OrderByRequest>("SELECT TOP(1) * FROM [Order].[OrderByRequest] WHERE Code = @code  ", CommandType.Text,
                                                                                         GetOrderByRequestFromReader, new SqlParameter("@Code", code));
            return orderByRequest;
        }

        public static List<int> GetIdList()
        {
            List<int> idList = SQLDataAccess.ExecuteReadList<int>("SELECT [OrderByRequestId] FROM [Order].[OrderByRequest]", CommandType.Text,
                                                             reader => SQLDataHelper.GetInt(reader, "OrderByRequestId"));
            return idList;
        }

        public static List<OrderByRequest> GetOrderByRequestList()
        {
            var orderByRequestList = SQLDataAccess.ExecuteReadList<OrderByRequest>("SELECT [OrderByRequestId] FROM [Order].[OrderByRequest]", CommandType.Text,
                                                                                    GetOrderByRequestFromReader);
            return orderByRequestList;
        }

        private static OrderByRequest GetOrderByRequestFromReader(SqlDataReader reader)
        {
            return new OrderByRequest
            {
                OrderByRequestId = SQLDataHelper.GetInt(reader, "OrderByRequestId"),
                ProductId = SQLDataHelper.GetInt(reader, "ProductID"),
                OfferId = SQLDataHelper.GetInt(reader, "OfferID"),
                ProductName = SQLDataHelper.GetString(reader, "ProductName"),
                ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                Quantity = SQLDataHelper.GetInt(reader, "Quantity"),
                UserName = SQLDataHelper.GetString(reader, "UserName"),
                Email = SQLDataHelper.GetString(reader, "Email"),
                Phone = SQLDataHelper.GetString(reader, "Phone"),
                Comment = SQLDataHelper.GetString(reader, "Comment"),
                IsComplete = SQLDataHelper.GetBoolean(reader, "IsComplete"),
                RequestDate = SQLDataHelper.GetDateTime(reader, "RequestDate"),
                Code = SQLDataHelper.GetString(reader, "Code"),
                CodeCreateDate = SQLDataHelper.GetDateTime(reader, "CodeCreateDate", DateTime.MinValue),
                LetterComment = SQLDataHelper.GetString(reader, "LetterComment")
            };
        }

        public static void AddOrderByRequest(OrderByRequest orderByRequest)
        {
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = " INSERT INTO [Order].[OrderByRequest] " +
                                     " ([ProductID], [ProductName], [ArtNo], [Quantity], [UserName], [Email], [Phone], [Comment], [IsComplete], [RequestDate], [OfferID], LetterComment ) " +
                                     " VALUES (@ProductID, @ProductName, @ArtNo, @Quantity, @UserName, @Email, @Phone, @Comment, @IsComplete, @RequestDate, @OfferID, @LetterComment); SELECT SCOPE_IDENTITY();";
                db.cmd.CommandType = CommandType.Text;
                db.cmd.Parameters.Clear();

                db.cmd.Parameters.AddWithValue("@ProductID", orderByRequest.ProductId);
                db.cmd.Parameters.AddWithValue("@ProductName", orderByRequest.ProductName);
                db.cmd.Parameters.AddWithValue("@ArtNo", orderByRequest.ArtNo);
                db.cmd.Parameters.AddWithValue("@Quantity", orderByRequest.Quantity);
                db.cmd.Parameters.AddWithValue("@UserName", orderByRequest.UserName);
                db.cmd.Parameters.AddWithValue("@Email", orderByRequest.Email);
                db.cmd.Parameters.AddWithValue("@Phone", orderByRequest.Phone);
                db.cmd.Parameters.AddWithValue("@Comment", orderByRequest.Comment);
                db.cmd.Parameters.AddWithValue("@IsComplete", orderByRequest.IsComplete);
                db.cmd.Parameters.AddWithValue("@RequestDate", orderByRequest.RequestDate);
                db.cmd.Parameters.AddWithValue("@OfferID", orderByRequest.OfferId);
                db.cmd.Parameters.AddWithValue("@LetterComment", orderByRequest.LetterComment ?? string.Empty);

                db.cnOpen();
                orderByRequest.OrderByRequestId = SQLDataHelper.GetInt(db.cmd.ExecuteScalar());
                db.cnClose();
            }
        }

        public static void UpdateOrderByRequest(OrderByRequest orderByRequest)
        {
            using (var db = new SQLDataAccess())
            {
                db.cmd.CommandText = " UPDATE [Order].[OrderByRequest] SET [ProductID] = @ProductID, [ProductName] = @ProductName, [ArtNo] = @ArtNo, [Quantity] = @Quantity, [UserName] = @UserName, [Email] = @Email, [Phone] = @Phone, [Comment] = @Comment, [IsComplete] = @IsComplete, [RequestDate] = @RequestDate, [OfferID] = @OfferID, LetterComment=@LetterComment " +
                                     " WHERE OrderByRequestId = @OrderByRequestId";
                db.cmd.CommandType = CommandType.Text;
                db.cmd.Parameters.Clear();

                db.cmd.Parameters.AddWithValue("@OrderByRequestId", orderByRequest.OrderByRequestId);
                db.cmd.Parameters.AddWithValue("@ProductID", orderByRequest.ProductId);
                db.cmd.Parameters.AddWithValue("@ProductName", orderByRequest.ProductName);
                db.cmd.Parameters.AddWithValue("@ArtNo", orderByRequest.ArtNo);
                db.cmd.Parameters.AddWithValue("@Quantity", orderByRequest.Quantity);
                db.cmd.Parameters.AddWithValue("@UserName", orderByRequest.UserName);
                db.cmd.Parameters.AddWithValue("@Email", orderByRequest.Email);
                db.cmd.Parameters.AddWithValue("@Phone", orderByRequest.Phone);
                db.cmd.Parameters.AddWithValue("@Comment", orderByRequest.Comment);
                db.cmd.Parameters.AddWithValue("@IsComplete", orderByRequest.IsComplete);
                db.cmd.Parameters.AddWithValue("@RequestDate", orderByRequest.RequestDate);
                db.cmd.Parameters.AddWithValue("@OfferID", orderByRequest.OfferId);
                db.cmd.Parameters.AddWithValue("@LetterComment", orderByRequest.LetterComment);

                db.cnOpen();
                db.cmd.ExecuteNonQuery();
                db.cnClose();
            }
        }

        public static void DeleteOrderByRequest(int orderByRequestId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [Order].[OrderByRequest] WHERE OrderByRequestId = @OrderByRequestId", CommandType.Text, new SqlParameter("@OrderByRequestId", orderByRequestId));
        }

        public static string CreateCode(int orderByRequestId)
        {
            var code = Guid.NewGuid().ToString();
            SQLDataAccess.ExecuteNonQuery("UPDATE [Order].[OrderByRequest] SET [Code] = @Code, [CodeCreateDate] = GETDATE() WHERE OrderByRequestId = @OrderByRequestId", CommandType.Text,
                                                new SqlParameter("@OrderByRequestId", orderByRequestId), new SqlParameter("@Code", code));
            return code;
        }

        public static void DeleteCode(string code)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [Order].[OrderByRequest] SET [Code] = '' WHERE [Code] = @Code", CommandType.Text, new SqlParameter("@Code", code));
        }

        public static void SendConfirmationMessage(int orderByRequestId)
        {
            var orderByRequest = GetOrderByRequest(orderByRequestId);
            var code = CreateCode(orderByRequestId);

            var offer = OfferService.GetOffer(orderByRequest.OfferId);
            var clsParam = new ClsMailParamOnSendLinkByRequest
                               {
                                   OrderByRequestId = orderByRequest.OrderByRequestId.ToString(),
                                   UserName = orderByRequest.UserName,
                                   ArtNo = orderByRequest.ArtNo,
                                   ProductName = orderByRequest.ProductName,
                                   Quantity = orderByRequest.Quantity.ToString(),
                                   Code = code,
                                   Color = offer != null && offer.Color != null ? offer.Color.ColorName : "",
                                   Size = offer != null && offer.Size != null ? offer.Size.SizeName : "",
                                   Comment = orderByRequest.LetterComment, 
                               };

            string message = SendMail.BuildMail(clsParam);
            SendMail.SendMailNow(orderByRequest.Email, Resources.Resource.Admin_OrderByRequest_OrderLink, message, true);
        }

        public static void SendFailureMessage(int orderByRequestId)
        {
            var orderByRequest = GetOrderByRequest(orderByRequestId);
            var offer = OfferService.GetOffer(orderByRequest.OfferId);

            var clsParam = new ClsMailParamOnSendFailureByRequest
            {
                OrderByRequestId = orderByRequest.OrderByRequestId.ToString(),
                UserName = orderByRequest.UserName,
                ArtNo = orderByRequest.ArtNo,
                ProductName = orderByRequest.ProductName,
                Quantity = orderByRequest.Quantity.ToString(),
                Color = offer != null && offer.Color != null ? offer.Color.ColorName : "",
                Size = offer != null && offer.Size != null ? offer.Size.SizeName : "",
            };

            string message = SendMail.BuildMail(clsParam);
            SendMail.SendMailNow(orderByRequest.Email, Resources.Resource.Admin_OrderByRequest_ImpossibleOrder, message, true);
        }
    }
}