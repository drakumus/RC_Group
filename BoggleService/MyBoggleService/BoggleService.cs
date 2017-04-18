using System;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using static System.Net.HttpStatusCode;
using System.Net.Http;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;

namespace Boggle
{
    public class BoggleService
    {
        // The connection string to the DB
        private static string BoggleDB;
        public BoggleService()
        {
            
            BoggleDB = ConfigurationManager.ConnectionStrings["BoggleDataBase"].ConnectionString;
        }

        /// <summary>
        /// The most recent call to SetStatus determines the response code used when
        /// an http response is sent.
        /// </summary>
        /// <param name="status"></param>

        /*
        private static void SetStatus(HttpStatusCode status)
        {
            //TODO: need to set status as an out parameter to each of your service methods. 
            //WebOperationContext.Current.OutgoingResponse.StatusCode = status;
        }*/


        public string RequestParser(string requestType, string url, string result, out HttpStatusCode status)
        {
            status = Forbidden;
            string output = "";
            Regex usersReg = new Regex(@"users$");
            Regex joinReg = new Regex(@"games$");
            Regex gamesReg = new Regex(@"games*\/[0-9]+$");
            Regex briefReg = new Regex(@"games\/(\d+)\?brief=(\w*)$");
            Regex getGame = new Regex(@"[0-9]+$");
            
            if(requestType == "POST")
            {
                if(usersReg.IsMatch(url)) //POST users (creates user)
                {
                    dynamic data = JsonConvert.DeserializeObject(result);

                    PlayerInfo player = new PlayerInfo {
                        Nickname = data.Nickname
                    };
                    output = JsonConvert.SerializeObject(Register(player, out status));
                    //string json = JsonConvert.SerializeObject(Register(player, out status));
                    //var res = new HttpResponseMessage();
                    //res.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    //res.StatusCode = status;
                }
                else if(joinReg.IsMatch(url)) //POST games (join game)
                {
                    dynamic data = JsonConvert.DeserializeObject(result);

                    TimeThing time = new TimeThing
                    {
                        UserToken = data.UserToken,
                        TimeLimit = data.TimeLimit
                    };

                    output = JsonConvert.SerializeObject(JoinGame(time, out status));
                    //string json = JsonConvert.SerializeObject(JoinGame(time, out status));
                    ///var res = new HttpResponseMessage();
                    //res.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    //res.StatusCode = status;
                }
                else
                {
                    status = HttpStatusCode.BadRequest;
                }
            }
            else if (requestType == "PUT")
            {
                if(joinReg.IsMatch(url)) //PUT games (cancel join)
                {
                    dynamic data = JsonConvert.DeserializeObject(result);
                    WordThing thing = new WordThing
                    {
                        UserToken = data.UserToken
                    };
                    CancelJoin(thing , out status);
                }
                else if (gamesReg.IsMatch(url)) //PUT games/128 (play word)
                {
                    dynamic data = JsonConvert.DeserializeObject(result);

                    WordThing thing = new WordThing()
                    {
                        UserToken = data.UserToken,
                        Word = data.Word
                    };

                    output = JsonConvert.SerializeObject(PlayWord(thing, getGame.Match(url).Value, out status));
                }
                else
                {
                    status = HttpStatusCode.Forbidden;
                }
            }
            else if (requestType == "GET")
            {
                if (gamesReg.IsMatch(url)) //GET games/128
                {
                    string gameID = getGame.Match(url).Value;
                    string brief = "";
                    output = JsonConvert.SerializeObject(GameStatus(gameID, brief, out status));
                }
                else if (briefReg.IsMatch(url))
                {
                    string gameID = briefReg.Match(url).Groups[1].Value;
                    string brief = briefReg.Match(url).Groups[2].Value;

                    output = JsonConvert.SerializeObject(GameStatus(gameID, brief, out status));
                }
                else
                {
                    status = HttpStatusCode.Forbidden;
                }
            }
            return output;
        }


