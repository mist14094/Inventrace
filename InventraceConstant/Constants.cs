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
            "SELECT [StoreID] ,[StoreName] ,[StoreDesc] ,[AddressLine1] ,[AddressLine2] ,[City] ,[State] ,[ZipCode] ,[isWareHouse] ,[StoreManager] ,[PhoneNumber] ,[ManagerID] ,[FromLocation] ,[IsActive] ,[CreatedDate] ,[ModifiedDate] ,[CreatedBy]  FROM newadvantdb.dbo.stores";

        public string InsertStore =
            "InsertStore";

        public string UpdateStore =
            "UpdateStore";

        public string RemoveStore =
            "delete FROM newadvantdb.dbo.stores where StoreID={0}";

    }
}
