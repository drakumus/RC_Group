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
        private bool isConnected, isPlaying;

        public BoggleWindow()
        {
            InitializeComponent();
        }


        public char[] Letters
        {
            set
            {
                button1.Text = value[0].ToString();
            }
        }

        public string Player1Name
        {
            set
            {
                player1Label.Text = value;
            }
        }

        public int Player1Score
        {
            set
            {
                player1ScoreLabel.Text = value.ToString();
            }
        }

        public string Player2Name
        {
            set
            {
                player2Label.Text = value;
            }
        }

        public int Player2Score
        {
            set
            {
                player2ScoreLabel.Text = value.ToString();
            }
        }

        public int Time
        {
            set
            {
                SetText(timeLabel, value.ToString());
            }
        }

        public string ConnectButtonText
        {
            set
            {
                connectButton.Text = value;
            }
        }

        public string CreateGameButtonText
        {
            set
            {
                createGameButton.Text = value;
            }
        }

        public bool Connected
        {
            set
            {
                isConnected = value;
            }
        }

        public bool PlayingGame
        {
            set
            {
                value = isPlaying;
            }
        }

        public string MessageBoxText
        {
            set
            {
                MessageBox.Show(value);
            }
        }

        public string GameState
        {
            set
            {
                SetText(gameStateLabel, value);
            }
        }

        /// <summary>
        /// Passes name and server as parameters.
        /// </summary>
        public event Action<string, string> ConnectEvent;
        public event Action<string> WordAddedEvent;
        public event Action<string> CreateGameEvent;

        public void DoClose()
        {
            this.Close();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if(nameBox.Text != null && serverBox.Text != null)
            {
                ConnectEvent(nameBox.Text, serverBox.Text);
            }
            else
            {
                // TODO: Needs values alert
            }
        }

        private void createGameButton_Click(object sender, EventArgs e)
        {
            if(isConnected)
                CreateGameEvent(timeBox.Text);
            else
            {
                MessageBox.Show("Please connect to a server before attempting to join a game.");
            }
        }

        private void WordButton_Click(object sender, EventArgs e)
        {
            if (isConnected && !isPlaying)
                MessageBox.Show("Please join a game before attempting to interact with the game.");
            else if(isConnected && isPlaying)
                WordAddedEvent(wordBox.Text);
            else
            {
                MessageBox.Show("Please connect to a server before attempting to interact with the game.");
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm help = new HelpForm();
            help.Show();
        }

        /// <summary>
        /// Callback for SetText
        /// </summary>
        /// <param name="label"></param>
        /// <param name="text"></param>
        delegate void SetTextCallback(Label label, string text);
        /// <summary>
        /// Sets the text of specifiedLabel while checking for thread safety
        /// </summary>
        /// <param name="label"></param>
        /// <param name="text"></param>
        private void SetText(Label label, string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (label.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { label, text });
            }
            else
            {
                label.Text = text;
            }
        }
    }
}
