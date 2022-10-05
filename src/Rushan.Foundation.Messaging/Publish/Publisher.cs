using RabbitMQ.Client;
using Rushan.Foundation.Messaging.Enums;
using Rushan.Foundation.Messaging.Helpers;
using Rushan.Foundation.Messaging.Logger;
using Rushan.Foundation.Messaging.Persistence;
using Rushan.Foundation.Messaging.Serialization;
using System;

namespace Rushan.Foundation.Messaging.Publish
{
    internal class Publisher : IPublisher
    {
        private const byte DELIVERY_MODE = (byte)DeliveryMode.Persistent;

        private readonly IRabbitMQConnection _rabbitMQConnection;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;
        private readonly string _exchange;

        public Publisher(IRabbitMQConnection rabbitMQConnection,
            ISerializer serializer,            
            ILogger logger,
            string exchange)
        {
            _rabbitMQConnection = rabbitMQConnection;
            _serializer = serializer;
            _exchange = exchange;
            _logger = logger;
        }

        public void Publish<TMessage>(TMessage message)
        {
            using (var channel = _rabbitMQConnection.GetConnection().CreateModel())
            {
                var routingKey = message.GetType().FullName.ToLowerInvariant();

                var properties = channel.CreateBasicProperties();

                properties.ContentType = _serializer.ContentType;
                properties.DeliveryMode = DELIVERY_MODE;
                properties.Type = message.GetType().FullName;
                properties.AppId = ApplicationHelper.GetApplicationName();
                properties.CorrelationId = Guid.NewGuid().ToString();

                try
                {
                    var serializedMessage = _serializer.Serialize(message);

                    channel.BasicPublish(_exchange, routingKey, properties, serializedMessage);
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"An error occured on message publish: '{message.GetType().FullName}'");
                }
            }
        }
    }
}
