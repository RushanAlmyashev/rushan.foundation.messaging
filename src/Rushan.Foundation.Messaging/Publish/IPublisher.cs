namespace Rushan.Foundation.Messaging.Publish
{
    internal interface IPublisher
    {
        void Publish<TMessage>(TMessage message);
    }
}