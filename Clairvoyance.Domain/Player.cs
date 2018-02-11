using Clairvoyance.Domain.Abilities;
using Clairvoyance.Domain.Algorithms;
using Clairvoyance.Domain.Effects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clairvoyance.Domain
{
    public class Player
    {
        private int _LandPlayedThisTurn;
        private int _LandCountAllowedToPlayEachTurn;
        private int _NextMulliganCardCount;
        public Deck Deck { get; private set; }
        public int StartingLife { get; private set; }
        public int Life { get; private set; }
        public Library Library { get; private set; }
        public Hand Hand { get; private set; }
        public Graveyard Graveyard { get; private set; }
        public Exile Exile { get; private set; }
        public Battlefield Battlefield { get; private set; }
        public OutOfTheGame OutOfTheGame { get; private set; }
        public IPlayerAI AI { get; private set; }
        public ManaPool ManaPool { get; set; }

        public bool CanPlayLand
        {
            get
            {
                return _LandPlayedThisTurn < _LandCountAllowedToPlayEachTurn
                    && Hand.HasLand;
            }
        }

        public Player(Deck deck, IPlayerAI playerAI, int startingLife = 20, int firstMulligan = 6)
        {
            _LandCountAllowedToPlayEachTurn = 1;
            _NextMulliganCardCount = firstMulligan;
            Deck = deck ?? throw new ArgumentNullException();
            foreach (var card in Deck.Cards)
            {
                foreach (var ability in card.Abilities)
                {
                    ability.Owner = this;
                }
            }
            // 103.3 Each player begins the game with a starting life total of 20.
            StartingLife = startingLife;
            Life = StartingLife;
            // 103.1 At the start of a game, the player's decks become their libraries.
            Library = new Library(Deck.Cards);
            Hand = new Hand();
            Graveyard = new Graveyard();
            Exile = new Exile();
            Battlefield = new Battlefield();
            OutOfTheGame = new OutOfTheGame();
            AI = playerAI ?? throw new ArgumentNullException();
            AI.Player = this; // TODO : Can we do without this back reference?
            ManaPool = new ManaPool();
        }

        public void ShuffleLibrary()
        {
            Library.Shuffle();
        }

        public void TakeMulligan()
        {
            // 103.4 A player who is dissatisfied with his or her initial hand 
            // may take a mulligan.
            Library = new Library(Deck.Cards);
            ShuffleLibrary();
            Hand = new Hand();
            Draw(_NextMulliganCardCount);
            _NextMulliganCardCount--;
        }

        public void Draw(int cardCount)
        {
            for (int i = 0; i < cardCount; i++)
            {
                Hand.Cards.Add(Library.Draw());
            }
        }

        public void TakeTurn(int turn, bool skipDrawingPhase)
        {
            // TODO : 500.1. A turn consists of five phases, in this order: 
            // - beginning, 
            // - precombat main, 
            // - combat, 
            // - postcombat main, 
            // - ending.

            StartTheTurn();
            var cardToPlay = AI.ChooseCard();
            while (cardToPlay != null)
            {
                Play(cardToPlay);
                cardToPlay = AI.ChooseCard();
            }
            PassTheTurn();
        }

        public void StartTheTurn()
        {
            _LandPlayedThisTurn = 0;
            // 302.6. When a creature enters the battlefield, it has “summoning sickness” 
            // until it has been under its controller’s control continuously since his or her most recent turn began.
            foreach (var creature in Battlefield.Creatures())
            {
                var ssaList = creature.Abilities.Where(a => a.GetType() == typeof(SummoningSicknessStaticAbility)).ToList();
                foreach (var ssa in ssaList)
                {
                    creature.Abilities.Remove(ssa);
                }
            }
            // xxx.x Untap permanents :
            foreach (var card in Battlefield.Cards)
            {
                card.Untap();
            }
            // xxx.x Draw a card :
            Draw(1);
        }

        public void Play(Card card)
        {
            // TODO : be able to play a card from another zone than the Hand...
            if (card.IsALand && CanPlayLand)
            {
                Hand.Pop(card);
                _LandPlayedThisTurn++;
                Battlefield.Enter(card);
            }
            else if (!card.IsALand && CanPayManaCost(card))
            {
                Hand.Pop(card);
                PayManaCost(card);
                Battlefield.Enter(card);
            }
        }

        public AvailableMana GetAvailableMana()
        {
            var availableMana = new AvailableMana();

            foreach (var availableManaAbility in GetAvailableAbilities<ManaActivatedAbility>())
            {
                availableMana.Add(availableManaAbility?.GetEffect<AddToManaPoolEffect>()?.AvailableMana);
            }

            //foreach (var cardInHand in Hand.Cards)
            //{
            //    availableMana.Add(cardInHand?.GetAvailableAbility<ManaActivatedAbility>());
            //}
            //foreach (var cardOnTheBattlefield in Battlefield.Cards)
            //{
            //    availableMana.Add(cardOnTheBattlefield?.GetAvailableAbility<ManaActivatedAbility>()?.GetEffect<AddToManaPoolEffect>()?.AvailableMana);
            //}

            return availableMana;
        }

        private bool CanPayManaCost(Card card)
        {
            bool canPay = false;

            if (card.Cmc == 0)
            {
                canPay = true;
            }
            else if (GetAvailableMana().IsEnoughFor(card.TypedManaCost))
            {
                canPay = true;
            }
            // TODO : manage X costs...

            return canPay;
        }

        public void PayManaCost(Card card)
        {
            var remainingManaCost = card.TypedManaCost.Clone();
            var availableManaEffects = GetAvailableAbilities<ManaActivatedAbility>()
                .Select(a => a.GetEffect<AddToManaPoolEffect>())
                .Where(e => e != null)
                .ToList();

            // TODO : take into account !e.AvailableMana.ProducesOnlyOneMana...

            // Pay Colorless
            if (remainingManaCost.Colorless > 0)
            {
                var onlyOneManaEffect = availableManaEffects
                    .Where(e => e.AvailableMana.ProducesOnlyOneMana)
                    .Where(e => e.AvailableMana.Colorless > 0 || e.AvailableMana.AnyType > 0)
                    .OrderBy(e => e.AvailableMana.Colorless > 0)
                    .ThenBy(e => e.AvailableMana.AnyType > 0)
                    .ToList();
                while (remainingManaCost.Colorless > 0)
                {
                    var ability = (IActivatedAbility)onlyOneManaEffect[0].Ability;
                    if (ability.IsAvailable)
                    {
                        ability.Activate();
                    }
                    onlyOneManaEffect.RemoveAt(0);
                    remainingManaCost.Colorless--;
                }
            }
            // Pay White
            if (remainingManaCost.White > 0)
            {
                var onlyOneManaEffect = availableManaEffects
                    .Where(e => e.AvailableMana.ProducesOnlyOneMana)
                    .Where(e => e.AvailableMana.White > 0 || e.AvailableMana.AnyColor > 0 || e.AvailableMana.AnyType > 0)
                    .OrderBy(e => e.AvailableMana.White > 0)
                    .ThenBy(e => e.AvailableMana.AnyColor > 0)
                    .ThenBy(e => e.AvailableMana.AnyType > 0)
                    .ToList();
                while (remainingManaCost.White > 0)
                {
                    var ability = (IActivatedAbility)onlyOneManaEffect[0].Ability;
                    if (ability.IsAvailable)
                    {
                        ability.Activate();
                    }
                    onlyOneManaEffect.RemoveAt(0);
                    remainingManaCost.White--;
                }
            }
            // Pay Blue
            if (remainingManaCost.Blue > 0)
            {
                var onlyOneManaEffect = availableManaEffects
                    .Where(e => e.AvailableMana.ProducesOnlyOneMana)
                    .Where(e => e.AvailableMana.Blue > 0 || e.AvailableMana.AnyColor > 0 || e.AvailableMana.AnyType > 0)
                    .OrderBy(e => e.AvailableMana.Blue > 0)
                    .ThenBy(e => e.AvailableMana.AnyColor > 0)
                    .ThenBy(e => e.AvailableMana.AnyType > 0)
                    .ToList();
                while (remainingManaCost.Blue > 0)
                {
                    var ability = (IActivatedAbility)onlyOneManaEffect[0].Ability;
                    if (ability.IsAvailable)
                    {
                        ability.Activate();
                    }
                    onlyOneManaEffect.RemoveAt(0);
                    remainingManaCost.Blue--;
                }
            }
            // Pay Black
            if (remainingManaCost.Black > 0)
            {
                var onlyOneManaEffect = availableManaEffects
                    .Where(e => e.AvailableMana.ProducesOnlyOneMana)
                    .Where(e => e.AvailableMana.Black > 0 || e.AvailableMana.AnyColor > 0 || e.AvailableMana.AnyType > 0)
                    .OrderBy(e => e.AvailableMana.Black > 0)
                    .ThenBy(e => e.AvailableMana.AnyColor > 0)
                    .ThenBy(e => e.AvailableMana.AnyType > 0)
                    .ToList();
                while (remainingManaCost.Black > 0)
                {
                    var ability = (IActivatedAbility)onlyOneManaEffect[0].Ability;
                    if (ability.IsAvailable)
                    {
                        ability.Activate();
                    }
                    onlyOneManaEffect.RemoveAt(0);
                    remainingManaCost.Black--;
                }
            }
            // Pay Red
            if (remainingManaCost.Red > 0)
            {
                var onlyOneManaEffect = availableManaEffects
                    .Where(e => e.AvailableMana.ProducesOnlyOneMana)
                    .Where(e => e.AvailableMana.Red > 0 || e.AvailableMana.AnyColor > 0 || e.AvailableMana.AnyType > 0)
                    .OrderBy(e => e.AvailableMana.Red > 0)
                    .ThenBy(e => e.AvailableMana.AnyColor > 0)
                    .ThenBy(e => e.AvailableMana.AnyType > 0)
                    .ToList();
                while (remainingManaCost.Red > 0)
                {
                    var ability = (IActivatedAbility)onlyOneManaEffect[0].Ability;
                    if (ability.IsAvailable)
                    {
                        ability.Activate();
                    }
                    onlyOneManaEffect.RemoveAt(0);
                    remainingManaCost.Red--;
                }
            }
            // Pay Green
            if (remainingManaCost.Green > 0)
            {
                var onlyOneManaEffect = availableManaEffects
                    .Where(e => e.AvailableMana.ProducesOnlyOneMana)
                    .Where(e => e.AvailableMana.Green > 0 || e.AvailableMana.AnyColor > 0 || e.AvailableMana.AnyType > 0)
                    .OrderBy(e => e.AvailableMana.Green > 0)
                    .ThenBy(e => e.AvailableMana.AnyColor > 0)
                    .ThenBy(e => e.AvailableMana.AnyType > 0)
                    .ToList();
                while (remainingManaCost.Green > 0)
                {
                    var ability = (IActivatedAbility)onlyOneManaEffect[0].Ability;
                    if (ability.IsAvailable)
                    {
                        ability.Activate();
                    }
                    onlyOneManaEffect.RemoveAt(0);
                    remainingManaCost.Green--;
                }
            }
            // Pay Generic
            if (remainingManaCost.Generic > 0)
            {
                var onlyOneManaEffect = availableManaEffects
                    .Where(e => e.AvailableMana.ProducesOnlyOneMana)
                    .Where(e => e.Ability.IsAvailable)
                    .ToList();
                while (remainingManaCost.Generic > 0)
                {
                    var ability = (IActivatedAbility)onlyOneManaEffect[0].Ability;
                    if (ability.IsAvailable)
                    {
                        ability.Activate();
                    }
                    onlyOneManaEffect.RemoveAt(0);
                    remainingManaCost.Generic--;
                }
            }
            // Pay X
            if ((remainingManaCost.XValue * remainingManaCost.X) > 0)
            {
                var onlyOneManaEffect = availableManaEffects
                    .Where(e => e.AvailableMana.ProducesOnlyOneMana)
                    .Where(e => e.Ability.IsAvailable)
                    .ToList();
                for (int i = 0; i < remainingManaCost.X; i++)
                {
                    var remainingX = remainingManaCost.XValue;
                    while (remainingX > 0)
                    {
                        var ability = (IActivatedAbility)onlyOneManaEffect[0].Ability;
                        if (ability.IsAvailable)
                        {
                            ability.Activate();
                        }
                        onlyOneManaEffect.RemoveAt(0);
                        remainingX--;
                    }
                }
            }

            ManaPool.Pay(card.TypedManaCost);
        }

        private IEnumerable<T> GetAvailableAbilities<T>() where T : class, IAbility
        {
            T ability;
            foreach (var cardInHand in Hand.Cards)
            {
                ability = cardInHand?.GetAvailableAbility<T>();
                if (ability != null) yield return ability;
            }
            foreach (var cardOnTheBattlefield in Battlefield.Cards)
            {
                ability = cardOnTheBattlefield?.GetAvailableAbility<T>();
                if (ability != null) yield return ability;
            }
        }

        public void PassTheTurn()
        {
            // TODO : discard cards more than 7 (or overriden number)
        }

        public void RemoveFromTheGame(Card card)
        {
            Library.Cards.Remove(card);
            Hand.Cards.Remove(card);
            Graveyard.Cards.Remove(card);
            Exile.Cards.Remove(card);
            Battlefield.Cards.Remove(card);
            OutOfTheGame.Cards.Add(card);
        }
    }
}