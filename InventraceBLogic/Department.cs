using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InventraceDLogic;
using System.Data;

namespace InventraceBLogic
{
   public class Department
    {
        private static InventraceDLogic.BasicDL _dl = new BasicDL();

        public int DeptId { get; set; }
        public string DepartnamntName { get; set; }
        public string DepartnamntDesc { get; set; }
        public int StoreId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifieDate { get; set; }
        public int CreatedBy { get; set; }


        public Department()
        {
            DeptId = 0;
            DepartnamntName = "";
            DepartnamntDesc = "";
            StoreId = 1;
            IsActive = true;
            CreatedDate = DateTime.Now;
            ModifieDate = DateTime.Now;
            CreatedBy = 0;
        }

        public string AddDepartment(Department department)
        {
            return _dl.AddDepartment(department.DepartnamntName, department.DepartnamntDesc, department.StoreId, department.IsActive, department.CreatedDate, department.ModifieDate, department.CreatedBy);
        }

        public string RemoveDepartment(Department department)
        {

            return _dl.RemoveDepartment(department.DeptId);

        }

        public string UpdateDepartment(Department department)
        {
            return _dl.UpdateDepartment(department.DeptId, department.DepartnamntName, department.DepartnamntDesc, department.StoreId, department.IsActive, department.CreatedDate, department.ModifieDate, department.CreatedBy);
        }

        public List<Department> GetAllDepartments()
        {
            return GetDepartment(_dl.GetAllDepartment());
        }

        public List<Department> GetDepartmentById(int deptId)
        {
            return GetDepartment(_dl.GetDepartmentById(deptId));
        }

        private List<Department> GetDepartment(DataTable dt)
        {
            var list = dt.AsEnumerable()
                .Select(row => new Department()
                {
                    DeptId = Convert.ToInt16(row["DeptId"]),
                    DepartnamntName = row["DepartnamntName"].ToString(),
                    DepartnamntDesc = row["DepartnamntDesc"].ToString(),
                    StoreId = Convert.ToInt16(row["StoreID"]),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifieDate = Convert.ToDateTime(row["ModifieDate"]),
                    CreatedBy = Convert.ToInt16(row["CreatedBy"])
                }).ToList();
            return list;
        }




       
    }
}
