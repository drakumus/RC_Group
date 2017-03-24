using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    class BoggleGame
    {
        /// <summary>
        /// The ip the user is connected to, or "" if not connected
        /// </summary>
        public static string ip;

        /// <summary>
        /// The token of the most recently registered user, or "0" if no user
        /// has ever registered
        /// </summary>
        public string userToken;

        /// <summary>
        /// The ID of the active game, or "-1" if no active game
        /// </summary>
        public string ID;

        /// <summary>
        /// Tells whether a player is trying to connect to a game
        /// </summary>
        public bool connecting;

        /// <summary>
        /// The state of the game (not connected, pending, active, completed)
        /// </summary>
        public string gameState;

        /// <summary>
        /// The amount of time left in the game
        /// </summary>
        public int timeRemaining;

        public Player player1;
        public Player player2;

        public char[] boardState;

        public List<Word> wordsPlayed;
        public BoggleGame()
        {
            userToken = "0";
            ID = "";
            connecting = false;
            gameState = "not connected";
            ip = "";
            player1 = new Player();
            player2 = new Player();

            wordsPlayed = new List<Word>();
        }

        public void UpdateGame(dynamic data)
        {
            try
            {
                string letters = data.Board;
                boardState = letters.ToCharArray();
                timeRemaining = data.TimeLeft;

                dynamic player1 = data.Player1;
                this.player1.UpdatePlayer(player1);

                dynamic player2 = data.Player2;
                this.player2.UpdatePlayer(player2);
            }
            catch (ArgumentNullException)
            {

            }
        }
    }
}
