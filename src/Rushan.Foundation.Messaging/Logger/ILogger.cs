using System;

namespace Rushan.Foundation.Messaging.Logger
{
    /// <summary>
    /// Logger interface, for log an events
    /// </summary>
    public interface ILogger
    {
        void Info(string message);

        void Error(string message);

        void Warn(string message);

        void Error(Exception ex, string message = null);

        void Debug(string message);

        void Trace(string message);
    }
}
