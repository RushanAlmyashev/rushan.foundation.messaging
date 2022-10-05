using System;

namespace Rushan.Foundation.Messaging.Exceptions
{
    /// <summary>
    /// Recieved message, could not be created/deserialized
    /// </summary>
    internal class MessageActivationException: Exception
    {
        public string MessageTypeHint { get; set; }

        public MessageActivationException(string messageTypeHint) 
        {
            MessageTypeHint = messageTypeHint;
        }
    }
}
