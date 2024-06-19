using System;
using System.Linq;
using System.Collections;

namespace Project2
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter an infix expression with '#' at the end");
            Console.WriteLine("Use '^' to denote power");
            Console.WriteLine("Unary operators +, - will be represented as @, ?");
            Console.WriteLine("When done, press 'e' to exit\n");
            while(true)
            {
                Console.Write("INPUT>");
                string userInput = Console.ReadLine();
                if (userInput == "e")
                {
                    break;
                }
                parseInput(userInput);

            }

        }


        static void parseInput(string userInput)
        {
            List<string> infixNotation = new List<string>();
            int i = 0;

            while(i < userInput.Length)
            {
                // if its a number check next i for more digits
                if (Char.IsDigit(userInput[i]))
                {
                    string num = userInput[i].ToString();
                    int z = 1;
                    while (i + z < userInput.Length && Char.IsDigit(userInput[i + z]))
                    {
                        num += userInput[i + z].ToString();
                        z += 1;
                    }
                    infixNotation.Add(num);
                    i += z;
                }

                // if its n check next two (not)
                else if ((userInput.Length)-i >= 3 && userInput[i] == 'n' && userInput[i+1] == 'o' && userInput[i+2] == 't')
                {
                    infixNotation.Add("not");
                    i += 3;
                }

                // if o check next (or)
                else if ((userInput.Length) - i >= 2 && userInput[i] == 'o' && userInput[i+1] == 'r')
                {
                    infixNotation.Add("or");
                    i += 2;
                }

                // if a check next two (and)
                else if ((userInput.Length)-i >= 3 && userInput[i] == 'a' && userInput[i + 1] == 'n' && userInput[i + 2] == 'd')
                {
                    infixNotation.Add("and");
                    i += 3;
                }

                else if ((userInput.Length) - i >= 4 && userInput[i] == 't' && userInput[i + 1] == 'r' && userInput[i + 2] == 'u' && userInput[i + 3] == 'e')
                {
                    infixNotation.Add("1");
                    i += 4;
                }

                else if ((userInput.Length) - i >= 5 && userInput[i] == 'f' && userInput[i + 1] == 'a' && userInput[i + 2] == 'l' && userInput[i + 3] == 's' && userInput[i + 4] == 'e')
                {
                    infixNotation.Add("0");
                    i += 5;
                }

                // if < or > or ! check if next is =  (!=, <=, >=)
                else if (userInput[i] == '<' || userInput[i] == '>' || userInput[i] == '!' || userInput[i] == '=')
                {
                    string operat = userInput[i].ToString();
                    if (userInput[i + 1] == '=')
                    {
                        operat += "=";
                        infixNotation.Add(operat);
                        i += 2;
                    }
                    else
                    {
                        infixNotation.Add(operat);
                        i += 1;
                    }
                }
                else
                {
                    infixNotation.Add(userInput[i].ToString());
                    i += 1;
                }
            }
            getPostFix(infixNotation);
        }

        static int getPriority(string op, int inStackOrIncoming)
        {
            // index of operator corrsospends to index of their priority
            Dictionary<string, int> operators = new Dictionary<string, int>()
            {
                { "^", 0 },
                { "@", 0 },
                { "?", 0 },
                {"not",0 },

                { "*", 1 },
                { "/", 1 },

                { "+", 2 },
                { "-", 2 },

                { "<", 3 },
                { "<=",3 },
                { ">", 3 },
                { ">=",3 },
                { "==",3 },
                { "!=",3 },

                {"and",4 },

                { "or",5 },

                { "(", 6 },

                { "#", 7 }

            };


            // in-stack, in-coming
            int[,] priorities =
            { 
                {6, 7}, // 0 -> ^, unary +(@), unary -(?), not
                {5, 5}, // 1 -> *, /
                {4, 4}, // 2 -> binary +, -
                {3, 3}, // 3 -> >, >=, <, <=, =, !=
                {2, 2}, // 4 -> and
                {1, 1}, // 5 -> or
                {0, 7}, // 6 -> (
                {0, 0}  // 7 -> #
            };

            int operatorPos = operators[op];
            return priorities[operatorPos, inStackOrIncoming];
        }

        static void getPostFix(List<string> infix)
        {
            List<string> postFix = new List<string>();
            string variables = "abcdefghijklmnopqrstuvwxyz";

            int isp = 0;
            int icp = 1;

            Stack<string> postfixStack = new Stack<string>();

            postfixStack.Push("#");

            for(int i = 0; i < infix.Count; i++)
            {
                switch (infix[i])
                {
                    case "#":
                        while (postfixStack.Count > 0)
                        {
                            string popped = postfixStack.Pop();
                            postFix.Add(popped);
                        }
                        break;

                    case "(":
                        postfixStack.Push(infix[i]);
                        break;

                    case ")":
                        while (postfixStack.Peek() != "(")    //unstack until '('
                        {
                            string popped = postfixStack.Pop();
                            postFix.Add(popped);

                        }
                        postfixStack.Pop();  // delete '('
                        break;


                    default:
                        // if its a variable/letter
                        if (variables.Contains(infix[i]))
                        {
                            postFix.Add(infix[i]);
                            break;
                        }
                        // if its a number
                        bool isNumber = int.TryParse(infix[i], out int number);

                        if (isNumber)
                        {
                            postFix.Add(infix[i]);
                            break;
                        }

                        else
                        {
                            string currOp = infix[i];

                            // if + or - AND previous isnt a digit and isnt a variable OR NULL OR A CLOSING BRACKET
                            if (infix[i] == "+" || infix[i] == "-")
                            {
                                bool isPrevANumber;
                                if (i > 0)
                                {
                                    isPrevANumber = int.TryParse(infix[i - 1], out int num);
                                }
                                else
                                {
                                    isPrevANumber = false;
                                }
                                if (i == 0 || isPrevANumber == false && variables.Contains(infix[i - 1]) == false && infix[i-1] != ")")
                                {
                                    if (infix[i] == "+")
                                    {
                                        currOp = "@";
                                    }
                                    else
                                    {
                                        currOp = "?";
                                    }
                                }
                            }

                            while (getPriority(postfixStack.Peek(), isp) >= getPriority(currOp, icp))
                            {
                                string popped = postfixStack.Pop();
                                postFix.Add(popped);
                            }
                            postfixStack.Push(currOp);
                            break;

                        }
                }

                
            }
            List<string> postNoHash = new List<string>(postFix);
            postNoHash.Remove(postNoHash[postNoHash.Count - 1]);
            string postFixToUser = string.Join(" ", postNoHash);
            Console.WriteLine("\nPostfix notation: " + postFixToUser+"\n");
            enterVariables(postFix, variables);
        }


        static void enterVariables(List<string> postFix, string variables)
        {
            for(int i = 0; i < postFix.Count(); i++)
            {
                if (variables.Contains(postFix[i]))
                {
                    Console.WriteLine($"Enter value for {postFix[i]}");
                    string variableValue = Console.ReadLine();
                    postFix[i] = variableValue;
                }
            }
            evalutePostFix(postFix);
        }


        static void evalutePostFix(List<string> postFix)
        {
            Stack<double> eval = new Stack<double>();

            foreach(string token in postFix)
            {
                switch (token)
                {
                    case "#":
                        break;

                    case "@":
                        double unaryVal = eval.Pop();
                        eval.Push(+unaryVal);
                        break;
                    case "?":
                        unaryVal = eval.Pop();
                        eval.Push(-unaryVal);
                        break;
                    case "not":
                        unaryVal= eval.Pop();
                        double result = (unaryVal == 0) ? 1 : 0;
                        eval.Push(result);
                        break;

                    default:
                        // token is an operand

                        bool isOperand = int.TryParse(token, out int number);
                        if (isOperand)
                        {
                            double num = Convert.ToDouble(token);
                            eval.Push(num);
                            break;
                        }
                        // perform operation
                        else
                        {
                            double valRight = eval.Pop();
                            double valLeft = eval.Pop();
                            switch (token)
                            {
                                case "^":
                                    eval.Push(Math.Pow(valLeft,valRight));
                                    break;

                                case "*":
                                    eval.Push(valLeft * valRight);
                                    break;

                                case "/":
                                    eval.Push(valLeft / valRight);
                                    break;

                                case "+":
                                    eval.Push(valLeft + valRight);
                                    break;

                                case "-":
                                    eval.Push(valLeft - valRight);
                                    break;

                                case "<":
                                    eval.Push(valLeft < valRight ? 1 : 0);
                                    break;

                                case "<=":
                                    eval.Push(valLeft <= valRight ? 1 : 0);
                                    break;

                                case ">":
                                    eval.Push(valLeft > valRight ? 1 : 0);
                                    break;

                                case ">=":
                                    eval.Push(valLeft >= valRight ? 1 : 0);
                                    break;

                                case "==":
                                    eval.Push(valLeft == valRight ? 1 : 0);
                                    break;

                                case "!=":
                                    eval.Push(valLeft != valRight ? 1 : 0);
                                    break;

                                case "and":
                                    double resultAnd = (valLeft != 0 && valRight != 0) ? 1 : 0;
                                    eval.Push(resultAnd);
                                    break;

                                case "or":
                                    double resultOr = (valLeft != 0 || valRight != 0) ? 1 : 0;
                                    eval.Push(resultOr);
                                    break;
                            }
                            break;
                        }
                }
            }

            while (eval.Count > 0)
            {
                double evaluatedAnswer = eval.Pop();
                if(evaluatedAnswer == 0 || evaluatedAnswer == 1)
                {
                    bool ansBool = (evaluatedAnswer != 0);
                    Console.WriteLine("Evaluated to: " + evaluatedAnswer + " or " + ansBool + "\n");
                }
                else
                {
                    Console.WriteLine("Evaluated to: " + evaluatedAnswer + "\n");
                }

            }
        }
    }
}