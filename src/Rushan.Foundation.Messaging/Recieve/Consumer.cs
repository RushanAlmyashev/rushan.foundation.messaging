using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rushan.Foundation.Messaging.Activator;
using Rushan.Foundation.Messaging.Exceptions;
using Rushan.Foundation.Messaging.Helpers;
using Rushan.Foundation.Messaging.Logger;
using Rushan.Foundation.Messaging.Persistence;
using System;
using System.Threading.Tasks;

namespace Rushan.Foundation.Messaging.Recieve
{
    internal class Consumer : IConsumer
    {
        private IModel _channel;

        private readonly IRabbitMQConnection _rabbitMQConnection;
        private readonly IActivator _activator;        
        private readonly ILogger _logger;
        private readonly string _exchangeName;
        private readonly ushort _qos;
        private readonly string _authLogin;

        internal Consumer(IRabbitMQConnection rabbitMQConnection,
            IActivator activator,
            ILogger logger,
            string messageBrokerUri,
            string exchangeName,
            ushort qos)
        {
            _rabbitMQConnection = rabbitMQConnection;
            _activator = activator;
            _logger = logger;
            
            _authLogin = ConnectionHelper.GetAuthUser(messageBrokerUri);
            _exchangeName = exchangeName;
            _qos = qos;
        }

        public void Subscribe(Subscriptor subscriptor)
        {
            var receiver = subscriptor.MessageReceiver;
            var messageTypes = subscriptor.MessageTypes;

            var model = GetOrCreateChannel();

            foreach (var messageType in messageTypes)
            {
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

                model.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            }
        }

        public void StopSubscription()
        {
            _channel?.Dispose();
        }

        private IModel GetOrCreateChannel()
        {
            while (true)
            {
                if (_channel == default)
                {
                    _channel = _rabbitMQConnection.GetConnection().CreateModel();
                    _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
                    _channel.BasicQos(prefetchSize: 0, prefetchCount: _qos, global: false);
                }

                if (!_channel.IsClosed)
                {
                    return _channel;
                }

                _channel.Dispose();
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
