using Shoplio.ConsoleApp.Utils;

namespace Shoplio.ConsoleApp.UI;

public sealed class MainMenu
{
    private readonly CustomerMenu _customerMenu = new();
    private readonly AdminMenu _adminMenu = new();

    public void Run()
    {
        var running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("== Shoplio Main Menu ==");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login as Customer");
            Console.WriteLine("3. Login as Administrator");
            Console.WriteLine("0. Exit");

            var choice = InputReader.ReadInt("Select an option: ");

            switch (choice)
            {
                case 1:
                    ShowRegistrationStub();
                    break;
                case 2:
                    _customerMenu.Show();
                    break;
                case 3:
                    _adminMenu.Show();
                    break;
                case 0:
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Press Enter to continue.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private static void ShowRegistrationStub()
    {
        Console.Clear();
        Console.WriteLine("Registration flow pending: implement AuthService and persistence.");
        Console.WriteLine("Press Enter to return.");
        Console.ReadLine();
    }
}
