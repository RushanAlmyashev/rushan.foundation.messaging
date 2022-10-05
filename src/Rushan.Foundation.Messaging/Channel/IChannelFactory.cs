using RabbitMQ.Client;

namespace Rushan.Foundation.Messaging.Channel
{
    internal interface IChannelFactory
    {
        IModel CreateRabbitMQChannel();
    }
}