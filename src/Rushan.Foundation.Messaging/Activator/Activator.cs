using Rushan.Foundation.Messaging.Exceptions;
using Rushan.Foundation.Messaging.Logger;
using Rushan.Foundation.Messaging.Serialization;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Rushan.Foundation.Messaging.Activator
{
    /// <inheritdoc />
    internal class Activator : IActivator
    {
        /// <summary>
        /// Dictioanary of activated message types, for improve performance
        /// </summary>
        private static readonly ConcurrentDictionary<string, Type> TypeMapCache = new ConcurrentDictionary<string, Type>();

        private readonly ISerializer _serializer;
        private readonly ILogger _logger;

        /// <summary>
        /// Initialize Activator constructor
        /// </summary>
        /// <param name="serializer">serializer for deserialize message</param>
        /// <param name="logger">logger</param>
        public Activator(ISerializer serializer, ILogger logger)
        {
            _serializer = serializer;
            _logger = logger;
        }


        /// <inheritdoc />
        public object CreateMessageInstance(string messageTypeHint, byte[] messageContent)
        {
            var messageType = GetType(messageTypeHint);
            if (messageType == null)
            {                
                throw new MessageActivationException(messageTypeHint);
            }
            
            try
            {
                var message = _serializer.Deserialize(messageContent, messageType);

                return message;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error on message Deserialize {ex.Message}");

                throw new MessageActivationException(messageTypeHint);
            }            
        }

        /// <inheritdoc />
        public Func<object, Task> CreateMessageHandler(string messageTypeHint, IMessageReceiver receiver)
        {
            var messageType = GetType(messageTypeHint);

            var receiveMessageMethod = receiver.GetType().GetMethod("ReceiveMessageAsync", new[] { messageType });

            return message => (Task) receiveMessageMethod.Invoke(receiver, new object[] { message });
        }

        /// <summary>
        /// Get type, from type hint
        /// </summary>
        /// <param name="typeHint">typeHint. Should contains full message name</param>
        /// <returns>Concrete type by typeHint</returns>
        private static Type GetType(string typeHint)
        {
            if (string.IsNullOrWhiteSpace(typeHint))
            {
                return null;
            }

            Type type;

            if (TypeMapCache.TryGetValue(typeHint, out type))
            {
                return type;
            }

            type = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetExportedTypes())
                .FirstOrDefault(t => t.FullName.Equals(typeHint));

            TypeMapCache.TryAdd(typeHint, type);

            return type;
        }
    }
}
