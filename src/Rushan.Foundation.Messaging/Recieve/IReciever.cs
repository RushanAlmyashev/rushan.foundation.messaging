namespace Rushan.Foundation.Messaging.Recieve
{
    internal interface IConsumer
    {
        void Subscribe(Subscriptor subscriptor);

        void StopSubscription();        
    }
}