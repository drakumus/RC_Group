using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    public interface IGameOver
    {
        string Player1Name { set; }
        string Player2Name { set; }
        int Player1Score { set; }
        int Player2Score { set; }

        Dictionary<string, int> Words1 { set; }
        Dictionary<string, int> Words2 { set; }
    }
}
