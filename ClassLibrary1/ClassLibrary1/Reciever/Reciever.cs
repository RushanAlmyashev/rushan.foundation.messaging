using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Foundation.Messaging.Reciever
{
    public class PublisherConsoleTest : IReciever
    {
        private readonly ConnectionFactory _connectionFactory;

        public PublisherConsoleTest()
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 49154,
                UserName = "guest",
                Password = "guest",
            };
        }

        public void Recieve()
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "Hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    Console.WriteLine($" [x] Recieved {message}");
                };

                channel.BasicConsume(queue: "Hello", autoAck: true, consumer: consumer);
            }
        }
    }
}
