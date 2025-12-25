namespace Clairvoyance.Collections.Domain;

public class AppConfiguration
{
    public string? DatabaseDirectory { get; set; }
    public IEnumerable<CollectionAppConfiguration>? CollectionApps { get; set; }
}
