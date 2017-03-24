using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BoggleClient
{
    class Controller
    {
        /// <summary>
        /// The view controlled by this Controller
        /// </summary>
        private IBoggleView window;

        /// <summary>
        /// The token of the most recently registered user, or "0" if no user
        /// has ever registered
        /// </summary>
        private string userToken;

        private string gameID;

        /// <summary>
        /// The ip the user is connected to, or "" if not connected
        /// </summary>
        private static string ip;

        private bool connecting;
        private bool joiningGame;

        /// <summary>
        /// For canceling the current operation
        /// </summary>
        private CancellationTokenSource tokenSource;

        public Controller(IBoggleView window)
        {
            this.window = window;
            userToken = "0";
            ip = "";
            gameID = "";
            connecting = false;
            joiningGame = false;

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
            if (connecting)
            {
                Cancel();
                return;
            }
            try
            {
                connecting = true;
                ip = server;
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
                        userToken = data.UserToken;
                        window.Connected = true;
                        window.ConnectButtonText = "Connected";
                    }
                    else
                    {
                        Console.WriteLine("Error registering: " + response.StatusCode);
                        Console.WriteLine(response.ReasonPhrase);
                        window.ConnectButtonText = "Connect";
                    }
                }
            }
            catch (TaskCanceledException)
            {
                window.ConnectButtonText = "Connect";
            }
            finally
            {
                connecting = false;
            }
        }

        private async void HandleCreateGame(string timeLimit)
        {
            if (joiningGame)
            {
                Cancel();
                return;
            }
            try
            {
                joiningGame = true;
                window.CreateGameButtonText = "Cancel";

                using (HttpClient client = CreateClient())
                {
                    // Create the parameter
                    dynamic game = new ExpandoObject();
                    game.UserToken = userToken;
                    game.TimeLimit = timeLimit;

                    // Compose and send the request
                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(game), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("games", content, tokenSource.Token);

                    // Deal with the response
                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        dynamic data = JsonConvert.DeserializeObject(result);
                        gameID = data.GameID;
                    }
                    else
                    {
                        Console.WriteLine("Error Joining: " + response.StatusCode);
                        Console.WriteLine(response.ReasonPhrase);
                    }
                }
            }
            catch (TaskCanceledException)
            {

            }
            finally
            {
                window.CreateGameButtonText = "Join Game";
                joiningGame = false;
            }
        }

        /// <summary>
        /// Creates an HttpClient for communicating with the server.
        /// </summary>
        private static HttpClient CreateClient()
        {
            // Create a client whose base address is the GitHub server
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ip);

            // Tell the server that the client will accept this particular type of response data
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // There is more client configuration to do, depending on the request.
            return client;
        }
    }
}
