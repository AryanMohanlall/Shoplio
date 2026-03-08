using Shoplio.ConsoleApp.Utils;
using SystemConsole = global::System.Console;
using System.Globalization;

namespace Shoplio.Console.Tests.Utils;

public sealed class InputReaderTests
{
    [Fact]
    public void ReadInt_RetriesUntilValidInteger()
    {
        var originalIn = SystemConsole.In;
        var originalOut = SystemConsole.Out;

        try
        {
            using var input = new StringReader("abc\n42\n");
            using var output = new StringWriter();
            SystemConsole.SetIn(input);
            SystemConsole.SetOut(output);

            var value = InputReader.ReadInt("Enter int: ");

            Assert.Equal(42, value);
            Assert.Contains("Invalid number. Try again.", output.ToString());
        }
        finally
        {
            SystemConsole.SetIn(originalIn);
            SystemConsole.SetOut(originalOut);
        }
    }

    [Fact]
    public void ReadDecimal_RetriesUntilValidDecimal()
    {
        var originalIn = SystemConsole.In;
        var originalOut = SystemConsole.Out;

        try
        {
            var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            using var input = new StringReader($"x\n12{separator}5\n");
            using var output = new StringWriter();
            SystemConsole.SetIn(input);
            SystemConsole.SetOut(output);

            var value = InputReader.ReadDecimal("Enter decimal: ");

            Assert.Equal(12.5m, value);
            Assert.Contains("Invalid decimal number. Try again.", output.ToString());
        }
        finally
        {
            SystemConsole.SetIn(originalIn);
            SystemConsole.SetOut(originalOut);
        }
    }

    [Fact]
    public void ReadRequired_RetriesUntilNonEmptyValueAndTrims()
    {
        var originalIn = SystemConsole.In;
        var originalOut = SystemConsole.Out;

        try
        {
            using var input = new StringReader("   \n  Alice  \n");
            using var output = new StringWriter();
            SystemConsole.SetIn(input);
            SystemConsole.SetOut(output);

            var value = InputReader.ReadRequired("Name: ");

            Assert.Equal("Alice", value);
            Assert.Contains("Value is required. Try again.", output.ToString());
        }
        finally
        {
            SystemConsole.SetIn(originalIn);
            SystemConsole.SetOut(originalOut);
        }
    }
}
