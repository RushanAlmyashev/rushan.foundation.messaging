using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rushan.Foundation.Messaging.Channel;
using Rushan.Foundation.Messaging.Configuration;
using Rushan.Foundation.Messaging.Helpers;
using Rushan.Foundation.Messaging.Logger;
using Rushan.Foundation.Messaging.Serialization;
using System;
using System.Threading.Tasks;

namespace Rushan.Foundation.Messaging.Recieve
{
    internal class Consumer : IConsumer
    {        
        private IModel _model;

        private readonly IChannelFactory _chanelFactory;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;
        private readonly string _exchangeName;
        private readonly string _authLogin;

        internal Consumer(MessagingConfiguration messagingConfiguration,
            IChannelFactory chanelFactory,
            ISerializer serializer,
            ILogger logger)
        {
            _chanelFactory = chanelFactory;
            _serializer = serializer;
            _logger = logger;
            _exchangeName = messagingConfiguration.Exchange;
            _authLogin = ConnectionHelper.GetAuthUser(messagingConfiguration.MessageBrokerUri);
        }        

        public void Subscribe(Subscriptor subscriptor)
        {
            var receiver = subscriptor.MessageReceiver;
            var messageTypes = subscriptor.MessageTypes;

            var model = GetOrCreateModel();            

            foreach (var messageType in messageTypes)
            {
                var routingKey = messageType.FullName.ToLowerInvariant();
                var queueName = QueueHelper.GetQueueName(receiver, _authLogin, routingKey);

                model.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
                
                if (!string.IsNullOrEmpty(_exchangeName))
                {
                    model.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
                }

                model.QueueBind(queueName, _exchangeName, routingKey);
                var consumer = new AsyncEventingBasicConsumer(model);

                consumer.Received += async (sender, eventArgs) =>
                {
                    var body = eventArgs.Body;

                    try
                    {
                        await InvokeMessageReceiverAsync(receiver, eventArgs);

                        model.BasicAck(eventArgs.DeliveryTag, false);
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
            _model?.Dispose();
        }

        private IModel GetOrCreateModel()
        {
            while (true)
            {
                if (_model == default)
                {
                    _model = _chanelFactory.GetRabbitMQChannel();                    
                }

                if (!_model.IsClosed)
                {
                    return _model;
                }

                _model.Dispose();
            }
        }

        

        private async Task InvokeMessageReceiverAsync(IMessageReceiver messageReceiver, BasicDeliverEventArgs deliverEventArgs)
        {
            var messageType = Activator.GetType(deliverEventArgs.BasicProperties.Type);
            var message = ExtractMessage(deliverEventArgs);
            
            var receiveMessageMethod = messageReceiver.GetType().GetMethod("ReceiveMessageAsync", new[] { messageType });

            await (Task) receiveMessageMethod.Invoke(messageReceiver, new object[] { message });
        }

        private object ExtractMessage(BasicDeliverEventArgs deliverEventArgs)
        {
            object message = null;
            var typeHint = Activator.GetType(deliverEventArgs.BasicProperties.Type);
            try
            {
                message = _serializer.Deserialize(deliverEventArgs.Body.ToArray(), typeHint);
            }
            catch
            {
                _logger?.Error($"Сообщение для типа {typeHint} не может быть десериализовано и будет удалено из очереди");

                _model.BasicNack(deliverEventArgs.DeliveryTag, multiple: false, requeue: false);
            }

            return message;
        }
    }
}
