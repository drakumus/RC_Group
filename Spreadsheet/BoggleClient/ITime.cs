using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    interface ITime
    {
        event Action CloseEvent;
        event Action<int> ConfirmEvent; //event for user button press time confirm
        event Action CancelEvent; //event for user button press cancel

        string status { set; } //used for various status messages in the program

    }
}
