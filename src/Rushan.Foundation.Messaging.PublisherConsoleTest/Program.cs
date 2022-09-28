
using Rushan.Foundation.Messaging;
using Rushan.Foundation.Messaging.Configuration;
using Rushan.Foundation.Messaging.Shared;
using System;

namespace PublisherConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new MessagingConfiguration
            {
                Exchange = "amq.topic",
                MessageBrokerUri = "amqp://guest:guest@localhost:5672",
            };

            var rabbitmq = new RabbitMqMessageBus(configuration);
            rabbitmq.StartMessageBus();

            var dummyMessage = new Dummy
            {
                Id = 5,
                Key = Guid.NewGuid(),
                Name = "Hello Istambul!",
                Value = DateTime.UtcNow
            };

            rabbitmq.Publish(dummyMessage);

            Console.WriteLine(" Press [enter] to exit.");
            
            Console.ReadLine();
        }
    }
}
