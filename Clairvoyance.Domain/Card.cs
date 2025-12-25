using Clairvoyance.Domain.Abilities;
using Clairvoyance.Domain.Costs;

namespace Clairvoyance.Domain;

/// <summary>
/// A Magic The Gathering © card.
/// </summary>
/// <remarks>
/// https://mtgjson.com
/// </remarks>
public class Card
{
    private static Card _Fake;
    public static Card Fake
    {
        get
        {
            if (_Fake == null) { _Fake = new Card(); }
            return _Fake;
        }
    }

    #region Data

    /// <summary>
    /// The card name. For split, double-faced and flip cards, 
    /// just the name of one side of the card. 
    /// Basically each 'sub-card' has its own record.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Only used for split, flip, double-faced, and meld cards. 
    /// Will contain all the names on this card, front or back. 
    /// For meld cards, the first name is the card with the meld ability, 
    /// which has the top half on its back, the second name is the card 
    /// with the reminder text, and the third name is the melded back face.
    /// </summary>
    public string[] Names { get; set; }
    /// <summary>
    /// The mana cost of this card. Consists of one or more mana symbols.
    /// </summary>
    public string ManaCost { get; set; }
    /// <summary>
    /// Converted mana cost. Always a number. 
    /// NOTE: cmc may have a decimal point as cards from unhinged may contain "half mana" 
    /// (such as 'Little Girl' with a cmc of 0.5). 
    /// Cards without this field have an implied cmc of zero as per rule 202.3a.
    /// </summary>
    public float Cmc { get; set; }
    /// <summary>
    /// The card colors. Usually this is derived from the casting cost, 
    /// but some cards are special (like the back of double-faced cards and Ghostfire).
    /// </summary>
    public string[] Colors { get; set; }
    /// <summary>
    /// This is created reading all card color information and costs. 
    /// It is the same for double-sided cards 
    /// (if they have different colors, the identity will have both colors). 
    /// It also identifies all mana symbols in the card (cost and text). 
    /// Mostly used on commander decks.
    /// </summary>
    public string[] ColorIdentity { get; set; }
    /// <summary>
    /// The card type. This is the type you would see on the card if printed today. 
    /// Note: The dash is a UTF8 'long dash' as per the MTG rules
    /// </summary>
    /// <example>"Legendary Creature — Angel"</example>
    public string Type { get; set; }

    /// <summary>
    /// The supertypes of the card. 
    /// These appear to the far left of the card type. 
    /// </summary>
    /// <example>Basic, Legendary, Snow, World or Ongoing</example>
    public string[] Supertypes { get; set; }
    /// <summary>
    /// The types of the card. 
    /// These appear to the left of the dash in a card type. 
    /// </summary>
    /// <example>Instant, Sorcery, Artifact, Creature, Enchantment, Land, Planeswalker</example>
    public string[] Types { get; set; }
    /// <summary>
    /// The subtypes of the card. 
    /// These appear to the right of the dash in a card type. 
    /// Usually each word is its own subtype. 
    /// </summary>
    /// <example>Trap, Arcane, Equipment, Aura, Human, Rat, Squirrel, etc.</example>
    public string[] Subtypes { get; set; }
    /// <summary>
    /// The rarity of the card.
    /// </summary>
    /// <example>Common, Uncommon, Rare, Mythic Rare, Special, Basic Land.</example>
    public string Rarity { get; set; }
    /// <summary>
    /// The text of the card. 
    /// May contain mana symbols and other symbols.
    /// </summary>
    public string Text { get; set; }
    /// <summary>
    /// The flavor text of the card.
    /// </summary>
    public string Flavor { get; set; }
    /// <summary>
    /// The artist of the card. 
    /// 
    /// This may not match what is on the card as MTGJSON corrects many card misprints.
    /// </summary>
    public string Artist { get; set; }
    /// <summary>
    /// The card number. 
    /// This is printed at the bottom-center of the card in small text. 
    /// This is a string, not an integer, because some cards have letters in their numbers.
    /// </summary>
    public string Number { get; set; }
    /// <summary>
    /// The power of the card. 
    /// This is only present for creatures. 
    /// This is a string, not an integer, because some cards have powers like: "1+*"
    /// </summary>
    public string Power { get; set; }
    /// <summary>
    /// The toughness of the card. 
    /// This is only present for creatures. 
    /// This is a string, not an integer, because some cards have toughness like: "1+*"
    /// </summary>
    public string Toughness { get; set; }
    /// <summary>
    /// The loyalty of the card. 
    /// This is only present for planeswalkers.
    /// </summary>
    public string Loyalty { get; set; }
    /// <summary>
    /// The card layout.
    /// </summary>
    /// <example>normal, split, flip, double-faced, 
    /// token, plane, scheme, phenomenon, leveler, vanguard, meld.</example>
    public string Layout { get; set; }
    /// <summary>
    /// The multiverseid of the card on Wizard's Gatherer web page.
    /// Cards from sets that do not exist on Gatherer will NOT have a multiverseid.
    /// Sets not on Gatherer are: ATH, ITP, DKM, RQS, DPA and all sets with a 4 letter code that starts with a lowercase 'p'.
    /// </summary>
    public int Multiverseid { get; set; }
    /// <summary>
    /// If a card has alternate art (for example, 4 different Forests, or the 2 Brothers Yamazaki) 
    /// then each other variation's multiverseid will be listed here, 
    /// NOT including the current card's multiverseid. 
    /// NOTE: Only present for sets that exist on Gatherer.
    /// </summary>
    public string[] Variations { get; set; }
    /// <summary>
    /// This used to refer to the mtgimage.com file name for this card.
    /// mtgimage.com has been SHUT DOWN by Wizards of the Coast.
    /// This field will continue to be set correctly and is now only useful for UID purposes.
    /// </summary>
    public string ImageName { get; set; }
    /// <summary>
    /// A unique id for this card. It is made up by doing an SHA1 hash of 
    /// setCode + cardName + cardImageName.
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// The watermark on the card. 
    /// Note: Split cards don't currently have this field set, 
    /// despite having a watermark on each side of the split card.
    /// </summary>
    public string Watermark { get; set; }
    /// <summary>
    /// If the border for this specific card is DIFFERENT than the border specified in the top level set JSON, 
    /// then it will be specified here. 
    /// (Example: Unglued has silver borders, except for the lands which are black bordered)
    /// </summary>
    public string Border { get; set; }
    /// <summary>
    /// If this card was a timeshifted card in the set.
    /// </summary>
    public bool Timeshifted { get; set; }
    /// <summary>
    /// Maximum hand size modifier. Only exists for Vanguard cards.
    /// </summary>
    public int Hand { get; set; }
    /// <summary>
    /// Starting life total modifier. Only exists for Vanguard cards.
    /// </summary>
    public int Life { get; set; }
    /// <summary>
    /// Set to true if this card is reserved by Wizards Official Reprint Policy.
    /// </summary>
    public bool Reserved { get; set; }
    /// <summary>
    /// The date this card was released. 
    /// This is only set for promo cards. 
    /// The date may not be accurate to an exact day and month, 
    /// thus only a partial date may be set (YYYY-MM-DD or YYYY-MM or YYYY). 
    /// Some promo cards do not have a known release date.
    /// </summary>
    /// <example>"2010-07-22" or "2010-07" or "2010"</example>
    public string ReleaseDate { get; set; }
    /// <summary>
    /// Set to true if this card was only released as part of a core box set. 
    /// These are technically part of the core sets and are tournament legal despite not being available in boosters.
    /// </summary>
    public bool Starter { get; set; }
    /// <summary>
    /// Number used by MagicCards.info for their indexing URLs (Most often it is the card number in the set)
    /// </summary>
    public string MciNumber { get; set; }

