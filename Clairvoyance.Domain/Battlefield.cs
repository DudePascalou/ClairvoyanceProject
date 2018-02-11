using Clairvoyance.Domain.Abilities;
using System.Collections.Generic;

namespace Clairvoyance.Domain
{
    public class Battlefield : CardSetBase, ICardSet
    {
        public Battlefield() : base()
        { }

        public Battlefield(IList<Card> cards) : base(cards)
        { }

        public void Enter(Card card)
        {
            // 302.6. When a creature enters the battlefield, it has “summoning sickness” 
            // until it has been under its controller’s control continuously since his or her most recent turn began.
            if (card.IsACreature &&
                !card.HasAbility<SummoningSicknessStaticAbility>() &&
                !card.HasAbility<HasteStaticAbility>())
            {
                card.Abilities.Add(new SummoningSicknessStaticAbility());
            }
            Cards.Add(card);
        }
    }
}