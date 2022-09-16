using RabbitMQ.Client;

namespace Rushan.Foundation.Messaging.Persistence
{
    internal interface IRabbitMQConnection
    {
        IConnection Connection { get; }

        void Stop();
    }
}
