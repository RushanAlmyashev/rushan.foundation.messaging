using RabbitMQ.Client;
using System;
using System.Text;

namespace Foundation.Messaging.Publisher
{
    public class Publisher : IPublisher
    {
        private readonly ConnectionFactory _connectionFactory;

        public Publisher()
        {
            _connectionFactory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 49154,
                UserName = "guest",
                Password = "guest",
            };
        }

        public void Publish()
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var chanelModel = connection.CreateModel())
            {
                chanelModel.QueueDeclare(queue: "Hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                string message = "Hello world!";

                var body = Encoding.UTF8.GetBytes(message);

                chanelModel.BasicPublish(exchange: "", routingKey: "Hello", basicProperties: null, body: body);

                Console.WriteLine($" [x] Sent {message}");
            }
        }
    }
}
