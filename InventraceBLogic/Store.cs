using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventraceBLogic
{
    class Store
    {

        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreDesc { get; set; }
        public string   AddressLine1{ get; set; }
        public string   AddressLine2 { get; set; }
        public string   City{ get; set; }
        public string   State { get; set; }
        public string   ZipCode{ get; set; }
        public string IsWareHouse{ get; set; }
        public string StoreManager { get; set; }
        public string PhoneNumber { get; set; }
        public string ManagerId { get; set; }
        public string FromLocation{ get; set; }
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
            return null;
        }

    }
}
