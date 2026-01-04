using Clairvoyance.Collections.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Clairvoyance.Collections.Moxfield;

public class MoxfieldLocalRepository : CollectionLocalRepositoryBase<MoxfieldConfiguration>
{
    public MoxfieldLocalRepository(IOptions<MoxfieldConfiguration> collectionAppConfig,
        ILoggerFactory loggerFactory, IMemoryCache memoryCache, JsonSerializerOptions jsonSerializerOptions)
        : base(collectionAppConfig, loggerFactory, memoryCache, jsonSerializerOptions)
    { }
}
