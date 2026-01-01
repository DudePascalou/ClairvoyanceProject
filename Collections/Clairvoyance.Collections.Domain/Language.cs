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
    private static readonly Language _Spanish = new("sp", "Spanish"); // Gatherer alternative code...
    public static readonly Language French = new("fr", "French");
    public static readonly Language AncientGreek = new("grc", "Ancient Greek");
    public static readonly Language Hebrew = new("he", "Hebrew");
    public static readonly Language Italian = new("it", "Italian");
    public static readonly Language Japanese = new("ja", "Japanese");
    private static readonly Language _Japanese = new("jp", "Japanese"); // Gatherer alternative code...
    public static readonly Language Korean = new("ko", "Korean");
    public static readonly Language Latin = new("la", "Latin");
    public static readonly Language Phyrexian = new("ph", "Phyrexian");
    public static readonly Language Portuguese = new("pt", "Portuguese");
    public static readonly Language Russian = new("ru", "Russian");
    public static readonly Language Sanskrit = new("sa", "Sanskrit");
    public static readonly Language SimplifiedChinese = new("zhs", "Simplified Chinese");
    private static readonly Language _SimplifiedChinese = new("简体", "Simplified Chinese"); // Gatherer alternative code...
    public static readonly Language TraditionalChinese = new("zht", "Traditional Chinese");
    private static readonly Language _TraditionalChinese = new("繁體", "Traditional Chinese"); // Gatherer alternative code...

    /// <summary>
    /// List of all defined languages.
    /// </summary>
    private static readonly List<Language> All =
    [
        Arabic, German, English, Spanish, _Spanish, French, AncientGreek, Hebrew, Italian,
        Japanese, _Japanese, Korean, Latin, Phyrexian, Portuguese, Russian, Sanskrit,
        SimplifiedChinese, TraditionalChinese, _SimplifiedChinese, _TraditionalChinese
    ];

    public static Language? ParseFromKey(string key)
    {
        if (!string.IsNullOrEmpty(key) &&
            All.Exists(l => l.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)))
        {
            return All.First(l => l.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
        }
        return null;
    }

    public static Language? ParseFromValue(string value)
    {
        if (!string.IsNullOrEmpty(value) &&
            All.Exists(l => l.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase)))
        {
            return All.First(l => l.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase));
        }
        return null;
    }

    public override string ToString() => Value;
}
