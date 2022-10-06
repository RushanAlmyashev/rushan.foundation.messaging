using System;
using System.Text;

namespace Rushan.Foundation.Messaging.Helpers
{
    /// <summary>
    /// Generate Queue Name
    /// </summary>
    internal static class QueueHelper
    {
        /// <summary>
        /// Generate unique queue name
        /// </summary>
        /// <param name="receiver">receiver instance, which should serve queue</param>
        /// <param name="authLogin">connection login</param>
        /// <param name="routingKey">Messaging routing</param>
        /// <returns>Generated queue name</returns>
        internal static string GetQueueName(IMessageReceiver receiver, string authLogin, string routingKey)
        {
            var receiverName = receiver.GetType().FullName.ToLowerInvariant();
            
            var queueName = string.Format($"{receiverName}-{routingKey.ToLowerInvariant()}");

            var queueHash = GetQueueHash(queueName);

            var result = string.Format($"{authLogin}-{queueHash}");

            return result;
        }

        private static string GetQueueHash(string queueName)
        {
            using (var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                queueName = BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(queueName)))
                    .ToLowerInvariant()
                    .Replace("-", "");
            }
            
            return queueName;
        }
    }
}
