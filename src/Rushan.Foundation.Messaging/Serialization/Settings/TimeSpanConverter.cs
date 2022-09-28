using System;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Rushan.Foundation.Messaging.Serialization.Settings
{
    internal class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (TimeSpan.TryParse(reader.GetString(), CultureInfo.InvariantCulture, out TimeSpan result))
            {
                return result;
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(format: null, CultureInfo.InvariantCulture));
        }
    }
}
