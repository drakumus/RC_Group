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

namespace SpreadsheetPractice
{
    public partial class Form1 : Form
    {
        private Spreadsheet sheet;

        private string currentCell;

        public Form1()
        {
            InitializeComponent();
            //TODO: Fix regex to accept what was listed in assignment
            sheet = new Spreadsheet(new Regex(".*"));
            spreadsheetPanel1.SelectionChanged += displaySelection;

            //initial cell setup
            spreadsheetPanel1.SetSelection(2, 3);
            updateBoxes(2, 3);
        }

        private void updateBoxes(int col, int row)
        {
            currentCell = translateCell(col, row);

            cellBox.Text = currentCell;
            valueBox.Text = sheet.GetCellValue(currentCell).ToString();
            contentsBox.Text = sheet.GetCellContents(currentCell).ToString();
        }

        private void updateBoxes(string cell)
        {
            var translatedCell = translateRowCol(cell);
            int col = translatedCell[0];
            int row = translatedCell[1];
            updateBoxes(col, row);
        }
        /// <summary>
        /// Event for when a cell is clicked in the spreadsheetPanel.
        /// </summary>
        /// <param name="ss">current SpreadsheetPanel</param>
        private void displaySelection(SpreadsheetPanel ss)
        {
            int row;
            int col;
            ss.GetSelection(out col, out row);

            updateBoxes(col, row);
        }

        /// <summary>
        /// Translates row, col integers to respective Cell.
        /// i.e. col 3, row 2; will return D3
        /// </summary>
        /// <param name="row">row integer</param>
        /// <param name="col">collumn integer</param>
        /// <returns>string Cell</returns>
        private string translateCell(int col, int row)
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

        /// <summary>
        /// Translates a given Cell i.e. "C1" to its respective col, row 
        /// i.e. for "C1" col 2 will be returned in index 0 and row 0 will be returned in index 1
        /// see editButton_Click for implementation
        /// </summary>
        /// <param name="cell">cell name i.e. C1</param>
        /// <returns>
        /// int[]
        /// int[0] = col
        /// int[1] = row
        /// </returns>
        private int[] translateRowCol (string cell)
        {
            string colS = cell.Substring(0, 1);
            int col = 0;
            int row;
            Int32.TryParse(cell.Substring(1, cell.Length-1), out row);
            row--;
            switch (colS)
            {
                case "A":
                    col = 0;
                    break;
                case "B":
                    col = 1;
                    break;
                case "C":
                    col = 2;
                    break;
                case "D":
                    col = 3;
                    break;
                case "E":
                    col = 4;
                    break;
                case "F":
                    col = 5;
                    break;
                case "G":
                    col = 6;
                    break;
                case "H":
                    col = 7;
                    break;
                case "I":
                    col = 8;
                    break;
                case "J":
                    col = 9;
                    break;
                case "K":
                    col = 10;
                    break;
                case "L":
                    col = 11;
                    break;
                case "M":
                    col = 12;
                    break;
                case "N":
                    col = 13;
                    break;
                case "O":
                    col = 14;
                    break;
                case "P":
                    col = 15;
                    break;
                case "Q":
                    col = 16;
                    break;
                case "R":
                    col = 17;
                    break;
                case "S":
                    col = 18;
                    break;
                case "T":
                    col = 19;
                    break;
                case "U":
                    col = 20;
                    break;
                case "V":
                    col = 21;
                    break;
                case "W":
                    col = 22;
                    break;
                case "X":
                    col = 23;
                    break;
                case "Y":
                    col = 24;
                    break;
                case "Z":
                    col = 25;
                    break;
            }

            return new int[] { col, row };
        }

        /// <summary>
        /// on menu select File>Close Closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sheet.Changed == false)
                Close();
            else
                MessageBox.Show("Please save before attempting to close");
        }

        /// <summary>
        /// Open...will be renamed in future
        /// Also Not Redrawing as intended...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int col;
            int row;

            if (sheet.Changed == false)
            {
                var result = new System.Windows.Forms.OpenFileDialog();
                if (result.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string filePath = result.FileName;

                    sheet = new Spreadsheet(new StreamReader(filePath), new Regex(".*"));
                    var here = sheet.GetNamesOfAllNonemptyCells().ToArray<string>();
                    foreach(string cell in sheet.GetNamesOfAllNonemptyCells())
                    {
                        var rowCol= translateRowCol(cell);
                        col = rowCol[0];
                        row = rowCol[1];
                        spreadsheetPanel1.SetValue(col, row, sheet.GetCellValue(cell).ToString());
                    }
                    spreadsheetPanel1.GetSelection(out col, out row);

                    updateBoxes(col, row);
                    
                }

            }
            else
                MessageBox.Show("Please save before attempting to create a new sheet");
        }

        /// <summary>
        /// on menu Select File>New clears current spreadsheet for a new one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sheet.Changed == false)
            {
                sheet = new Spreadsheet(new Regex(".*"));
                spreadsheetPanel1.Clear();
                valueBox.Text = "";
                contentsBox.Text = "";
            }
            else
                MessageBox.Show("Please save before attempting to create a new sheet");
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //destination can be fixed in the future. I know how to prompt for file selection view you see
            //in most programs so dont worry this is temporary.
            var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            var fullFileName = Path.Combine(desktopFolder, "Test.xml");
            var fs = new FileStream(fullFileName, FileMode.Create);
            sheet.Save(new StreamWriter(fs));
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            int col;
            int row;
            string value = "";

            ISet<string> cellsToUpdate = new HashSet<string>();

            bool isValidInput = true;

            try
            {
                cellsToUpdate = sheet.SetContentsOfCell(currentCell, contentsBox.Text);
                /* this logic won't work to prevent updates because for some reason certain cells are being
                 * initialized as FormulaError...otherwise would work as intended
                 * goal is to stop any updates which means reverting cell value when formula error is found.
                 * Some changes may need to be made to Spreedsheet otherwise old values must be stored here
                 * and updated on fail. Question now is what event to check for for formula fail. Currently
                 * any form of valid input results in a formula error no matter what so how do you check for
                 * updated cells having a formula error if all valid input is interpreted from formula error.

                foreach (string cell in cellsToUpdate)
                {
                     
                    if((Type)sheet.GetCellValue(cell) == typeof(FormulaError))
                    {
                        throw new InvalidExpressionException();
                    }
                }
                */

                //updates contents of dependent cells including currentCell
                foreach (string cell in cellsToUpdate)
                {
                    col = translateRowCol(cell)[0];
                    row = translateRowCol(cell)[1];
                    value = sheet.GetCellValue(cell).ToString();
                    //something wrong is happening here. When updating dependents it'll show dependent value instead of
                    //its own value.
                    valueBox.Text = value;
                    spreadsheetPanel1.SetValue(col, row, value);

                }
                
                //work around refreshing current cell value box at end. stored value is corrent but displayed value isnt.
                //could be caused by how c# pointers work.
                valueBox.Text = sheet.GetCellValue(currentCell).ToString();
            }
            catch
            {
                MessageBox.Show("Invalid Cell Input");
            }


        }

        
    }
}
