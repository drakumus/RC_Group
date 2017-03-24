﻿using System;
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
        private bool isConnected;

        public BoggleWindow()
        {
            InitializeComponent();
        }


        public string[] Letters
        {
            set
            {
                button1.Text = value[0];
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
                timeLabel.Text = value.ToString();
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

        public event Action CloseEvent;
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
                MessageBox.Show("Please connect to a server before attempting to interact with the game");
            }
        }

        private void WordButton_Click(object sender, EventArgs e)
        {
            if(isConnected)
                WordAddedEvent(wordBox.Text);
            else
            {
                MessageBox.Show("Please connect to a server before attempting to interact with the game");
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
