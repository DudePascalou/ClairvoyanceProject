namespace Clairvoyance.Collections.Domain;

/// <summary>
/// A basic representation of a Magic: The Gathering © card.
/// </summary>
public interface ICard
{
    /// <summary>
    /// Gets the unique identifier for the card.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the expansion code (usually 3 alphanumerical characters identifying the set).
    /// </summary>
    string ExpansionCode { get; }

    /// <summary>
    /// Gets the expansion number (usually a number identifying the card’s index in the set).
    /// </summary>
    string ExpansionNumber { get; }

    /// <summary>
    /// Gets the original name.
    /// </summary>
    string? Name { get; }
}
