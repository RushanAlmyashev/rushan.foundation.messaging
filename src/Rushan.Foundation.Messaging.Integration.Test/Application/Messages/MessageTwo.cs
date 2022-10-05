namespace Rushan.Foundation.Messaging.Integration.Tests.Application.Messages
{
    public class MessageTwo
    {
        public InternalMessageObject MessageObject { get; set; }
    }

    public class InternalMessageObject
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
