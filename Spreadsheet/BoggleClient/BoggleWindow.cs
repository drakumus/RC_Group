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
        private Button[] buttons;
        private List<string> wordList;

        public BoggleWindow()
        {
            InitializeComponent();
            buttons = new Button[] { button1, button2, button3, button4, button5, button6, button7, button8, button9, button10,
            button12, button13, button14, button15, button16};
            wordList = new List<string>();
        }


        public char[] Letters
        {
            set
            {
                for(int i=0; i < buttons.Length; i++)
                {
                    SetText(buttons[i], value[i].ToString());
                }
            }
        }

        public string Player1Name
        {
            set
            {
                SetText(player1Label, value);
            }
        }

        public int Player1Score
        {
            set
            {
                SetText(player1ScoreLabel, value.ToString());
            }
        }

        public string Player2Name
        {
            set
            {
                SetText(player2Label, value);
            }
        }

        public int Player2Score
        {
            set
            {
                SetText(player1ScoreLabel, value.ToString());
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
                isPlaying = value;
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

        public Dictionary<string, int> EnteredWords
        {
            set
            {
                foreach(string word in value.Keys)
                {
                    if (!wordList.Contains(word))
                    {
                        ListBox w = new ListBox();
                        w.Text = word + "\t" + value[word];
                        wordBox.Controls.Add(w);
                        wordList.Add(word);
                    }
                }
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
            else if (isConnected && isPlaying)
            {
                WordAddedEvent(wordBox.Text);
            }
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
        delegate void SetTextCallback(Control label, string text);
        /// <summary>
        /// Sets the text of specifiedLabel while checking for thread safety
        /// </summary>
        /// <param name="label"></param>
        /// <param name="text"></param>
        private void SetText(Control label, string text)
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
