using Clairvoyance.Domain;

namespace Clairvoyance.Tests.Resources
{
    public static class DeckExtensions
    {
        public static Deck With(this Deck deck, string cardJson)
        {
            deck.Add(CardBuilder.GetFromJson(cardJson));
            return deck;
        }

        public static Deck With(this Deck deck, int cardCount, string cardJson)
        {
            deck.Add(CardBuilder.GetFromJson(cardJson), cardCount);
            return deck;
        }
    }
}
