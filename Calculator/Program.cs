using CalculatorNS;
using System.Text.RegularExpressions;

internal class Program
{
    private static string _pattern = @"^\s*(([-+])?\d+(\.\d+)?|\((\s*([-+])?\d+(\.\d+)?(\s*([-+*/])\s*(\d+(\.\d+)?))*)\))(\s*([-+*/])\s*(([-+])?\d+(\.\d+)?|\((\s*([-+])?\d+(\.\d+)?(\s*([-+*/])\s*(\d+(\.\d+)?))*)\)))*\s*$";
    private static string _firstSymbolPatternCheck = @"^[(-]|\d";

    

    private static void Main(string[] args)
    {
        int mode = SelectCalculatorMode();

        string modeMsg = mode == 1 ? "Enter your expression: " : "Enter file path: ";

        Console.Write(modeMsg);
        string inputString = string.Empty;
        string inputFile = string.Empty;
        string otputFile = string.Empty;

        if (mode == 1)
        {
            inputString = ValidateInputString();
        }
        else
        {
            inputFile = FileExist();
            otputFile = DirectoryToOutputFile();
        }

        Calculator calculator = new(mode, inputString, inputFile, otputFile);

        if(mode == 1)
        {
            calculator.PrintResult();
        }
    }

    private static string ValidateInputString()
    {
        string trimmedString = Console.ReadLine().Trim();

        Regex rx = new(_pattern, RegexOptions.ExplicitCapture);
        bool res = rx.IsMatch(trimmedString);

        while (!res || trimmedString.Contains("/0") || !(char.IsDigit(trimmedString[0]) || trimmedString[0] == '-' || trimmedString[0] == '('))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Invalid input. Please enter a valid expression: ");
            Console.ResetColor();

            trimmedString = Console.ReadLine();
            res = rx.IsMatch(trimmedString);
        }

        return trimmedString;
    }

    private static int SelectCalculatorMode()
    {
        int output;
        Console.Write($"Select calculator mode (1 - console using, 2 - file calculation): ");
        string rowsStr = Console.ReadLine();

        bool res = int.TryParse(rowsStr, out output);
        while (!res || output < 1 || output > 2)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"Invalid input, please enter valid calculator mode(1 - console using, 2 - file calculation): ");
            Console.ResetColor();
            rowsStr = Console.ReadLine();
            res = int.TryParse(rowsStr, out output);
        }

        return output;
    }

    private static string FileExist()
    {
        string filePath = Console.ReadLine();

        bool result = File.Exists(filePath);

        while (!result)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Invalid file path or file doesn't exist.");
            Console.ResetColor();
            Console.WriteLine();
            Console.Write("Please enter valid path to input file: ");
            filePath = Console.ReadLine();
            result = File.Exists(filePath);
        }

        return filePath;
    }

    private static string DirectoryToOutputFile()
    {
        Console.Write("Enter path to output file: ");
        string outputFilePath = Console.ReadLine();

        bool isValidPath = false;

        do
        {
            Console.ForegroundColor = ConsoleColor.Red;

            if (Path.HasExtension(outputFilePath))
            {
                string directory = Path.GetDirectoryName(outputFilePath);

                if (Directory.Exists(directory))
                {
                    isValidPath = true;
                }
                else
                {
                    Console.WriteLine("Directory does not exist.");
                }
            }
            else
            {
                Console.WriteLine("Invalid path.");
            }

            Console.ResetColor();

            if (!isValidPath)
            {
                Console.Write("Please enter a valid path to output file: ");
                outputFilePath = Console.ReadLine();
            }

        } while (!isValidPath);

        return outputFilePath;
    }
}