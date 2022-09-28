using RabbitMQ.Client;

namespace Rushan.Foundation.Messaging.Persistence
{
    internal interface IRabbitMQConnection
    {
        /// <summary>
        /// Get persistence connection 
        /// </summary>
        IConnection Connection { get; }
        
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
