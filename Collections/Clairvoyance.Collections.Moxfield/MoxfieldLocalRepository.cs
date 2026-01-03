using Clairvoyance.Collections.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Clairvoyance.Collections.Moxfield;

public class MoxfieldLocalRepository : CollectionLocalRepositoryBase<MoxfieldConfiguration>
{
    public MoxfieldLocalRepository(IOptions<MoxfieldConfiguration> collectionAppConfig,
        ILoggerFactory loggerFactory, JsonSerializerOptions jsonSerializerOptions)
        : base(collectionAppConfig, loggerFactory, jsonSerializerOptions)
    { }
}
