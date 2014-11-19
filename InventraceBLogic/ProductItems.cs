using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace InventraceBLogic
{
    class ProductItems
    {
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

        public void AddProductItems(ProductItems productItems)
        {

        }

        public void RemoveProductItems(ProductItems productItems)
        {

        }

        public void UpdateProductItems(ProductItems productItems)
        {

        }

        public List<ProductItems> GetProductItems()
        {
            return null;
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

