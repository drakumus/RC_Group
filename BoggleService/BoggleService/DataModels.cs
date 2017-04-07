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
        public int GameID { get; set; }
        public int TimeLimit { get; set; }
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