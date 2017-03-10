using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SS;
using System.Text.RegularExpressions;
using System.IO;
using SSGui;
using Formulas;
using System.Data;

namespace SpreadsheetController
{
    class Controller
    {
        private ISpreadsheetView window;
        private Spreadsheet sheet;

        private string currentCell;

        string regPattern = @"^[a-zA-Z][1-9]\d?$";

        public Controller(ISpreadsheetView window)
        {
            this.sheet = new Spreadsheet(new Regex(regPattern));
            this.window = window;
            window.CloseEvent += HandleClose;
            window.EditEvent += HandleEdit;
            window.FileChosenEvent += HandleFileChosen;
            window.SaveEvent += HandleSave;
            window.NewEvent += HandleNew;
            window.UpdateEvent += updateBoxes;

            //initial cell setup
            int startRow = 0;
            int startCol = 0;
            updateBoxes(startCol, startRow);
        }
        public Controller(ISpreadsheetView window, string filePath) : this(window)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                this.sheet = new Spreadsheet(reader, new Regex(regPattern));
            }

            // update the values of cells in the view
            int col;
            int row;
            foreach (string cell in this.sheet.GetNamesOfAllNonemptyCells())
            {
                var rowCol = translateRowCol(cell);
                col = rowCol[0];
                row = rowCol[1];
                window.SetValue(col, row, sheet.GetCellValue(cell).ToString());
            }
        }

        /// <summary>
        /// Takes col, row input and updates the text for the active cell's cell and value box
        /// which are read only for the user. This is primarily used to update current selection
        /// on spreadsheet load, cell update, or New spreadsheet.
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        private void updateBoxes(int col, int row)
        {
            currentCell = translateCell(col, row);

            window.Cell = currentCell;
            window.Value = sheet.GetCellValue(currentCell).ToString();
            window.Contents = sheet.GetCellContents(currentCell).ToString();
        }

        /// <summary>
        /// Translates to col, row form for identifying cell then updates boxes see updateBoxes(int col, int row)
        /// </summary>
        /// <param name="cell"></param>
        private void updateBoxes(string cell)
        {
            var translatedCell = translateRowCol(cell);
            int col = translatedCell[0];
            int row = translatedCell[1];
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
        private int[] translateRowCol(string cell)
        {
            string colS = cell.Substring(0, 1);
            int col = 0;
            int row;
            Int32.TryParse(cell.Substring(1, cell.Length - 1), out row);
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

        private void HandleClose()
        {
            if (sheet.Changed == false)
            {
                window.DoClose();
            }
            else
            {
                window.Message = "Please save before attempting to close";
            }
        }

        private void HandleNew()
        {
            window.OpenNew();
        }

        private void HandleFileChosen(string filePath)
        {
            try
            {
                window.OpenNew(filePath);
            }
            catch (SpreadsheetReadException)
            {
                window.Message = "File data cannot be opened as a spreadsheet";
            }
            catch (SpreadsheetVersionException)
            {
                window.Message = "File data contains data that conflicts with itself";
            }
        }

        private void HandleSave()
        {
            StreamWriter writer;

            var saveDialog = new System.Windows.Forms.SaveFileDialog();
            saveDialog.Filter = "Spreadsheet Files (*.ss)|*.ss|All files (*.*)|*.*";
            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = saveDialog.FileName;
                using (writer = new StreamWriter(filePath))
                {
                    sheet.Save(writer);
                }
            }
        }

        private void HandleEdit(string contents)
        {
            int col;
            int row;
            string value = "";

            ISet<string> cellsToUpdate = new HashSet<string>();

            object oldContent = sheet.GetCellContents(currentCell);

            try
            {
                cellsToUpdate = sheet.SetContentsOfCell(currentCell, contents);

                //updates contents of dependent cells including currentCell
                foreach (string cell in cellsToUpdate)
                {
                    col = translateRowCol(cell)[0];
                    row = translateRowCol(cell)[1];
                    value = sheet.GetCellValue(cell).ToString();
                    window.SetValue(col, row, value);
                }

                //work around refreshing current cell value box at end. stored value is corrent but displayed value isnt.
                //could be caused by how c# pointers work.
                window.Value = sheet.GetCellValue(currentCell).ToString();
            }
            catch (FormulaFormatException)
            {
                sheet.SetContentsOfCell(currentCell, oldContent.ToString());
                window.Message = "Invalid Cell Input";
            }
            catch (UndefinedVariableException)
            {
                sheet.SetContentsOfCell(currentCell, oldContent.ToString());
                window.Message = "Cell is not on spreadsheet";
            }
            catch (InvalidExpressionException)
            {
                sheet.SetContentsOfCell(currentCell, oldContent.ToString());
                window.Message = "Invalid Cell Input";
            }
            catch (CircularException)
            {
                window.Message = "Cell cannot reference itself";
            }
        }
    }
}
