namespace Rushan.Foundation.Messaging
{
    public interface IMessaging
    {
        void Publish<TMessage>(TMessage message);
    }
}