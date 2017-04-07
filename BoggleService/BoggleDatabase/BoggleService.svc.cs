using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Dynamic;
using System.IO;
using static System.Net.HttpStatusCode;

namespace Boggle
{
    public class BoggleService : IBoggleService
    {
        private readonly static Dictionary<string, string> users = new Dictionary<string, string>(); //UserToken, Nickname
        private readonly static Dictionary<int, Game> games = new Dictionary<int, Game>();
        private static int pending = -1;
        private static readonly object sync = new object();

        /// <summary>
        /// The most recent call to SetStatus determines the response code used when
        /// an http response is sent.
        /// </summary>
        /// <param name="status"></param>
        private static void SetStatus(HttpStatusCode status)
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = status;
        }

        /// <summary>
        /// Registers a new user.
        /// If nickname is null or is empty after trimming, responds with status code Forbidden.
        /// Otherwise, creates a user, returns the user's token, and responds with status code Created. 
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public Nickname Register(PlayerInfo player)
        {
            lock (sync)
            {
                if (player.Nickname == null || player.Nickname.Trim().Length == 0)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                string userToken = Guid.NewGuid().ToString();
                users.Add(userToken, player.Nickname);
                SetStatus(Created);
                Nickname data = new Nickname();
                data.UserToken = userToken;
                return data;
            }
        }

        /// <summary>
        /// Attempts to join a game.
        /// If userToken isn't known, timeLimit less than 5 or timeLimit greater than 120, responds with status code Forbidden.
        /// Otherwise, if there is no pending game, ceates one and responds with Accepted.
        /// If there is a pending game, starts the game and responds with Created.
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="timeLimit"></param>
        /// <returns></returns>
        public GameIDThing JoinGame(TimeThing data)
        {
            lock (sync)
            {
                int gameID;
                Game game;
                GameIDThing id;

                if (data.UserToken == null)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                if (!users.ContainsKey(data.UserToken))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                if (data.TimeLimit < 5 || data.TimeLimit > 120)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                if (pending == -1)
                {
                    gameID = games.Count + 1;
                    game = new Game()
                    {
                        GameState = "pending",
                        TimeLimit = data.TimeLimit,
                        Player1 = new PlayerInfo()
                        {
                            UserToken = data.UserToken,
                            Nickname = users[data.UserToken]
                        }
                    };
                    games.Add(gameID, game);

                    pending = gameID;
                    SetStatus(Accepted);
                    id = new GameIDThing()
                    {
                        GameID = gameID
                    };
                    return id;
                }
                gameID = pending;
                game = games[gameID];
                game.GameState = "active";
                game.TimeLimit = (game.TimeLimit + data.TimeLimit) / 2;
                game.TimeLeft = game.TimeLimit;
                game.CountdownTimer = new System.Timers.Timer();
                game.CountdownTimer.Interval = 1000;
                game.CountdownTimer.Elapsed += game.CountdownTimerEvent;
                game.CountdownTimer.Enabled = true;
                game.Player2 = new PlayerInfo()
                {
                    UserToken = data.UserToken,
                    Nickname = users[data.UserToken]
                };

                pending = -1;
                SetStatus(Created);
                id = new GameIDThing()
                {
                    GameID = gameID
                };
                return id;
            }
        }

        /// <summary>
        /// Attemts to cancel a pending request to join a game.
        /// If userToken isn't known or is not a player in a pending game, responds with Forbidden.
        /// Otherwise, removes the UserToken from the pending game and responds with OK.
        /// </summary>
        /// <param name="userToken"></param>
        public void CancelJoin(WordThing data)
        {
            lock (sync)
            {
                if (data.UserToken == null)
                {
                    SetStatus(Forbidden);
                    return;
                }
                if (!users.ContainsKey(data.UserToken))
                {
                    SetStatus(Forbidden);
                    return;
                }
                Game game = games[pending];
                if (game.Player1.UserToken == data.UserToken)
                {
                    SetStatus(OK);
                    pending = -1;
                }
                else
                {
                    SetStatus(Forbidden);
                }
            }
        }

