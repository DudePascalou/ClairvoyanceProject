using System.Text.Json;
using System.Text.Json.Serialization;

namespace Clairvoyance.Collections.Domain;

public sealed class LanguageJsonConverter : JsonConverter<Language>
{
    public override Language Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return default;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string for Language.");
        }

        var s = reader.GetString();
        if (string.IsNullOrEmpty(s))
        {
            return default;
        }

        var parsed = Language.ParseFromKey(s) ?? Language.ParseFromValue(s);
        if (parsed is null)
        {
            throw new JsonException($"Unknown language key/value '{s}'.");
        }

        return parsed.Value;
    }

    public override void Write(Utf8JsonWriter writer, Language value, JsonSerializerOptions options)
    {
        if (string.IsNullOrEmpty(value.Key))
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Key);
    }
}
