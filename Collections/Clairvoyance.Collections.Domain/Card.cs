namespace Clairvoyance.Collections.Domain;

/// <inheritdoc/>
public class Card : ICard
{
    /// <inheritdoc/>
    public string Id => ExpansionCode + ExpansionNumber;

    /// <inheritdoc/>
    public string ExpansionCode { get; }

    /// <inheritdoc/>
    public string ExpansionNumber { get; }

    /// <inheritdoc/>
    public string? Name { get; }

    public Card(string cardId)
    {
        ExpansionCode = GetExpansionCode(cardId) ?? throw new ArgumentNullException(nameof(cardId));
        ExpansionNumber = cardId[ExpansionCode.Length..] ?? throw new ArgumentNullException(nameof(cardId));
    }

    public Card(string expansionCode, string expansionNumber, string? name = null)
    {
        ExpansionCode = expansionCode ?? throw new ArgumentNullException(nameof(expansionCode));
        ExpansionNumber = expansionNumber ?? throw new ArgumentNullException(nameof(expansionNumber));
        Name = name;
    }

    private static string GetExpansionCode(string cardId)
    {
        if (string.IsNullOrEmpty(cardId)) return cardId;
        int i = cardId.Length - 1;
        while (i >= 0 && char.IsDigit(cardId[i])) { i--; }
        return cardId[..(i + 1)];
    }
}
