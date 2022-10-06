using Rushan.Foundation.Messaging.Serialization.Settings;
using System;

namespace Rushan.Foundation.Messaging.Serialization
{
    internal class JsonMessageSerializer : ISerializer
    {        
        private static readonly System.Text.Json.JsonSerializerOptions _textJsonSettings = JsonSerializerSettings.GetTextJsonSerializerSettings();

        public string ContentType => "application/json";


        public byte[] Serialize<T>(T payload)
        {
            var byteArray = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(payload, _textJsonSettings);

            return byteArray;
        }

        public object Deserialize(byte[] serializedPayload, Type type)
        {
            var payload = System.Text.Json.JsonSerializer.Deserialize(serializedPayload, type, _textJsonSettings);

            return payload;
        }
    }
}
