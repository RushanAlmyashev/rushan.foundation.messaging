namespace Rushan.Foundation.Messaging.Tests.Messages
{
    public class MessageTwo
    {
        public InternalMessageObject MessageObject {get ;set;}
    }

    public class InternalMessageObject
    {
        public int Id {get;set;}
        
        public string Name {get;set;}
    }
}
