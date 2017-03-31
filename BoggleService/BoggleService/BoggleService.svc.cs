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
        private readonly static Dictionary<string, string> users = new Dictionary<string, string>();
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
