using System.Text.Json;
using System.Text.Json.Serialization;

namespace Clairvoyance.Collections.Domain;

public sealed class GradingJsonConverter : JsonConverter<Grading>
{
    public override Grading Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        var parsed = Grading.ParseFromKey(s) ?? Grading.ParseFromValue(s);
        if (parsed is null)
        {
            throw new JsonException($"Unknown grading key/value '{s}'.");
        }

        return parsed.Value;
    }

    public override void Write(Utf8JsonWriter writer, Grading value, JsonSerializerOptions options)
    {
        if (string.IsNullOrEmpty(value.Key))
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Key);
    }
}
