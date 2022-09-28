using RabbitMQ.Client;

namespace Rushan.Foundation.Messaging
{
    internal interface IChanelFactory
    {
        IModel GetRabbitMQChanel();
    }
}