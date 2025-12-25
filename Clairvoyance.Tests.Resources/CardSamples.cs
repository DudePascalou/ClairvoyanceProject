using Clairvoyance.Domain;

namespace Clairvoyance.Tests.Resources;

public static class CardSamples
{
    public static Card Artifact
    {
        get
        {
            var card = Card.Fake;
            card.Type = CardType.Artifact;
            card.Types = new string[1] { CardType.Artifact };
            return card;
        }
    }
    public static Card Creature
    {
        get
        {
            var card = Card.Fake;
            card.Type = CardType.Creature;
            card.Types = new string[1] { CardType.Creature };
            return card;
        }
    }
    public static Card Enchantment
    {
        get
        {
            var card = Card.Fake;
            card.Type = CardType.Enchantment;
            card.Types = new string[1] { CardType.Enchantment };
            return card;
        }
    }
    public static Card Instant
    {
        get
        {
            var card = Card.Fake;
            card.Type = CardType.Instant;
            card.Types = new string[1] { CardType.Instant };
            return card;
        }
    }
    public static Card Land
    {
        get
        {
            var card = Card.Fake;
            card.Type = CardType.Land;
            card.Types = new string[1] { CardType.Land };
            return card;
        }
    }
    public static Card Planeswalker
    {
        get
        {
            var card = Card.Fake;
            card.Type = CardType.Planeswalker;
            card.Types = new string[1] { CardType.Planeswalker };
            return card;
        }
    }
    public static Card Sorcery
    {
        get
        {
            var card = Card.Fake;
            card.Type = CardType.Sorcery;
            card.Types = new string[1] { CardType.Sorcery };
            return card;
        }
    }
}
