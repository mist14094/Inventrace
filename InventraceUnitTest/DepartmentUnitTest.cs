using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InventraceBLogic;

namespace InventraceUnitTest
{
    /// <summary>
    /// Summary description for DepartmentUnitTest
    /// </summary>
    [TestClass]
    public class DepartmentUnitTest
    {
        private DateTime CreatedDate = DateTime.Now;
        private DateTime ModifieDate = DateTime.Now;
        private List<Department> addDepartments;
        private string deptId;

        [TestMethod]
        public void AddDepartment()
        {
            Department dept = new Department();

            dept.DepartnamntName = "Watches";
            dept.DepartnamntDesc = "All type of Watches for Both gents and ladies";
            dept.CreatedDate = CreatedDate;
            dept.ModifieDate = ModifieDate;
            dept.DeptId = 1;
            dept.StoreId = 1;

            string deptId = dept.AddDepartment(dept);
            
            //List<Store> dfList = bl.GetStores();
        }

        [TestMethod]
        public void SelectAllDepartments()
        {
            Department dept = new Department();
             addDepartments = dept.GetAllDepartments();

            //List<Store> dfList = bl.GetStores();
        }

        [TestMethod]
        public void SelectDepartmentByID()
        {
            Department dept = new Department();
            dept.DeptId = Convert.ToInt16(deptId);
            addDepartments = dept.GetDepartmentById(dept.DeptId);

            //List<Store> dfList = bl.GetStores();
        }
    }
}
