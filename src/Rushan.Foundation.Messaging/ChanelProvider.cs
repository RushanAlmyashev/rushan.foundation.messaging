using Rushan.Foundation.Messaging.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rushan.Foundation.Messaging
{
    internal class ChanelProvider
    {
        private readonly DeliveryMode

        private readonly IRabbitMQConnection _rabbitMQConnection;

        public ChanelProvider(IRabbitMQConnection rabbitMQConnection)
        {
            _rabbitMQConnection = rabbitMQConnection;
        }


    }
}
