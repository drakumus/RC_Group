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
    }
}
