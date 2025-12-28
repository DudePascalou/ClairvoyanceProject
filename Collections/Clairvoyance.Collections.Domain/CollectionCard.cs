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
    public Grading? Grading { get; }

    /// <inheritdoc/>
    public bool IsFoil { get; }

    /// <inheritdoc/>
    public Language? Language { get; }

    public CollectionCard(CardId cardId, string appCardId, string appCollectionId,
        bool isFoil = false, Language? language = null, Grading? grading = null)
    {
        CardId = cardId;
        AppCardId = appCardId ?? throw new ArgumentNullException(nameof(appCardId));
        AppCollectionId = appCollectionId ?? throw new ArgumentNullException(nameof(appCollectionId));
        IsFoil = isFoil;
        Language = language;
        Grading = grading;
    }

    public override string ToString()
    {
        var grading = Grading != null ? ", " + Grading.ToString() : string.Empty;
        var language = Language != null ? ", " + Language.ToString() : string.Empty;
        var foil = IsFoil ? ", Foil" : string.Empty;
        return $"{CardId} ({AppCollectionId}{grading}{language}{foil})";
    }
}
