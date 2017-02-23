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
            ss.SetCellContents("hello", new Formula());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("15", 2);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidName4()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a3*", "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a3*", null);
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
            ss.SetCellContents("a1", "bloop");
            int count = 0;
            foreach (string name in ss.GetNamesOfAllNonemptyCells())
            {
                count++;
            }
            Assert.AreEqual(count, 1);
            ss.SetCellContents("a1", "");
            count = 0;
            foreach(string name in ss.GetNamesOfAllNonemptyCells())
            {
                count++;
            }
            Assert.AreEqual(count, 0);
        }

        [TestMethod]
        public void Test3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a1", 20);
            Assert.AreEqual((double)ss.GetCellContents("a1"), 20);
        }        

        [TestMethod]
        public void Test4()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetCellContents("a1", new Formula("b2+c3"));
            Assert.IsInstanceOfType(ss.GetCellContents("a1"), typeof(Formula));
        }
    }
}
