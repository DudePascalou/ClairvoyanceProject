using Clairvoyance.Collections.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Clairvoyance.Collections.CardHunter;

public class CardHunterLocalRepository : CollectionLocalRepositoryBase<CardHunterConfiguration>
{
    public CardHunterLocalRepository(IOptions<CardHunterConfiguration> collectionAppConfig,
        ILoggerFactory loggerFactory, JsonSerializerOptions jsonSerializerOptions)
        : base(collectionAppConfig, loggerFactory, jsonSerializerOptions)
    { }
}
