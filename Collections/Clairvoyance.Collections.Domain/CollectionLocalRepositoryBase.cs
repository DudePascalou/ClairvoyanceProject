using Clairvoyance.Services.Scryfall;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Clairvoyance.Collections.Domain;

public abstract class CollectionLocalRepositoryBase<T>
    where T : CollectionAppConfigurationBase
{
    private readonly ILogger _Logger;
    private readonly JsonSerializerOptions _JsonSerializerOptions;
    private readonly SetService _SetService;

    public string BaseDirectory { get; }

    protected CollectionLocalRepositoryBase(IOptions<AppConfiguration> appConfig,
        IOptions<T> collectionAppConfig,
        ILoggerFactory loggerFactory,
        JsonSerializerOptions jsonSerializerOptions,
        SetService setService)
    {
        _ = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _Logger = loggerFactory.CreateLogger(GetType());
        _JsonSerializerOptions = jsonSerializerOptions;
        _SetService = setService;

        _ = appConfig.Value?.DatabaseDirectory ?? throw new ArgumentNullException(nameof(appConfig));
        _ = collectionAppConfig.Value?.Key ?? throw new ArgumentNullException(nameof(collectionAppConfig));
        BaseDirectory = Path.Combine(appConfig.Value.DatabaseDirectory, collectionAppConfig.Value.Key);
    }

    public async Task SaveCardsAsync(IEnumerable<CollectionCard> cards, CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(BaseDirectory))
        {
            Directory.CreateDirectory(BaseDirectory);
        }

        var cardsByExpansion = cards
            .GroupBy(c => c.CardId.ExpansionCode)
            .OrderBy(g => g.Key);

        foreach (var cardsGroup in cardsByExpansion)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _Logger.LogInformation("Cancellation requested. Stopping card persistance.");
                break;
            }

            var expansionFilePath = Path.Combine(BaseDirectory, GetExpansionFileName(cardsGroup.Key));
            var existingCardsJson = File.Exists(expansionFilePath)
                ? await File.ReadAllTextAsync(expansionFilePath, cancellationToken)
                : "[]";
            var existingCards = JsonSerializer.Deserialize<List<CollectionCard>>(existingCardsJson, _JsonSerializerOptions) ?? [];
            existingCards.AddRange(cardsGroup); // TODO: merge cards, instead of just adding them.
            var json = JsonSerializer.Serialize(existingCards, _JsonSerializerOptions);

            await File.WriteAllTextAsync(expansionFilePath, json, cancellationToken);
        }
    }

    private string GetExpansionFileName(string expansionCode)
    {
        // Example: "20230929-ABC-Expansion_Name.json"
        const string fileNameTemplate = "{0:yyyyMMdd}-{1}-{2}.json";
        var set = _SetService.GetSetByCode(expansionCode);
        if (set == null)
        {
            _Logger.LogWarning("Set with code '{SetCode}' not found in SetService. Skipping saving cards for this set.", expansionCode);
            return string.Format(fileNameTemplate, DateTime.MinValue, expansionCode, "unknown");
        }
        else
        {
            return string.Format(fileNameTemplate, set.ReleasedAt ?? DateTime.MinValue, expansionCode, set.Name.Replace(":", string.Empty).Replace(' ', '_'));
        }
    }
}
