using RabbitMQ.Client;
using Rushan.Foundation.Messaging.Persistence;

namespace Rushan.Foundation.Messaging.Channel
{
    internal class ChannelFactory : IChannelFactory
    {        
        private readonly IRabbitMQConnection _rabbitMQConnection;

        public ChannelFactory(IRabbitMQConnection rabbitMQConnection)
        {
            _rabbitMQConnection = rabbitMQConnection;            
        }


        public IModel CreateRabbitMQChannel()
        {            
            var model = _rabbitMQConnection.GetConnection().CreateModel();                  

            return model;            
        }
    }
}
