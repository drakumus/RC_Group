using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    class Player
    {
        public string name { get; private set; }

        public int score { get; private set; }

        public List<Word> wordsPlayed { get; }

        public Player()
        {
            name = "Player";
            score = 0;
            wordsPlayed = new List<Word>();
        }

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
