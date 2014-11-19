//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;

namespace AdvantShop.Modules.Interfaces
{
    public interface IUserControlinSc
    {
        List<int> ProductIds { get; set; }

        bool HasProducts { get; set; }
    }
}
