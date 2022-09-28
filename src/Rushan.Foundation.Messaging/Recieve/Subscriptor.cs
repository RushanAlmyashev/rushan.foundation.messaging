using System;
using System.Linq;

namespace Rushan.Foundation.Messaging.Recieve
{
    internal class Subscriptor
    {
        internal IMessageReceiver MessageReceiver { get; private set; }        

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
