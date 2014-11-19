//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Taxes
{
    public class TaxValue
    {
        public string TaxName { get; set; }
        public float TaxSum { get; set; }
        public int TaxID { get; set; }
        public bool TaxShowInPrice { get; set; }
    }
}