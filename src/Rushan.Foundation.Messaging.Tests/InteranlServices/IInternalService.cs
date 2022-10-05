using Rushan.Foundation.Messaging.Tests.Messages;

namespace Rushan.Foundation.Messaging.Tests.InteranlServices
{
    public interface IInternalService
    {
        Task MessageHandlerAsync(MessageOne message);

        Task MessageHandlerAsync(MessageTwo message);
    }
}