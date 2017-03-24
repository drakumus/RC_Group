﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace BoggleClient
{
    class Controller
    {
        /// <summary>
        /// The view controlled by this Controller
        /// </summary>
        private IBoggleView window;

        private BoggleGame game;

        System.Timers.Timer refreshTimer;
        System.Timers.Timer countdownTimer;

        /// <summary>
        /// For canceling the current operation
        /// </summary>
        private CancellationTokenSource tokenSource;

        public Controller(IBoggleView window)
        {
            this.window = window;
            game = new BoggleGame();

            refreshTimer = new System.Timers.Timer();
            refreshTimer.Elapsed += RefreshTimerEvent;
            refreshTimer.Interval = 1000;
            countdownTimer = new System.Timers.Timer();
            countdownTimer.Elapsed += CountdownTimerEvent;
            countdownTimer.Interval = 1000;

            // Add Events
            window.ConnectEvent += HandleConnect;
            window.CreateGameEvent += HandleCreateGame;

        }

        /// <summary>
        /// Cancels the current operation (currently unimplemented)
        /// </summary>
        private void Cancel()
        {
            tokenSource.Cancel();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="server"></param>
        private async void HandleConnect(string name, string server)
        {
            if(server == "")
            {
                window.MessageBoxText = "Please enter a server IP";
                return;
            }
            if (game.connecting)
            {
                Cancel();
                return;
            }
            try
            {
                window.CreateGameButtonText = "Join Game";
                game.gameState = "not connected";
                window.GameState = game.gameState;
                game.connecting = true;
                BoggleGame.ip = server;
                window.ConnectButtonText = "Cancel";

                using (HttpClient client = CreateClient())
                {
                    // Create the parameter
                    dynamic user = new ExpandoObject();
                    user.Nickname = name;

                    // Compose and send the request
                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("users", content, tokenSource.Token);

                    // Deal with the response
                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        dynamic data = JsonConvert.DeserializeObject(result);
                        game.userToken = data.UserToken;
                        window.Connected = true;
                    }
                    else
                    {
                        window.MessageBoxText = "Error registering: " + response.StatusCode;
                        Console.WriteLine("Error registering: " + response.StatusCode);
                        Console.WriteLine(response.ReasonPhrase);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                game.connecting = false;
                window.ConnectButtonText = "Connect";
            }
            finally
            {
                window.ConnectButtonText = "Connect";
                game.connecting = false;
            }
        }

        private async void HandleCreateGame(string timeLimit)
        {
            if (game.gameState == "pending")
            {
                CancelJoinRequest();
                return;
            }
            try
            {
                game.gameState = "pending";
                window.GameState = game.gameState;
                window.CreateGameButtonText = "Cancel";

                using (HttpClient client = CreateClient())
                {
                    // Create the parameter
                    dynamic join = new ExpandoObject();
                    join.UserToken = game.userToken;
                    join.TimeLimit = timeLimit;

                    // Compose and send the request
                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(join), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("games", content, tokenSource.Token);

                    // Deal with the response
                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        dynamic data = JsonConvert.DeserializeObject(result);
                        game.gameID = data.GameID;
                        Refresh();
                        refreshTimer.Enabled = true;
                    }
                    else
                    {
                        window.MessageBoxText = "Error Joining: " + response.StatusCode;
                        Console.WriteLine("Error Joining: " + response.StatusCode);
                        Console.WriteLine(response.ReasonPhrase);

                        window.CreateGameButtonText = "Join Game";
                        game.gameState = "not connected";
                        window.GameState = game.gameState;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                window.CreateGameButtonText = "Join Game";
                game.gameState = "not connected";
                window.GameState = game.gameState;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CancelJoinRequest()
        {
            Cancel();

            using (HttpClient client = CreateClient())
            {
                // Create the parameter
                dynamic cancel = new ExpandoObject();
                cancel.UserToken = game.userToken;

                // Compose and send the request
                StringContent content = new StringContent(JsonConvert.SerializeObject(cancel), Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PutAsync("games", content).Result;

                // Deal with the response
                if (response.IsSuccessStatusCode)
                {
                    window.CreateGameButtonText = "Join Game";
                    game.gameState = "not connected";
                    window.GameState = game.gameState;
                }
                else
                {
                    window.MessageBoxText = "Error cancelling: " + response.StatusCode;
                    Console.WriteLine("Error cancelling: " + response.StatusCode);
                    Console.WriteLine(response.ReasonPhrase);
                    Refresh();
                }
            }
        }

        private void Refresh()
        {
            using (HttpClient client = CreateClient())
            {
                // Compose and send the request
                string url = string.Format("games/{0}", game.gameID);
                HttpResponseMessage response = client.GetAsync(url).Result;

                // Deal with the response
                if (response.IsSuccessStatusCode)
                {
                    String result = response.Content.ReadAsStringAsync().Result;
                    dynamic data = JsonConvert.DeserializeObject(result);
                    game.gameState = data.GameState;
                    window.GameState = game.gameState;
                    if(game.gameState != "pending")
                    {
                        game.timeRemaining = data.TimeLeft;
                        countdownTimer.Enabled = true;

                        dynamic player1 = data.Player1;
                        game.player1.UpdatePlayer(player1);
                        window.Player1Name = game.player1.name;
                        window.Player1Score = game.player1.score;

                        dynamic player2 = data.Player2;
                        game.player2.UpdatePlayer(player2);
                        window.Player2Name = game.player2.name;
                        window.Player2Score = game.player2.score;
                        if(game.gameState == "completed")
                        {
                            refreshTimer.Enabled = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event for refreshTimer Elapsed, gets the game status
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void RefreshTimerEvent(object source, ElapsedEventArgs e)
        {
            Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void CountdownTimerEvent(object source, ElapsedEventArgs e)
        {
            game.timeRemaining--;
            window.Time = game.timeRemaining;
            if(game.timeRemaining == 0)
            {
                countdownTimer.Enabled = false;
            }
        }

        // TODO: Need to add Play Word

        /// <summary>
        /// Creates an HttpClient for communicating with the server.
        /// </summary>
        private static HttpClient CreateClient()
        {
            // Create a client whose base address is the GitHub server
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(BoggleGame.ip);

            // Tell the server that the client will accept this particular type of response data
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // There is more client configuration to do, depending on the request.
            return client;
        }
    }
}
