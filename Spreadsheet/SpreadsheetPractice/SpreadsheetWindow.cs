using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;
using SSGui;
using Formulas;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace SpreadsheetController
{
    public partial class SpreadsheetWindow : Form, ISpreadsheetView
    {

        public SpreadsheetWindow()
        {
            InitializeComponent();
            spreadsheetPanel1.SelectionChanged += displaySelection;

            int startRow = 0;
            int startCol = 0;
            spreadsheetPanel1.SetSelection(startCol, startRow);
        }

        /// <summary>
        /// Sets the value box in the UI.
        /// </summary>
        public string Value
        {
            set
            {
                valueBox.Text = value;
            }
        }

        /// <summary>
        /// Sets the cell box in the UI.
        /// </summary>
        public string Cell
        {
            set
            {
                cellBox.Text = value;
            }
        }

        /// <summary>
        /// Sets the contents box in the UI.
        /// </summary>
        public string Contents
        {
            set
            {
                contentsBox.Text = value;
            }
        }

        /// <summary>
        /// Shows the message in the UI.
        /// </summary>
        public string Message
        {
            set
            {
                MessageBox.Show(value);
            }
        }

        public event Action CloseEvent;

        public event Action<string> FileChosenEvent;

        public event Action NewEvent;

        public event Action SaveEvent;

        public event Action<string> EditEvent;

        public event Action<int, int> UpdateEvent;

        /// <summary>
        /// Event for when a cell is clicked in the spreadsheetPanel.
        /// </summary>
        /// <param name="ss">current SpreadsheetPanel</param>
        private void displaySelection(SpreadsheetPanel ss)
        {
            int row;
            int col;
            ss.GetSelection(out col, out row);

            if (UpdateEvent != null)
            {
                UpdateEvent(col, row);
            }
        }

        /// <summary>
        /// on menu select File>Close Closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(CloseEvent != null)
            {
                CloseEvent();
            }
        }

        /// <summary>
        /// Opens an xml.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //prompts for OpenFileDialog and saves the result if selection is OK to filePath.
            //filePath is then used to initialize a new spreadsheet
            var result = new System.Windows.Forms.OpenFileDialog();
            result.Filter = "Spreadsheet Files (*.ss)|*.ss|All files (*.*)|*.*";

            if (result.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (CloseEvent != null)
                {
                    FileChosenEvent(result.FileName);
                }
            }
        }

        /// <summary>
        /// on menu Select File>New clears current spreadsheet for a new one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }

        /// <summary>
        /// Saves the spreadsheet to the specified path.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveEvent != null)
            {
                SaveEvent();
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (EditEvent != null)
            {
                EditEvent(contentsBox.Text);
            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm help = new HelpForm();
            help.Show();
        }

        public void DoClose()
        {
            Close();
        }

        public void OpenNew()
        {
            SpreadsheetApplicationContext.GetContext().RunNew();
        }

        public void OpenNew(string filePath)
        {
            SpreadsheetApplicationContext.GetContext().RunNew(filePath);
        }

        public void SetValue(int col, int row, string cellValue)
        {
            spreadsheetPanel1.SetValue(col, row, cellValue);
        }
    }
}
