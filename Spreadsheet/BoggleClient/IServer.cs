using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    interface IServer
    {
        event Action CloseEvent;
        event Action<string, string> ConfirmServer; //event for when server+name are selected
        event Action CancelServer; //event for if the user selects cancel

        string status { set; } //used for various status messages in the program

    }
}
