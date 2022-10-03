using Polly;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using Rushan.Foundation.Messaging.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rushan.Foundation.Messaging.Configuration;

namespace Rushan.Foundation.Messaging.Integration.Tests.Utils
{
    internal class BrokerHelper
    {
        public static IConnectionFactory GetConnectionFactory()
        {
            ConnectionFactory factory = new ConnectionFactory();

            factory.UserName = ConnectionFactory.DefaultUser;
            factory.Password = ConnectionFactory.DefaultPass;
            factory.VirtualHost = ConnectionFactory.DefaultVHost;
            factory.HostName = BrokerConstants.HostName;
            factory.Port = BrokerConstants.Port5672;
            factory.AutomaticRecoveryEnabled = true;
            factory.DispatchConsumersAsync = true;
            factory.RequestedConnectionTimeout = TimeSpan.FromSeconds(5);

            return factory;
        }

        public static void AwaitingBrokerStart()
        {
            var factory = GetConnectionFactory();

            IConnection connection = null;

            var policy = Policy.Handle<BrokerUnreachableException>()
                                      .WaitAndRetry(180, retryAttempt => TimeSpan.FromSeconds(2));
            policy.Execute(() =>
            {
                connection = factory.CreateConnection();
            });

            connection?.Dispose();
        }

        public static string GetConnectionString()
        {
            var builder = new StringBuilder();

            builder.Append("amqp://")
                .Append(ConnectionFactory.DefaultUser)
                .Append(":")
                .Append(ConnectionFactory.DefaultPass)
                .Append("@")
                .Append(BrokerConstants.HostName)
                .Append(ConnectionFactory.DefaultVHost);

            return builder.ToString();
        }

        public static MessagingConfiguration GetConfiguration()
        {
            return new MessagingConfiguration()
            {
                MessageBrokerUri = GetConnectionString(),
                Exchange = BrokerConstants.ExchangeName
            };
        }
    }
}
