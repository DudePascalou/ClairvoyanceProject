using System.Collections.Generic;

namespace Clairvoyance.Domain.Statistics
{
    /// <summary>
    /// Gather playability statistics of a <see cref="Deck"/>.
    /// Playability is based on what cards can be played on first turns of a game
    /// (ignoring what opponent is doing).
    /// </summary>
    public class DeckPlayabilityStatistics
    {
        public Deck Deck { get; private set; }
        public int TurnCount { get; private set; }
        public IList<GamePlayabilityStatistics> Games { get; private set; }
        public IList<TurnPlayabilityStatistics> AverageTurns { get; set; }
        public IList<TurnPlayabilityStatistics> MaxTurns { get; set; }
        public IList<TurnPlayabilityStatistics> MinTurns { get; set; }

        public DeckPlayabilityStatistics(Deck deck, int turnCount)
        {
            Deck = deck;
            TurnCount = turnCount;
            Games = new List<GamePlayabilityStatistics>(TurnCount);
            AverageTurns = new List<TurnPlayabilityStatistics>(TurnCount);
            MaxTurns = new List<TurnPlayabilityStatistics>(TurnCount);
            MinTurns = new List<TurnPlayabilityStatistics>(TurnCount);
        }

        public GamePlayabilityStatistics AddGame()
        {
            var newGame = new GamePlayabilityStatistics(TurnCount);
            Games.Add(newGame);
            return newGame;
        }

        public void Compute()
        {
            // TODO : compute stats...
        }
    }
}