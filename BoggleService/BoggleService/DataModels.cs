using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Timers;

namespace Boggle
{
    [DataContract]
    public class PlayerInfo
    {
        [DataMember(EmitDefaultValue = false)]
        public string Nickname { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string UserToken { get; set; }

        [DataMember]
        public int Score;

        [DataMember(EmitDefaultValue = false)]
        public List<WordItem> WordsPlayed = new List<WordItem>();
    }

    public class Nickname
    {
        public string UserToken { get; set; }
    }

    public class GameIDThing
    {
        public int GameID { get; set; }
    }

    [DataContract]
    public class Status
    {
        [DataMember]
        public string GameState { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int TimeLeft { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public PlayerInfo Player1 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public PlayerInfo Player2 { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Board { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int TimeLimit { get; set; }
    }

    public class Game
    {
        public BoggleBoard Board = new BoggleBoard();
        public int TimeLimit { get; set; }
        public int TimeLeft { get; set; }
        public string GameState { get; set; }

        public List<string> WordsPlayed = new List<string>();

        public Timer CountdownTimer { get; set; }

        public PlayerInfo Player1 { get; set; }
        public PlayerInfo Player2 { get; set; }

        public void CountdownTimerEvent(object source, ElapsedEventArgs e)
        {
            TimeLeft--;
            if (TimeLeft <= 0)
            {
                TimeLeft = 0;
                GameState = "completed";
                CountdownTimer.Enabled = false;
            }
        }
    }

    [DataContract]
    public class WordItem
    {
        [DataMember(EmitDefaultValue = false)]
        public string Word { get; set; }

        [DataMember]
        public int Score { get; set; }
    }

    public class WordThing
    {
        public string UserToken { get; set; }
        public string Word { get; set; }
    }

    public class TimeThing
    {
        public string UserToken { get; set; }
        public int TimeLimit { get; set; }
    }
}