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
    public partial class GameOverWindow : Form, IGameOver
    {
        Dictionary<string, int> p1Words;
        Dictionary<string, int> p2Words;

        public GameOverWindow()
        {
            InitializeComponent();
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

        public List<Word> Player1Words
        {
            set
            {
                foreach (Word w in value)
                {
                    player1Box.Items.Add(w.word + "\t" + w.score);
                }
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

        public List<Word> Player2Words
        {
            set
            {
                foreach (Word w in value)
                {
                    player2Box.Items.Add(w.word + "\t" + w.score);
                }
            }
        }
        /*
        public Dictionary<string, int> Words1 
        {
            set
            {
                foreach(string word in value.Keys)
                {
                    ListBox wordBox = new ListBox();
                    wordBox.Text = word + "\t" + value[word].ToString();
                    player1Box.Controls.Add(wordBox);
                }
            }
        }

        public Dictionary<string, int> Words2
        {
            set
            {
                foreach (string word in value.Keys)
                {
                    ListBox wordBox = new ListBox();
                    wordBox.Text = word + "\t" + value[word].ToString();
                    player2Box.Controls.Add(wordBox);
                }
            }
        }*/
    }
}
