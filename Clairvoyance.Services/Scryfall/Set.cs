using System.Text.Json.Serialization;

namespace Clairvoyance.Services.Scryfall;

/// <summary>
/// Represents a Scryfall set object.
/// Property names are mapped to Scryfall's snake_case JSON fields.
/// </summary>
public sealed class Set
{
    /// <summary>
    /// A content type for this object, always set.
    /// </summary>
    [JsonPropertyName("object")]
    public string Object { get; init; } = null!;

    /// <summary>
    /// A unique ID for this set on Scryfall that will not change.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    /// <summary>
    /// The unique three to six-letter code for this set.
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; init; } = null!;

    /// <summary>
    /// The unique code for this set on MTGO, which may differ from the regular code.
    /// </summary>
    [JsonPropertyName("mtgo_code")]
    public string? MtgoCode { get; init; }

    /// <summary>
    /// The unique code for this set on Arena, which may differ from the regular code.
    /// </summary>
    [JsonPropertyName("arena_code")]
    public string? ArenaCode { get; init; }

    /// <summary>
    /// This set’s ID on TCGplayer’s API, also known as the groupId.
    /// </summary>
    [JsonPropertyName("tcgplayer_id")]
    public int? TcgplayerId { get; init; }

    /// <summary>
    /// The English name of the set.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    /// <summary>
    /// A computer-readable classification for this set.
    /// </summary>
    [JsonPropertyName("set_type")]
    public string SetType { get; init; } = null!;

    /// <summary>
    /// The date the set was released or the first card was printed in the set.
    /// Uses <see cref="DateTime"/>; Scryfall provides a date (no time) in GMT-8.
    /// </summary>
    [JsonPropertyName("released_at")]
    public DateTime? ReleasedAt { get; init; }

    /// <summary>
    /// The block code for this set, if any.
    /// </summary>
    [JsonPropertyName("block_code")]
    public string? BlockCode { get; init; }

    /// <summary>
    /// The block or group name code for this set, if any.
    /// </summary>
    [JsonPropertyName("block")]
    public string? Block { get; init; }

    /// <summary>
    /// The set code for the parent set, if any.
    /// </summary>
    [JsonPropertyName("parent_set_code")]
    public string? ParentSetCode { get; init; }

    /// <summary>
    /// The number of cards in this set.
    /// </summary>
    [JsonPropertyName("card_count")]
    public int CardCount { get; init; }

    /// <summary>
    /// The denominator for the set’s printed collector numbers.
    /// </summary>
    [JsonPropertyName("printed_size")]
    public int? PrintedSize { get; init; }

    /// <summary>
    /// True if this set was only released in a video game.
    /// </summary>
    [JsonPropertyName("digital")]
    public bool Digital { get; init; }

    /// <summary>
    /// True if this set contains only foil cards.
    /// </summary>
    [JsonPropertyName("foil_only")]
    public bool FoilOnly { get; init; }

    /// <summary>
    /// True if this set contains only nonfoil cards.
    /// </summary>
    [JsonPropertyName("nonfoil_only")]
    public bool NonfoilOnly { get; init; }

    /// <summary>
    /// A link to this set’s permapage on Scryfall’s website.
    /// </summary>
    [JsonPropertyName("scryfall_uri")]
    public Uri ScryfallUri { get; init; } = null!;

    /// <summary>
    /// A link to this set object on Scryfall’s API.
    /// </summary>
    [JsonPropertyName("uri")]
    public Uri Uri { get; init; } = null!;

    /// <summary>
    /// A URI to an SVG file for this set’s icon on Scryfall’s CDN.
    /// </summary>
    [JsonPropertyName("icon_svg_uri")]
    public Uri IconSvgUri { get; init; } = null!;

    /// <summary>
    /// A Scryfall API URI that you can request to begin paginating over the cards in this set.
    /// </summary>
    [JsonPropertyName("search_uri")]
    public Uri SearchUri { get; init; } = null!;
}
