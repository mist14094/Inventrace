//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Orders;
using System.Collections.Generic;

namespace AdvantShop.Modules.Interfaces
{
    public interface IOrderRenderIntoHtml : IModule
    {
        string DoRenderIntoFinalStep();

        string DoRenderIntoFinalStep(IOrder order);

        string DoRenderIntoFinalStep(IOrder order, IList<IOrderItem> items);
    }
}
