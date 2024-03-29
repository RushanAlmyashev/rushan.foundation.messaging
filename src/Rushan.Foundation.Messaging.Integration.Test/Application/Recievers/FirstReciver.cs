﻿using Rushan.Foundation.Messaging.Integration.Tests.Application.InteranlServices;
using Rushan.Foundation.Messaging.Integration.Tests.Application.Messages;

namespace Rushan.Foundation.Messaging.Integration.Tests.Application.Recievers
{
    public class FirstReciver : IMessageReceiver<MessageOne>,
        IMessageReceiver<MessageTwo>
    {
        private readonly IInternalService _internalService;

        public FirstReciver(IInternalService internalService)
        {
            _internalService = internalService;
        }

        public async Task ReceiveMessageAsync(MessageOne message)
        {
            await _internalService.MessageHandlerAsync(message);
        }


        public async Task ReceiveMessageAsync(MessageTwo message)
        {
            await _internalService.MessageHandlerAsync(message);
        }
    }
}
