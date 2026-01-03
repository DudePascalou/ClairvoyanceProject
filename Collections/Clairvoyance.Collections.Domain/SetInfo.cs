namespace Clairvoyance.Collections.Domain;

public class SetInfo
{
    public static readonly SetInfo Unknown = new() { Code = "---", Name = "Unknown", CardCount = 0, ReleaseDate = DateOnly.MinValue };

    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public int CardCount { get; init; }
    public DateOnly ReleaseDate { get; init; }
    public string Url { get; init; } = null!;

    public override string ToString()
    {
        return $"[{Code}] {Name} ({CardCount} cards, released on {ReleaseDate:yyyy-MM-dd})";
    }
}
