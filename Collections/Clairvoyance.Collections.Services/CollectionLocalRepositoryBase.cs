using Clairvoyance.Collections.Domain;
using Clairvoyance.Core.Exceptions;
using Clairvoyance.Core.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Clairvoyance.Collections.Services;

public abstract class CollectionLocalRepositoryBase<T>
    where T : CollectionAppConfigurationBase
{
    private const string _SetsFileName = "_Sets.json";
    private readonly ILogger _Logger;
    private readonly T _Configuration;
    private readonly IMemoryCache _Cache;
    private readonly JsonSerializerOptions _JsonSerializerOptions;

    protected CollectionLocalRepositoryBase(IOptions<T> collectionAppConfig,
        ILoggerFactory loggerFactory,
        IMemoryCache memoryCache,
        JsonSerializerOptions jsonSerializerOptions)
    {
        _ = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _Logger = loggerFactory.CreateLogger(GetType());
        _Cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _JsonSerializerOptions = jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
        _Configuration = collectionAppConfig?.Value ?? throw new ArgumentNullException(nameof(collectionAppConfig));
    }

    #region Sets

    public async Task<IEnumerable<SetInfo>> LoadSetsAsync(CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_Configuration.BaseDirectory!))
        {
            throw new NotFoundException("Collection directory not found.");
        }
        var setsFilePath = Path.Combine(_Configuration.BaseDirectory!, _SetsFileName);

        if (_Cache.TryGetValue(setsFilePath, out IEnumerable<SetInfo>? cachedSets))
        {
            return cachedSets!;
        }

        if (!SetsExists())
        {
            throw new NotFoundException("Sets file not found.");
        }

        var json = await File.ReadAllTextAsync(setsFilePath, cancellationToken);
        var sets = JsonSerializer.Deserialize<List<SetInfo>>(json, _JsonSerializerOptions);

        _Cache.Set(setsFilePath, sets, TimeSpan.FromDays(1));

        return sets ?? [];
    }

    public async Task SaveSetsAsync(IEnumerable<SetInfo> sets,
        CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_Configuration.BaseDirectory!))
        {
            Directory.CreateDirectory(_Configuration.BaseDirectory!);
        }

        var setsFilePath = Path.Combine(_Configuration.BaseDirectory!, _SetsFileName);
        var json = JsonSerializer.Serialize(sets, _JsonSerializerOptions);
        await File.WriteAllTextAsync(setsFilePath, json, cancellationToken);
    }

    public bool SetsExists()
    {
        var setsFilePath = Path.Combine(_Configuration.BaseDirectory!, _SetsFileName);
        return File.Exists(setsFilePath);
    }

    #endregion

    #region Collection Cards

    public async Task<IEnumerable<CollectionCard>> LoadCollectionAsync(CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_Configuration.BaseDirectory!))
        {
            throw new NotFoundException("Collection directory not found.");
        }

        var collectionFiles = Directory.GetFiles(_Configuration.BaseDirectory!, "*.json")
            .Where(f => !f.EndsWith(_SetsFileName, StringComparison.InvariantCultureIgnoreCase))
            .Where(f => !f.Contains(SetInfo.Unknown.Name, StringComparison.InvariantCultureIgnoreCase))
            .ToList();

        var allCards = new List<CollectionCard>();
        foreach (var filePath in collectionFiles)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _Logger.LogDebug("Cancellation requested. Stopping collection loading.");
                break;
            }

            var json = await File.ReadAllTextAsync(filePath, cancellationToken);
            var cards = JsonSerializer.Deserialize<List<CollectionCard>>(json, _JsonSerializerOptions);
            if (cards != null)
            {
                allCards.AddRange(cards);
            }
        }

        return allCards;
    }

    public async Task SaveCollectionAsync(IEnumerable<CollectionCard> cards,
        CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(_Configuration.BaseDirectory!))
        {
            Directory.CreateDirectory(_Configuration.BaseDirectory!);
        }

        var sets = (await LoadSetsAsync(cancellationToken)).ToList();
        var cardsByExpansion = cards
            .GroupBy(c => c.CardId.ExpansionCode)
            .OrderBy(g => g.Key);

        foreach (var cardsGroup in cardsByExpansion)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _Logger.LogDebug("Cancellation requested. Stopping card persistance.");
                break;
            }

            var expansionFilePath = Path.Combine(_Configuration.BaseDirectory!, GetExpansionFileName(cardsGroup.Key, sets));
            var existingCardsJson = File.Exists(expansionFilePath)
                ? await File.ReadAllTextAsync(expansionFilePath, cancellationToken)
                : "[]";
            var existingCards = JsonSerializer.Deserialize<List<CollectionCard>>(existingCardsJson, _JsonSerializerOptions) ?? [];
            existingCards.AddRange(cardsGroup); // TODO: merge cards, instead of just adding them.
            var json = JsonSerializer.Serialize(existingCards, _JsonSerializerOptions);

            await File.WriteAllTextAsync(expansionFilePath, json, cancellationToken);
        }
    }

    private string GetExpansionFileName(string expansionCode, ICollection<SetInfo> sets)
    {
        var set = sets.FirstOrDefault(s => expansionCode.Equals(s.Code, StringComparison.InvariantCultureIgnoreCase));
        if (set == null)
        {
            set = SetInfo.Unknown;
            _Logger.LogWarning("Expansion with code '{ExpansionCode}' not found.", expansionCode);
        }

        // Example: "20230929-ABC-Expansion_Name.json"
        const string fileNameTemplate = "{0:yyyyMMdd}-{1}-{2}.json";
        return string.Format(fileNameTemplate, set.ReleaseDate, set.Code, set.Name.CleanFileName());
    }

    #endregion
}
