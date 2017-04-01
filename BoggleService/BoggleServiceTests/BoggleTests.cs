﻿using System;
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
            player.Nickname = name;
            Response r = client.DoPostAsync("users", player).Result;
            //Assert.AreEqual(Created, r.Status);
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

            Response r = client.DoPostAsync("games", player).Result;
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
        /// <param name="user1Token"></param>
        /// <param name="user1Time"></param>
        /// <returns></returns>
        private Response MakeGame(string user1Token, int user1Time, bool useAsserts)
        {
            Response r = MakePlayer("Tod");
            Assert.AreEqual(Created, r.Status);
            
            string user2Token = r.Data.UserToken;

            Response r1 = JoinGame(user1Token, user1Time);
            if(useAsserts)
                Assert.AreEqual(Accepted, r1.Status);
            Response r2 = JoinGame(user2Token, 20);
            if(useAsserts)
                Assert.AreEqual(Created, r2.Status);
            return r1;
        }

        private Response PlayWord(string userToken, string word, int gameID)
        {
            dynamic playerData = new ExpandoObject();
            playerData.UserToken = userToken;
            playerData.Word = word;

            Response r = client.DoPutAsync(playerData, "games/" + gameID.ToString()).Result;
            return r;
        }

        private Response GameStatus(int gameID, bool brief)
        {
            Response r = client.DoGetAsync("games/" + gameID.ToString()).Result;

            return r;
        }

        /// <summary>
        /// Test create player
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            Response r = MakePlayer("Joe");
            Assert.AreEqual(Created, r.Status);
        }

        /// <summary>
        /// Test Join Game Process
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            Response r1 = MakePlayer("Joe");
            string player1Token = r1.Data.UserToken;

            int gameID = MakeGame(player1Token, 10, true).Data.GameID;
        }

        /// <summary>
        /// Test Retrieving Game Status before second player joins. Note: this method passes 
        /// </summary>
        [TestMethod]
        public void TestMethod3()
        {
            string player1Token = MakePlayer("Joe").Data.UserToken;
            Response t = JoinGame(player1Token, 25);
            int gameID = t.Data.GameID;
            Response r = GameStatus(gameID, false);
            Assert.AreEqual(OK, r.Status);
            Assert.AreEqual("pending", r.Data.GameState);
            StopIIS();
        }

        /// <summary>
        /// Test Cancel Game Join
        /// </summary>
        [TestMethod]
        public void TestMethod4()
        {
            string player1Token = MakePlayer("Joe").Data.UserToken;
            JoinGame(player1Token, 25);
            Assert.AreEqual(OK, CancelJoinGame(player1Token).Status);
        }

        /// <summary>
        /// Test Play Word invalid word and proper scoring return
        /// </summary>
        [TestMethod]
        public void TestMethod5()
        {
            string player1Token = MakePlayer("Joe").Data.UserToken;
            int gameID = MakeGame(player1Token, 25, true).Data.GameID;
            int score = PlayWord(player1Token, "tartwordofthedays", gameID).Data.Score;
            Assert.AreEqual(-1, score);
        }

        /// <summary>
        /// Test null input for Regester/Make Player
        /// </summary>
        [TestMethod]
        public void TestMethod6()
        {
            Response r = MakePlayer(null);
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Test empty input for Register/Make Player
        /// </summary>
        [TestMethod]
        public void TestMethod7()
        {
            Response r = MakePlayer("");
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Test null Token for JoinGame
        /// </summary>
        [TestMethod]
        public void TestMethod8()
        {
            Response r = JoinGame(null, 5);
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Tests invalid token passed
        /// </summary>
        [TestMethod]
        public void TestMethod9()
        {
            Response r = JoinGame("invalidToken", 5);
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Tests invalid time passed lower bound
        /// </summary>
        [TestMethod]
        public void TestMethod10()
        {
            string user1Token = MakePlayer("Joe").Data.UserToken;
            Response r = JoinGame(user1Token, 0);
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Tests invalid time passed upper bound
        /// </summary>
        [TestMethod]
        public void TestMethod11()
        {
            string user1Token = MakePlayer("Joe").Data.UserToken;
            Response r = JoinGame(user1Token, 10000);
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Test for first user joining game
        /// </summary>
        [TestMethod]
        public void TestMethod12()
        {
            string user1Token = MakePlayer("Joe").Data.UserToken;
            Response r = JoinGame(user1Token, 50);
            Assert.AreEqual(Accepted, r.Status);
        }

        /// <summary>
        /// Test status for second user
        /// </summary>
        [TestMethod]
        public void TestMethod13()
        {
            string user1Token = MakePlayer("Joe").Data.UserToken;
            Response r = JoinGame(user1Token, 50);
            Assert.AreEqual(Created, r.Status);
        }

        /// <summary>
        /// PLAY WORD TESTS
        /// ------------------------------------------------------------------------------------------------------------------------
        /// Tests Null Word played
        /// </summary>
        [TestMethod]
        public void TestMethod14()
        {
            Response r1 = MakePlayer("Joe");
            string player1Token = r1.Data.UserToken;
            int gameID = MakeGame(player1Token, 100, true).Data.GameID;
            Response r = PlayWord(player1Token, null, gameID);
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Tests null user token
        /// </summary>
        [TestMethod]
        public void TestMethod15()
        {
            Response r1 = MakePlayer("Joe");
            string player1Token = r1.Data.UserToken;
            int gameID = MakeGame(player1Token, 100, true).Data.GameID;
            Response r = PlayWord(player1Token, "", gameID);
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Tests null playerToken
        /// </summary>
        [TestMethod]
        public void TestMethod16()
        {
            Response r1 = MakePlayer("Joe");
            string player1Token = r1.Data.UserToken;
            int gameID = MakeGame(player1Token, 100, true).Data.GameID;
            Response r = PlayWord(null, "tower", gameID);
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Tests invalid object time for gameID
        /// </summary>
        [TestMethod]
        public void TestMethod17()
        {
            Response r1 = MakePlayer("Joe");
            string player1Token = r1.Data.UserToken;
            int gameID = MakeGame(player1Token, 100, true).Data.GameID;
            
            dynamic playerData = new ExpandoObject();
            playerData.UserToken = player1Token;
            playerData.Word = "towel";

            Response r = client.DoPutAsync(playerData, "games/" + "quack").Result;
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Tests for invalid game ID
        /// </summary>
        [TestMethod]
        public void TestMethod18()
        {
            Response r1 = MakePlayer("Joe");
            string player1Token = r1.Data.UserToken;
            int gameID = MakeGame(player1Token, 100, true).Data.GameID;
            Response r = PlayWord(player1Token, "tower", 1999);
            Assert.AreEqual(Forbidden, r.Status);
        }

        /// <summary>
        /// Tests play word on pending game
        /// </summary>
        [TestMethod]
        public void TestMethod19()
        {
            Response r1 = MakePlayer("Joe");
            
            string player1Token = r1.Data.UserToken;
            dynamic player = new ExpandoObject();
            player.UserToken = player1Token;
            player.TimeLimit = 55;

            Response r2 = client.DoPostAsync("games", player).Result;

            Response r = PlayWord(player1Token, "boggle", r2.Data.GameID);
            Assert.AreEqual(Conflict, r);

            StopIIS();
        }

        /// <summary>
        /// GAME STATUS TESTS
        /// -------------------------------------------------------------------------------------
        /// 
        /// </summary>
        [TestMethod]
        public void TestMethod20()
        {
            Response r1 = MakePlayer("Joe");
            string player1Token = r1.Data.UserToken;
            int gameID = MakeGame(player1Token, 20, false).Data.GameID;

            Response r = client.DoGetAsync("games/quack").Result;

            Assert.AreEqual(Forbidden, r.Status);
        }

        [TestMethod]
        public void TestMethod21()
        {
            Response r = client.DoGetAsync("games/12121").Result;

            Assert.AreEqual(Forbidden, r.Status);
        }
    }
}
