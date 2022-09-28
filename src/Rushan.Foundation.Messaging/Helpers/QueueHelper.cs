using Rushan.Foundation.Messaging.Recieve;
using System;
using System.Text;

namespace Rushan.Foundation.Messaging.Helpers
{
    internal static class QueueHelper
    {
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
