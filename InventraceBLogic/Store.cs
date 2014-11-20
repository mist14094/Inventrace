using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using InventraceDLogic;

namespace InventraceBLogic
{
    

    class Store
    {

        private static InventraceDLogic.BasicDL _dl = new BasicDL();

        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreDesc { get; set; }
        public string   AddressLine1{ get; set; }
        public string   AddressLine2 { get; set; }
        public string   City{ get; set; }
        public string   State { get; set; }
        public string   ZipCode{ get; set; }
        public bool IsWareHouse{ get; set; }
        public int StoreManager { get; set; }
        public string PhoneNumber { get; set; }
        public int ManagerId { get; set; }
        public int FromLocation{ get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifieDate { get; set; }
        public int CreatedBy { get; set; }

        
        public void AddStore(Store store)
        {

        }

        public void RemoveStore(Store store)
        {
            
        }

        public void UpdateStore(Store store)
        {
            
        }

        public List<Store> GetStores()
        {

            return GetStore(_dl.GetAllStores());
        }

        private  List<Store> GetStore(DataTable dt)
        {
            var st = new Store();
            var list = dt.AsEnumerable()
                .Select(row => new Store()
                {
                    StoreId = Convert.ToInt16(row["StoreID"]),
                    StoreDesc = row["StoreDesc"].ToString(),
                    AddressLine1 = row["AddressLine1"].ToString(),
                    AddressLine2 = row["AddressLine2"].ToString(),
                    City = row["City"].ToString(),
                    State = row["State"].ToString(),
                    ZipCode = row["ZipCode"].ToString(),
                    IsWareHouse= Convert.ToBoolean(row["isWareHouse"].ToString()),
                    StoreManager =Convert.ToInt16(row["StoreManager"].ToString()),
                    PhoneNumber = row["PhoneNumber"].ToString(),
                    ManagerId = Convert.ToInt16(row["ManagerID"].ToString()),
                    FromLocation = Convert.ToInt16(row["FromLocation"]),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifieDate = Convert.ToDateTime(row["ModifiedDate"]),
                    CreatedBy = Convert.ToInt16(row["CreatedBy"])
                }).ToList();
            return list;
        }

    }
}