        /// <summary>
        /// Registers a new user.
        /// If nickname is null or is empty after trimming, responds with status code Forbidden.
        /// Otherwise, creates a user, returns the user's token, and responds with status code Created. 
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public Nickname Register(PlayerInfo player, out HttpStatusCode status)
        {
            if (player.Nickname == null || player.Nickname.Trim().Length == 0)
            {
                status = Forbidden;
                return null;
            }
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command =
                        new SqlCommand("INSERT INTO Users (UserID, Nickname) values(@UserID, @Nickname)",
                        conn,
                        trans))
                    {
                        // generate userToken
                        string userToken = Guid.NewGuid().ToString();

                        // replace placeholders
                        command.Parameters.AddWithValue("@UserID", userToken);
                        command.Parameters.AddWithValue("@Nickname", player.Nickname.Trim());

                        // make sure 1 row was modified
                        if (command.ExecuteNonQuery() == 1)
                        {
                            status = Created;
                            trans.Commit();
                            Nickname data = new Nickname();
                            data.UserToken = userToken;
                            return data;
                        }
                        status = Forbidden;
                        return null;
                    }
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
        public GameIDThing JoinGame(TimeThing player, out HttpStatusCode status)
        {

            if (player.UserToken == null)
            {
                status = Forbidden;
                return null;
            }
            if (player.TimeLimit < 5 || player.TimeLimit > 120)
            {
                status = Forbidden;
                return null;
            }
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // check if Users contains UserID
                    using (SqlCommand command = new SqlCommand("select UserID from Users where UserID = @UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", player.UserToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                // user not found
                                status = Forbidden;
                                //trans.Commit();
                                return null;
                            }
                        }
                    }
                    //store player1 from player2 query
                    string player1 = "";
                    // check for pending game and get player1 id
                    Game game = new Game();
                    // check for pending game
                    using (SqlCommand command =
                        new SqlCommand("SELECT Player1, GameID, TimeLimit FROM Games WHERE Player2 IS NULL", conn, trans))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                player1 = (string)reader["Player1"];
                                game.GameID = (int)reader["GameID"];
                                // get half of time limit
                                int oldTimeLimit = (int)reader["TimeLimit"];
                                game.TimeLimit = (oldTimeLimit + player.TimeLimit) / 2;
                            }
                        }
                    }
                    if (player1 == player.UserToken)
                    {
                        status = Conflict;
                        return null;
                    }
                    if (game.GameID != 0)
                    {

                        using (SqlCommand command =
                            new SqlCommand("UPDATE Games SET Player2 = @Player2, Board = @Board, TimeLimit = @TimeLimit, StartTime = @StartTime WHERE GameID = @GameID",
                            conn,
                            trans))
                        {
                            command.Parameters.AddWithValue("@Player2", player.UserToken);
                            command.Parameters.AddWithValue("@Board", new BoggleBoard().ToString());
                            command.Parameters.AddWithValue("@TimeLimit", game.TimeLimit);
                            command.Parameters.AddWithValue("@StartTime", DateTime.Now);
                            command.Parameters.AddWithValue("@GameID", game.GameID);

                            // make sure 1 row was modified
                            if (command.ExecuteNonQuery() == 1)
                            {
                                GameIDThing data = new GameIDThing();
                                data.GameID = game.GameID;
                                status = Created;
                                trans.Commit();
                                return data;
                            }
                            status = Forbidden;
                            return null;
                        }
                    }

                    // no pending game
                    using (SqlCommand command =
                        new SqlCommand("INSERT INTO Games(Player1, TimeLimit) output inserted.GameID VALUES(@Player1, @TimeLimit)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@Player1", player.UserToken);
                        command.Parameters.AddWithValue("@TimeLimit", player.TimeLimit);
                        GameIDThing data = new GameIDThing();
                        data.GameID = (int)command.ExecuteScalar();
                        status = Accepted;
                        trans.Commit();
                        return data;
                    }
                }
            }
        }

        /// <summary>
        /// Attemts to cancel a pending request to join a game.
        /// If userToken isn't known or is not a player in a pending game, responds with Forbidden.
        /// Otherwise, removes the UserToken from the pending game and responds with OK.
        /// </summary>
        /// <param name="userToken"></param>
        public void CancelJoin(WordThing player, out HttpStatusCode status)
        {
            if (player.UserToken == null)
            {
                status = Forbidden;
                return;
            }
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using (SqlCommand command = new SqlCommand("DELETE FROM Games WHERE Player1 = @Player1 and Player2 IS NULL", conn, trans))
                    {
                        command.Parameters.AddWithValue("@Player1", player.UserToken);

                        if (command.ExecuteNonQuery() == 0)
                        {
                            status = Forbidden;
                        }
                        else
                        {
                            status = OK;
                        }
                        trans.Commit();
                    }
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
        /// <param name="player"></param>
        /// <returns></returns>
        public WordItem PlayWord(WordThing player, string gameID, out HttpStatusCode status)
        {
            //playerID null check
            if (player.UserToken == null)
            {
                status = Forbidden;
                return null;
            }
            //word null check
            if (player.Word == null || player.Word.Trim().Length == 0)
            {
                status = Forbidden;
                return null;
            }
            int id;
            if (!int.TryParse(gameID, out id))
            {
                status = Forbidden;
                return null;
            }
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    // check if Users contains UserID
                    using (SqlCommand command = new SqlCommand("select UserID from Users where UserID = @UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", player.UserToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                // user not found
                                status = Forbidden;
                                trans.Commit();
                                return null;
                            }
                        }
                    }
                    string board = "";
                    using (SqlCommand command = new SqlCommand("select Player2, Board, TimeLimit, StartTime from Games where GameID = @GameID and Player1 = @Player or Player2 = @Player", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", gameID);
                        command.Parameters.AddWithValue("@Player", player.UserToken);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            if (!reader.HasRows)
                            {
                                // game not found
                                status = Forbidden;
                                //trans.Commit();
                                return null;
                            }
                            else if (reader["Player2"].ToString() == "")
                            {
                                status = Conflict;
                                return null;
                            }
                            else
                            {

                                board = reader["Board"].ToString();
                                int timeLimit = (int)reader["TimeLimit"];
                                int colIndex = reader.GetOrdinal("StartTime");
                                DateTime startTime = reader.GetDateTime(colIndex);

                                if (startTime.AddSeconds(timeLimit) < DateTime.Now)
                                {
                                    status = Conflict;
                                    //trans.Commit();
                                    return null;
                                }
                            }
                        }
                    }
                    if (board == "")
                    {
                        status = Forbidden;
                        trans.Commit();
                        return null;
                    }

                    // get word score
                    WordItem word = new WordItem();
                    word.Word = player.Word;
                    using (SqlCommand command = new SqlCommand("select Word from Words where GameID = @GameID and Word = @Word", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", gameID);
                        command.Parameters.AddWithValue("@Word", word.Word);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                word.Score = ScoreWord(word.Word, board);
                            }
                            else
                            {
                                word.Score = 0;
                            }
                        }
                    }
                    using (SqlCommand command = new SqlCommand("insert into Words(GameID, Player, Word, Score) values(@GameID, @Player, @Word, @Score)", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", gameID);
                        command.Parameters.AddWithValue("@Player", player.UserToken);
                        command.Parameters.AddWithValue("@Word", word.Word);
                        command.Parameters.AddWithValue("@Score", word.Score);

                        // make sure 1 row was modified
                        if (command.ExecuteNonQuery() == 1)
                        {
                            word.Word = null;
                            status = OK;
                            trans.Commit();
                            return word;
                        }
                        status = Forbidden;
                        return null;
                    }
                }
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
        public Status GameStatus(string gameID, string brief, out HttpStatusCode outStatus)
        {
            int id;

            if (!int.TryParse(gameID, out id))
            {
                outStatus = Forbidden;
                return null;
            }
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    Status status = new Status();
                    status.Player1 = new PlayerInfo();
                    status.Player2 = new PlayerInfo();
                    using (SqlCommand command = new SqlCommand("select Player1, Player2, TimeLimit, StartTime, Board from Games where GameID = @GameID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", gameID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                // game not found
                                outStatus = Forbidden;
                                //trans.Commit();
                                return null;
                            }
                            reader.Read();
                            outStatus = OK;
                            if (reader["Player2"].ToString() == "")
                            {
                                status.GameState = "pending";
                                //trans.Commit();
                                return status;
                            }
                            status.Player1.UserToken = reader["Player1"].ToString();
                            status.Player2.UserToken = reader["Player2"].ToString();

                            status.Board = reader["Board"].ToString();
                            int timeLimit = (int)reader["TimeLimit"];
                            status.TimeLimit = timeLimit;
                            int colIndex = reader.GetOrdinal("StartTime");
                            DateTime startTime = reader.GetDateTime(colIndex);
                            DateTime endTime = startTime.AddSeconds(timeLimit);
                            if (endTime < DateTime.Now)
                            {
                                status.GameState = "completed";
                            }
                            else
                            {
                                status.TimeLeft = (endTime - DateTime.Now).Seconds;
                                status.GameState = "active";
                            }
                        }
                    }

                    // player1 words
                    using (SqlCommand command = new SqlCommand("select Word, Score from Words where Player = @UserID and GameID = @GameID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", status.Player1.UserToken);
                        command.Parameters.AddWithValue("@GameID", gameID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                WordItem word = new WordItem()
                                {
                                    Word = reader["Word"].ToString(),
                                    Score = (int)reader["Score"]
                                };
                                status.Player1.Score += word.Score;
                                status.Player1.WordsPlayed.Add(word);
                            }
                        }
                    }

                    // player2 words
                    using (SqlCommand command = new SqlCommand("select Word, Score from Words where Player = @UserID and GameID = @GameID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", status.Player2.UserToken);
                        command.Parameters.AddWithValue("@GameID", gameID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                WordItem word = new WordItem()
                                {
                                    Word = reader["Word"].ToString(),
                                    Score = (int)reader["Score"]
                                };
                                status.Player2.Score += word.Score;
                                status.Player2.WordsPlayed.Add(word);
                            }
                        }
                    }
                    if (brief == "yes")
                    {
                        status.Player1.WordsPlayed = null;
                        status.Player2.WordsPlayed = null;
                        status.Player1.UserToken = null;
                        status.Player2.UserToken = null;
                        status.TimeLimit = 0;
                        status.Board = null;
                        trans.Commit();
                        return status;
                    }

                    // player1 nickname
                    using (SqlCommand command = new SqlCommand("select Nickname from Users where UserID = @UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", status.Player1.UserToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                // game not found
                                outStatus= Forbidden;
                                trans.Commit();
                                return null;
                            }
                            reader.Read();
                            status.Player1.Nickname = reader["Nickname"].ToString();

                            status.Player1.UserToken = null;
                        }
                    }

                    // player2 nickname
                    using (SqlCommand command = new SqlCommand("select Nickname from Users where UserID = @UserID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@UserID", status.Player2.UserToken);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                // game not found
                                outStatus = Forbidden;
                                trans.Commit();
                                return null;
                            }

                            reader.Read();
                            status.Player2.Nickname = reader["Nickname"].ToString();

                            status.Player2.UserToken = null;
                        }
                    }
                    if (status.GameState == "active")
                    {
                        status.Player1.WordsPlayed = null;
                        status.Player2.WordsPlayed = null;
                        trans.Commit();
                        return status;
                    }
                    trans.Commit();
                    return status;
                }
            }
        }

        /// <summary>
        /// Helper method for getting the score of a word in a game
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private int ScoreWord(string word, string letters)
        {
            //TODO: add dictionary check. File path for dictionary "dictionary.txt" nothing more.
            int length = word.Length;
            BoggleBoard board = new BoggleBoard(letters);
            if (!board.CanBeFormed(word) && word.Length > 2)
            {
                return -1;
            }
            switch (word.Length)
            {
                case 1:
                case 2:
                    return 0;
                case 3:
                case 4:
                    return 1;
                case 5:
                    return 2;
                case 6:
                    return 3;
                case 7:
                    return 5;
                default:
                    return 11;
            }
        }

        /// <summary>
        /// Returns a Stream version of index.html.
        /// </summary>
        /// <returns></returns>
        public Stream API(out HttpStatusCode status)
        {
            status = OK;
            //WebOperationContext.Current.OutgoingResponse.ContentType = "text/html";
            return File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "index.html");
        }

        /*
        /// <summary>
        /// remove this.
        /// </summary>
        /// <param name="composite"></param>
        /// <returns></returns>
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
             composite;
        }
        */
    }
}
