//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;

namespace AdvantShop.Customers
{
    [Serializable]
    public struct RecentlyViewRecord
    {
        public int ProductID { get; set; }

        public DateTime ViewTime { get; set; }

        public string ImgPath { get; set; }

        public float Price { get; set; }

        public string Name { get; set; }

        public string UrlPath { get; set; }

        public float  Discount { get; set; }
    }
}