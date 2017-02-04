// Greg Rosich - u0917936

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;

namespace DependencyGraphTests
{
    /// <summary>
    /// This class is designed to test the DependencyGraph class
    /// </summary>
    [TestClass]
    public class DependencyGraphTestCases
    {
        /// <summary>
        /// Tests to make sure that a has dependents
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            Assert.AreEqual(graph.HasDependents("a"), true);
        }

        /// <summary>
        /// Tests to make sure that the graph size is 2
        /// </summary>
        [TestMethod]
        public void TestMethod2()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            Assert.AreEqual(graph.Size, 2);
        }

        /// <summary>
        /// Tests to make sure that b and c have dependees
        /// </summary>
        [TestMethod]
        public void TestMethod3()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            Assert.AreEqual(graph.HasDependees("b"), true);
            Assert.AreEqual(graph.HasDependees("c"), true);
        }

        /// <summary>
        /// Tests to make sure ReplaceDependees and RemoveDependency works
        /// </summary>
        [TestMethod]
        public void TestMethod4()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            graph.ReplaceDependees("d", graph.GetDependents("a"));
            graph.RemoveDependency("c", "d");
            Assert.AreEqual(graph.Size, 3);
        }

        /// <summary>
        /// Tests to make sure ReplaceDependents and ReplaceDependees works
        /// </summary>
        [TestMethod]
        public void TestMethod5()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            graph.AddDependency("b", "c");
            graph.ReplaceDependees("2", graph.GetDependents("a"));
            Assert.AreEqual(graph.Size, 5);
        }


        /// <summary>
        /// Tests to make sure ReplaceDependents and ReplaceDependees works
        /// </summary>
        [TestMethod]
        public void TestMethod6()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            graph.AddDependency("b", "c");
            graph.ReplaceDependents("b", graph.GetDependents("a"));
            Assert.AreEqual(graph.Size, 4);
        }

        /// <summary>
        /// Tests to make sure GetDependees gets the correct value
        /// </summary>
        [TestMethod]
        public void TestMethod7()
        {
            DependencyGraph graph = new DependencyGraph();
            graph.AddDependency("a", "b");
            graph.AddDependency("a", "c");
            graph.AddDependency("b", "d");
            graph.AddDependency("d", "d");
            foreach(string s in graph.GetDependees("b"))
            {
                Assert.AreEqual(s, "a");
            }
        }

    }
}
