using System.Collections.Generic;

namespace Clairvoyance.Domain
{
    public class Hand : CardSetBase, ICardSet
    {
        public Hand() : base()
        { }

        public Hand(IList<Card> cards) : base(cards)
        { }

        public bool Pop(Card card)
        {
            var cardIndex = Cards.IndexOf(card);
            if (cardIndex > -1)
            {
                Cards.RemoveAt(cardIndex);
            }
            return cardIndex > -1;
        }

        public void Push(Card card)
        {
            Cards.Add(card);
        }
    }
}