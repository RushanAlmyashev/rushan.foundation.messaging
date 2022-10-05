﻿using System;
using System.Threading.Tasks;

namespace Rushan.Foundation.Messaging
{    
    public interface IMessageReceiver<TMessage> : IMessageReceiver
    {
        Task ReceiveMessageAsync(TMessage message);
    }

    public interface IMessageReceiver
    {
    }
}
