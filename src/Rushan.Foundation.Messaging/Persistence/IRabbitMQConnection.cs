using RabbitMQ.Client;

namespace Rushan.Foundation.Messaging.Persistence
{
    internal interface IRabbitMQConnection
    {
        /// <summary>
        /// Get persistence connection 
        /// </summary>
        IConnection GetConnection();
        
        /// <summary>
        /// Initialize connectinon Persistance
        /// </summary>
        void Connect();

        /// <summary>
        /// Dispose connection persistence
        /// </summary>
        void Disconnect();
    }
}
