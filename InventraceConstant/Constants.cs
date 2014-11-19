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


        public string GetAllDataBaseConnection =
            "SELECT [Sno] ,[DataBaseName] ,[ConnectionString] ,[EquivalentString] ,[Enabled] ,[DateCreated]  FROM [Jarvis].[dbo].[DataBaseInstanceNames]";

        public string GetConnectionString =
            "SELECT [ConnectionString]  FROM [Jarvis].[dbo].[DataBaseInstanceNames] WHERE Sno={0}";


    }
}
