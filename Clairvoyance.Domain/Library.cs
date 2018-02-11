using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Clairvoyance.Domain
{
    public class Library : CardSetBase, ICardSet
    {
        public Library() : base()
        { }

        public Library(IList<Card> cards) : base(cards)
        { }

        public void Shuffle()
        {
            // https://stackoverflow.com/a/1262619/244916
            var provider = new RNGCryptoServiceProvider();
            int n = Cards.Count;
            while (n > 1)
            {
                var box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                var k = (box[0] % n);
                n--;
                var value = Cards[k];
                Cards[k] = Cards[n];
                Cards[n] = value;
            }
        }

        public Card Draw()
        {
            if (Cards.Count == 0) { return null; }
            var topCard = Cards[0];
            Cards.RemoveAt(0);
            return topCard;
        }
    }
}