        /// <summary>
        /// Plays a word in a game.
        /// If word is null or empty when trimmed, or if gameID or userToken is missing or invalid,
        /// of if userToken is not a player in the game identified by gameID, responds with Forbidden.
        /// Otherwise, if the game state is anything other than active, responds with Conflict.
        /// Otherwise, records the trimmed word as being played by userToken in the game identified by gameID.
        /// Returns the score for word in the context of the game and responds with the status OK.
        /// </summary>
        /// <param name="gameID"></param>
        /// <param name="userToken"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public WordItem PlayWord(WordThing data, string gameID)
        {
            lock (sync)
            {
                //playerID null check
                if (data.UserToken == null)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                //word null check
                if (data.Word == null || data.Word.Trim().Length == 0)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                int id;
                if (!int.TryParse(gameID, out id))
                {
                    SetStatus(Forbidden);
                    return null;
                }

                //check for valid game ID
                if (id > 0 && !games.ContainsKey(id))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                //initialize activePlayer, activeGame
                PlayerInfo activePlayer = null;
                Game activeGame = games[id];

                //check for valid game status
                if (activeGame.GameState != "active")
                {
                    SetStatus(Conflict);
                    return null;
                }

                //assign active Player
                if (data.UserToken == activeGame.Player1.UserToken)
                {
                    activePlayer = activeGame.Player1;
                }
                if (data.UserToken == activeGame.Player2.UserToken)
                {
                    activePlayer = activeGame.Player2;
                }
                //valid check for userToken
                if (activePlayer == null)
                {
                    SetStatus(Forbidden);
                    return null;
                }

                //check for valid word and score.
                WordItem wordItem = new WordItem()
                {
                    Word = data.Word,
                    Score = ScoreWord(data.Word, activeGame),
                };

                //update?
                activePlayer.WordsPlayed.Add(wordItem);
                activeGame.WordsPlayed.Add(data.Word);

                SetStatus(OK);
                WordItem result = new WordItem();
                result.Score = wordItem.Score;
                return result;
            }
        }

        /// <summary>
        /// Gets the status of a game.
        /// If gameID is invalid, responds with Forbidden.
        /// Otherwise, returns information about the game named by gameID.
        /// Note that the information returned depends on whether "Breif=yes" was included as a parameter
        /// as well as on the state of the game. Responds with status code OK.
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public Status GameStatus(string gameID, string brief)
        {
            lock (sync)
            {
                int id;
                if (!int.TryParse(gameID, out id))
                {
                    SetStatus(Forbidden);
                    return null;
                }

                //check for valid game ID
                if (id > 0 && !games.ContainsKey(id))
                {
                    SetStatus(Forbidden);
                    return null;
                }

                //returns referenced game

                Game game = games[id];
                Status status = new Status();
                SetStatus(OK);
                status.GameState = game.GameState;
                if (status.GameState == "pending")
                {
                    return status;
                }
                status.TimeLeft = game.TimeLeft;
                status.Player1 = new PlayerInfo()
                {
                    Score = game.Player1.Score
                };
                status.Player2 = new PlayerInfo()
                {
                    Score = game.Player2.Score
                };
                if (brief == "yes")
                {
                    return status;
                }
                status.Board = game.Board.ToString();
                status.TimeLimit = game.TimeLimit;
                status.Player1.Nickname = game.Player1.Nickname;
                status.Player2.Nickname = game.Player2.Nickname;
                if (game.GameState == "completed")
                {
                    status.Player1.WordsPlayed = game.Player1.WordsPlayed;
                    status.Player2.WordsPlayed = game.Player2.WordsPlayed;
                }
                return status;
            }
        }

        /// <summary>
        /// Helper method for getting the score of a word in a game
        /// </summary>
        /// <param name="word"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        private int ScoreWord(string word, Game game)
        {
            //duplicate check
            if (game.WordsPlayed.Contains(word))
            {
                return 0;
            }

            //normal scoring
            int length = word.Length;
            if (!game.Board.CanBeFormed(word))
            {
                return -1;
            }

            if (length < 3 && length > 0)
            {
                return 0;
            }
            else if (length < 5)
            {
                return 1;
            }
            else if (length == 5)
            {
                return 2;
            }
            else if (length == 6)
            {
                return 3;
            }
            else if (length == 7)
            {
                return 4;
            }
            else
            {
                return 11;
            }
        }



        /// <summary>
        /// Returns a Stream version of index.html.
        /// </summary>
        /// <returns></returns>
        public Stream API()
        {
            SetStatus(OK);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            return File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "index.html");
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
