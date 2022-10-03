using Rushan.Foundation.Messaging.Channel;
using Rushan.Foundation.Messaging.Configuration;
using Rushan.Foundation.Messaging.Enums;
using Rushan.Foundation.Messaging.Logger;
using Rushan.Foundation.Messaging.Persistence;
using Rushan.Foundation.Messaging.Publish;
using Rushan.Foundation.Messaging.Recieve;
using Rushan.Foundation.Messaging.Serialization;
using System;
using System.Collections.Generic;

namespace Rushan.Foundation.Messaging
{
    public class RabbitMqMessageBus
    {
        private BusState _state = BusState.Stopped;

        private readonly IRabbitMQConnection _rabbitMQConnection;
        private readonly IChannelFactory _chanelFactory;
        
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;

        private readonly IPublisher _publisher;
        private readonly IConsumer _consumer;

        private readonly List<Subscriptor> _subscriptions = new();

        public RabbitMqMessageBus(MessagingConfiguration messagingConfiguration,
            ILogger logger = null,
            ISerializer serializer = null)
        {
            _logger = logger ?? new EmptyLogger();
            _serializer = serializer ?? new JsonMessageSerializer();

            _rabbitMQConnection = new RabbitMQConnectionPersistence(messagingConfiguration.MessageBrokerUri, _logger);
            _chanelFactory = new ChannelFactory(_rabbitMQConnection, messagingConfiguration.Qos);

            _publisher = new Publisher(messagingConfiguration, _chanelFactory, _serializer, _logger);
            _consumer = new Consumer(messagingConfiguration, _chanelFactory, _serializer, _logger);
        }

        public void Publish<TMessage>(TMessage message)
        {
            _publisher.Publish(message);
        }


        public void Subscribe<TMessage>(IMessageReceiver<TMessage> receiver)
        {
            if (receiver == null)
            {
                throw new ArgumentNullException(nameof(receiver));
            }

            var subscription = new Subscriptor(receiver);           
            
            _subscriptions.Add(subscription);            
        }


        public void StartMessageBus()
        {
            if (_state == BusState.Started)
            {
                _logger?.Info($"RabbitMQ client is already started");                
                return;
            }
            
            _rabbitMQConnection.Start();
            StartSubscriptionsInvokation();
            
            _state = BusState.Started;
            _logger?.Info($"RabbitMQ client in state: {_state}");
        }


        public void StopMessageBus()
        {
            if (_state == BusState.Stopped)
            {
                _logger?.Info($"RabbitMQ client is already stoped");                
                return;
            }

            _consumer.StopSubscription();
            _rabbitMQConnection.Stop();

            _state = BusState.Stopped;
            _logger?.Info($"RabbitMQ client in state: {_state}");            
        }


        public void StartSubscriptionsInvokation()
        {
            for (var i = 0; i < _subscriptions.Count; i++)
            {
                _consumer.Subscribe(_subscriptions[i]);
            }
        }
    }
}
