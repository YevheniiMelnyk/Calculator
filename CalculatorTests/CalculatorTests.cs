using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalculatorNS.Tests
{
    [TestClass()]
    public class CalculatorTests
    {

        [TestMethod()]
        public void Calculator_FileMode_Test_NoException()
        {
            string inputFilePath = Path.GetTempFileName();
            string outputFilePath = Path.GetTempFileName();

            Calculator calculator = new(2, "", inputFilePath, outputFilePath);
            File.Delete(inputFilePath);
            File.Delete(outputFilePath);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calculator_InvalidMode_Exception()
        {
            Calculator calculator = new(5, "", "", "");
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Calculator_FileMode_PathToInputFileWhichDoesntExist()
        {
            string filePath = Path.Combine(@"InvalidPath", "input.txt");

            string outputFilePath = Path.GetTempFileName();
            Calculator calculator = new(2, "", filePath, outputFilePath);
            File.Delete(outputFilePath);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Calculator_FileMode_InvalidPathToInputFile()
        {
            string filePath = "";
            string outputFilePath = Path.GetTempFileName();
            Calculator calculator = new(2, "", filePath, outputFilePath);
            File.Delete(outputFilePath);

        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Calculator_FileMode_InvalidPathToOutputFile()
        {
            string filePath = "";
            string inputFilePath = Path.GetTempFileName();
            Calculator calculator = new(2, "", inputFilePath, filePath);
            File.Delete(inputFilePath);
        }

        [TestMethod()]
        public void Calculator_ConsoleMode_Test_NoException()
        {
            Calculator calculator = new(1, "-3-3", "", "");
        }

        [TestMethod()]
        public void Calculator_ConsoleMode_ValidInputString_NoException()
        {
            string inputString = "2 + 2";
            Calculator calculator = new(1, inputString, "", "");
            Assert.IsNotNull(calculator);
        }

        [TestMethod()]
        public void Calculator_ConsoleMode_ValidInputString_IsCorrectResult()
        {
            string inputString = "2+2*2";
            Calculator calculator = new(1, inputString, "", "");
            decimal actualResult = calculator.GetExpressionResult();
            decimal expectedResult = 6;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calculator_ConsoleMode_InputStringIsEmpty_NullException()
        {
            string inputString = string.Empty;
            Calculator calculator = new(1, inputString, "", "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calculator_ConsoleMode_InputStringIsNull_NullException()
        {
            string inputString = null;
            Calculator calculator = new(1, inputString, "", "");
        }
    }
}