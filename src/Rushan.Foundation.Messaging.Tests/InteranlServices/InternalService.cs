using Rushan.Foundation.Messaging.Tests.Messages;

namespace Rushan.Foundation.Messaging.Tests.InteranlServices
{
    internal class InternalService : IInternalService
    {
        public async Task MessageHandlerAsync(MessageOne message)
        {
            TestContext.WriteLine($"message.Id = {message.Id}");
            TestContext.WriteLine($"message.Name = {message.Name}");
            TestContext.WriteLine($"message.Key = {message.Key}");
            TestContext.WriteLine($"message.Value = {message.Value}");

            await Task.CompletedTask;
        }

        public async Task MessageHandlerAsync(MessageTwo message)
        {
            TestContext.WriteLine($"message.Id = {message.MessageObject.Id}");
            TestContext.WriteLine($"message.Name = {message.MessageObject.Name}");

            await Task.CompletedTask;
        }
    }
}
