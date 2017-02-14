// Written by Joe Zachary for CS 3500, January 2017.
// Greg Rosich - u0917936

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Formulas;

namespace FormulaTestCases
{
    /// <summary>
    /// These test cases are in no sense comprehensive!  They are intended to show you how
    /// client code can make use of the Formula class, and to show you how to create your
    /// own (which we strongly recommend).  To run them, pull down the Test menu and do
    /// Run > All Tests.
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        /// This tests that a syntax error with an invalid token results 
        /// in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ConstructInvalidToken()
        {
            Formula f = new Formula("2 +_/5");
        }

        /// <summary>
        /// This tests that a syntax error with no token results in a 
        /// FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ConstructMissingToken()
        {
            Formula f = new Formula("");
        }

        /// <summary>
        /// This tests that a syntax error with the number of closing parenthesis
        /// being greater than the number of opening parenthesis 
        /// results in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ConstructMoreClosed()
        {
            Formula f = new Formula("4+6)");
        }

        /// <summary>
        /// This tests that a syntax error with the number of opening parenthesis
        /// is unequal to the number of closing parenthesis results in a
        /// FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ConstructMoreOpen()
        {
            Formula f = new Formula("(4+6");
        }

        /// <summary>
        /// This tests that a syntax error with the first token being 
        /// something other than a number, a variable, or an opening parenthesis
        /// results in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ConstructFirstToken()
        {
            Formula f = new Formula("+6*2");
        }


        /// <summary>
        /// This tests that a syntax error with the last token being 
        /// something other than a number, a variable, or a closing parenthesis
        /// results in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ConstructLastToken()
        {
            Formula f = new Formula("6*2*");
        }

        /// <summary>
        /// This tests that a syntax error with multiple operators
        /// results in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ConstructMultipleOperators()
        {
            Formula f = new Formula("2+/3");
        }

        /// <summary>
        /// This tests that a syntax error with an open parenthesis 
        /// having something other than a number, a variable, or an opening parenthesis
        /// following it results in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ConstructOpenOperator()
        {
            Formula f = new Formula("2+(+6/3)");
        }

        /// <summary>
        /// This tests that a syntax error with multiple variables
        /// results in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ConstructMultipleVariables()
        {
            Formula f = new Formula("22+3 x");
        }


        /// <summary>
        /// This tests that a syntax error with a closing parenthesis 
        /// having something other than an operator or a closing parenthesis
        /// following it results in a FormulaFormatException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ConstructClosedVariable()
        {
            Formula f = new Formula("2x+(6.6/3)y");
        }

        /// <summary>
        /// Makes sure that "2+3" evaluates to 5.  Since the Formula
        /// contains no variables, the delegate passed in as the
        /// parameter doesn't matter.  We are passing in one that
        /// maps all variables to zero.
        /// </summary>
        [TestMethod]
        public void Evaluate1()
        {
            Formula f = new Formula("2+3");
            Assert.AreEqual(f.Evaluate(v => 0), 5.0, 1e-6);
        }

        /// <summary>
        /// The Formula consists of a single variable (x5).  The value of
        /// the Formula depends on the value of x5, which is determined by
        /// the delegate passed to Evaluate.  Since this delegate maps all
        /// variables to 22.5, the return value should be 22.5.
        /// </summary>
        [TestMethod]
        public void Evaluate2()
        {
            Formula f = new Formula("x5");
            Assert.AreEqual(f.Evaluate(v => 22.5), 22.5, 1e-6);
        }

        /// <summary>
        /// Here, the delegate passed to Evaluate always throws a
        /// UndefinedVariableException (meaning that no variables have
        /// values).  The test case checks that the result of
        /// evaluating the Formula is a FormulaEvaluationException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaEvaluationException))]
        public void Evaluate3()
        {
            Formula f = new Formula("x + y");
            f.Evaluate(v => { throw new UndefinedVariableException(v); });
        }

        /// <summary>
        /// The delegate passed to Evaluate is defined below.  We check
        /// that evaluating the formula returns in 10.
        /// </summary>
        [TestMethod]
        public void Evaluate4()
        {
            Formula f = new Formula("x + y");
            Assert.AreEqual(f.Evaluate(Lookup4), 10.0, 1e-6);
        }

        /// <summary>
        /// This uses one of each kind of token.
        /// </summary>
        [TestMethod]
        public void Evaluate5 ()
        {
            Formula f = new Formula("(x + y) * (z / x) * 1.0");
            f.GetVariables();
            f.ToString();
        }

        /// <summary>
        /// A Lookup method that maps x to 4.0, y to 6.0, and z to 8.0.
        /// All other variables result in an UndefinedVariableException.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public double Lookup4(String v)
        {
            switch (v)
            {
                case "x": return 4.0;
                case "y": return 6.0;
                case "z": return 8.0;
                default: throw new UndefinedVariableException(v);
            }
        }
    }
}
