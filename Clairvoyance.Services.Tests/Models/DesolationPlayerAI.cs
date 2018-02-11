using Clairvoyance.Core;
using Clairvoyance.Domain;
using Clairvoyance.Domain.Abilities;
using Clairvoyance.Domain.Algorithms;
using Clairvoyance.Domain.Effects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clairvoyance.Services.Tests.Models
{
    /// <summary>
    /// <see cref="IPlayerAI"/> implementation 
    /// to choose a card to play for my Desolation Deck.
    /// </summary>
    class DesolationPlayerAI : IPlayerAI
    {
        /// <summary>
        /// 
        /// </summary>
        private Func<IOrderedEnumerable<Card>> _CardsToPlayFirstQuery;
        public Player Player { get; set; }

        //private IDictionary<string, TypedMana> _AvailableManaInHandByName;
        //private IDictionary<string, TypedMana> _AvailableManaOnBattlefieldByName;

        public DesolationPlayerAI()
        {
            //_AvailableManaInHandByName = new Dictionary<string, TypedMana>
            //{
            //    { "Elvish Spirit Guide", TypedMana.Parse("{G}") }
            //};
            //_AvailableManaOnBattlefieldByName = new Dictionary<string, TypedMana>
            //{
            //    { "Magus of the Library", TypedMana.Parse("{C}") },
            //    { "Llanowar Elves", TypedMana.Parse("{G}") },
            //    { "Hedron Archive", TypedMana.Parse("{C}{C}") },
            //    // TODO :{ "Lotus Petal", TypedMana.Parse("{A}") },
            //    { "Island", TypedMana.Parse("{U}") },
            //    { "Forest", TypedMana.Parse("{G}") },
            //    { "Ghost Quarter", TypedMana.Parse("{C}") },
            //    { "Mishra's Factory", TypedMana.Parse("{C}") },
            //    { "Spawning Bed", TypedMana.Parse("{C}") },
            //    { "Unclaimed Territory", TypedMana.Parse("{C}") },
            //};
            _CardsToPlayFirstQuery = () => Player.Hand.Cards
                .Where(c => !c.IsALand)
                .OrderByDescending(c => c.Cmc) // 1. Aggro : play highest CMC card first
                .ThenByDescending(c => c.HasAbility<ManaActivatedAbility>()) // 2. Ramp : play mana producing cards first
                .ThenByDescending(c => c.HasAbility<DrawCardActivatedAbility>()); // 3. Fasten drawing
        }

        public Card ChooseCard()
        {
            // 1. Play Lotus Petal, if any :
            if (Player.Hand.Cards.Any(c => c.Name == "Lotus Petal"))
            {
                return Player.Hand.Cards.First(c => c.Name == "Lotus Petal");
            }
            // 2. Play a land, if any and possible :
            if (Player.CanPlayLand)
            {
                return ChooseLand();
            }
            // 3. Add player's available mana :
            var totalManaAvailable = Player.GetAvailableMana();
            // 4.5.6. Try to cast the most expensive card in hand or a ramp card, or a draw card :
            var cardToPlay = _CardsToPlayFirstQuery().FirstOrDefault(c => totalManaAvailable.IsEnoughFor(c.TypedManaCost));
            if (cardToPlay != null)
            {
                return cardToPlay;
            }

            // TODO 7.Draw card

            return null;
        }

        private Card ChooseLand()
        {
            // TODO : bilands, lands that enters the battlefield tapped,...
            var lands = Player.Hand.Lands().ToList();
            if (!lands.Any())
            {
                // 1. No land...
                return null;
            }
            else if (lands.Count == 1)
            {
                // 2. Only one land in hand :
                return lands[0];
            }
            else if (lands.Select(c => c.Name).Distinct().Count() == 1)
            {
                // 3. Same land :
                return lands[0];
            }
            else
            {
                // 4. Choose the best land to play :
                // 4.1. Be able to cast a card in Hand :
                var totalManaAvailable = Player.GetAvailableMana();
                var sortedCards = _CardsToPlayFirstQuery()
                    .Where(c => !totalManaAvailable.IsEnoughFor(c.TypedManaCost))
                    .ToList();
                var potentialCardsToCastByLand = new Dictionary<Card, List<Card>>();
                foreach (var land in lands)
                {
                    var potentialManaAvailable = totalManaAvailable.Clone();
                    potentialManaAvailable.Add(land?.GetAbility<ManaActivatedAbility>()?.GetEffect<AddToManaPoolEffect>()?.AvailableMana);
                    var potentialCardsToPlay = _CardsToPlayFirstQuery().Where(c => totalManaAvailable.IsEnoughFor(c.TypedManaCost)).ToList();
                    if (potentialCardsToPlay.Any())
                    {
                        if (!potentialCardsToCastByLand.ContainsKey(land))
                        {
                            potentialCardsToCastByLand.Add(land, potentialCardsToPlay);
                        }
                        else
                        {
                            potentialCardsToCastByLand[land].AddRange(potentialCardsToPlay);
                        }
                    }
                }

                Card bestCardToPlay = null;
                Card bestLandToPlay = null;
                foreach (var kv in potentialCardsToCastByLand)
                {
                    var bestCardToPlayForLand = kv.Value.OrderByDescending(c => c.Cmc).First();
                    if (bestCardToPlay == null)
                    {
                        bestCardToPlay = bestCardToPlayForLand;
                        bestLandToPlay = kv.Key;
                    }
                    else if (bestCardToPlayForLand.Cmc > bestCardToPlay.Cmc)
                    {
                        bestCardToPlay = bestCardToPlayForLand;
                        bestLandToPlay = kv.Key;
                    }
                    else if (bestCardToPlayForLand.Cmc == bestCardToPlay.Cmc)
                    {
                        // Choose the one with ramp or draw card ability :
                        if (bestCardToPlayForLand.HasAbility<ManaActivatedAbility>() ||
                            bestCardToPlayForLand.HasAbility<DrawCardActivatedAbility>())
                        {
                            bestCardToPlay = bestCardToPlayForLand;
                            bestLandToPlay = kv.Key;
                        }
                        // TODO : smarten this part...
                    }
                }

                // 4.2. No card can be cast with a land from hand, 
                // choose a random land :
                if (bestLandToPlay == null)
                {
                    var landIndex = CryptoRandom.NextInt(0, lands.Count - 1);
                    bestLandToPlay = lands[landIndex];
                }

                return bestLandToPlay;
            }
        }
    }
}