    #endregion

    public Card()
    {
        Abilities = new List<IAbility>();
    }

    public Card(string name, string manaCost)
    {
        Name = name;
        ManaCost = manaCost;
        Abilities = new List<IAbility>();
    }

    public override string ToString()
    {
        return string.Format("{0}\t{1}", Name, ManaCost);
    }

    #region IsA

    private bool IsA(string type)
    {
        // Most of the time, a card is of one type.
        // Check first the main type for optimization :
        return Type == type || Types.Any(t => t == type);
    }

    public bool IsAnArtifact { get { return IsA(CardType.Artifact); } }
    public bool IsACreature { get { return IsA(CardType.Creature); } }
    public bool IsAnEnchantment { get { return IsA(CardType.Enchantment); } }
    public bool IsAnInstant { get { return IsA(CardType.Instant); } }
    public bool IsALand { get { return IsA(CardType.Land); } }
    public bool IsAPlaneswalker { get { return IsA(CardType.Planeswalker); } }
    public bool IsASorcery { get { return IsA(CardType.Sorcery); } }

    #endregion

    #region Equality

    public override bool Equals(object obj)
    {
        var card = obj as Card;
        if (card == null)
        {
            return false;
        }
        else
        {
            return Instance != Guid.Empty
                && card.Instance != Guid.Empty
                && Instance == card.Instance;
        }
    }

    public override int GetHashCode()
    {
        return Instance.GetHashCode();
    }

    #endregion

    #region In Game

    /// <summary>
    /// Gets or sets a <see cref="Guid"/> to uniquely identify a card in a game.
    /// </summary>
    public Guid Instance { get; set; }

    public TypedMana TypedManaCost { get { return TypedMana.Parse(ManaCost); } }

    #endregion

    #region Abilities

    public ICollection<IAbility> Abilities { get; set; }

    public bool HasAbility<T>() where T : IAbility
    {
        return Abilities.Any(a => a.GetType() == typeof(T));
    }

    public T GetAbility<T>() where T : class, IAbility
    {
        return Abilities.FirstOrDefault(a => a.GetType() == typeof(T)) as T;
    }

    /// <summary>
    /// Gets the <typeparamref name="T"/> ability that is available (<see cref="IAbility.Condition"/> is true
    /// and <see cref="ICost.CanPay()"/> in case of <see cref="IActivatedAbility"/>).
    /// </summary>
    /// <typeparam name="T"><see cref="IAbility"/> type.</typeparam>
    /// <returns>The <typeparamref name="T"/> ability that is available.</returns>
    public T GetAvailableAbility<T>() where T : class, IAbility
    {
        return Abilities.FirstOrDefault(a => a.IsAvailable && a.GetType() == typeof(T)) as T;
    }

    #endregion

    #region Tap/Untap

    public bool IsTapped { get; private set; }

    public bool CanTap()
    {
        return !IsTapped && !HasAbility<SummoningSicknessStaticAbility>();
    }

    public bool CanUntap()
    {
        return IsTapped && !HasAbility<SummoningSicknessStaticAbility>();
    }

    public void Tap()
    {
        IsTapped = true;
    }

    public void Untap()
    {
        IsTapped = false;
    }

    #endregion
}