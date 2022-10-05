using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rushan.Foundation.Messaging.Activator
{
    internal interface IActivator
    {
        public object CreateMessageInstance(string messageTypeHint, byte[] messageContent);

        public Func<object, Task> CreateMessageHandler(string messageTypeHint, IMessageReceiver receiver);
    }
}
