using System.Text.Json;
using System.Text.Json.Serialization;

namespace Clairvoyance.Collections.Domain;

public sealed class CardIdJsonConverter : JsonConverter<CardId>
{
    public override CardId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return default;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string for Grading.");
        }

        var s = reader.GetString();
        if (string.IsNullOrEmpty(s))
        {
            return default;
        }

        return new CardId(s);
    }

    public override void Write(Utf8JsonWriter writer, CardId value, JsonSerializerOptions options)
    {
        if (string.IsNullOrEmpty(value.ExpansionCode) && string.IsNullOrEmpty(value.ExpansionNumber))
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.ToString());
    }
}
