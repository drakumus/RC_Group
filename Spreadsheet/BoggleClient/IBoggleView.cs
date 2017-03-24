using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    interface IBoggleView
    {
        event Action<string> WordAddedEvent; //event for when client attempts to add a word
        event Action<string, string> ConnectEvent;
        event Action<string> CreateGameEvent;

        
        string[] Letters { set; } //used for showing sides of a cube
        string Player1Name { set; } //set Player1 in Window
        string Player2Name { set; } //set Player2 in Window
        
        string ConnectButtonText { set; }
        string CreateGameButtonText { set; }

        int Time { set; }
        int Player1Score { set; } //set Player1 score in Window
        int Player2Score { set; } //set Player2 score in Window

        bool Connected { set; }
    }
}
