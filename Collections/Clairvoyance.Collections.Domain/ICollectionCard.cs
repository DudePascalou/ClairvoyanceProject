namespace Clairvoyance.Collections.Domain;

/// <summary>
/// A card specimen in a user’s collection.
/// </summary>
public interface ICollectionCard
{
    /// <summary>
    /// Gets the card’s unique identifier
    /// from the App database.
    /// </summary>
    string AppCardId { get; }

    /// <summary>
    /// Gets the unique identifier for the card specimen 
    /// in the user’s collection.
    /// </summary>
    string AppCollectionId { get; }

    /// <summary>
    /// Gets the card’s unique identifier.
    /// </summary>
    CardId CardId { get; }

    /// <summary>
    /// Gets the grading information of the current card specimen.
    /// </summary>
    Grading? Grading { get; }

    /// <summary>
    /// Indicates whether the current card specimen is a foil version.
    /// </summary>
    bool IsFoil { get; }

    /// <summary>
    /// Gets the language of the current card specimen.
    /// </summary>
    Language? Language { get; }
}
