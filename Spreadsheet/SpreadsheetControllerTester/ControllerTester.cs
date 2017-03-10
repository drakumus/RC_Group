using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetController;
using Formulas;
using SS;
using SSGui;

namespace SpreadsheetControllerTester
{
    [TestClass]
    public class ControllerTester
    {
        [TestMethod]
        public void TestMethod1()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireCloseEvent();
            Assert.IsTrue(stub.CalledDoClose);
        }

        [TestMethod]
        public void TestMethod2()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireNewEvent();
            Assert.IsTrue(stub.CalledOpenNew);
        }

        [TestMethod]
        public void TestMethod3()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireSaveEvent();
        }

        [TestMethod]
        public void TestMethod4()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireEditEvent("=bshj");
            Assert.AreEqual(stub.Message, "Invalid Cell Input");
        }

        [TestMethod]
        public void TestMethod5()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireEditEvent("=z100");
            Assert.AreEqual(stub.Message, "Invalid Cell Input");
        }

        [TestMethod]
        public void TestMethod6()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireEditEvent("4");
            stub.FireCloseEvent();
            Assert.AreEqual(stub.Message, "Please save before attempting to close");
        }

        /// <summary>
        /// covers majority of remaining code but requires a file in correct directory yet to be determined
        /// </summary>
        [TestMethod]
        public void TestMethod7()
        {
            SpreadsheetViewStub stub = new SpreadsheetViewStub();
            Controller controller = new Controller(stub);
            stub.FireFileChosenEvent("wrong.ss");
            Assert.AreEqual(stub.Message, "File data cannot be opened as a spreadsheet");
        }
        
    }
}
