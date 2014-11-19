using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using InventraceConstants;
using NLog;

namespace InventraceDLogic
{
    public class BasicDL
    {
        internal Logger Nlog = LogManager.GetCurrentClassLogger();
         InventraceConstants.Constants _constants = new Constants();

       public DataTable GetAllDataBaseConnection()
       {
           Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
           var dataTable = new DataTable();
           var selectCommand = new SqlCommand
           {
               CommandText = string.Format(_constants.GetConnectionString)

           };
           var adapter = new SqlDataAdapter(selectCommand);
           var connection = new SqlConnection();
           selectCommand.Connection = connection;

           try
           {
               connection.Open();
               adapter.Fill(dataTable);
               return dataTable;
           }
           catch (Exception ex)
           {
               Nlog.Trace(
                   this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                   System.Reflection.MethodBase.GetCurrentMethod().Name + "::Error", ex);
               throw ex;
           }
           finally
           {
               connection.Close();
               connection.Dispose();
               Nlog.Trace(message:
                   this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" +
                   System.Reflection.MethodBase.GetCurrentMethod().Name + "::Leaving");
           }
       }
    }
}
