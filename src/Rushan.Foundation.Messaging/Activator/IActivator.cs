using System;
using System.Threading.Tasks;

namespace Rushan.Foundation.Messaging.Activator
{
    /// <summary>
    /// An activate message instance an func for execute message, based on hint
    /// </summary>
    internal interface IActivator
    {
        /// <summary>
        /// Create message instanec, based on message type and content
        /// </summary>
        /// <param name="messageTypeHint">message hint, which consist full message type</param>
        /// <param name="messageContent">conent of the message</param>
        /// <returns>Generated message instance, based by instruction</returns>
        public object CreateMessageInstance(string messageTypeHint, byte[] messageContent);

        /// <summary>
        /// Return message handler, based on reciever class and message type
        /// </summary>
        /// <param name="messageTypeHint">message hint, which consist full message type</param>
        /// <param name="receiver">reciever class</param>
        /// <returns>Returns message handlers func.</returns>
        public Func<object, Task> CreateMessageHandler(string messageTypeHint, IMessageReceiver receiver);
    }
}
