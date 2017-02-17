using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS
{
    /// <summary>
    /// This class is a helper class for Spreadsheet. Each cell contains a name
    /// and contents. The contents can be either a string, double, or a Formula
    /// </summary>
    class Cell
    {
        public string name { get; }
        public object contents { get; }
    
        /// <summary>
        /// Creates a cell object with the name and contents specified
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contents"></param>
        public Cell(string name, object contents)
        {
            this.name = name;
            this.contents = contents;
        }
    }
}
