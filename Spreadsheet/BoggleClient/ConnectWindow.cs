using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoggleClient
{
    public partial class ConnectWindow : Form, IServer
    {
        public ConnectWindow()
        {
            InitializeComponent();
        }

        public string status
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public event Action CancelServer;
        public event Action CloseEvent;
        public event Action<string, string> ConfirmServer;
    }
}
