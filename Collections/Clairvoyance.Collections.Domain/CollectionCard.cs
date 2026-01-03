namespace Clairvoyance.Collections.Domain;

/// <inheritdoc/>
public class CollectionCard : ICollectionCard
{
    /// <inheritdoc/>
    public string AppCollectionId { get; }

    /// <inheritdoc/>
    public string AppCardId { get; }

    /// <inheritdoc/>
    public CardId CardId { get; }

    /// <inheritdoc/>
    public string Grading { get; }

    /// <inheritdoc/>
    public bool IsFoil { get; }

    /// <inheritdoc/>
    public string Language { get; }

    public CollectionCard(CardId cardId, string appCardId, string appCollectionId,
        string grading, string language, bool isFoil = false)
    {
        AppCardId = appCardId ?? throw new ArgumentNullException(nameof(appCardId));
        AppCollectionId = appCollectionId ?? throw new ArgumentNullException(nameof(appCollectionId));
        CardId = cardId;
        Grading = grading ?? throw new ArgumentNullException(nameof(grading));
        IsFoil = isFoil;
        Language = language ?? throw new ArgumentNullException(nameof(language));
    }

    public override string ToString()
    {
        var grading = Grading != null ? ", " + Grading.ToString() : string.Empty;
        var language = Language != null ? ", " + Language.ToString() : string.Empty;
        var foil = IsFoil ? ", Foil" : string.Empty;
        return $"{CardId} ({AppCollectionId}{grading}{language}{foil})";
    }
}
