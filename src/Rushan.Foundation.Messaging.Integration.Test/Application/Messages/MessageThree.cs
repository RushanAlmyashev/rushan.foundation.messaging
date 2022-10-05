namespace Rushan.Foundation.Messaging.Integration.Tests.Application.Messages
{
    public class MessageThree: IEquatable<MessageThree>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public DateTime Date { get; set; }

        public Guid Value { get; set; }

        public bool Equals(MessageThree other)
        {
            return Id == other.Id &&
                    Name == other.Name &&
                    Age == other.Age &&
                    Value == other.Value &&
                    Date == other.Date;
        }
    }
}
