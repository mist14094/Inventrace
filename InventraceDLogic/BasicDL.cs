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

        public DataTable GetAllStores()
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.GetAllStores)

            };
            var adapter = new SqlDataAdapter(selectCommand);
            var connection = new SqlConnection();
            selectCommand.Connection = connection;

            try
            {
                connection.Open();
                adapter.Fill(dataTable: dataTable);
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


        public string InsertStore(int storeId, string storeName, string storeDesc, string addressLine1, string addressLine2, string city, string zipCode, string state, 
            bool isWareHouse, int storeManager, string phoneNumber, int managerId, int fromLocation, bool isActive, DateTime createdDate, DateTime modifieDate, int createdBy)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(Constants.InsertMainChartConfiguration),
                CommandType = CommandType.StoredProcedure,

            };
            SqlParameter[] Param = new SqlParameter[]
            {
                new SqlParameter("@ChartPrimaryHeader",SqlDbType.VarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,ChartPrimaryHeader),
                new SqlParameter("@ChartSecondaryHeader",SqlDbType.VarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,ChartSecondaryHeader),
                new SqlParameter("@AllowMultipleSelection",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,AllowMultipleSelection),
                new SqlParameter("@ExportOptionsExporttoImage",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,ExportOptionsExporttoImage),
                new SqlParameter("@ExportOptionsAllowPrint",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,ExportOptionsAllowPrint),
                new SqlParameter("@Height",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,Height),
                new SqlParameter("@HeightMode",SqlDbType.VarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,HeightMode),
                new SqlParameter("@IsInverted",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,IsInverted),
                new SqlParameter("@Width",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,Width),
                new SqlParameter("@WidthMode",SqlDbType.VarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,WidthMode),
                new SqlParameter("@ZoomMode",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,ZoomMode),
                new SqlParameter("@AxisMarkersEnabled",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,AxisMarkersEnabled),
                new SqlParameter("@AxisMarkersMode",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,AxisMarkersMode),
                new SqlParameter("@AxisMarkersWidth",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,AxisMarkersWidth),
                new SqlParameter("@TooltipSettingsChartBound",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,TooltipSettingsChartBound),
                new SqlParameter("@ModifiedDate",SqlDbType.DateTime,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,ModifiedDate),

            };
            var adapter = new SqlDataAdapter(selectCommand);
            var connection = new SqlConnection(Constants.DefaultConnectionString);

            selectCommand.Connection = connection;
            foreach (SqlParameter parameter in Param)
            {
                selectCommand.Parameters.Add(parameter);
            }
            try
            {
                connection.Open();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    return dataTable.Rows[0][0].ToString();
                }
                else
                {
                    return "";
                }

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
