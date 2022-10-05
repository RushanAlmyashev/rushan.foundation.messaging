using Rushan.Foundation.Messaging.Integration.Tests.Application.Messages;

namespace Rushan.Foundation.Messaging.Integration.Tests.Application.InteranlServices
{
    internal class InternalService : IInternalService
    {
        private static List<MessageOne> _messageOnes = new();
        private static List<MessageTwo> _messageTwos = new();
        private static List<MessageThree> _messageThrees = new();

        public List<MessageOne> MessageOnes => _messageOnes;
        public List<MessageTwo> MessageTwos => _messageTwos;
        public List<MessageThree> MessageThrees => _messageThrees;

        public async Task MessageHandlerAsync(MessageOne message)
        {
            _messageOnes.Add(message);

            await Task.CompletedTask;
        }

        public async Task MessageHandlerAsync(MessageTwo message)
        {
            _messageTwos.Add(message);

            await Task.CompletedTask;
        }

        public async Task MessageHandlerAsync(MessageThree message)
        {
            _messageThrees.Add(message);

            await Task.CompletedTask;
        }
    }
}
