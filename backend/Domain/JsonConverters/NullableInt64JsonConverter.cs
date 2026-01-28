using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.JsonConverters
{
    public class NullableInt64JsonConverter : JsonConverter<long?>
    {
        public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var number))
            {
                return number;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                if (long.TryParse(value, out var parsed))
                {
                    return parsed;
                }
            }

            throw new JsonException("Invalid value for nullable Int64.");
        }

        public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteNumberValue(value.Value);
                return;
            }

            writer.WriteNullValue();
        }
    }
}
