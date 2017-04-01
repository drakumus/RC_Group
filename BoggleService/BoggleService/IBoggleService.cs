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
        Nickname Register(PlayerInfo nickname);

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
        GameIDThing JoinGame(TimeThing data);

        /// <summary>
        /// Attemts to cancel a pending request to join a game.
        /// If userToken isn't known or is not a player in a pending game, responds with Forbidden.
        /// Otherwise, removes the UserToken from the pending game and responds with OK.
        /// </summary>
        /// <param name="userToken"></param>
        [WebInvoke(Method = "PUT", UriTemplate = "/games")]
        void CancelJoin(WordThing userToken);

        /// <summary>
        /// Plays a word in a game.
        /// If word is null or empty when trimmed, or if gameID or userToken is missing or invalid,
        /// of if userToken is not a player in the game identified by gameID, responds with Forbidden.
        /// Otherwise, if the game state is anything other than active, responds with Conflict.
        /// Otherwise, records the trimmed word as being played by userToken in the game identified by gameID.
        /// Returns the score for word in the context of the game and responds with the status OK.
        /// </summary>
        /// <param name="gameID">gameID</param>
        /// <param name="userToken">user playing word's token</param>
        /// <param name="word">word being played</param>
        /// <returns></returns>
        [WebInvoke(Method = "PUT", UriTemplate = "/games/{GameID}")]
        WordItem PlayWord(WordThing data, string gameID);

        /// <summary>
        /// Gets the status of a game.
        /// If gameID is invalid, responds with Forbidden.
        /// Otherwise, returns information about the game named by gameID.
        /// Note that the information returned depends on whether "Breif=yes" was included as a parameter
        /// as well as on the state of the game. Responds with status code OK.
        /// </summary>
        /// <param name="gameID"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/games/{GameID}?brief={brief}")]
        Status GameStatus(string gameID, string brief);



        /// <summary>
        /// Sends back index.html as the response body.
        /// </summary>
        [WebGet(UriTemplate = "/api")]
        Stream API();
    }
}
