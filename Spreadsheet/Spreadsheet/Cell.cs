using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;

namespace SS
{
    /// <summary>
    /// This class is a helper class for Spreadsheet. Each cell contains a name
    /// and contents. The contents can be either a string, double, or a Formula
    /// </summary>
    class Cell
    {
        public string Name { get; private set; }
        private double Value;
        private string Text;
        private Formula Formula;
        // NOTE: Changed the way FormulaErrors are stored
        public object Error;

        private Cell(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// Creates a cell object with the name and value specified
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Cell(string name, double value) : this(name)
        {
            this.Value = value;
        }

        /// <summary>
        /// Creates a cell object with the name and formula specified
        /// </summary>
        /// <param name="name"></param>
        /// <param name="formula"></param>
        public Cell(string name, Formula formula) : this(name)
        {
            this.Formula = formula;
        }

        /// <summary>
        /// Creates a cell object with the name and text specified
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        public Cell(string name, string text) : this(name)
        {
            this.Text = text;
        }
        
        public object GetContents()
        {

            {
                if (Text != null)
                {
                    return Text;
                }
                if (Formula != new Formula())
                {
                    return Formula;
                }
                return Value;
            }
        }

        public object GetValue()
        {

            {
                if(Error != null)
                {
                    return Error;
                }
                if (Text != null)
                {
                    return Text;
                }
                return Value;
            }
        }

        public void EvaluateFormula(Lookup lookup)
        {
            if(Formula == null)
            {
                throw new ArgumentNullException();
            }
            try
            {
                Value = Formula.Evaluate(lookup);
            }
            catch
            {
                Error = new FormulaError();
                return;
            }
            Error = null;
        }
    }
}
