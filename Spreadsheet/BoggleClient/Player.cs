using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    /// <summary>
    /// Helper class for containing player data
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Nickname of the player
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// Players total score
        /// </summary>
        public int score { get; private set; }

        /// <summary>
        /// words the player has played
        /// </summary>
        public List<Word> wordsPlayed { get; }

        /// <summary>
        /// Constructs an empty player
        /// </summary>
        public Player()
        {
            name = "Player";
            score = 0;
            wordsPlayed = new List<Word>();
        }

        /// <summary>
        /// Updates the player with Json data
        /// </summary>
        /// <param name="data"></param>
        public void UpdatePlayer(dynamic data)
        {
            try
            {
                score = data.Score;
                name = data.Nickname;
                foreach (dynamic word in data.WordsPlayed)
                {
                    wordsPlayed.Add(new Word(word));
                }
            }
            catch(NullReferenceException)
            {
                
            }
        }
    }
}
