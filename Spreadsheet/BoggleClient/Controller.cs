using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    class Controller
    {
        private IBoggleView window;

        public Controller(IBoggleView window)
        {
            this.window = window;

            // Add Events
            window.CloseEvent += HandleClose;
            window.ConnectEvent += HandleConnect;


        }

        private void HandleClose()
        {
            // Closw window
            throw new NotImplementedException();
        }

        private void HandleConnect()
        {
            throw new NotImplementedException();
        }


    }
}
