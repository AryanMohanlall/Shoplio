namespace Shoplio.ConsoleApp.Utils;

public static class InputReader
{
    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (int.TryParse(input, out var value))
            {
                return value;
            }

            Console.WriteLine("Invalid number. Try again.");
        }
    }

    public static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();

            if (decimal.TryParse(input, out var value))
            {
                return value;
            }

            Console.WriteLine("Invalid decimal number. Try again.");
        }
    }

    public static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            Console.WriteLine("Value is required. Try again.");
        }
    }
}
