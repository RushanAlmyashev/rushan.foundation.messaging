namespace Rushan.Foundation.Messaging.Integration.Tests.Application.Messages
{
    public class MessageTwo: IEquatable<MessageTwo>
    {
        public InternalMessageObject MessageObject { get; set; }

        public bool Equals(MessageTwo other)
        {
            return this.MessageObject.Equals(other.MessageObject);
        }
    }

    public class InternalMessageObject: IEquatable<InternalMessageObject>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Equals(InternalMessageObject other)
        {
            return this.Id == other.Id && this.Name == other.Name;
        }
    }
}
