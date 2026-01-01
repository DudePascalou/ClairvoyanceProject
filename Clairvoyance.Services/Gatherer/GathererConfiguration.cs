namespace Clairvoyance.Services.Gatherer;

public class GathererConfiguration
{
    /// <summary>
    /// Gets or sets the base URL for the Gatherer website.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the local JSON file path for storing expansions data.
    /// </summary>
    public string? SetsJsonFilePath { get; set; }

    /// <summary>
    /// Gets or sets the pagination query for the expansions resources.
    /// </summary>
    public string? SetsPaginationQuery { get; set; }

    /// <summary>
    /// Gets or sets the path for the expansions resources.
    /// </summary>
    public string? SetsPath { get; set; }
}
