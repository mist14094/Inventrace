//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

namespace AdvantShop.Modules.Interfaces
{
    public interface ISendMails : IModule
    {
        bool IsActive { get; }

        void SendMailsToReg(string title, string message);

        void SendMailsToNotReg(string title, string message);

        void SendMailsToAll(string title, string message);

        void SubscribeEmail(string email);

        void UnsubscribeEmail(string email);
    }
}