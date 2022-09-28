namespace Rushan.Foundation.Messaging.Configuration
{
    public class MessagingConfiguration
    {
        public string MessageBrokerUri { get; set; }

        public string Exchange { get; set; }

        public ushort Qos { get; set; } = 5;

        public bool LogMessage { get; set; } = false;
    }
}
