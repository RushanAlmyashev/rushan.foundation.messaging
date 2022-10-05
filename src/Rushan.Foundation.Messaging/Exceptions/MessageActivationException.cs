using System;

namespace Rushan.Foundation.Messaging.Exceptions
{
    internal class MessageActivationException: Exception
    {
        public string MessageTypeHint { get; set; }

        public MessageActivationException(string messageTypeHint) 
        {
            MessageTypeHint = messageTypeHint;
        }
    }
}
