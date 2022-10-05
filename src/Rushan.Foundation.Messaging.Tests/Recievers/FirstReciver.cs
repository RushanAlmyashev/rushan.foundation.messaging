using Rushan.Foundation.Messaging.Tests.InteranlServices;
using Rushan.Foundation.Messaging.Tests.Messages;

namespace Rushan.Foundation.Messaging.Tests.Recievers
{
    public class FirstReciver : IMessageReceiver<MessageOne>,
        IMessageReceiver<MessageTwo>
    {
        private readonly IInternalService _internalService;

        public FirstReciver(IInternalService internalService)
        {
            _internalService = internalService;
        }

        public async Task ReceiveMessageAsync(MessageOne message)
        {
            TestContext.WriteLine($"recieved message {nameof(MessageOne)}");

            await _internalService.MessageHandlerAsync(message);
        }


        public async Task ReceiveMessageAsync(MessageTwo message)
        {
            TestContext.WriteLine($"recieved message {nameof(MessageTwo)}");

            await _internalService.MessageHandlerAsync(message);
        }
    }
}
