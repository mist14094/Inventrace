using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventraceBLogic
{
    class Department
    {

        public int DeptId { get; set; }
        public string DepartnamntName { get; set; }
        public string DepartnamntDesc { get; set; }
        public int StoreId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifieDate { get; set; }
        public int CreatedBy { get; set; }

        public void AddDepartMent(Department department)
        {

        }

        public void RemoveStore(Department department)
        {
            
        }

        public void UpdateStore(Department department)
        {
            
        }

        public List<Department> GetDepartments()
        {
            return null;
        }

    }
}
