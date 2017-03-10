using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetController;

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
    }
}
