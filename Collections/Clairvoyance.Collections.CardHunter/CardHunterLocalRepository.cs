using Clairvoyance.Collections.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Clairvoyance.Collections.CardHunter;

public class CardHunterLocalRepository : CollectionLocalRepositoryBase<CardHunterConfiguration>
{
    public CardHunterLocalRepository(IOptions<CardHunterConfiguration> collectionAppConfig,
        ILoggerFactory loggerFactory, IMemoryCache memoryCache, JsonSerializerOptions jsonSerializerOptions)
        : base(collectionAppConfig, loggerFactory, memoryCache, jsonSerializerOptions)
    { }
}
