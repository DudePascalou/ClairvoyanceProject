using Clairvoyance.Domain;
using Clairvoyance.Domain.Statistics;
using System.Linq;

namespace Clairvoyance.Services
{
    public class MtgStatisticsService
    {
        public MtgStatisticsService()
        { }

        /// <summary>
        /// Compute statistics of what cards can be played within <paramref name="turnCount"/> turns
        /// on a goldfish mode (considering opponent is doing nothing).
        /// TODO : statistics are computed among a large number of cases.
        /// </summary>
        /// <param name="turnCount">Turns count among which compute statistics.</param>
        public DeckPlayabilityStatistics ComputePlayabilityStatistics(Deck deck, Player player, int turnCount)
        {
            var stats = new DeckPlayabilityStatistics(deck, turnCount);
            var gameStats = stats.AddGame();

            player.ShuffleLibrary();
            player.Draw(7);
            gameStats.AddStartingHand(player.Hand);

            for (int turn = 1; turn <= turnCount; turn++)
            {
                player.TakeTurn(turn, turn == 1);

                var turnStats = gameStats.AddNewTurn();
                turnStats.Creatures = player.Battlefield.Creatures().ToList();
                turnStats.Lands = player.Battlefield.Lands().ToList();
            }

            return stats;
        }
    }
}