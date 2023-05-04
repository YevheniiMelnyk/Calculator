using System.Globalization;
using System.Text.RegularExpressions;

namespace CalculatorNS
{
    public class Calculator
    {
        private readonly CultureInfo _culture = CultureInfo.CreateSpecificCulture("en-US");
        private decimal _expressionResult;
        private string _pattern = @"^\s*(([-+])?\d+(\.\d+)?|\((\s*([-+])?\d+(\.\d+)?(\s*([-+*/])\s*(\d+(\.\d+)?))*)\))(\s*([-+*/])\s*(([-+])?\d+(\.\d+)?|\((\s*([-+])?\d+(\.\d+)?(\s*([-+*/])\s*(\d+(\.\d+)?))*)\)))*\s*$";

        public Calculator(int mode, string inputString = "", string inputFilePath = "", string outputFilePath = "")
        {
            if (mode < 1 || mode > 2)
            {
                throw new ArgumentException("Mode must be either 1 or 2.", nameof(mode));
            }

            if (mode == 1 && string.IsNullOrEmpty(inputString))
            {
                throw new ArgumentNullException("Expression cannot be null or empty.", nameof(inputString));
            }

            if (mode == 2 && (string.IsNullOrEmpty(inputFilePath) || !File.Exists(inputFilePath)))
            {
                throw new FileNotFoundException("Input file not found.", inputFilePath);
            }

            if (mode == 2 && string.IsNullOrEmpty(outputFilePath))
            {
                throw new FileNotFoundException("Output file path cannot be null or empty.", nameof(outputFilePath));
            }

            try
            {
                switch (mode)
                {
                    case 1:
                        ValidateAndCalculate(inputString);
                        break;
                    case 2:
                        string[] stringsFromFile = File.ReadAllLines(inputFilePath);
                        WriteAnswerToFile(stringsFromFile, outputFilePath);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: {0}", ex.Message);
            }
        }

        public void PrintResult()
        {
            Console.WriteLine($"Result: {GetExpressionResult().ToString(_culture)}");
        }

        public decimal GetExpressionResult()
        {
            return _expressionResult;
        }

        private void WriteAnswerToFile(string[] lines, string outputFilePath)
        {
            if (lines == null)
            {
                throw new ArgumentNullException(nameof(lines), "lines cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(outputFilePath))
            {
                throw new ArgumentException("Invalid output file path.", nameof(outputFilePath));
            }

            if (Path.HasExtension(outputFilePath))
            {
                string directory = Path.GetDirectoryName(outputFilePath);
                if (!Directory.Exists(directory))
                {
                    throw new DirectoryNotFoundException("Invalid file path. Directory not found.");
                }
            }
            else
            {
                throw new ArgumentException("Invalid file path.", nameof(outputFilePath));
            }

            using (StreamWriter writer = new(outputFilePath))
            {
                foreach (var row in lines)
                {
                    writer.Write(ValidateAndCalculate(row));
                    writer.WriteLine();
                }
            }
        }

        private string ValidateAndCalculate(string row)
        {
            if (string.IsNullOrWhiteSpace(row))
            {
                throw new ArgumentException("Invalid argument.", nameof(row));
            }

            string trimmedRow = row.Trim();

            Regex rx = new(_pattern, RegexOptions.ExplicitCapture);

            bool res = rx.IsMatch(trimmedRow);

            if (res && (char.IsDigit(trimmedRow[0]) || trimmedRow[0] == '-' || trimmedRow[0] == '('))
            {
                try
                {
                    decimal result = SplitAndCalculate(trimmedRow);
                    _expressionResult = result;
                    return $"{trimmedRow} = {result.ToString(_culture)}";
                }
                catch (Exception ex)
                {
                    return $"{trimmedRow} = Error. {ex.Message}";
                }
            }
            else
            {
                return $"{trimmedRow} = Error. Incorrect input format.";
            }
        }

        private decimal SplitAndCalculate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentException("Expression is null or empty.", nameof(expression));
            }

            if (!Regex.IsMatch(expression, _pattern))
            {
                throw new ArgumentException("Expression contains invalid characters.", nameof(expression));
            }
            List<string> splittedList = SplitExpression(expression);

            return Calculate(splittedList);
        }

        private static List<string> SplitExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException(nameof(expression), "Invalid expression.");
            }

            List<string> splittedExpression = new();
            int i = 0;
            bool lastWasOperatorOrBracket = true;

