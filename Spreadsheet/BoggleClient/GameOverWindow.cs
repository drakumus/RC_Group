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

        public string[] Words1
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public string[] Words2
        {
            set
            {
                throw new NotImplementedException();
            }
        }

        public event Action CloseEvent;
    }
}
