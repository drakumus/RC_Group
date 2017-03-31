using System.Collections.Generic;

namespace Boggle
{
    public class PlayerInfo
    {
        public string Nickname { get; set; }
        public string UserToken { get; set; }
        public int Score;
        public readonly List<WordItem> WordsPlayed = new List<WordItem>();
    }

    public class Game
    {
        public readonly BoggleBoard Board = new BoggleBoard();
        public int TimeLimit { get; set; }
        public int TimeLeft { get; set; }
        public string GameState { get; set; }

        public PlayerInfo Player1Info { get; set; }
        public PlayerInfo Player2Info { get; set; }
    }

    public class WordItem
    {
        public string Word { get; set; }
        public string Score { get; set; }
    }
}