using System.Text.Json.Serialization;

namespace Clairvoyance.Services.Scryfall;

public class JsonContent<T> where T : class
{
    [JsonPropertyName("object")]
    public string Object { get; init; } = null!;

    [JsonPropertyName("data")]
    public T Data { get; init; } = null!;
}
