using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using static System.Net.HttpStatusCode;

namespace Boggle
{
    public class BoggleService : IBoggleService
    {
        private readonly static Dictionary<string, string> users = new Dictionary<string, string>(); //UserToken, Nickname
        private readonly static Dictionary<int, Game> games = new Dictionary<int, Game>();
        private static int pending;
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
        public string JoinGame(string userToken, int timeLimit)
        {
            lock (sync)
            {
                if(userToken == null)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                if (!users.ContainsKey(userToken))
                {
                    SetStatus(Forbidden);
                    return null;
                }
                if (timeLimit < 5 || timeLimit > 120)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                if (pending == -1)
                {
                    int gameID = games.Count + 1;
                    Game game = new Game()
                    {
                        GameState = "pending",
                        TimeLimit = timeLimit,
                        TimeLeft = timeLimit,
                        Player1Info = new PlayerInfo()
                        {
                            UserToken = userToken,
                            Nickname = users[userToken]
                        }
                    };
                    games.Add(gameID, game);

                    pending = gameID;
                    SetStatus(Accepted);
                    return gameID.ToString();
                }
                else
                {
                    int gameID = pending;
                    Game game = games[gameID];
                    game.GameState = "active";
                    game.CountdownTimer.Enabled = true;
                    game.Player2Info = new PlayerInfo()
                    {
                        UserToken = userToken,
                        Nickname = users[userToken]
                    };

                    pending = -1;
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
                if (game.Player1Info.UserToken == userToken)
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

        //play word
        public string PlayWord(int gameID, string userToken, string word)
        {
            lock (sync)
            {
                //playerID null check
                if (userToken == null)
                {
                    SetStatus(Forbidden);
                    return null;
                }
                //word null check
                if (word == null)
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
                if (userToken == activeGame.Player1Info.UserToken)
                {
                    activePlayer = activeGame.Player1Info;
                }
                if (userToken == activeGame.Player2Info.UserToken)
                {
                    activePlayer = activeGame.Player2Info;
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
                    Word = word,
                    Score = ScoreWord(word, activeGame),
                };

                //update?
                activePlayer.WordsPlayed.Add(wordItem);
                activeGame.WordsPlayed.Add(word);

                return wordItem.Score.ToString();
            }
        }

        /// <summary>
        /// Gets the status of a game.
        /// If gameID is invalid, responds with Forbidden.
        /// Otherwise, returns information about the game named by gameID
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns></returns>
        public Game GameStatus(int gameID)
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
                return games[gameID];
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
