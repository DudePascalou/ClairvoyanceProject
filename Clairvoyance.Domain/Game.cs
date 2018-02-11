using System;
using System.Collections.Generic;
using System.Linq;

namespace Clairvoyance.Domain
{
    public class Game
    {
        public IList<Player> Players { get; set; }
        public Player StartingPlayer { get; private set; }
        public int Turn { get; set; }

        public Game(params Player[] players)
        {
            if (players == null) { throw new ArgumentNullException("players"); }

            Players = players.ToList();
        }

        /// <summary>
        /// Shuffling libraries, Drawing starting hands, choosing starting player, taking mulligans.
        /// </summary>
        public void Prepare()
        {
            foreach (var player in Players)
            {
                // 103.1 At the start of a game, each player shuffles his or her deck
                // so that the cards are in random order.
                player.ShuffleLibrary();
                // 103.4 Each player draws a number of cards equal to his or her starting hand size,
                // which is normally seven.
                player.Draw(7);
            }

            // TODO : 103.2. After the decks have been shuffled, 
            // the players determine which one of them will choose who takes the first turn. 
            // In the first game of a match (including a single-game match), 
            // the players may use any mutually agreeable method 
            // (flipping a coin, rolling dice, etc.) to do so. 
            // In a match of several games, the loser of the previous game chooses 
            // who takes the first turn. 
            // If the previous game was a draw, the player who made the choice 
            // in that game makes the choice in this game. 
            // The player chosen to take the first turn is the starting player. 
            // The game’s default turn order begins with the starting player and proceeds clockwise.
            StartingPlayer = Players[0];

            // TODO 103.4. A player who is dissatisfied with his or her initial hand may take a mulligan
        }

        public void Start()
        {
            Turn = 0;
            while (true)
            {
                Turn++;
                foreach (var player in Players)
                {
                    // 103.7a In a two-player game, the player who plays first skips the draw step (see rule 504, “Draw Step”) of his or her first turn.
                    var skipDrawingPhase = (player == StartingPlayer && Turn == 1);
                    //player.TakeTurn(Turn, skipDrawingPhase);
                }
                // TODO : End the game. See rule 104. 
                if (Turn > 100) break; // Temporary limit of 100 turns...
            }
        }
    }
}