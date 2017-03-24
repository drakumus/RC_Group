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
    public partial class BoggleWindow : Form, IBoggleView
    {
        public BoggleWindow()
        {
            InitializeComponent();
        }

        public string[] Letters
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Word
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public event Action CloseEvent;
        public event Action<string> WordAdded;
        public event Action ConnectEvent;
    }
}
