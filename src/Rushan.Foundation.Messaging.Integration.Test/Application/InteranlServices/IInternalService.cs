using Rushan.Foundation.Messaging.Integration.Tests.Application.Messages;

namespace Rushan.Foundation.Messaging.Integration.Tests.Application.InteranlServices
{
    public interface IInternalService
    {
        Task MessageHandlerAsync(MessageOne message);

        Task MessageHandlerAsync(MessageTwo message);

        Task MessageHandlerAsync(MessageThree message);
    }
}