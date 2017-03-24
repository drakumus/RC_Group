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
        public GameOverWindow()
        {
            InitializeComponent();
        }

        public string Player1Name
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Player1Score
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

        public int Player2Score
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public Dictionary<string, int> Words1 
        {
            set
            {
                foreach(string word in value.Keys)
                {
                    ListBox wordBox = new ListBox();
                    wordBox.Text = word + "\t" + value[word].ToString();
                    //Player1Box.Controls.Add()
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
                    //Player1Box.Controls.Add()
                }
            }
        }

        public event Action CloseEvent;

        public void DoClose()
        {
            throw new NotImplementedException();
        }

        public void DoShow()
        {
            throw new NotImplementedException();
        }
    }
}
