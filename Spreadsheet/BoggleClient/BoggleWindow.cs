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

        /// <summary>
        /// initialize boggle window
        /// </summary>
        public BoggleWindow()
        {
            InitializeComponent();
            buttons = new Button[] { button1, button2, button3, button4, button5, button6, button7, button8, button9, button10, button11,
            button12, button13, button14, button15, button16};
            wordList = new List<string>();
        }

        /// <summary>
        /// fill board
        /// </summary>
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


        /// <summary>
        /// set label for player1 Name
        /// </summary>
        public string Player1Name
        {
            set
            {
                SetText(player1Label, value);
            }
        }

        /// <summary>
        /// set label for Player1 Score
        /// </summary>
        public int Player1Score
        {
            set
            {
                SetText(player1ScoreLabel, value.ToString());
            }
        }

        /// <summary>
        /// player2 label set
        /// </summary>
        public string Player2Name
        {
            set
            {
                SetText(player2Label, value);
            }
        }

        /// <summary>
        /// Set label for player 2 socre
        /// </summary>
        public int Player2Score
        {
            set
            {
                SetText(player2ScoreLabel, value.ToString());
            }
        }

        /// <summary>
        /// Time label set
        /// </summary>
        public int Time
        {
            set
            {
                SetText(timeLabel, value.ToString());
            }
        }

        /// <summary>
        /// set text for the connect button. 
        /// </summary>
        public string ConnectButtonText
        {
            set
            {
                connectButton.Text = value;
            }
        }

        /// <summary>
        /// Set text for create Game Button
        /// </summary>
        public string CreateGameButtonText
        {
            set
            {
                SetText(createGameButton, value);
            }
        }

        /// <summary>
        /// Bool for if the user is connected. Used to prevent input that could cause errors.
        /// </summary>
        public bool Connected
        {
            set
            {
                isConnected = value;
            }
        }

        /// <summary>
        /// bool used for reseting game UI and other input safety checks.
        /// </summary>
        public bool PlayingGame
        {
            set
            {
                if (value == false)
                {
                    this.Invoke((MethodInvoker)delegate {reset(); });
                }
                isPlaying = value;
            }
        }

        /// <summary>
        /// Text Setter to show a message box from controller.
        /// </summary>
        public string MessageBoxText
        {
            set
            {
                MessageBox.Show(value);
            }
        }

        /// <summary>
        /// game state being presented as status.
        /// </summary>
        public string GameState
        {
            set
            {
                SetText(gameStateLabel, value);
            }
        }

        /// <summary>
        /// reset method used as a helper for reseting UI.
        /// </summary>
        private void reset()
        {
            WordsListBox.Items.Clear();
            wordBox.Clear();
            timeLabel.Text = "0";
            createGameButton.Text="Join Game";
            player1Label.Text = "Player1";
            player2Label.Text = "Player2";
            player1ScoreLabel.Text = "0";
            player2ScoreLabel.Text = "0";
        }

        /*
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
        */
        /// <summary>
        /// Passes name and server as parameters.
        /// </summary>
        public event Action<string, string> ConnectEvent;
        public event Action<string> WordAddedEvent;
        public event Action<string> CreateGameEvent;

        /// <summary>
        /// used to close safely
        /// </summary>
        public void DoClose()
        {
            this.Close();
        }

        /// <summary>
        /// Connect button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectButton_Click(object sender, EventArgs e)
        {
            ConnectEvent(nameBox.Text, serverBox.Text);
        }

        /// <summary>
        /// Create Game Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createGameButton_Click(object sender, EventArgs e)
        {
            if(isConnected)
                CreateGameEvent(timeBox.Text);
            else
            {
                MessageBox.Show("Please connect to a server before attempting to join a game.");
            }
        }

        /// <summary>
        /// Button click to score a word.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WordButton_Click(object sender, EventArgs e)
        {
            if (isConnected && !isPlaying)
                MessageBox.Show("Please join a game before attempting to interact with the game.");
            else if (isConnected && isPlaying)
            {
                WordAddedEvent(wordBox.Text);
                string w = wordBox.Text;
                w = w.ToLower();
                w = w.Trim();
                /*List word = new ListBox();
                word.Text = w;*/
                WordsListBox.Items.Add(w);
            }
            else
            {
                MessageBox.Show("Please connect to a server before attempting to interact with the game.");
            }
        }

        /// <summary>
        /// Button to open the help window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm help = new HelpForm();
            help.Show();
        }
        
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
                this.Invoke((MethodInvoker)delegate { SetText(label, text); });
            }
            else
            {
                label.Text = text;
            }
        }
    }
}
