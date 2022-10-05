namespace Rushan.Foundation.Messaging.Tests.Messages
{
    public class MessageOne: IEquatable<MessageOne>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Guid Key { get; set; }

        public DateTime Value { get; set; }

        public bool Equals(MessageOne other)
        {
            return  this.Id == other.Id &&
                    this.Name == other.Name &&
                    this.Key == other.Key &&
                    this.Value == other.Value;
        }
    }
}
