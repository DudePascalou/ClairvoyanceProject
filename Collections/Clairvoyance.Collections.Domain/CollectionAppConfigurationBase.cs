namespace Clairvoyance.Collections.Domain;

/// <summary>
/// Base class for collection applications configuration.
/// </summary>
public abstract class CollectionAppConfigurationBase
{
    /// <summary>
    /// Gets or sets the base directory for storing collection data.
    /// </summary>
    public string? BaseDirectory { get; set; }

    /// <summary>
    /// Gets or sets the base URL for the online collection application.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the URL path for the user’s collection.
    /// </summary>
    public string? CollectionPath { get; set; }

    /// <summary>
    /// Gets or sets the name of the online collection application.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the URL pagination query for the expansions resources.
    /// </summary>
    public string? SetsPaginationQuery { get; set; }

    /// <summary>
    /// Gets or sets the URL path for the expansions resources.
    /// </summary>
    public string? SetsPath { get; set; }
}
