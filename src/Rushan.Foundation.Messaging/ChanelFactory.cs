using RabbitMQ.Client;
using Rushan.Foundation.Messaging.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rushan.Foundation.Messaging
{
    internal class ChanelFactory : IChanelFactory
    {
        private readonly ushort _qos;
        private readonly IRabbitMQConnection _rabbitMQConnection;

        public ChanelFactory(IRabbitMQConnection rabbitMQConnection,
            ushort qos)
        {
            _rabbitMQConnection = rabbitMQConnection;
            _qos = qos;
        }


        public IModel GetRabbitMQChanel()
        {
            var model = _rabbitMQConnection.Connection.CreateModel();
            
            model.BasicQos(prefetchSize: 0, prefetchCount: _qos, global: false);            

            return model;
        }
    }
}
