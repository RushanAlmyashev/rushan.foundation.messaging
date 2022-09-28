using RabbitMQ.Client;
using Rushan.Foundation.Messaging.Configuration;
using Rushan.Foundation.Messaging.Enums;
using Rushan.Foundation.Messaging.Helpers;
using Rushan.Foundation.Messaging.Logger;
using Rushan.Foundation.Messaging.Serialization;
using System;

namespace Rushan.Foundation.Messaging.Publish
{
    internal class Publisher : IPublisher
    {
        private const byte DELIVERY_MODE = (byte)DeliveryMode.Persistent;

        private readonly IChanelFactory _chanelFactory;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;
        private readonly string _exchange;

        public Publisher(MessagingConfiguration messagingConfiguration,
            IChanelFactory chanelFactory,
            ISerializer serializer,
            ILogger logger)
        {
            _chanelFactory = chanelFactory;
            _serializer = serializer;
            _exchange = messagingConfiguration.Exchange;
            _logger = logger;
        }

        public void Publish<TMessage>(TMessage message)
        {
            using (var chanel = _chanelFactory.GetRabbitMQChanel())
            {
                var routingKey = message.GetType().FullName.ToLowerInvariant();

                var properties = chanel.CreateBasicProperties();

                properties.ContentType = _serializer.ContentType;
                properties.DeliveryMode = DELIVERY_MODE;
                properties.Type = message.GetType().FullName;
                properties.AppId = ApplicationHelper.GetApplicationName();
                properties.CorrelationId = Guid.NewGuid().ToString();

                try
                {
                    var serializedMessage = _serializer.Serialize(message);

                    chanel.BasicPublish(_exchange, routingKey, properties, serializedMessage);
                }
                catch (Exception e)
                {
                    _logger?.Error(e, $"Ошибка при публикации сообщения: '{message.GetType().FullName}'");
                }
            }
        }
    }
}
