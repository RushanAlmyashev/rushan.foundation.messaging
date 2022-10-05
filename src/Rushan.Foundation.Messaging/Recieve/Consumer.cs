using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rushan.Foundation.Messaging.Activator;
using Rushan.Foundation.Messaging.Exceptions;
using Rushan.Foundation.Messaging.Helpers;
using Rushan.Foundation.Messaging.Logger;
using Rushan.Foundation.Messaging.Persistence;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Rushan.Foundation.Messaging.Recieve
{
    internal class Consumer : IConsumer
    {
        private ConcurrentBag<IModel> _channels = new ConcurrentBag<IModel>();        

        private readonly IRabbitMQConnection _rabbitMQConnection;
        private readonly IActivator _activator;        
        private readonly ILogger _logger;
        private readonly string _exchangeName;
        private readonly ushort _fetchCount;
        private readonly string _authLogin;

        internal Consumer(IRabbitMQConnection rabbitMQConnection,
            IActivator activator,
            ILogger logger,
            string messageBrokerUri,
            string exchangeName,
            ushort fetchCount)
        {
            _rabbitMQConnection = rabbitMQConnection;
            _activator = activator;
            _logger = logger;
            
            _authLogin = ConnectionHelper.GetAuthUser(messageBrokerUri);
            _exchangeName = exchangeName;
            _fetchCount = fetchCount;
        }

        public void StartSubscriptionInvokation(Subscriptor subscriptor)
        {
            var receiver = subscriptor.MessageReceiver;
            var messageTypes = subscriptor.MessageTypes;

            
            foreach (var messageType in messageTypes)
            {
                var model = CreateChannel();


                var routingKey = messageType.FullName.ToLowerInvariant();
                var queueName = QueueHelper.GetQueueName(receiver, _authLogin, routingKey);
                                
                model.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
                model.QueueBind(queueName, _exchangeName, routingKey);

                var consumer = new AsyncEventingBasicConsumer(model);

                consumer.Received += async (sender, eventArgs) =>
                {
                    try
                    {
                        await InvokeMessageReceiverAsync(receiver, eventArgs.BasicProperties.Type, eventArgs.Body.ToArray());

                        model.BasicAck(eventArgs.DeliveryTag, false);
                    }
                    catch (MessageActivationException ex)
                    {
                        _logger.Error($"Message {ex.MessageTypeHint} could not be deserialized. Message will be removed from the queue.");

                        model.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Oops something went wrong exception: {ex.Message}");
                    }
                };

                var consumerTag = model.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

                _logger?.Info($"Reciever '{receiver.GetType().FullName}' is connected to the queue: '{queueName}' for consume message type: '{messageType.FullName}' with routingKey: '{routingKey}' and consumerTag: '{consumerTag}'");
            }
        }

        public void StopSubscriptionInvocation()
        {
            foreach (var channel in _channels)
            {
                channel.Dispose();
            }          
        }

        private IModel CreateChannel()
        {
            IModel channel;

            while (true)
            {
                channel = _rabbitMQConnection.GetConnection().CreateModel();
                channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
                channel.BasicQos(prefetchSize: 0, prefetchCount: _fetchCount, global: false);

                if (channel.IsOpen)
                {
                    _channels.Add(channel);
                    
                    return channel;
                }

                channel.Dispose();
            }
        }        

        public async Task InvokeMessageReceiverAsync(IMessageReceiver messageReceiver, string messageTypeHint, byte[] messageContent)
        {
            var message = _activator.CreateMessageInstance(messageTypeHint, messageContent);

            var messageHandler = _activator.CreateMessageHandler(messageTypeHint, messageReceiver);            

            await messageHandler(message);            
        }
    }
}