            while (i < expression.Length)
            {
                string element = "";

                if (char.IsDigit(expression[i]) || (expression[i] == '-' && (lastWasOperatorOrBracket || i == 0)))
                {
                    if (expression[i] == '-')
                    {
                        element += "-";
                        i++;
                    }

                    bool hasDecimalPoint = false;
                    while (i < expression.Length && (char.IsDigit(expression[i]) || (!hasDecimalPoint && expression[i] == '.')))
                    {
                        if (expression[i] == '.')
                        {
                            hasDecimalPoint = true;
                        }
                        element += expression[i];
                        i++;
                    }

                    splittedExpression.Add(element);
                    lastWasOperatorOrBracket = false;
                }
                else if (expression[i] == '+' || expression[i] == '-' || expression[i] == '*' || expression[i] == '/')
                {
                    splittedExpression.Add(expression[i].ToString());
                    i++;
                    lastWasOperatorOrBracket = true;
                }
                else if (expression[i] == '(' || expression[i] == ')')
                {
                    splittedExpression.Add(expression[i].ToString());
                    i++;
                    lastWasOperatorOrBracket = true;
                }
                else if (char.IsWhiteSpace(expression[i]))
                {
                    i++;
                }
                else
                {
                    throw new ArgumentException($"Invalid character: {expression[i]}");
                }
            }
            if (splittedExpression.Count == 1 && splittedExpression[0] == "-")
            {
                throw new ArgumentException("Invalid expression.");
            }
            return splittedExpression;
        }

        private decimal Calculate(List<string> splitExpression)
        {
            if (splitExpression == null || !splitExpression.Any())
            {
                throw new ArgumentNullException(nameof(splitExpression), "Splitted expression cannot be null or empty.");
            }

            Stack<decimal> stackNumbers = new();
            Stack<string> stackOperators = new();

            foreach (var element in splitExpression)
            {
                bool isNumber = decimal.TryParse(element, NumberStyles.Float, _culture, out decimal number);

                if (isNumber)
                {
                    stackNumbers.Push(number);
                    continue;
                }

                if (element == "(")
                {
                    stackOperators.Push(element);
                    continue;
                }

                if (element == ")")
                {
                    while (stackOperators.Count > 0 && stackOperators.Peek() != "(")
                    {
                        decimal num2 = stackNumbers.Pop();
                        decimal num1 = stackNumbers.Pop();
                        string op = stackOperators.Pop();

                        decimal result = ExecuteOperation(num1, num2, op);
                        stackNumbers.Push(result);
                    }

                    stackOperators.Pop();

                    if (stackOperators.Count > 0 && (stackOperators.Peek() == "*" || stackOperators.Peek() == "/"))
                    {
                        decimal num2 = stackNumbers.Pop();
                        decimal num1 = stackNumbers.Pop();
                        string op = stackOperators.Pop();

                        decimal result = ExecuteOperation(num1, num2, op);
                        stackNumbers.Push(result);
                    }

                    continue;
                }

                if (element.StartsWith('-') && element.Length > 1)
                {
                    stackNumbers.Push(decimal.Parse(element, NumberStyles.Float, _culture));
                    continue;
                }

                while (stackOperators.Count > 0 && GetOperatorPriority(element) <= GetOperatorPriority(stackOperators.Peek()))
                {
                    decimal num2 = stackNumbers.Pop();
                    decimal num1 = stackNumbers.Pop();
                    string op = stackOperators.Pop();

                    decimal result = ExecuteOperation(num1, num2, op);
                    stackNumbers.Push(result);
                }
                stackOperators.Push(element);
            }

            while (stackOperators.Count > 0)
            {
                decimal num2 = stackNumbers.Pop();
                decimal num1 = stackNumbers.Pop();
                string op = stackOperators.Pop();

                decimal result = ExecuteOperation(num1, num2, op);

                stackNumbers.Push(result);
            }

            return stackNumbers.Pop();
        }

        private int GetOperatorPriority(string op)
        {
            if (string.IsNullOrEmpty(op))
            {
                throw new ArgumentException("Wrong arguments.", nameof(op));
            }

            switch (op)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                default:
                    return 0;
            }
        }

        private decimal ExecuteOperation(decimal num1, decimal num2, string op)
        {
            if (string.IsNullOrEmpty(op))
            {
                throw new ArgumentException("Wrong arguments.", nameof(op));
            }

            switch (op)
            {
                case "+":
                    return num1 + num2;
                case "-":
                    return num1 - num2;
                case "*":
                    return num1 * num2;
                case "/":
                    if (num2 == 0)
                    {
                        throw new DivideByZeroException("Cannot divide by zero.");
                    }
                    return num1 / num2;
                default:
                    throw new ArgumentException($"Invalid operator: {op}.");
            }
        }
    }
}
