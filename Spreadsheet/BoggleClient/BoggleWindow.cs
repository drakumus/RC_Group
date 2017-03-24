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

        public string Player1Name
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Player1Score
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Player2Name
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public string Player2Score
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Server
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Time
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        int IBoggleView.Name
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public event Action CloseEvent;
        public event Action ConnectEvent;
        public event Action<string> WordAddedEvent;
        public event Action CreateGameEvent;

        public void DoClose()
        {
            this.Close();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            ConnectEvent();
        }

        private void createGameButton_Click(object sender, EventArgs e)
        {
            CreateGameEvent();
        }

        private void WordButton_Click(object sender, EventArgs e)
        {
            WordAddedEvent(wordBox.Text);
        }
    }
}
