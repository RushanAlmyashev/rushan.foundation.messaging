namespace Rushan.Foundation.Messaging.Serialization.Settings
{
    internal static class JsonSerializerSettings
    {
        internal static System.Text.Json.JsonSerializerOptions GetTextJsonSerializerSettings()
        {
            var result = new System.Text.Json.JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.BasicLatin, System.Text.Unicode.UnicodeRanges.Cyrillic),
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            };

            result.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(System.Text.Json.JsonNamingPolicy.CamelCase, true));
            result.Converters.Add(new TimeSpanConverter());

            return result;
        }
    }
}
