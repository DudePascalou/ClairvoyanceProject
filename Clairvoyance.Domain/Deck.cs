namespace Clairvoyance.Domain;

/// <summary>
/// A deck of Magic The Gathering © cards.
/// </summary>
public class Deck : CardSetBase, ICardSet
{
    private static Deck _Empty;
    public static Deck Empty
    {
        get
        {
            if (_Empty == null) { _Empty = new Deck("No name", Format.None, new List<Card>()); }
            return _Empty;
        }
    }

    public string Name { get; set; }
    public ICollection<Card> SideboardCards { get; set; }
    public Format Format { get; set; }

    public Deck() : base()
    { }

    //public Deck(IList<Card> cards) : base(cards)
    //{
    //    foreach (var card in Cards)
    //    {
    //        Add(card);
    //    }
    //}

    public Deck(string name, Format format) : this(name, format, new List<Card>())
    { }

    public Deck(string name, Format format, IList<Card> cards, IList<Card> sideboardCards = null) : base(cards)
    {
        Name = name;
        Format = format;
        SideboardCards = sideboardCards ?? new List<Card>();
        foreach (var card in Cards)
        {
            card.Instance = Guid.NewGuid();
        }
    }

    public void Add(Card card, bool toSideboard = false)
    {
        card.Instance = Guid.NewGuid();
        if (toSideboard)
        {
            SideboardCards.Add(card);
        }
        else
        {
            Cards.Add(card);
        }
    }

    public void Add(Card card, int count, bool toSideboard = false)
    {
        for (int i = 0; i < count; i++)
        {
            Add(card, toSideboard);
        }
    }

    public bool IsLegal()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return string.Format("{0} ({1}, {2} cards)", Name, Format, Cards.Count);
    }
}