using System.Threading.Tasks;

namespace Rushan.Foundation.Messaging
{
    /// <summary>
    /// Message receiver interface.
    /// All Message handlers should be inheritance from IMessageReceiver<TMessage> interface
    /// </summary>
    /// <typeparam name="TMessage">Generic message</typeparam>
    public interface IMessageReceiver<TMessage> : IMessageReceiver where TMessage : class
    {
        /// <summary>
        /// Message handler method
        /// </summary>
        /// <param name="message">Recieved message</param>
        /// <returns></returns>
        Task ReceiveMessageAsync(TMessage message);
    }

    /// <summary>
    /// Base message receiver interface.
    /// </summary>
    public interface IMessageReceiver
    {
    }
}
