namespace Rushan.Foundation.Messaging.Integration.Tests.Application.Messages
{
    public class MessageOne : IEquatable<MessageOne>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Guid Key { get; set; }

        public DateTime Value { get; set; }

        public bool Equals(MessageOne other)
        {
            return Id == other.Id &&
                    Name == other.Name &&
                    Key == other.Key &&
                    Value == other.Value;
        }
    }
}
