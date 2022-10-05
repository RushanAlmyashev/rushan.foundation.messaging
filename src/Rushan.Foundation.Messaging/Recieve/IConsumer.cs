namespace Rushan.Foundation.Messaging.Recieve
{
    internal interface IConsumer
    {
        void StartSubscriptionInvokation(Subscriptor subscriptor);

        void StopSubscriptionInvocation();        
    }
}