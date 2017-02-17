using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using Formulas;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents("a03");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents("hello");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents("15");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName4()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents("a3*");
        }

        [TestMethod]
        public void Test1()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.AreEqual(ss.GetCellContents("a3"), "");
        }
        
        [TestMethod]
        public void Test2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a1", 10);
        }
    }
}
