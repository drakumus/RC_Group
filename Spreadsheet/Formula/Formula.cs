// Skeleton written by Joe Zachary for CS 3500, January 2017
// Greg Rosich - u0917936

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public class Formula
    {
        private List<string> problem = new List<string>();

        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        public Formula(String formula)
        { 
            int openParenthesis = 0;
            bool wasValue = false;
            bool wasOperator = false;
            bool wasOpen = false;
            bool wasClosed = false;
            foreach (string token in GetTokens(formula)) {
                if ((wasOperator || wasOpen) && (IsOperator(token) || token == ")"))
                {
                    throw new FormulaFormatException("Open parenthesis or operators must be followed by a number, a variable, or an opening parenthesis");
                }
                if ((wasValue || wasClosed) && (IsValue(token) || token == "("))
                {
                    throw new FormulaFormatException("Closed parenthesis or values must be followed by an operator or a closing parenthesis");
                }
                problem.Add(token);
                wasOperator = IsOperator(token);
                wasValue = IsValue(token);
                if (token == "(")
                {
                    openParenthesis++;
                    wasOpen = true;
                    wasClosed = false;
                }
                else if (token == ")")
                {
                    openParenthesis--;
                    wasOpen = false;
                    wasClosed = true;
                    if (openParenthesis < 0)
                    {
                        throw new FormulaFormatException("Closing paranthesis without an open beforehand");
                    }
                } if (!IsOperator(token) && !IsValue(token))
                {
                    throw new FormulaFormatException("Tnvalid token");
                }
                else
                {
                    wasOpen = false;
                    wasClosed = false;
                }
            }
            if(problem.Count < 3)
            {
                throw new FormulaFormatException("No formula exists");
            }
            if(openParenthesis != 0)
            {
                throw new FormulaFormatException("Unequal number of open and close parenthesis");
            }
            string first = problem[0];
            if(!IsValue(first) && first != "(")
            {
                throw new FormulaFormatException("First token must be a number, a variable, or an opening parenthesis");
            }
            string last = problem[problem.Count - 1];
            if (!IsValue(last) && last != ")")
            {
                throw new FormulaFormatException("Last token must be a number, a variable, or a closing parenthesis");
            }
        }
        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();
            foreach (string token in problem)
            {
                double value;
                if (double.TryParse(token, out value))
                {
                    if (operators.Count == 0 || values.Count == 0)
                    {
                        values.Push(value);
                    }
                    else
                    {
                        if (operators.Count != 0)
                        {
                            switch (operators.Peek())
                            {
                                case "*":
                                    operators.Pop();
                                    values.Push(values.Pop() * value);
                                    break;
                                case "/":
                                    operators.Pop();
                                    try
                                    {
                                        values.Push(values.Pop() / value);
                                    }
                                    catch (DivideByZeroException)
                                    {
                                        throw new System.DivideByZeroException("Cannot divide by zero");
                                    }
                                    break;
                                default:
                                    values.Push(value);
                                    break;
                            }
                        }
                    }
                }
                else if (token == "+" || token == "-")
                {
                    if(operators.Count == 0)
                    {
                        operators.Push(token);
                    } else
                    {
                        AddSubt(values, operators);
                        operators.Push(token);
                    }
                }
                else if (token == "*" || token == "/")
                {
                    operators.Push(token);
                }
                else if (token == "(")
                {
                    operators.Push(token);
                }
                else if (token == ")")
                {
                    AddSubt(values, operators);
                    operators.Pop();
                    if (operators.Count != 0)
                    {
                        switch (operators.Peek())
                        {
                            case "*":
                                operators.Pop();
                                values.Push(values.Pop() * values.Pop());
                                break;
                            case "/":
                                operators.Pop();
                                try
                                {
                                    value = values.Pop();
                                    values.Push(values.Pop() / value);
                                }
                                catch (DivideByZeroException)
                                {
                                    throw new System.DivideByZeroException("Cannot divide by zero");
                                }
                                break;
                        }
                    }
                }
                else
                {
                    try
                    {
                        value = lookup(token);
                        if (operators.Count == 0 || values.Count == 0)
                        {
                            values.Push(value);
                        }
                        else
                        {
                            if (operators.Count != 0)
                            {
                                switch (operators.Peek())
                                {
                                    case "*":
                                        operators.Pop();
                                        values.Push(values.Pop() * value);
                                        break;
                                    case "/":
                                        operators.Pop();
                                        try
                                        {
                                            values.Push(values.Pop() / value);
                                        }
                                        catch (DivideByZeroException)
                                        {
                                            throw new System.DivideByZeroException("Cannot divide by zero");
                                        }
                                        break;
                                    default:
                                        values.Push(value);
                                        break;
                                }
                            }
                        }
                    }
                    catch (UndefinedVariableException)
                    {
                        throw new FormulaEvaluationException("Undefined variable(s): cannot Lookup");
                    }
                }
            }
            if(operators.Count == 1)
            {
                AddSubt(values, operators);
            }
            return values.Pop();
        }

        /// <summary>
        /// Method to attempt to add or subtract 2 values in the value stack
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        private static void AddSubt(Stack<double> values, Stack<string> operators)
        {
            if(operators.Count != 0)
            {
                switch (operators.Peek())
                {
                    case "+":
                        operators.Pop();
                        values.Push(values.Pop() + values.Pop());
                        break;
                    case "-":
                        operators.Pop();
                        values.Push(-values.Pop() + values.Pop());
                        break;
                }
            }
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            // PLEASE NOTE:  I have added white space to this regex to make it more readable.
            // When the regex is used, it is necessary to include a parameter that says
            // embedded white space should be ignored.  See below for an example of this.
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern.  It contains embedded white space that must be ignored when
            // it is used.  See below for an example of this.
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            // PLEASE NOTE:  Notice the second parameter to Split, which says to ignore embedded white space
            /// in the pattern.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }

        /// <summary>
        /// Given a token, determines if the token is an operator
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool IsOperator(string token)
        {
            string opPattern = @"[\+\-*/]";
            return Regex.IsMatch(token, opPattern, RegexOptions.Singleline);
        }

        /// <summary>
        /// Given a token, determines if the token is a number or variable
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool IsVariable(string token)
        {
            String varPattern = @"^[a-zA-Z][0-9a-zA-Z]*$";
            return Regex.IsMatch(token, varPattern, RegexOptions.Singleline);
        }

        private static bool IsDouble(string token)
        {
            double value;
            return double.TryParse(token, out value);
        }

        private static bool IsValue(string token)
        {
            bool var = IsVariable(token);
            bool doub = IsDouble(token);
            return var || doub;
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string var);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    [Serializable]
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    [Serializable]
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}
