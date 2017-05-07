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
    public struct Formula
    {
        private List<string> tokens;

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
        public Formula(string formula) : this(formula, n => n, v => true)
        {

        }

        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// If the formula contains variables, they are "normalized" though the normalizer
        /// and then validated through the validator.
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="normalizer"></param>
        /// <param name="validator"></param>
        public Formula(String formula, Normalizer normalizer, Validator validator)
        {
            if (formula == null || normalizer == null || validator == null)
            {
                throw new ArgumentNullException("Cannot pass null perameter");
            }
            tokens = new List<string>();
            string prevToken = "";
            int openParenthesis = 0;
            foreach (string token in GetTokens(formula)) {
                checkFormat(token, prevToken, ref openParenthesis);
                if (IsVariable(token))
                {
                    string normalized = normalizer(token);
                    foreach (string normToken in GetTokens(normalized))
                    {
                        checkFormat(normToken, prevToken, ref openParenthesis);
                        if (!validator(normToken))
                        {
                            throw new FormulaFormatException("Variable not valid according to validatior");
                        }
                        tokens.Add(normToken);
                        prevToken = normToken;
                    }
                }
                else
                {
                    tokens.Add(token);
                    prevToken = token;
                }
            }
            if(tokens.Count == 0)
            {
                throw new FormulaFormatException("No formula exists");
            }
            if(openParenthesis != 0)
            {
                throw new FormulaFormatException("Unequal number of open and close parenthesis");
            }
            string first = tokens[0];
            if(!IsValue(first) && first != "(")
            {
                throw new FormulaFormatException("First token must be a number, a variable, or an opening parenthesis");
            }
            string last = tokens[tokens.Count - 1];
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
            if(tokens == null)
            {
                return 0;
            }
            if(lookup == null)
            {
                throw new ArgumentNullException("Cannot pass null perameter");
            }
            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();
            foreach (string token in tokens)
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
                                    if (value != 0)
                                    {
                                        values.Push(values.Pop() / value);
                                    }
                                    else
                                    {
                                        throw new FormulaEvaluationException("Cannot divide by zero");
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
                                if (values.Peek() != 0)
                                {
                                    value = values.Pop();
                                    values.Push(values.Pop() / value);
                                }
                                else
                                {
                                    throw new FormulaEvaluationException("Cannot divide by zero");
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
                                        if (value != 0)
                                        {
                                            values.Push(values.Pop() / value);
                                        }
                                        else
                                        {
                                            throw new FormulaEvaluationException("Cannot divide by zero");
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
        /// Checks the current token and previous token to make sure that they do not
        /// break any formatting rules
        /// </summary>
        /// <param name="token"></param>
        /// <param name="prevToken"></param>
        private static void checkFormat(string token, string prevToken, ref int openParenthesis)
        {
            if ((IsOperator(prevToken) || prevToken == "(") && (IsOperator(token) || token == ")"))
            {
                throw new FormulaFormatException("Open parenthesis or operators must be followed by a number, a variable, or an opening parenthesis");
            }
            if ((IsValue(prevToken) || prevToken == ")") && (IsValue(token) || token == "("))
            {
                throw new FormulaFormatException("Closed parenthesis or values must be followed by an operator or a closing parenthesis");
            }

            if (token == "(")
            {
                openParenthesis++;
            }
            else if (token == ")")
            {
                openParenthesis--;
                if (openParenthesis < 0)
                {
                    throw new FormulaFormatException("Closing paranthesis without an open beforehand");
                }
            }
            else if (!IsOperator(token) && !IsValue(token))
            {
                throw new FormulaFormatException("Invalid token");
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
        /// Gets a set of the variables
        /// </summary>
        /// <returns></returns>
        public ISet<string> GetVariables()
        {
            // NOTE: Added try catch
            HashSet<string> variables = new HashSet<string>();
            try
            {
                foreach (string token in tokens)
                {
                    if (IsVariable(token))
                    {
                        variables.Add(token);
                    }
                }
            }
            catch (NullReferenceException)
            {
                
            }
            return variables;
        }

        /// <summary>
        /// Given a token, determines if the token is an operator
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool IsOperator(string token)
        {
            string opPattern = @"^[\+\-*/]$";
            return Regex.IsMatch(token, opPattern, RegexOptions.Singleline);
        }

        /// <summary>
        /// Given a token, determines if the token is a variable
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool IsVariable(string token)
        {
            String varPattern = @"^[a-zA-Z][0-9a-zA-Z]*$";
            return Regex.IsMatch(token, varPattern, RegexOptions.Singleline);
        }

        /// <summary>
        /// Given a token, determines if the token is a number
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool IsDouble(string token)
        {
            double value;
            return double.TryParse(token, out value);
        }

        /// <summary>
        /// Given a token, determines if the token is a number or variable
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool IsValue(string token)
        {
            bool var = IsVariable(token);
            bool doub = IsDouble(token);
            return var || doub;
        }

        /// <summary>
        /// Converts all tokens into a formula
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // NOTE: Added try catch
            string s = "";
            try
            {
                foreach (string token in tokens)
                {
                    s += token;
                }
            } catch(NullReferenceException)
            {
                return "0";
            }
            return s;
        }

        public static bool operator ==(Formula f, Formula g)
        {
            return f.ToString() == g.ToString();
        }

        public static bool operator !=(Formula f, Formula g)
        {
            return f.ToString() != g.ToString();
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
    /// Converts variables into a canonical form
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public delegate string Normalizer(string s);

    /// <summary>
    /// Imposes extra restrictions on the validity of a variable,
    /// beyond the ones already built into the formula definition
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public delegate bool Validator(string s);

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
