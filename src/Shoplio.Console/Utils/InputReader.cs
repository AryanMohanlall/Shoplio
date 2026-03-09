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

    public static int ReadInt(string prompt, bool allowEmpty)
    {
        if (!allowEmpty)
        {
            return ReadInt(prompt);
        }

        Console.Write(prompt);
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            return 0;
        }

        if (int.TryParse(input, out var value))
        {
            return value;
        }

        return 0;
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

    public static string ReadPassword(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);

            // Fallback for redirected input (tests/pipes) where ReadKey is unavailable.
            if (Console.IsInputRedirected)
            {
                var redirected = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(redirected))
                {
                    return redirected.Trim();
                }

                Console.WriteLine("Password is required. Try again.");
                continue;
            }

            var buffer = new List<char>();
            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    if (buffer.Count > 0)
                    {
                        buffer.RemoveAt(buffer.Count - 1);
                    }

                    continue;
                }

                if (!char.IsControl(key.KeyChar))
                {
                    buffer.Add(key.KeyChar);
                }
            }

            Console.WriteLine();

            var value = new string(buffer.ToArray()).Trim();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            Console.WriteLine("Password is required. Try again.");
        }
    }
}
