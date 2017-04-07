using System;
using System.Linq;
using System.Net;
using System.ServiceModel.Web;
using static System.Net.HttpStatusCode;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace Boggle
{
    public class Boggle : IBoggleService
    {
        // The connection string to the DB
        private static string BoggleDB;

        static Boggle()
        {
            BoggleDB = ConfigurationManager.ConnectionStrings["BoggleDB"].ConnectionString;
        }

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
            if (player.Nickname == null || player.Nickname.Trim().Length == 0)
            {
                SetStatus(Forbidden);
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
                            SetStatus(Created);
                            trans.Commit();
                            Nickname data = new Nickname();
                            data.UserToken = userToken;
                            return data;
                        }
                        SetStatus(Forbidden);
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
        public GameIDThing JoinGame(TimeThing player)
        {

            if (player.UserToken == null)
            {
                SetStatus(Forbidden);
                return null;
            }
            if (player.TimeLimit < 5 || player.TimeLimit > 120)
            {
                SetStatus(Forbidden);
                return null;
            }
            using(SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                using(SqlTransaction trans = conn.BeginTransaction())
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
                                SetStatus(Forbidden);
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
                        using(SqlDataReader reader = command.ExecuteReader())
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
                    if(player1 == player.UserToken)
                    {
                        SetStatus(Conflict);
                        return null;
                    }
                    if(game.GameID != 0)
                    {
                        DateTime startTime = DateTime.Now;

                        using (SqlCommand command =
                            new SqlCommand("UPDATE Games SET Player2 = @Player2, Board = @Board, TimeLimit = @TimeLimit, StartTime = @StartTime WHERE GameID = @GameID",
                            conn,
                            trans))
                        {
                            command.Parameters.AddWithValue("@Player2", player.UserToken);
                            command.Parameters.AddWithValue("@Board", new BoggleBoard().ToString());
                            command.Parameters.AddWithValue("@TimeLimit", game.TimeLimit);
                            command.Parameters.AddWithValue("@StartTime", startTime);
                            command.Parameters.AddWithValue("@GameID", game.GameID);
                            
                            // make sure 1 row was modified
                            if (command.ExecuteNonQuery() == 1)
                            {
                                GameIDThing data = new GameIDThing();
                                data.GameID = game.GameID;
                                SetStatus(Created);
                                trans.Commit();
                                return data;
                            }
                            SetStatus(Forbidden);
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
                        SetStatus(Accepted);
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
        public void CancelJoin(WordThing player)
        {
            if (player.UserToken == null)
            {
                SetStatus(Forbidden);
                return;
            }
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    using(SqlCommand command = new SqlCommand("DELETE FROM Games WHERE Player1 = @Player1 and Player2 IS NULL", conn, trans))
                    {
                        command.Parameters.AddWithValue("@Player1", player.UserToken);

                        if (command.ExecuteNonQuery() == 0)
                        {
                            SetStatus(Forbidden);
                        }
                        else
                        {
                            SetStatus(OK);
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
        public WordItem PlayWord(WordThing player, string gameID)
        {
            //playerID null check
            if (player.UserToken == null)
            {
                SetStatus(Forbidden);
                return null;
            }
            //word null check
            if (player.Word == null || player.Word.Trim().Length == 0)
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
                                SetStatus(Forbidden);
                                trans.Commit();
                                return null;
                            }
                        }
                    }
                    string board = "";
                    using (SqlCommand command = new SqlCommand("select Board, TimeLimit, StartTime from Games where GameID = @GameID and Player1 = @Player or Player2 = @Player", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", gameID);
                        command.Parameters.AddWithValue("@Player", player.UserToken);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                // game not found
                                SetStatus(Forbidden);
                                //trans.Commit();
                                return null;
                            }
                            else
                            {
                                reader.Read();
                                board = reader["Board"].ToString();
                                int timeLimit = (int)reader["TimeLimit"];
                                int colIndex = reader.GetOrdinal("StartTime");
                                DateTime startTime = reader.GetDateTime(colIndex);
                                if (startTime.AddSeconds(timeLimit) > DateTime.Now)
                                {
                                    SetStatus(Conflict);
                                    //trans.Commit();
                                    return null;
                                }
                            }
                        }
                    }
                    if(board == "")
                    {
                        SetStatus(Forbidden);
                        trans.Commit();
                        return null;
                    }

                    // get word score
                    WordItem word = new WordItem();
                    word.Word = player.Word;
                    using(SqlCommand command = new SqlCommand("select Word from Words where GameID = @GameID and Word = @Word", conn, trans))
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
                    using(SqlCommand command = new SqlCommand("insert into Words(GameID, Player, Word, Score) value (@GameID, @Player, @Word, @Score", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", gameID);
                        command.Parameters.AddWithValue("@Player", player.UserToken);
                        command.Parameters.AddWithValue("@Word", word.Word);
                        command.Parameters.AddWithValue("@Score", word.Score);
                        
                        // make sure 1 row was modified
                        if (command.ExecuteNonQuery() == 1)
                        {
                            word.Word = null;
                            SetStatus(OK);
                            trans.Commit();
                            return word;
                        }
                        SetStatus(Forbidden);
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
        public Status GameStatus(string gameID, string brief)
        {
            int id;
            if (!int.TryParse(gameID, out id))
            {
                SetStatus(Forbidden);
                return null;
            }
            using (SqlConnection conn = new SqlConnection(BoggleDB))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    Status status = new Status();
                    using (SqlCommand command = new SqlCommand("select Player1, Player2, TimeLimit, StartTime, Board from Games where GameID = @GameID", conn, trans))
                    {
                        command.Parameters.AddWithValue("@GameID", gameID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                // game not found
                                SetStatus(Forbidden);
                                trans.Commit();
                                return null;
                            }
                            reader.Read();
                            SetStatus(OK);
                            if (reader["Player2"] == null)
                            {
                                status.GameState = "pending";
                                trans.Commit();
                                return status;
                            }
                            string s = reader.GetName(1);
                            status.Player1.UserToken = reader["Player1"].ToString();
                            status.Player2.UserToken = reader["Player2"].ToString();
                            status.Board = reader["Board"].ToString();
                            int timeLimit = (int)reader["TimeLimit"];
                            int colIndex = reader.GetOrdinal("StartTime");
                            DateTime startTime = reader.GetDateTime(colIndex);
                            if (startTime.AddSeconds(timeLimit) > DateTime.Now)
                            {
                                status.GameState = "completed";
                            }
                            else
                            {
                                status.GameState = "active";
                            }
                            status.TimeLeft = (DateTime.Now - startTime).Seconds;
                            status.TimeLimit = timeLimit;
                        }
                    }

                    // player1 words
                    using(SqlCommand command = new SqlCommand("select Word, Score from Words where UserID = @UserID and GameID = @GameID", conn, trans))
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

                    // player1 words
                    using (SqlCommand command = new SqlCommand("select Word, Score from Words where UserID = @UserID and GameID = @GameID", conn, trans))
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
                                SetStatus(Forbidden);
                                trans.Commit();
                                return null;
                            }
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
                                SetStatus(Forbidden);
                                trans.Commit();
                                return null;
                            }
                            status.Player2.Nickname = reader["Nickname"].ToString();

                            status.Player2.UserToken = null;
                        }
                    }
                    if(status.GameState == "active")
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
            int length = word.Length;
            BoggleBoard board = new BoggleBoard(letters);
            if (!board.CanBeFormed(word))
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
            return composite;
        }
    }
}
