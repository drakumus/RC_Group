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

        /// <summary>
        /// The ip the user is connected to, or "" if not connected
        /// </summary>
        private static string IP;

        private bool Connecting;
        private bool CreatingGame;

        /// <summary>
        /// For canceling the current operation
        /// </summary>
        private CancellationTokenSource tokenSource;

        public Controller(IBoggleView window)
        {
            this.window = window;
            userToken = "0";
            IP = "";
            Connecting = false;
            CreatingGame = false;

            // Add Events
            window.CloseEvent += HandleClose;
            window.ConnectEvent += HandleConnect;


        }

        private void HandleClose()
        {
            // Closw window
            window.DoClose();
        }

        private async void HandleConnect(string name, string server)
        {
            try
            {
                Connecting = true;
                IP = server;
                window.ConnectButtonText = "Cancel";

                using (HttpClient client = CreateClient())
                {
                    // Create the parameter
                    dynamic user = new ExpandoObject();
                    user.name = name;

                    // Compose and send the request
                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("users", content, tokenSource.Token);

                    // Deal with the response
                    if (response.IsSuccessStatusCode)
                    {
                        string result = response.Content.ReadAsStringAsync().Result;
                        userToken = (string)JsonConvert.DeserializeObject(result);
                        //window.Connected = true;
                    }
                    else
                    {
                        Console.WriteLine("Error registering: " + response.StatusCode);
                        Console.WriteLine(response.ReasonPhrase);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                
            }
            finally
            {
                window.ConnectButtonText = "Connect";
            }
        }


        /// <summary>
        /// Creates an HttpClient for communicating with the server.
        /// </summary>
        private static HttpClient CreateClient()
        {
            // Create a client whose base address is the GitHub server
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(IP);

            // Tell the server that the client will accept this particular type of response data
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // There is more client configuration to do, depending on the request.
            return client;
        }
    }
}
