using Polly;
using Polly.Contrib.WaitAndRetry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Rushan.Foundation.Messaging.Enums;
using Rushan.Foundation.Messaging.Helpers;
using Rushan.Foundation.Messaging.Logger;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Rushan.Foundation.Messaging.Persistence
{
    /// <summary>
    /// Provide persistant connection to rabbitMQ bus
    /// </summary>
    internal class RabbitMQConnectionPersistence : IRabbitMQConnection
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private const int MAX__CONNECTION_RETRY_COUNT = 12; //~68 minutes
        private ConnectionState _connectionState;

        private ILogger _logger;
        private IConnectionFactory _connectionFactory;

        private IAutorecoveringConnection _connection = null;


        internal RabbitMQConnectionPersistence() { }

        public RabbitMQConnectionPersistence(string messageBrokerUri, ILogger logger)
        {
            _logger = logger;
            _connectionFactory = GetConnectionFactory(messageBrokerUri);                        
        }

        private bool IsConnected => _connection != default
                                    && _connection.IsOpen;
            
        public IConnection GetConnection()
        {
            if (!IsConnected)
            {
                EstabilishConnectWithRabbitMQ();
            }

            return _connection;
        }

        public void Connect()
        {
            EstabilishConnectWithRabbitMQ();

            _connectionState = ConnectionState.Connected;
        }

        public void Disconnect()
        {
            _connection.Close();
            _connection.Dispose();

            _connectionState = ConnectionState.Disconnected;
        }


        private void EstabilishConnectWithRabbitMQ()
        {
            if (_connectionState == ConnectionState.Disconnected)
            {
                throw new Exception("RabbitMQ connection was closed!");
            }

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

                if (_connection == null)
                {
                    ConnectToRabbitMQ();
                }
                else
                {
                    AwaitAutorecoveryComplete();
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }            
        }

        
        private void ConnectToRabbitMQ()
        {
            var retryDelays = Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(2), MAX__CONNECTION_RETRY_COUNT);
            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(retryDelays,
                    (ex, timeOut) =>
                    {
                        _logger.Error(ex, $"Connection failed!. Exception: {ex.Message}, backoff timeout {timeOut}");
                    });

            policy.Execute(() =>
            {
                _connection = _connectionFactory.CreateConnection() as IAutorecoveringConnection;
            });

            if (_connection.IsOpen == false)
            {
                return;
            }

            _connection.CallbackException += OnCallbackException;
            _connection.ConnectionBlocked += OnConnectionBlocked;
            _connection.ConnectionShutdown += OnConnectionShutdown;
            _connection.RecoverySucceeded += OnRecoverySucceeded;
            _connection.ConnectionRecoveryError += OnRecoveryError;

            return;            
        }

        private void AwaitAutorecoveryComplete()
        {
            var maxRetryDelay = TimeSpan.FromSeconds(3);
            
            var retryDelays = Backoff.ExponentialBackoff(TimeSpan.FromMilliseconds(10), 1000)
                .Select(s => TimeSpan.FromTicks(Math.Min(s.Ticks, maxRetryDelay.Ticks)));

            var policy = Policy.HandleResult(_connection.IsOpen)
                        .WaitAndRetry(retryDelays,
                        (isOpen, backoffTimout) =>
                        {
                            _logger.Error($"Awaiting for autorecoverable operation. Connection status isOpen = {isOpen}, backoff timeout = {backoffTimout}");
                        });

            policy.Execute(() => _connection.IsOpen);

            if (!IsConnected)
            {
                throw new Exception("Connection recover operation has been failed");
            }
        }

        private IConnectionFactory GetConnectionFactory(string messageBrokerUri)
        {
            if (string.IsNullOrEmpty(messageBrokerUri))
            {
                _logger.Error($"connectionString is not defined");

                throw new ArgumentNullException(nameof(messageBrokerUri));
            }

            var connectionFactory = new ConnectionFactory();
            connectionFactory.Uri = new Uri(messageBrokerUri);
            connectionFactory.DispatchConsumersAsync = true;
            connectionFactory.AutomaticRecoveryEnabled = true;
            connectionFactory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);

            connectionFactory.ClientProperties["platform_messaging_version"] = ApplicationHelper.GetMessagingVersion();
            connectionFactory.ClientProperties["application_name"] = ApplicationHelper.GetApplicationName();
            connectionFactory.ClientProperties["machine_name"] = Environment.MachineName.ToLowerInvariant();

            return connectionFactory;
        }


        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (e.Initiator == ShutdownInitiator.Application && e.Cause == null)
            {
                _logger.Info($"RabbitMQ connection shutting down by applicacation'{e.ReplyText}'");
            }
            else
            {
                _logger.Error($"RabbitMQ connection shutting down by reason: '{e.ReplyText}', '{e.ReplyCode}', '{e.MethodId}', '{e.Initiator}', '{e.ClassId}'");
            }
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e) => 
            _logger.Error(e.Exception, "An error occurred on callback in RabbitMQ");
        
        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e) =>       
            _logger.Warn($"Connection is blocked by reason '{e.Reason}'");                  

        private void OnRecoverySucceeded(object sender, EventArgs e) =>
            _logger.Info($"RabbitMQ connection recovered");

        private void OnRecoveryError(object sender, ConnectionRecoveryErrorEventArgs e) =>
            _logger.Error(e.Exception, $"RabbitMQ connection recovery operation failed");
    }
}
