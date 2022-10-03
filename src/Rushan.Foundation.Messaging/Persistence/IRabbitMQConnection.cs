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
        void Start();

        /// <summary>
        /// Dispose connection persistence
        /// </summary>
        void Stop();
    }
}
