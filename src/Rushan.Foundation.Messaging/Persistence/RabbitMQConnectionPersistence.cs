using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Rushan.Foundation.Messaging.Logger;
using System;
using System.Threading;

namespace Rushan.Foundation.Messaging.Persistence
{
    /// <summary>
    /// Provide persistant connection to rabbitMQ bus
    /// </summary>
    internal class RabbitMQConnectionPersistence: IRabbitMQConnection
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        
        private IConnection _connection => _lazyConnection.Value;

        
        private readonly ILogger _logger;
        private readonly Lazy<IConnection> _lazyConnection;                


        public RabbitMQConnectionPersistence(string connectionString, ILogger logger)
        {
            _lazyConnection = new Lazy<IConnection>(() => ConnectToRabbitMQ(connectionString));
            _logger = logger;
        }

        public IConnection Connection => _connection;        
        

        public void Stop()
        {
            _connection.Close();
        }


        private IConnection ConnectToRabbitMQ(string connectionString)
        {
            _semaphoreSlim.Wait();

            try
            {
                if (_lazyConnection.IsValueCreated && _connection.IsOpen)
                {
                    _logger.Warn("RabbitMQ connection is already open");

                    return _connection;
                }

                connectionString = Environment.ExpandEnvironmentVariables(connectionString);
                _logger.Info($"Initializing connection to rabbitMQ via {connectionString}");


                var connectionFactory = new ConnectionFactory();

                connectionFactory.Uri = new Uri(connectionString);
                connectionFactory.AutomaticRecoveryEnabled = true;
                connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);


                var connection = connectionFactory.CreateConnection() as IAutorecoveringConnection;

                connection.ConnectionBlocked += OnConnectionBlocked;
                connection.CallbackException += OnCallbackException;
                connection.ConnectionRecoveryError += OnRecoveryError;
                connection.RecoverySucceeded += OnRecoverySucceeded;
                connection.ConnectionShutdown += OnConnectionShutdown;

                return connection;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }


        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e) => 
            _logger.Warn($"An error occurred on connection to RabbitMQ by reason: '{e.Reason}'");

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e) =>
            _logger.Error(e.Exception, "An error occurred on callback in RabbitMQ");

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e) =>         
            _logger.Warn($"RabbitMQ connection shutting down by reason: {e.ReplyText}, details: {e}");

        private void OnRecoverySucceeded(object sender, EventArgs e) =>
            _logger.Info($"RabbitMQ connection recovered");

        private void OnRecoveryError(object sender, ConnectionRecoveryErrorEventArgs e) =>
            _logger.Error(e.Exception, $"RabbitMQ connection recovery operation failed");
    }
}
