﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;
using System.Text.RegularExpressions;
using Dependencies;
using System.IO;
using System.Xml;

namespace SS
{
    /// <summary>
    /// A spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string s is a valid cell name if and only if it consists of one or more letters, 
    /// followed by a non-zero digit, followed by zero or more digits.
    /// 
    /// For example, "A15", "a15", "XY32", and "BC7" are valid cell names.  On the other hand, 
    /// "Z", "X07", and "hello" are not valid cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important, and it is important that you understand the distinction and use
    /// the right term when writing code, writing comments, and asking questions.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In an empty spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
    /// The value of a Formula, of course, can depend on the values of variables.  The value 
    /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
    /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
    /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
    /// is a double, as specified in Formula.Evaluate.
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        DependencyGraph dg;
        Dictionary<string, Cell> cells;
        Regex IsValid;

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get;

            protected set;
        }

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular expression accepts every string.
        /// </summary>
        public Spreadsheet()
        {
            this.dg = new DependencyGraph();
            this.cells = new Dictionary<string, Cell>();
            this.IsValid = new Regex(@".*");
            this.Changed = false;
        }

        /// <summary>
        /// Creates an empty Spreadsheet whose IsValid regular expression is provided as the parameter
        /// </summary>
        /// <param name="isValid"></param>
        public Spreadsheet(Regex isValid) : this()
        {
            this.IsValid = isValid;
        }

        /// Creates a Spreadsheet that is a duplicate of the spreadsheet saved in source.
        ///
        /// See the AbstractSpreadsheet.Save method and Spreadsheet.xsd for the file format 
        /// specification.  
        ///
        /// If there's a problem reading source, throws an IOException.
        ///
        /// Else if the contents of source are not consistent with the schema in Spreadsheet.xsd, 
        /// throws a SpreadsheetReadException.  
        ///
        /// Else if the IsValid string contained in source is not a valid C# regular expression, throws
        /// a SpreadsheetReadException.  (If the exception is not thrown, this regex is referred to
        /// below as oldIsValid.)
        ///
        /// Else if there is a duplicate cell name in the source, throws a SpreadsheetReadException.
        /// (Two cell names are duplicates if they are identical after being converted to upper case.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a 
        /// SpreadsheetReadException.  (Use oldIsValid in place of IsValid in the definition of 
        /// cell name validity.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a
        /// SpreadsheetVersionException.  (Use newIsValid in place of IsValid in the definition of
        /// cell name validity.)
        ///
        /// Else if there's a formula that causes a circular dependency, throws a SpreadsheetReadException. 
        ///
        /// Else, create a Spreadsheet that is a duplicate of the one encoded in source except that
        /// the new Spreadsheet's IsValid regular expression should be newIsValid.
        public Spreadsheet(TextReader source, Regex newIsValid) : this()
        {
            // NOTE: Cleaned up this constructor so that errors are clear
            XmlReader reader = XmlReader.Create(source);
            using (reader)
            {
                bool firstSSAttribute = true;
                while (reader.Read())
                {
                    string name = reader.Name;
                    if (name == "spreadsheet")
                    {
                        if (firstSSAttribute)
                        {
                            string isValid = reader.GetAttribute("IsValid");
                            if(isValid == null)
                            {
                                throw new SpreadsheetReadException("Missing attribute: IsValid");
                            }
                            try
                            {
                                
                                this.IsValid = new Regex(isValid);
                            }
                            catch
                            {
                                throw new SpreadsheetReadException("Invalid Regex");
                            }
                            firstSSAttribute = false;
                        }
                    }
                    else if(name == "cell")
                    {
                        // NOTE: Added correct checks for getting cell attributes
                        string cellName = reader.GetAttribute("name");
                        string cellContents = reader.GetAttribute("contents");
                        if(cellName == null)
                        {
                            throw new SpreadsheetReadException("Missing attribute: name");
                        }
                        if(cellContents == null)
                        {
                            throw new SpreadsheetReadException("Missing attribute: contents");
                        }
                        if (cells.ContainsKey(cellName.ToUpper()))
                        {
                            throw new SpreadsheetReadException("Duplicate cell name");
                        }
                        if (!IsValid.IsMatch(cellName))
                        {
                            throw new SpreadsheetReadException("Source is not valid with its own IsValid");
                        }
                        if (!newIsValid.IsMatch(cellName))
                        {
                            throw new SpreadsheetVersionException("Source is not valid with newIsValid");
                        }
                        try
                        {
                            SetContentsOfCell(cellName, cellContents);
                        }
                        catch
                        {
                            throw new SpreadsheetVersionException("");
                        }
                    }
                    else if(name != "" && name != "xml")
                    {
                        throw new SpreadsheetReadException("Incorrect name");
                    }
                }
            }
            this.IsValid = newIsValid;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object GetCellContents(string name)
        {
            if (name == null || !ValidateName(ref name))
            {
                throw new InvalidNameException();
            }

            if (!cells.ContainsKey(name))
            {
                return "";
            }
            return cells[name].GetContents();
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            foreach(string name in cells.Keys)
            {
                yield return name;
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (name == null || !ValidateName(ref name))
            {
                throw new InvalidNameException();
            }

            foreach (string variable in formula.GetVariables())
            {
                dg.AddDependency(name, variable);
            }

            GetDependentCells(name);
            // NOTE: Forgot to evaluate the new cell and then re-evaluate Dependent cells 
            Cell cell = new Cell(name, formula);
            cell.EvaluateFormula(s => (double)cells[s].GetValue());
            cells[name] = cell;

            Changed = true;
            return GetDependentCells(name);
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            if(text == null)
            {
                throw new ArgumentNullException();
            }
            if (name == null || !ValidateName(ref name))
            {
                throw new InvalidNameException();
            }
            if (text == "")
            {
                if (cells.ContainsKey(name))
                {
                    cells.Remove(name);
                }
            }
            else
            {
                cells[name] = new Cell(name, text);
            }

            Changed = true;
            return GetDependentCells(name);
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            if (name == null || !ValidateName(ref name))
            {
                throw new InvalidNameException();
            }
            cells[name] = new Cell(name, number);

            Changed = true;
            return GetDependentCells(name);
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if(name == null)
            {
                throw new ArgumentNullException();
            }
            if (!ValidateName(ref name))
            {
                throw new InvalidNameException();
            }
            
            return dg.GetDependees(name);
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the IsValid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        public override void Save(TextWriter dest)
        {
            XmlWriterSettings setting = new XmlWriterSettings();
            setting.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(dest, setting))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("IsValid", IsValid.ToString());
                foreach (string name in cells.Keys)
                {
                    writer.WriteStartElement("cell");
                    writer.WriteAttributeString("name", name);

                    string contents = "";
                    object cellContents = cells[name].GetContents();
                    if(cellContents is Formula)
                    {
                        contents = "=" + cellContents.ToString();
                    }
                    else if(cellContents is double)
                    {
                        contents = cellContents.ToString();
                    }
                    else
                    {
                        contents = (string)cellContents;
                    }
                    writer.WriteAttributeString("contents", contents);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
            Changed = false;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (name == null || !ValidateName(ref name))
            {
                throw new InvalidNameException();
            }

            if (!cells.ContainsKey(name))
            {
                return "";
            }
            Cell cell = cells[name];
            object contents = cell.GetContents();
            if(contents is Formula)
            {
                if(cell.Error != null)
                {
                    return cell.Error;
                }
                return cell.GetValue();
            }
            return contents;
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException();
            }
            if (name == null || !ValidateName(ref name))
            {
                throw new InvalidNameException();
            }

            double value;
            if(Double.TryParse(content, out value))
            {
                return SetCellContents(name, value);
            }
            string pattern = @"^=";
            if (Regex.IsMatch(content, pattern))
            {
                content = content.Substring(1);
                Formula f = new Formula(content, s => s.ToUpper(), s => ValidateName(ref s));
                return SetCellContents(name, f);
            }

            return SetCellContents(name, content);
        }

        /// <summary>
        /// Checks if the name supplied is a valid cell name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool ValidateName(ref string name)
        {
            // NOTE: Forgot to normalize all names
            name = name.ToUpper();
            string pattern = @"^[a-zA-Z]+[1-9]\d*$";
            return Regex.IsMatch(name, pattern) && IsValid.IsMatch(name);
        }

        /// <summary>
        /// Gets a set consisting of name plus the names of all other cells
        /// whose value depends, directly or indirectly, on the named cell.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ISet<string> GetDependentCells(string name)
        {
            HashSet<string> set = new HashSet<string>();
            foreach (string dependent in GetCellsToRecalculate(name))
            {
                set.Add(dependent);
                if(name != dependent)
                {
                    Cell cell = cells[dependent];
                    if (cell.GetContents() is Formula)
                    {
                        cell.EvaluateFormula(s => (double)cells[s].GetValue());
                    }
                }
            }
            return set;
        }
    }
}
