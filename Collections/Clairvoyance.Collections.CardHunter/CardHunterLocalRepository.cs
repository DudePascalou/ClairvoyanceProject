using Clairvoyance.Collections.Domain;
using Clairvoyance.Collections.Services;
using Clairvoyance.Services.Scryfall;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Clairvoyance.Collections.CardHunter;

public class CardHunterLocalRepository : CollectionLocalRepositoryBase<CardHunterConfiguration>
{
    public CardHunterLocalRepository(IOptions<AppConfiguration> appConfig,
        IOptions<CardHunterConfiguration> collectionAppConfig,
        ILoggerFactory loggerFactory,
        JsonSerializerOptions jsonSerializerOptions,
        SetService setService)
        : base(appConfig, collectionAppConfig, loggerFactory, jsonSerializerOptions, setService)
    { }
}
