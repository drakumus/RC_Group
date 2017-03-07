using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Formulas;
using System.IO;
using System.Text.RegularExpressions;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void Null1()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.IsFalse(s.Changed);
            s.GetCellValue(null);
        }

        [TestMethod]
        public void CellValue1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2+A3"));
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", ("=A2+A4"));
            s.SetContentsOfCell("A4", ("=A2+A5"));
            s.SetContentsOfCell("A5", "82.5");
            Assert.AreEqual(s.GetCellValue("A1"), 100.5);
            Assert.IsTrue(s.Changed);
        }

        [TestMethod]
        public void CellValue2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2+A3"));
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", ("=A2/A4"));
            s.SetContentsOfCell("A4", ("=A2+A5"));
            s.SetContentsOfCell("A5", "18");
            Assert.AreEqual(s.GetCellValue("A1"), 6.25);
            Assert.IsTrue(s.Changed);
        }

        [TestMethod]
        public void CellValue3()
        {
            Spreadsheet s = new Spreadsheet();
            Assert.AreEqual(s.GetCellValue("A1"), "");
            Assert.IsFalse(s.Changed);
        }

        [TestMethod]
        public void CellValue4()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2+A3"));
            s.SetContentsOfCell("A2", "top");
            s.SetContentsOfCell("A3", ("=A2+A4"));
            s.SetContentsOfCell("A4", ("=A2+A5"));
            s.SetContentsOfCell("A5", "82.5");
            Assert.AreEqual(s.GetCellValue("A2"), "top");
        }

        [TestMethod]
        public void CellValue5()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2+A3"));
            s.SetContentsOfCell("A2", "top");
            s.SetContentsOfCell("A3", ("=A2+A4"));
            s.SetContentsOfCell("A4", ("=A2+A5"));
            s.SetContentsOfCell("A5", "82.5");
            Assert.AreEqual(s.GetCellValue("A1"), new FormulaError());
        }

        [TestMethod]
        public void Save1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", ("=A2+A3"));
            s.SetContentsOfCell("A2", "6");
            s.SetContentsOfCell("A3", ("=A2+A4"));
            s.SetContentsOfCell("A4", ("=A2+A5"));
            s.SetContentsOfCell("A5", "82.5");
            Assert.IsTrue(s.Changed);
            StreamWriter sw = new StreamWriter("Test.xml");
            s.Save(sw);
            Assert.IsFalse(s.Changed);
        }
    }
}
