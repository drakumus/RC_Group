using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boggle
{
    class Program
    {
        /// <summary>
        /// Launches a BoggleServer on port 60000.  Keeps the main
        /// thread active so we can send output to the console.
        /// </summary>
        static void Main(string[] args)
        {
            new BoggleServer(60000);
            Console.ReadLine();
        }
    }
}
