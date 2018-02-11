using Clairvoyance.Domain;
using System.Linq;

namespace Clairvoyance.Tests.Resources
{
    public static class DeckBuilder
    {
        public static Deck Build(string name)
        {
            return Build(name, 0);
        }

        public static Deck Build(string name, int fakeCardsCount)
        {
            return new Deck(name, Format.None, Enumerable.Repeat(Card.Fake, fakeCardsCount).ToList());
        }
    }
}
