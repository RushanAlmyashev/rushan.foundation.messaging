using Rushan.Foundation.Messaging.Integration.Tests.Application.InteranlServices;
using Rushan.Foundation.Messaging.Integration.Tests.Application.Messages;

namespace Rushan.Foundation.Messaging.Integration.Tests.Application.Recievers
{
    public class SecondReciever : IMessageReceiver<MessageThree>
    {
        private readonly IInternalService _internalService;

        public SecondReciever(IInternalService internalService)
        {
            _internalService = internalService;
        }

        public async Task ReceiveMessageAsync(MessageThree message)
        {
            TestContext.WriteLine($"recieved message {nameof(MessageThree)}");

            await _internalService.MessageHandlerAsync(message);
        }
    }
}
