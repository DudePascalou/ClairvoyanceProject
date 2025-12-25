using Clairvoyance.Core;

namespace Clairvoyance.Collections.Domain;

/// <summary>
/// Grading from Professional Sports Authenticator.
/// https://www.psacard.com/resources/gradingstandards/#cards
/// </summary>
public readonly struct Grading : IKeyValue
{
    public string Key { get; }
    public string Value { get; }

    private Grading(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public static readonly Grading GemMint = new("10", "Gem Mint");
    public static readonly Grading Mint = new("9", "Mint");
    public static readonly Grading NearMintMint = new("8", "Near Mint-Mint");
    public static readonly Grading NearMint = new("7", "Near Mint");
    public static readonly Grading ExcellentMint = new("6", "Excellent-Mint");
    public static readonly Grading Excellent = new("5", "Excellent");
    public static readonly Grading VeryGoodExcellent = new("4", "Very Good-Excellent");
    public static readonly Grading VeryGood = new("3", "Very Good");
    public static readonly Grading Good = new("2", "Good");
    public static readonly Grading Fair = new("1.5", "Fair");
    public static readonly Grading Poor = new("1", "Poor");

    private static readonly IReadOnlyList<Grading> All =
    [
        GemMint, Mint, NearMintMint, NearMint, ExcellentMint, Excellent,
        VeryGoodExcellent, VeryGood, Good, Fair, Poor
    ];

    public static Grading? ParseFromKey(string key) => All.FirstOrDefault(g => g.Key == key);

    public static Grading? ParseFromValue(string value) => All.FirstOrDefault(g => g.Value == value);

    public override string ToString() => Value;
}
