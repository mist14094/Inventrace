using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Data;
using InventraceDLogic;

namespace InventraceBLogic
{
    class ProductItems
    {
        InventraceDLogic.BasicDL _dl = new BasicDL();

        public int ProductId { get; set; }
        public int ProductItemId { get; set; }
        public int RFID { get; set; }
        public Status ProductStatus { get; set; }
        public int ZoneId { get; set; }
        public bool HasExitRead { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifieDate { get; set; }
        public int CreatedBy { get; set; }
        public bool IsPrinted { get; set; }
        public bool IsRFIDItem { get; set; }

        public ProductItems()
        {
            ProductId = 0;
            ProductItemId = 0;
            RFID = 0;
            ProductStatus =(Status)1;
            ZoneId = 1;
            HasExitRead = true;
            IsActive = true;
            CreatedDate = DateTime.Now;
            ModifieDate = DateTime.Now;
            CreatedBy = 0;
            IsRFIDItem = true;
            IsPrinted = true;
        }


        public string AddProductItems(ProductItems pItems)
        {
            return _dl.InsertProductItem(pItems.ProductId, pItems.RFID,Convert.ToInt16((pItems.ProductStatus).ToString()), pItems.ZoneId, pItems.HasExitRead, pItems.IsActive, pItems.CreatedDate, pItems.ModifieDate, pItems.CreatedBy, pItems.IsRFIDItem, pItems.IsPrinted);
        }

        public string RemoveProductItem(ProductItems pItems)
        {

            return _dl.RemoveProductItem(pItems.ProductItemId);

        }

        public string UpdateProductItem(ProductItems pItems)
        {
            return _dl.UpdateProductItem(pItems.ProductId, pItems.ProductItemId, pItems.RFID, Convert.ToInt16((pItems.ProductStatus).ToString()), pItems.ZoneId, pItems.HasExitRead, pItems.IsActive, pItems.CreatedDate, pItems.ModifieDate, pItems.CreatedBy, pItems.IsRFIDItem, pItems.IsPrinted);
        }

        public List<ProductItems> GetAllProductItems()
        {
            return GetProductItem(_dl.GetAllProductItems());
        }

        public List<ProductItems> GetProductItemById(int pItemsId)
        {
            return GetProductItem(_dl.GetProductItemById(pItemsId));
        }

        private List<ProductItems> GetProductItem(DataTable dt)
        {
            var list = dt.AsEnumerable()
                .Select(row => new ProductItems()
                {
                    ProductId = Convert.ToInt16(row["ProductId"]),
                    ProductItemId = Convert.ToInt16(row["ProductItemId"]),
                    RFID = Convert.ToInt16(row["RFID"]),
                    ProductStatus = (Status)Convert.ToInt16(row["ProductStatus"]),
                    ZoneId = Convert.ToInt16(row["ZoneId"]),
                    HasExitRead = Convert.ToBoolean(row["HasExitRead"]),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                    ModifieDate = Convert.ToDateTime(row["ModifiedDate"]),
                    CreatedBy = Convert.ToInt16(row["CreatedBy"]),
                     IsPrinted = Convert.ToBoolean(row["IsPrinted"]),
                    IsRFIDItem = Convert.ToBoolean(row["IsRFIDItem"])
                    
                }).ToList();
            return list;
        }


    }

        public enum Status
        {
            [Description("New")]
            New = 1,
            [Description("Hold")]
            Hold= 2,
            [Description("Sold")]
            Sold = 3,
            [Description("Transfer")]
            Transfer = 4
         }





}

