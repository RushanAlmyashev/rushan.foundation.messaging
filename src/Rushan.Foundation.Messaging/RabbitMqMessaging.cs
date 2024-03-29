﻿using Rushan.Foundation.Messaging.Activator;
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

    public class RabbitMqMessaging: IMessaging
    {
        private BusState _state = BusState.Stopped;

        private readonly IRabbitMQConnection _rabbitMQConnection;        
        
        private readonly ISerializer _serializer;
        private readonly IActivator _activator;
        private readonly ILogger _logger;

        private readonly IPublisher _publisher;
        private readonly IConsumer _consumer;

        private readonly List<Subscriptor> _subscriptions = new List<Subscriptor>();

        public RabbitMqMessaging(MessagingConfiguration messagingConfiguration,
            ILogger logger = null,
            ISerializer serializer = null)
        {
            var messageBrokerUri = Environment.ExpandEnvironmentVariables(messagingConfiguration.MessageBrokerUri);
            var exchange = Environment.ExpandEnvironmentVariables(messagingConfiguration.Exchange);
            var fetchCount = messagingConfiguration.FetchCount;

            _logger = logger ?? new EmptyLogger();
            _serializer = serializer ?? new JsonMessageSerializer();
            _activator = new Activator.Activator(_serializer, _logger);

            _rabbitMQConnection = new RabbitMQConnectionPersistence(messageBrokerUri, _logger);           
            
            _publisher = new Publisher(_rabbitMQConnection, _serializer, _logger, exchange);
            _consumer = new Consumer(_rabbitMQConnection, _activator, _logger, messageBrokerUri, exchange, fetchCount);
        }

        public void Publish<TMessage>(TMessage message)
        {
            _publisher.Publish(message);
        }

        public void Subscribe(IMessageReceiver receiver)
        {
            if (receiver == null)
            {
                throw new ArgumentNullException(nameof(receiver));
            }

            var subscription = new Subscriptor(receiver);           
            
            _subscriptions.Add(subscription);            
        }


        public void Start()
        {
            if (_state == BusState.Started)
            {
                _logger.Info($"RabbitMQ client is already started");                
                return;
            }
            
            _rabbitMQConnection.Connect();
            StartSubscriptionsInvocation();
            
            _state = BusState.Started;
            _logger.Info($"RabbitMQ client in state: {_state}");
        }


        public void Stop()
        {
            if (_state == BusState.Stopped)
            {
                _logger.Info($"RabbitMQ client is already stoped");                
                return;
            }

            _consumer.StopSubscriptionInvocation();
            _rabbitMQConnection.Disconnect();

            _state = BusState.Stopped;
            _logger.Info($"RabbitMQ client in state: {_state}");            
        }


        private void StartSubscriptionsInvocation()
        {
            foreach(var subscription in _subscriptions)
            {
                _consumer.StartSubscriptionInvokation(subscription);
            }
        }
    }
}
