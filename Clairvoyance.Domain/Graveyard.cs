using System.Collections.Generic;

namespace Clairvoyance.Domain
{
    public class Graveyard : CardSetBase, ICardSet
    {
        public Graveyard() : base()
        { }

        public Graveyard(IList<Card> cards) : base(cards)
        { }
    }
}