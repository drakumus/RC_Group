using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using static System.Net.HttpStatusCode;

namespace Boggle
{
    public class BoggleService : IBoggleService
    {
        private readonly static Dictionary<string, string> users = new Dictionary<string, string>(); //UserToken, Nickname
        private readonly static Dictionary<string, Game> games = new Dictionary<string, Game>();
        private static string pending;
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
        public string Register(string nickname)
        {
            lock (sync)
            {
                if(nickname == null || nickname.Trim().Length == 0)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                else
                {
                    string userToken = Guid.NewGuid().ToString();
                    users.Add(userToken, nickname);
                    SetStatus(Created);
                    return userToken;
                }
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
        public string JoinGame(TimeThing data)
        {
            lock (sync)
            {
                if(data.UserToken == null)
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
                if (pending == null)
                {
                    string gameID = (games.Count + 1).ToString();
                    Game game = new Game()
                    {
                        GameState = "pending",
                        TimeLimit = data.TimeLimit,
                        TimeLeft = data.TimeLimit,
                        Player1 = new PlayerInfo()
                        {
                            UserToken = data.UserToken,
                            Nickname = users[data.UserToken]
                        }
                    };
                    games.Add(gameID, game);

                    pending = gameID;
                    SetStatus(Accepted);
                    return gameID.ToString();
                }
                else
                {
                    string gameID = pending;
                    Game game = games[gameID];
                    game.GameState = "active";
                    game.CountdownTimer.Enabled = true;
                    game.Player2 = new PlayerInfo()
                    {
                        UserToken = data.UserToken,
                        Nickname = users[data.UserToken]
                    };

                    pending = null;
                    SetStatus(Created);
                    return gameID.ToString();
                }
            }
        }

        /// <summary>
        /// Attemts to cancel a pending request to join a game.
        /// If userToken isn't known or is not a player in a pending game, responds with Forbidden.
        /// Otherwise, removes the UserToken from the pending game and responds with OK.
        /// </summary>
        /// <param name="userToken"></param>
        public void CancelJoin(string userToken)
        {
            lock (sync)
            {
                if (userToken == null)
                {
                    SetStatus(Forbidden);
                    return;
                }
                if (!users.ContainsKey(userToken))
                {
                    SetStatus(Forbidden);
                    return;
                }
                Game game = games[pending];
                if (game.Player1.UserToken == userToken)
                {
                    SetStatus(OK);
                    pending = null;
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
        public string PlayWord(WordThing data, string gameID)
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

                //check for valid game ID
                if (!games.ContainsKey(gameID))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                //initialize activePlayer, activeGame
                PlayerInfo activePlayer = null;
                Game activeGame = games[gameID];

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
                return wordItem.Score.ToString();
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
        public dynamic GameStatus(string gameID, string brief)
        {
            lock (sync)
            {
                //checks for valid gameID
                if (!games.ContainsKey(gameID))
                {
                    SetStatus(Forbidden);
                    return null;
                }

                //returns referenced game
                SetStatus(OK);

                Game game = games[gameID];
                dynamic status = new ExpandoObject();
                status.GameState = game.GameState;
                status.TimeLeft = game.TimeLeft;
                status.Player1.Score = game.Player1.Score;
                status.Player2.Score = game.Player2.Score;
                if (brief == "yes")
                {
                    return status;
                }
                status.Board = game.Board.ToString();
                status.TimeLimit = game.TimeLimit;
                status.Player1.Nickname = game.Player1.Nickname;
                status.Player2.Nickname = game.Player2.Nickname;
                if(game.GameState == "completed")
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

        /// <summary>
        /// Demo.  You can delete this.
        /// </summary>
        public string WordAtIndex(int n)
        {
            if (n < 0)
            {
                SetStatus(Forbidden);
                return null;
            }

            string line;
            using (StreamReader file = new System.IO.StreamReader(AppDomain.CurrentDomain.BaseDirectory + "dictionary.txt"))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (n == 0) break;
                    n--;
                }
            }

            if (n == 0)
            {
                SetStatus(OK);
                return line;
            }
            else
            {
                SetStatus(Forbidden);
                return null;
            }
        }
    }
}
