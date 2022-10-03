using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Rushan.Foundation.Messaging.Logger;
using System;
using System.Net.Sockets;
using System.Runtime;
using System.Threading;

namespace Rushan.Foundation.Messaging.Persistence
{
    /// <summary>
    /// Provide persistant connection to rabbitMQ bus
    /// </summary>
    internal class RabbitMQConnectionPersistence : IRabbitMQConnection
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private const int CONNECTION_RETRY_COUNT = 5;

        private ILogger _logger;
        private string _messageBrokerUri;

        private static IConnection _connection = null;


        internal RabbitMQConnectionPersistence() { }

        public RabbitMQConnectionPersistence(string messageBrokerUri, ILogger logger)
        {
            _messageBrokerUri = Environment.ExpandEnvironmentVariables(messageBrokerUri);
            _logger = logger;
        }

        private bool IsConnected => _connection != default
                                    && _connection.IsOpen;

        public IConnection GetConnection()
        {
            if (!IsConnected)
            {
                ConnectToRabbitMQ();
            }

            return _connection;
        }

        public void Start()
        {
            ConnectToRabbitMQ();            
        }

        public void Stop()
        {
            _connection.Close();
        }


        private void ConnectToRabbitMQ()
        {
            if (IsConnected)
            {
                return;
            }

            _semaphoreSlim.Wait();

            try
            {
                if (IsConnected)
                {
                    return;
                }

                if (_logger == null)
                {
                    throw new NullReferenceException("_logger is not difined in 'RabbitMQConnectionPersistence'");
                }

                if (string.IsNullOrEmpty(_messageBrokerUri))
                {
                    _logger.Error($"connectionString is not defined");
                }

                _logger.Info($"Initializing connection to rabbitMQ via {_messageBrokerUri}");


                var connectionFactory = new ConnectionFactory();

                connectionFactory.Uri = new Uri(_messageBrokerUri);
                connectionFactory.DispatchConsumersAsync = true;
                connectionFactory.AutomaticRecoveryEnabled = true;
                connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);

                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(CONNECTION_RETRY_COUNT,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (ex, time) =>
                        {
                            _logger.Error(ex, "Connection error.");
                        });

                policy.Execute(() =>
                {
                    _connection = connectionFactory.CreateConnection();
                });

                if (_connection.IsOpen == false)
                {
                    return;
                }

                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;
                _connection.ConnectionShutdown += OnConnectionShutdown;

                return;
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "Connection failed.");
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }


        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            _logger.Error(e.Exception, "An error occurred on callback in RabbitMQ");
            ConnectToRabbitMQ();
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {            
            _logger.Warn($"RabbitMQ connection shutting down by reason: {e.ReplyText}, details: {e}");
            ConnectToRabbitMQ();
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            _logger.Warn($"Connection is blocked by reason '{e.Reason}'");
            ConnectToRabbitMQ();
        }
    }
}
