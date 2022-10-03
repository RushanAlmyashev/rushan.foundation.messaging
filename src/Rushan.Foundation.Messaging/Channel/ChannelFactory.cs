using RabbitMQ.Client;
using Rushan.Foundation.Messaging.Persistence;

namespace Rushan.Foundation.Messaging.Channel
{
    internal class ChannelFactory : IChannelFactory
    {
        private readonly ushort _qos;
        private readonly IRabbitMQConnection _rabbitMQConnection;

        public ChannelFactory(IRabbitMQConnection rabbitMQConnection,
            ushort qos)
        {
            _rabbitMQConnection = rabbitMQConnection;
            _qos = qos;
        }


        public IModel GetRabbitMQChannel()
        {
            var model = _rabbitMQConnection.GetConnection().CreateModel();

            model.BasicQos(prefetchSize: 0, prefetchCount: _qos, global: false);

            return model;
        }
    }
}
