using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    interface IBoggleView
    {
        event Action CloseEvent;
        event Action<string> WordAddedEvent; //event for when client attempts to add a word
        event Action ConnectEvent;
        event Action CreateGameEvent;

        string[] Letters { set; } //used for showing sides of a cube
        string Player1Name { set; } //set Player1 in Window
        string Player2Name { set; } //set Player2 in Window
        string Player1Score { set; } //set Player1 score in Window
        string Player2Score { set; } //set Player2 score in Window
        int Time { set; }
        int Name { set; }
        int Server { set; }

        void DoClose();
    }
}
