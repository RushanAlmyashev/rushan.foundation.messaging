using System;
using System.Linq;

namespace Rushan.Foundation.Messaging.Recieve
{
    /// <summary>
    /// Subscriptor class, contains information about MessageReceiver, and recived message type
    /// </summary>
    internal class Subscriptor
    {
        /// <summary>
        /// Message reciever class
        /// </summary>
        internal IMessageReceiver MessageReceiver { get; private set; }        

        /// <summary>
        /// Message types, which reciever can handle
        /// </summary>
        internal Type[] MessageTypes { get; private set; }        


        public Subscriptor(IMessageReceiver receiver)
        {            
            var messageTypes = receiver
                    .GetType()
                    .GetInterfaces()
                    .Where(t => typeof(IMessageReceiver).IsAssignableFrom(t))
                    .Where(t => t.IsGenericType)
                    .SelectMany(t => t.GetGenericArguments())
                    .Distinct()
                .ToArray();

            MessageTypes = messageTypes;
            MessageReceiver = receiver;
        }
    }    
}
