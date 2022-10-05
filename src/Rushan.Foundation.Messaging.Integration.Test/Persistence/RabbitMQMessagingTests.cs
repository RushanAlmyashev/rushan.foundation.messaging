using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rushan.Foundation.Messaging.Integration.Tests.RabbitMQDockering;
using Rushan.Foundation.Messaging.Logger;
using Rushan.Foundation.Messaging.Persistence;
using System.Text;

namespace Rushan.Foundation.Messaging.Integration.Tests.Persistence
{
    [Category("Integration")]
    [NonParallelizable]
    public class RabbitMQMessagingTests
    {       
        private IRabbitMQConnection _target;
        private ILogger _logger;

        [OneTimeSetUp]
        public void Setup()
        {
            _logger = new EmptyLogger();
            var connectionString = BrokerHelper.GetConnectionString();

            _target = new RabbitMQConnectionPersistence(connectionString, _logger);
            _target.Connect();           
        }

        [TearDown]
        public void TearDown()
        {
            _target.Disconnect();
        }

        [Test]
        public async Task WhenSendMessageViaChanel_RecievingEqualAsync()
        {
            var message = "Hello Istanbul";
            var actualMessage = string.Empty;

            using (var channel = _target.GetConnection().CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                 routingKey: "hello",
                                 basicProperties: null,
                                 body: body);
            }

            using (var channel = _target.GetConnection().CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    actualMessage = Encoding.UTF8.GetString(body);
                };

                channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

                await Task.Delay(DockerServiceHelper.AssertionBrokerDelayMs);
            }

            Assert.True(actualMessage.Equals(message));
        }
    }
}
