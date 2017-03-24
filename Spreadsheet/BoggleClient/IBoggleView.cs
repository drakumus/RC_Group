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
        event Action<string> WordAdded; //event for when client attempts to add a word
        event Action ConnectEvent;

        string Word { set; } //used when adding word to live game list

        string[] Letters { set; } //used for showing sides of a cube

    }
}
