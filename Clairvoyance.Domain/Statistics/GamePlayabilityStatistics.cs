using System.Collections.Generic;

namespace Clairvoyance.Domain.Statistics
{
    /// <summary>
    /// Gather playability statistics of a game.
    /// Playability is based on what cards can be played on first turns of a game
    /// (ignoring what opponent is doing).
    /// </summary>
    public class GamePlayabilityStatistics
    {
        public Hand StartingHand { get; private set; }
        public int TurnCount { get; private set; }
        public IList<TurnPlayabilityStatistics> Turns { get; set; }

        public GamePlayabilityStatistics(int turnCount)
        {
            TurnCount = turnCount;
            Turns = new List<TurnPlayabilityStatistics>(TurnCount);
        }

        public void AddStartingHand(Hand hand)
        {
            StartingHand = hand;
        }

        public TurnPlayabilityStatistics AddNewTurn()
        {
            var newTurn = new TurnPlayabilityStatistics();
            Turns.Add(newTurn);
            return newTurn;
        }
    }
}