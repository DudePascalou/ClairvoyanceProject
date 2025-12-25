using System.Text.Json;
using System.Text.Json.Serialization;

namespace Clairvoyance.Collections.Domain;

public sealed class ICardJsonConverter : JsonConverter<ICard?>
{
    public override ICard? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null) return null;

        // Deserialize into the concrete Card type
        var card = JsonSerializer.Deserialize<Card?>(ref reader, options);
        return card;
    }

    public override void Write(Utf8JsonWriter writer, ICard? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        // Serialize using the runtime concrete type (Card)
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
