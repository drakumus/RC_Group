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
        Dictionary<string, int> Words1 { set; }
        Dictionary<string, int> Words2 { set; }
    }
}
