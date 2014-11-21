using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace InventraceConstants
{
    public class Constants
    {
        public static string DefaultString = ConfigurationManager.ConnectionStrings["AdvantConnectionString"].ConnectionString;
        public string DefaultConnectionString
        {
            get
            {
                return DefaultString;
            }
            set
            {
                DefaultString = value;
            }
        }


        public string GetAllStores =
            "SELECT [StoreID] ,[StoreName] ,[StoreDesc] ,[AddressLine1] ,[AddressLine2] ,[City] ,[State] ,[ZipCode] ,[isWareHouse] ,[StoreManager] ,[PhoneNumber] ,[ManagerID] ,[FromLocation] ,[IsActive] ,[CreatedDate] ,[ModifiedDate] ,[CreatedBy]  FROM dbo.stores";

        public string InsertStore =
            "AddStore";

        public string UpdateStore =
            "UpdateStore";
        public string RemoveStore =
            "delete FROM dbo.stores where StoreID={0}";

        #region Department Query
        
        public string GetAllDepartments =
           "SELECT [DeptId], [DepartnamntName], [DepartnamntDesc], [StoreId], [IsActive], [CreatedDate], [ModifieDate], [CreatedBy] FROM [dbo].[INV_Department]";

        public string GetDepartmentById =
          "SELECT [DeptId], [DepartnamntName], [DepartnamntDesc], [StoreId], [IsActive], [CreatedDate], [ModifieDate], [CreatedBy] FROM [dbo].[INV_Department] WHERE DeptId = {0}";

        public string InsertDepartment =
            "INV_sp_InsertDepartment";

        public string UpdateDepartment =
            "INV_sp_UpdateDepartment";

        public string RemoveDepartment =
            "INV_sp_DeleteDepartment";

        #endregion

        #region Zone Query

        public string GetAllZone =
           "SELECT [ZoneId], [ZoneName], [ZoneDesc], [DepartmentId], [IsExitZone], [EnableShiftInventory], [CreatedDate], [ModifieDate], [CreatedBy]  FROM [dbo].[INV_Zone]";

        public string GetZoneById =
          "SELECT [ZoneId], [ZoneName], [ZoneDesc], [DepartmentId], [IsExitZone], [EnableShiftInventory], [CreatedDate], [ModifieDate], [CreatedBy]  FROM [dbo].[INV_Zone] WHERE ZoneId = {0}";

        public string InsertZone =
            "INV_sp_InsertZone";

        public string UpdateZone =
            "INV_sp_UpdateZone";

        public string RemoveZone =
            "DELETE FROM [dbo].[INV_Zone] WHERE ZoneId = {0}";

        #endregion

        #region ProductItem Query

        public string GetAllProductItems =
           "SELECT [ProductId], [ProductItemId], [RFID], [ProductStatus], [ZoneId], [HasExitRead], [IsActive], [CreatedDate], [ModifieDate], [CreatedBy], [IsPrinted], [IsRFIDItem]  FROM [dbo].[INV_ProductItem]";

        public string GetProductItemById =
          "SELECT [ProductId], [ProductItemId], [RFID], [ProductStatus], [ZoneId], [HasExitRead], [IsActive], [CreatedDate], [ModifieDate], [CreatedBy], [IsPrinted], [IsRFIDItem]  FROM [dbo].[INV_ProductItem] WHERE ProductItemId = {0}";

        public string InsertProductItem =
            "INV_sp_InsertProductItem";

        public string UpdateProductItem =
            "INV_sp_UpdateProductItem";

        public string RemoveProductItem =
            "DELETE FROM [dbo].[INV_ProductItem] WHERE ProductItemId = {0}";

        #endregion

    }
}
