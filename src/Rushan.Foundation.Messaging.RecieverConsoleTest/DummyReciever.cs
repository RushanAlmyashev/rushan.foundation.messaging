using Rushan.Foundation.Messaging.Recieve;
using Rushan.Foundation.Messaging.Shared;
using System;
using System.Threading.Tasks;

namespace Rushan.Foundation.Messaging.RecieverConsoleTest
{
    public class DummyReciever : IMessageReceiver<Dummy>
    {
        public async Task ReceiveMessageAsync(Dummy message)
        {            
            Console.WriteLine($"message.Id = {message.Id}");
            Console.WriteLine($"message.Name = {message.Name}");
            Console.WriteLine($"message.Key = {message.Key}");
            Console.WriteLine($"message.Value = {message.Value}");            
        }
    }
}
