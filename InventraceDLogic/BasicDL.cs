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
            var connection = new SqlConnection(Constants.DefaultString);
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

        public string RemoveStore(string StoreID)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.RemoveStore,StoreID)

            };
            var adapter = new SqlDataAdapter(selectCommand);
            var connection = new SqlConnection(Constants.DefaultString);
            selectCommand.Connection = connection;

            try
            {
                connection.Open();
                adapter.Fill(dataTable: dataTable);
                return "";
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

        public string InsertStore(string storeName, string storeDesc, string addressLine1, string addressLine2, string city, string zipCode, string state, 
            bool isWareHouse, int storeManager, string phoneNumber, int managerId, int fromLocation, bool isActive, DateTime createdDate, DateTime modifieDate, int createdBy)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.InsertStore),
                CommandType = CommandType.StoredProcedure,

            };
            SqlParameter[] Param = new SqlParameter[]
            {
                new SqlParameter("@StoreName",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,storeName),
                new SqlParameter("@StoreDesc",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,storeDesc),
                new SqlParameter("@AddressLine1",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,addressLine1),
                new SqlParameter("@AddressLine2",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,addressLine2),
                new SqlParameter("@City",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,city),
                new SqlParameter("@State",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,state),
                new SqlParameter("@ZipCode",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,zipCode),
                new SqlParameter("@isWareHouse",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isWareHouse),
                new SqlParameter("@StoreManager",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,storeManager),
                new SqlParameter("@PhoneNumber",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,phoneNumber),
                new SqlParameter("@ManagerID",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,managerId),
                new SqlParameter("@FromLocation",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,fromLocation),
                new SqlParameter("@IsActive",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isActive),
                new SqlParameter("@CreatedDate",SqlDbType.DateTime,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,createdDate),
                new SqlParameter("@ModifiedDate",SqlDbType.DateTime,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,modifieDate),
                new SqlParameter("@CreatedBy",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,createdBy),

            };
            var adapter = new SqlDataAdapter(selectCommand);
            var connection = new SqlConnection(Constants.DefaultString);

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

        public string UpdateStore(int StoreID, string storeName, string storeDesc, string addressLine1, string addressLine2, string city, string zipCode, string state,
            bool isWareHouse, int storeManager, string phoneNumber, int managerId, int fromLocation, bool isActive, DateTime createdDate, DateTime modifieDate, int createdBy)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.UpdateStore),
                CommandType = CommandType.StoredProcedure,

            };
            SqlParameter[] Param = new SqlParameter[]
            {
                new SqlParameter("@StoreID",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,StoreID),
                new SqlParameter("@StoreName",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,storeName),
                new SqlParameter("@StoreDesc",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,storeDesc),
                new SqlParameter("@AddressLine1",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,addressLine1),
                new SqlParameter("@AddressLine2",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,addressLine2),
                new SqlParameter("@City",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,city),
                new SqlParameter("@State",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,state),
                new SqlParameter("@ZipCode",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,zipCode),
                new SqlParameter("@isWareHouse",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isWareHouse),
                new SqlParameter("@StoreManager",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,storeManager),
                new SqlParameter("@PhoneNumber",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,phoneNumber),
                new SqlParameter("@ManagerID",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,managerId),
                new SqlParameter("@FromLocation",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,fromLocation),
                new SqlParameter("@IsActive",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isActive),
                new SqlParameter("@CreatedDate",SqlDbType.DateTime,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,createdDate),
                new SqlParameter("@ModifiedDate",SqlDbType.DateTime,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,modifieDate),
                new SqlParameter("@CreatedBy",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,createdBy),

            };
            var adapter = new SqlDataAdapter(selectCommand);
            var connection = new SqlConnection(Constants.DefaultString);

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
