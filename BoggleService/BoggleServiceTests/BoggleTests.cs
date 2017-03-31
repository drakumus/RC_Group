using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.HttpStatusCode;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Dynamic;

namespace Boggle
{
    /// <summary>
    /// Provides a way to start and stop the IIS web server from within the test
    /// cases.  If something prevents the test cases from stopping the web server,
    /// subsequent tests may not work properly until the stray process is killed
    /// manually.
    /// </summary> 
    public static class IISAgent
    {
        // Reference to the running process
        private static Process process = null;

        /// <summary>
        /// Starts IIS
        /// </summary>
        public static void Start(string arguments)
        {
            if (process == null)
            {
                ProcessStartInfo info = new ProcessStartInfo(Properties.Resources.IIS_EXECUTABLE, arguments);
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.UseShellExecute = false;
                process = Process.Start(info);
            }
        }

        /// <summary>
        ///  Stops IIS
        /// </summary>
        public static void Stop()
        {
            if (process != null)
            {
                process.Kill();
            }
        }
    }
    [TestClass]
    public class BoggleTests
    {
        /// <summary>
        /// This is automatically run prior to all the tests to start the server
        /// </summary>
        [ClassInitialize()]
        public static void StartIIS(TestContext testContext)
        {
            IISAgent.Start(@"/site:""BoggleService"" /apppool:""Clr4IntegratedAppPool"" /config:""..\..\..\.vs\config\applicationhost.config""");
        }

        /// <summary>
        /// This is automatically run when all tests have completed to stop the server
        /// </summary>
        [ClassCleanup()]
        public static void StopIIS()
        {
            IISAgent.Stop();
        }

        private RestTestClient client = new RestTestClient("http://localhost:60000/BoggleService.svc/");


        /// <summary>
        /// Requests user token for a provided nickname and returns said token.
        /// </summary>
        /// <param name="name"></param>
        private Response MakePlayer(string name)
        {
            dynamic player = new ExpandoObject();
            player.Nickname = "Player1";
            Response r = client.DoPostAsync("users", player).Result;
            return r;
        }

        /// <summary>
        /// Attempts to Join a game. To be used with CancelJoinGame tests.
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="userTime"></param>
        /// <returns></returns>
        private Response JoinGame(string userToken, int userTime)
        {
            dynamic player = new ExpandoObject();
            player.UserToken = userToken;
            player.TimeLimit = userTime;

            Response r = client.DoPostAsync("games", player);
            return r;
        }

        /// <summary>
        /// Cancels an attempted game join
        /// </summary>
        /// <param name="userToken"></param>
        /// <returns></returns>
        private Response CancelJoinGame(string userToken)
        {

            dynamic player = new ExpandoObject();
            player.UserToken = userToken;

            Response r = client.DoPutAsync(player, "games").Result;
            return r;
        }

        /// <summary>
        /// Generates a game ID and has two users join a game.
        /// </summary>
        /// <param name="user1Token">User 1's token</param>
        /// <param name="user1Time">User 1's time</param>
        /// <param name="user2Token">User 2's token</param>
        /// <param name="user2Time">User 2's time</param>
        /// <returns>Response returned. If successfull the return.Data will be set</returns>
        private Response MakeGame(string user1Token, int user1Time, string user2Token, int user2Time)
        {
            dynamic player1 = new ExpandoObject();
            dynamic player2 = new ExpandoObject();

            player1.UserToken = user1Token;
            player1.TimeLimit = user1Time;

            player2.UserToken = user1Token;
            player2.TimeLimit = user1Time;

            Response r1 = client.DoPostAsync("games", player1).Result;
            Response r2 = client.DoPostAsync("games", player2).Result;
            return r1;
        }

        private Response PlayWord(string userToken, string word, int gameID)
        {
            dynamic playerData = new ExpandoObject();
            playerData.UserToken = userToken;
            playerData.Word = word;

            Response r = client.DoPutAsync(playerData, "games/" + gameID.ToString());
            return r;
        }

        private Response GameStatus(int gameID)
        {
            Response r = client.DoGetAsync("games", gameID.ToString()).Result;

            return r;
        }

        /// <summary>
        /// Note that DoGetAsync (and the other similar methods) returns a Response object, which contains
        /// the response Stats and the deserialized JSON response (if any).  See RestTestClient.cs
        /// for details.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            dynamic user = new ExpandoObject();
            user.Nickname = "Joe";
            Response r = client.DoPostAsync("users", user).Result;
            Assert.AreEqual(Created, r.Status);
        }
    }
}
