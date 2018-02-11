using System.Collections.Generic;

namespace Clairvoyance.Domain
{
    public class Exile : CardSetBase, ICardSet
    {
        public Exile() : base()
        { }

        public Exile(IList<Card> cards) : base(cards)
        { }
    }
}