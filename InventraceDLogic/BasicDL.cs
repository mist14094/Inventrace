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

        #region Store DAL
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
                CommandText = string.Format(_constants.RemoveStore, StoreID)

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

        
        #endregion
        
        #region Department DAL

        public string AddDepartment(string departnamntName, string departnamntDesc, int storeId, bool isActive, DateTime createdDate, DateTime modifieDate, int createdBy)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.InsertDepartment),
                CommandType = CommandType.StoredProcedure,

            };
            SqlParameter[] Param = new SqlParameter[]
            {
                new SqlParameter("@DepartnamntName",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,departnamntName),
                new SqlParameter("@DepartnamntDesc",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,departnamntDesc),
                new SqlParameter("@StoreId",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,storeId),
                new SqlParameter("@CreatedDate",SqlDbType.DateTime,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,createdDate),
                new SqlParameter("@ModifieDate",SqlDbType.DateTime,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,modifieDate),
                new SqlParameter("@CreatedBy",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,createdBy),
                new SqlParameter("@IsActive",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isActive),

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

        public string UpdateDepartment(int deptId, string departnamntName, string departnamntDesc, int storeId, bool isActive, DateTime createdDate, DateTime modifieDate, int createdBy)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.UpdateDepartment),
                CommandType = CommandType.StoredProcedure,

            };
            SqlParameter[] Param = new SqlParameter[]
            {
                new SqlParameter("@DeptId",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,deptId),
                new SqlParameter("@DepartnamntName",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,departnamntName),
                new SqlParameter("@DepartnamntDesc",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,departnamntDesc),
                new SqlParameter("@StoreId",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,storeId),
                new SqlParameter("@CreatedDate",SqlDbType.DateTime,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,createdDate),
                new SqlParameter("@ModifieDate",SqlDbType.DateTime,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,modifieDate),
                new SqlParameter("@CreatedBy",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,createdBy),
                new SqlParameter("@IsActive",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isActive),

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

        public DataTable GetDepartmentById(int deptId)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.GetDepartmentById, deptId)

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

        public DataTable GetAllDepartment()
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = _constants.GetAllDepartments

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

        public string RemoveDepartment(int deptId)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.RemoveDepartment),
                CommandType = CommandType.StoredProcedure,

            };
            SqlParameter[] Param = new SqlParameter[]
            {
                new SqlParameter("@DeptId",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,deptId),

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
        
        #endregion

        #region Zone DAL
        public DataTable GetAllZones()
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.GetAllZone)

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
        public DataTable GetZoneById(int zoneId)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.GetZoneById, zoneId)

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
        public string RemoveZone(int zoneId)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.RemoveZone, zoneId)

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
        public string InsertZone(string zoneName, string zoneDesc, int departmentId, bool isExitZone, bool enableShiftInventory, DateTime createdDate, DateTime modifieDate, int createdBy)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.InsertZone),
                CommandType = CommandType.StoredProcedure,

            };
            SqlParameter[] Param = new SqlParameter[]
            {
                new SqlParameter("@ZoneName",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,zoneName),
                new SqlParameter("@ZoneDesc",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,zoneDesc),
                new SqlParameter("@DepartmentId",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,departmentId),
                new SqlParameter("@IsExitZone",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isExitZone),
                new SqlParameter("@EnableShiftInventory",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,enableShiftInventory),
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
        public string UpdateZone(int zoneId, string zoneName, string zoneDesc, int departmentId, bool isExitZone, bool enableShiftInventory, DateTime createdDate, DateTime modifieDate, int createdBy)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.UpdateZone),
                CommandType = CommandType.StoredProcedure,

            };
            SqlParameter[] Param = new SqlParameter[]
            {
                new SqlParameter("@ZoneId",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,zoneId),
                new SqlParameter("@ZoneName",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,zoneName),
                new SqlParameter("@ZoneDesc",SqlDbType.NVarChar,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,zoneDesc),
                new SqlParameter("@DepartmentId",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,departmentId),
                new SqlParameter("@IsExitZone",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isExitZone),
                new SqlParameter("@EnableShiftInventory",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,enableShiftInventory),
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


        #endregion

        #region Product Item DAL

        public DataTable GetAllProductItems()
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.GetAllProductItems)

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

        public DataTable GetProductItemById(int productItemId)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.GetProductItemById, productItemId)

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


        public string RemoveProductItem(int productItemId)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.RemoveProductItem, productItemId)

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

        public string InsertProductItem(int productId, int rfid, int productStatus, int zoneId, bool hasExitRead, bool isActive, DateTime createdDate, DateTime modifieDate, int createdBy, bool isRfidItem, bool isPrinted)
        {
            Nlog.Trace(message: this.GetType().Namespace + ":" + MethodBase.GetCurrentMethod().DeclaringType.Name + ":" + System.Reflection.MethodBase.GetCurrentMethod().Name + "::Entering");
            var dataTable = new DataTable();
            var selectCommand = new SqlCommand
            {
                CommandText = string.Format(_constants.InsertProductItem),
                CommandType = CommandType.StoredProcedure,

            };
            SqlParameter[] Param = new SqlParameter[]
            {
                new SqlParameter("@ProductId",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,productId),
                new SqlParameter("@RFID",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,rfid),
                new SqlParameter("@ProductStatus",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,productStatus),
                new SqlParameter("@ZoneId",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,zoneId),
                new SqlParameter("@HasExitRead",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,hasExitRead),
                new SqlParameter("@IsActive",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isActive),
                new SqlParameter("@CreatedDate",SqlDbType.DateTime,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,createdDate),
                new SqlParameter("@ModifiedDate",SqlDbType.DateTime,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,modifieDate),
                new SqlParameter("@CreatedBy",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,createdBy),
                new SqlParameter("@IsPrinted",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isPrinted),
                new SqlParameter("@IsRFIDItem",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isRfidItem),
              

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

        public string UpdateProductItem(int productId, int productItemId, int rfid, int productStatus, int zoneId, bool hasExitRead, bool isActive, DateTime createdDate, DateTime modifieDate, int createdBy, bool isRfidItem, bool isPrinted)
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
                new SqlParameter("@ProductItemId",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,productItemId),
                new SqlParameter("@ProductId",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,productId),
                new SqlParameter("@RFID",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,rfid),
                new SqlParameter("@ProductStatus",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,productStatus),
                new SqlParameter("@ZoneId",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,zoneId),
                new SqlParameter("@HasExitRead",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,hasExitRead),
                new SqlParameter("@IsActive",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isActive),
                new SqlParameter("@CreatedDate",SqlDbType.DateTime,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,createdDate),
                new SqlParameter("@ModifiedDate",SqlDbType.DateTime,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,modifieDate),
                new SqlParameter("@CreatedBy",SqlDbType.Int,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,createdBy),
                new SqlParameter("@IsPrinted",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isPrinted),
                new SqlParameter("@IsRFIDItem",SqlDbType.Bit,50,ParameterDirection.Input,false,10,0,"",DataRowVersion.Proposed,isRfidItem),
              
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


        #endregion
    }
}
