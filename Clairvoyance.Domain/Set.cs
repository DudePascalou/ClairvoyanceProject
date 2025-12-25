namespace Clairvoyance.Domain;

/// <summary>
/// A Magic The Gathering © Set.
/// </summary>
/// <remarks>
/// https://mtgjson.com
/// </remarks>
public class Set
{
    /// <summary>
    /// The name of the set.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The set's abbreviated code.
    /// </summary>
    public string Code { get; set; }
    /// <summary>
    /// The code that magiccards.info uses for the set. 
    /// Only present if magiccards.info has this set
    /// </summary>
    public string MagicCardsInfoCode { get; set; }
    /// <summary>
    /// When the set was released (YYYY-MM-DD). 
    /// For promo sets, the date the first card was released.
    /// </summary>
    public string ReleaseDate { get; set; }
    /// <summary>
    /// The type of border on the cards, either "white", "black" or "silver"
    /// </summary>
    public string Border { get; set; }
    /// <summary>
    /// Type of set.
    /// </summary>
    /// <example>
    /// "core", "expansion", "reprint", "box", "un",
    /// "from the vault", "premium deck", "duel deck",
    /// "starter", "commander", "planechase", "archenemy",
    /// "promo", "vanguard", "masters", "conspiracy", "masterpiece"
    /// </example>
    public string Type { get; set; }
    /// <summary>
    /// The block this set is in,
    /// </summary>
    public string Block { get; set; }
    /// <summary>
    /// Booster contents for this set.
    /// </summary>
    public string[] Booster { get; set; }
    public Translation[] Translations { get; set; }
    public string MkmName { get; set; }
    public int MkmId { get; set; }
    /// <summary>
    /// Present and set to true if the set was only released online.
    /// </summary>
    public bool OnlineOnly { get; set; }
    /// <summary>
    /// The cards in the set.
    /// </summary>
    public Card[] Cards { get; set; }

    /*
"gathererCode" : "NE",         // The code that Gatherer uses for the set. Only present if different than 'code'
"oldCode" : "NEM",             // An old style code used by some Magic software. Only present if different than 'gathererCode' and 'code'
     */

    public override string ToString()
    {
        return string.Format("[{0}] {1}", Code, Name);
    }
}