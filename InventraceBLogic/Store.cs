using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using InventraceDLogic;

namespace InventraceBLogic
{
    

    public class Store
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

        public Store()
        {
            StoreName = "";
            StoreDesc = "";
            AddressLine1 = "";
            AddressLine2 = "";
            City = "";
            State = "";
            ZipCode = "";
            IsWareHouse = false;
            StoreManager = 0;
            PhoneNumber = "";
            IsActive = true;
            FromLocation = 0;
            ManagerId = 0;
            CreatedDate = DateTime.Now;
            ModifieDate = DateTime.Now;
            CreatedBy = 0;
        }

        public string AddStore(Store store)
        {
            return _dl.InsertStore(store.StoreName, store.StoreDesc, store.AddressLine1, store.AddressLine2, store.City,
                store.ZipCode, store.State, store.IsWareHouse,
                store.StoreManager, store.PhoneNumber, store.ManagerId, store.FromLocation, store.IsActive,
                store.CreatedDate, store.ModifieDate, store.CreatedBy);
        }

        public string RemoveStore(Store store)
        {
            return _dl.RemoveStore(store.StoreId.ToString(CultureInfo.InvariantCulture));

        }

        public string UpdateStore(Store store)
        {
            return _dl.UpdateStore(store.StoreId, store.StoreName, store.StoreDesc, store.AddressLine1,
                store.AddressLine2, store.City,
                store.ZipCode, store.State, store.IsWareHouse,
                store.StoreManager, store.PhoneNumber, store.ManagerId, store.FromLocation, store.IsActive,
                store.CreatedDate, store.ModifieDate, store.CreatedBy);
        }

        public List<Store> GetStores()
        {

            return GetStore(_dl.GetAllStores());
        }


        private  List<Store> GetStore(DataTable dt)
        {
             var list = dt.AsEnumerable()
                .Select(row => new Store()
                {
                    StoreId = Convert.ToInt16(row["StoreID"]),
                    StoreName =  row["StoreName"].ToString(),
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
