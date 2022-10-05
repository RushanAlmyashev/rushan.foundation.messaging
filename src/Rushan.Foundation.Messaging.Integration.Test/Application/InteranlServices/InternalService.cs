using Rushan.Foundation.Messaging.Integration.Tests.Application.Messages;

namespace Rushan.Foundation.Messaging.Integration.Tests.Application.InteranlServices
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

        public async Task MessageHandlerAsync(MessageThree message)
        {
            TestContext.WriteLine($"message.Id = {message.Id}");
            TestContext.WriteLine($"message.Name = {message.Name}");
            TestContext.WriteLine($"message.Age = {message.Age}");
            TestContext.WriteLine($"message.Date = {message.Date}");
            TestContext.WriteLine($"message.Value = {message.Value}");

            await Task.CompletedTask;
        }
    }
}
