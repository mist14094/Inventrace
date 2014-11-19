//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------
namespace AdvantShop.Modules.Interfaces
{
    public interface IOrderConfirmation : IModule
    {
        bool IsActive { get; }
        string PageName { get; }
        string FileUserControlOrderConfirmation { get; }
    }
}