namespace Rushan.Foundation.Messaging.Publish
{
    /// <summary>
    /// Provide sending message to queues
    /// </summary>
    internal interface IPublisher
    {
        /// <summary>
        /// Send message to queues
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message">message</param>
        void Publish<TMessage>(TMessage message);
    }
}