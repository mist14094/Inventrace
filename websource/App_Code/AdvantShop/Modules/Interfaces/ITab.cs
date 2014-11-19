//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Modules.Interfaces
{
    public interface ITab
    {
        int TabTitleId { get; set; }

        int TabBodyId { get; set; }

        string Title { get; set; }

        string Body { get; set; }

        string TabGroup { get; set; }

        int ProductId { get; set; }

        int SortOrder { get; set; }
    }
}
