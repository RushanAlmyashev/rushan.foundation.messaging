using System;

namespace Rushan.Foundation.Messaging.Serialization
{
    /// <summary>
    /// Contract for Serializer implementation
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serialized cotnent type
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Serializes the specified payload.
        /// </summary>
        /// <param name="serializedPayload">The payload.</param>
        /// <returns>Return the serialized payload</returns>
        object Deserialize(byte[] serializedPayload, Type type);


        /// <summary>
        /// Deserializes the specified bytes.
        /// </summary>
        /// <typeparam name="T">The type of the expected object.</typeparam>
        /// <param name="serializedPayload">The serialized payload.</param>
        /// <returns>
        /// The instance of the specified Payload
        /// </returns>
        byte[] Serialize<T>(T payload);
    }
}