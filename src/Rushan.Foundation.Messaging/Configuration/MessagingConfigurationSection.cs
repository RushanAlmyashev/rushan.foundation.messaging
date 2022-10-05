namespace Rushan.Foundation.Messaging.Configuration
{
    /// <summary>
    /// Messaging configuration
    /// </summary>
    public class MessagingConfiguration
    {
        /// <summary>
        /// Connection Uri to message brocker
        /// </summary>
        public string MessageBrokerUri { get; set; }

        /// <summary>
        /// Exchange name 
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// Prefetch messages, for improve performance
        /// For more information https://www.rabbitmq.com/amqp-0-9-1-reference.html
        /// </summary>
        public ushort FetchCount { get; set; } = 5;        
    }
}
