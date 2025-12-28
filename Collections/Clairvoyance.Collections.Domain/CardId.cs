namespace Clairvoyance.Collections.Domain;

/// <summary>
/// A representation of a Magic: The Gathering © card’s unique identifier, 
/// composed of an <see cref="ExpansionCode"/> and an <see cref="ExpansionNumber"/>.
/// </summary>
public readonly struct CardId
{
    /// <summary>
    /// Gets the expansion code of the card.
    /// </summary>
    public string ExpansionCode { get; }

    /// <summary>
    /// Gets the expansion number of the card in the expansion.
    /// </summary>
    public string ExpansionNumber { get; }

    public CardId(string cardId)
    {
        var idParts = cardId.Split('_', StringSplitOptions.RemoveEmptyEntries);
        if (idParts.Length == 2)
        {
            // Standard format XXX_888:
            ExpansionCode = idParts[0];
            ExpansionNumber = idParts[1];
        }
        else
        {
            // Fallback to XXX888 format:
            // TODO: won’t work for expansions with digits at the end of their code (e.g., "J25")
            ExpansionCode = GetExpansionCode(cardId) ?? throw new ArgumentException("Invalid card ID format.", nameof(cardId));
            ExpansionNumber = cardId[ExpansionCode.Length..];
        }
    }

    public CardId(string expansionCode, string expansionNumber)
    {
        ExpansionCode = expansionCode?.Trim('_') ?? throw new ArgumentNullException(nameof(expansionCode));
        ExpansionNumber = expansionNumber?.Trim('_') ?? throw new ArgumentNullException(nameof(expansionNumber));
    }

    public override readonly string ToString() => $"{ExpansionCode}_{ExpansionNumber}";

    private static string? GetExpansionCode(string cardId)
    {
        if (string.IsNullOrEmpty(cardId)) { return null; }

        int i = cardId.Length - 1;
        while (i >= 0 && char.IsDigit(cardId[i])) { i--; }

        return cardId[..(i + 1)];
    }
}
