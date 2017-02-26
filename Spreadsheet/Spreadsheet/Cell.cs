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
        public object Contents { get
            {
                if (Text != null)
                {
                    return Text;
                }
                if (!Object.ReferenceEquals(formula, null))
                {
                    return formula;
                }
                return Value;
            }
        }
        public double Value { get; private set; }
        private string Text { get; set; }
        private Formula formula;

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
            this.formula = formula;
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
    }
}
