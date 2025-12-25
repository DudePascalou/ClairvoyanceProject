using Clairvoyance.Core;

namespace Clairvoyance.Collections.Domain;

/// <summary>
/// A language used or a Magic: The Gathering © card.
/// </summary>
public readonly struct Language : IKeyValue
{
    public string Key { get; }
    public string Value { get; }

    private Language(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public static readonly Language Arabic = new("ar", "Arabic");
    public static readonly Language German = new("de", "German");
    public static readonly Language English = new("en", "English");
    public static readonly Language Spanish = new("es", "Spanish");
    public static readonly Language French = new("fr", "French");
    public static readonly Language AncientGreek = new("grc", "Ancient Greek");
    public static readonly Language Hebrew = new("he", "Hebrew");
    public static readonly Language Italian = new("it", "Italian");
    public static readonly Language Japanese = new("ja", "Japanese");
    public static readonly Language Korean = new("ko", "Korean");
    public static readonly Language Latin = new("la", "Latin");
    public static readonly Language Phyrexian = new("ph", "Phyrexian");
    public static readonly Language Portuguese = new("pt", "Portuguese");
    public static readonly Language Russian = new("ru", "Russian");
    public static readonly Language Sanskrit = new("sa", "Sanskrit");
    public static readonly Language SimplifiedChinese = new("zhs", "Simplified Chinese");
    public static readonly Language TraditionalChinese = new("zht", "Traditional Chinese");

    /// <summary>
    /// List of all defined languages.
    /// </summary>
    private static readonly IReadOnlyList<Language> All =
    [
        Arabic, German, English, Spanish, French, AncientGreek, Hebrew, Italian,
        Japanese, Korean, Latin, Phyrexian, Portuguese, Russian, Sanskrit,
        SimplifiedChinese, TraditionalChinese
    ];

    public static Language? ParseFromKey(string key) => All.FirstOrDefault(l => l.Key == key);

    public static Language? ParseFromValue(string value) => All.FirstOrDefault(l => l.Value == value);

    public override string ToString() => Value;
}
