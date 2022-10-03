using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rushan.Foundation.Messaging.Channel;
using Rushan.Foundation.Messaging.Integration.Tests.Utils;
using Rushan.Foundation.Messaging.Logger;
using Rushan.Foundation.Messaging.Persistence;
using System.Text;

namespace Rushan.Foundation.Messaging.Integration.Tests.Chanel
{
    [Category("Integration")]    
    [NonParallelizable]
    public class ChannelFactoryTests
    {
        private IChannelFactory _target;

        private IRabbitMQConnection _rabbitMQConnection;
        private Mock<ILogger> _logger;        

        [OneTimeSetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger>();
            var connectionString = BrokerHelper.GetConnectionString();
            ushort qos = 5;

            _rabbitMQConnection = new RabbitMQConnectionPersistence(connectionString, _logger.Object);
            _rabbitMQConnection.Start();
            _target = new ChannelFactory(_rabbitMQConnection, qos);
        }

        [TearDown]
        public void TearDown()
        {
            _rabbitMQConnection.Stop();
        }

        [Test]
        public async Task WhenSendMessageViaChanel_RecievingEqualAsync()
        {
            var message = "Hello Istanbul";
            var actualMessage = string.Empty;

            using (var channel = _target.GetRabbitMQChannel())
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

            using (var channel = _target.GetRabbitMQChannel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (model, ea)  =>
                {
                    var body = ea.Body.ToArray();
                    actualMessage = Encoding.UTF8.GetString(body);           
                };

                channel.BasicConsume(queue: "hello",
                                     autoAck: true,
                                     consumer: consumer);


                await Task.Delay(DockerServiceHelper.AssertionBrokerDelayMs);                
            }

            Assert.True(actualMessage.Equals(message));
        }
    }
}
