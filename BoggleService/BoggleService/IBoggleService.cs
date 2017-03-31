using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Boggle
{
    [ServiceContract]
    public interface IBoggleService
    {
        /// <summary>
        /// Registers a new user.
        /// If nickname is null or is empty after trimming, responds with status code Forbidden.
        /// Otherwise, creates a user, returns the user's token, and responds with status code Created. 
        /// </summary>
        /// <param name="nickname"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/users")]
        string Register(string nickname);

        /// <summary>
        /// Attempts to join a game.
        /// If userToken isn't known, timeLimit less than 5 or timeLimit greater than 120, responds with status code Forbidden.
        /// Otherwise, if there is no pending game, ceates one and responds with Accepted.
        /// If there is a pending game, starts the game and responds with Created.
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="timeLimit"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/games")]
        string JoinGame(string userToken, int timeLimit);

        /// <summary>
        /// Plays a word in an active game
        /// </summary>
        /// <param name="gameID">gameID</param>
        /// <param name="userToken">user playing word's token</param>
        /// <param name="word">word being played</param>
        /// <returns></returns>
        [WebInvoke(Method = "PUT", UriTemplate = "games/{GameID}")]
        string PlayWord(int gameID, string userToken, string word);

        /// <summary>
        /// Returns the status of the game
        /// TODO: Handle brief
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns></returns>
        [WebInvoke(Method = "GET", UriTemplate = "games/{GameID}")]
        Game GameStatus(int gameID);
        /// <summary>
        /// Sends back index.html as the response body.
        /// </summary>
        [WebGet(UriTemplate = "/api")]
        Stream API();

        /// <summary>
        /// Returns the nth word from dictionary.txt.  If there is
        /// no nth word, responds with code 403. This is a demo;
        /// you can delete it.
        /// </summary>
        [WebGet(UriTemplate = "/word?index={n}")]
        string WordAtIndex(int n);


    }
}
