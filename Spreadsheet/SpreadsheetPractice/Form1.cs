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

namespace SpreadsheetPractice
{
    public partial class Form1 : Form
    {
        private Spreadsheet sheet;
        private int row;
        private int col;
        private string currentCell;

        public Form1()
        {
            InitializeComponent();
            sheet = new Spreadsheet();
            spreadsheetPanel1.SelectionChanged += displaySelection;
            spreadsheetPanel1.SetSelection(2, 3);
            currentCell = "";
            row = 0;
            col = 0;
        }

        private void displaySelection(SpreadsheetPanel ss)
        {
            ss.GetSelection(out col, out row);

            currentCell = translateCell(row, col);

            cellBox.Text = currentCell;
            valueBox.Text = sheet.GetCellValue(currentCell).ToString();
            contentsBox.Text = sheet.GetCellContents(currentCell).ToString();
        }

        private string translateCell(int row, int col)
        {
            int rowInt = row + 1;
            string colString = "";
            switch (col)
            {
                case 0:
                    colString = "A";
                    break;
                case 1:
                    colString = "B";
                    break;
                case 2:
                    colString = "C";
                    break;
                case 3:
                    colString = "D";
                    break;
                case 4:
                    colString = "E";
                    break;
                case 5:
                    colString = "F";
                    break;
                case 6:
                    colString = "G";
                    break;
                case 7:
                    colString = "H";
                    break;
                case 8:
                    colString = "I";
                    break;
                case 9:
                    colString = "J";
                    break;
                case 10:
                    colString = "K";
                    break;
                case 11:
                    colString = "L";
                    break;
                case 12:
                    colString = "M";
                    break;
                case 13:
                    colString = "N";
                    break;
                case 14:
                    colString = "O";
                    break;
                case 15:
                    colString = "P";
                    break;
                case 16:
                    colString = "Q";
                    break;
                case 17:
                    colString = "R";
                    break;
                case 18:
                    colString = "S";
                    break;
                case 19:
                    colString = "T";
                    break;
                case 20:
                    colString = "U";
                    break;
                case 21:
                    colString = "V";
                    break;
                case 22:
                    colString = "W";
                    break;
                case 23:
                    colString = "X";
                    break;
                case 24:
                    colString = "Y";
                    break;
                case 25:
                    colString = "Z";
                    break;
            }
            return colString + rowInt.ToString();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var fullFileName = Path.Combine(desktopFolder, "Test.xml");
            var fs = new FileStream(fullFileName, FileMode.Create);
            sheet.Save(new StreamWriter(fs));
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            string value = "";

            try
            {
                sheet.SetContentsOfCell(currentCell, contentsBox.Text);
            }
            catch
            {
                MessageBox.Show("Invalid Cell Input");
            }

            value = sheet.GetCellValue(currentCell).ToString();
            valueBox.Text = value;
            spreadsheetPanel1.SetValue(col, row, value);
        }
    }
}
