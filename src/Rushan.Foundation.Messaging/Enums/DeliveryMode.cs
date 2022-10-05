namespace Rushan.Foundation.Messaging.Enums
{
    /// <summary>
    /// Messaging delivery mode. 
    /// Important when message brocker restart
    /// For more information https://www.rabbitmq.com/persistence-conf.html
    /// </summary>
    internal enum DeliveryMode : byte
    {
        NonPersistent = 1,

        Persistent = 2
    }
}
