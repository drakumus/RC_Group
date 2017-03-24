using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    public interface IGameOver
    {
        event Action CloseEvent;
        string[] Words1 { set; }
        string[] Words2 { set; }
    }
}
