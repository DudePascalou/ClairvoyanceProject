using System.Text.Json;

namespace Clairvoyance.Collections.Domain;

public class CollectionLocalRepository
{
    private readonly JsonSerializerOptions _JsonSerializerOptions;
    public string BaseDirectory { get; }

    public CollectionLocalRepository(string baseDirectory, JsonSerializerOptions jsonSerializerOptions)
    {
        BaseDirectory = baseDirectory;
        _JsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task SaveCardsAsync(IEnumerable<CollectionCard> cards, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(BaseDirectory))
        {
            Directory.CreateDirectory(BaseDirectory);
        }

        var cardsByExpansion = cards
            .GroupBy(c => new Card(c.CardId).ExpansionCode) // TODO: find another way to parse expansion code
            .OrderBy(g => g.Key);

        foreach (var card in cardsByExpansion)
        {
            var expansionFilePath = Path.Combine(BaseDirectory, card.Key + ".json");
            var existingCardsJson = File.Exists(expansionFilePath)
                ? await File.ReadAllTextAsync(expansionFilePath, cancellationToken)
                : "[]";
            var existingCards = JsonSerializer.Deserialize<List<CollectionCard>>(existingCardsJson, _JsonSerializerOptions) ?? [];
            existingCards.AddRange(card);
            var json = JsonSerializer.Serialize(existingCards, _JsonSerializerOptions);
            await File.WriteAllTextAsync(expansionFilePath, json, cancellationToken);
        }
    }
}
