using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Services.Interfaces;
using Shoplio.ConsoleApp.Utils;

namespace Shoplio.ConsoleApp.UI;

public sealed class MainMenu(
    IAuthService authService,
    IProductService productService,
    ICartService cartService,
    IOrderService orderService,
    IReviewService reviewService,
    IReportService reportService)
{
    private readonly IAuthService _authService = authService;
    private readonly CustomerMenu _customerMenu = new(productService, cartService, orderService, reviewService);
    private readonly AdminMenu _adminMenu = new(productService, orderService, reportService, reviewService);

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
                    RegisterUser();
                    break;
                case 2:
                    LoginAs(Role.Customer);
                    break;
                case 3:
                    LoginAs(Role.Administrator);
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

    private void RegisterUser()
    {
        Console.Clear();
        Console.WriteLine("== Register ==");

        var name = InputReader.ReadRequired("Name: ");
        var email = InputReader.ReadRequired("Email: ");
        var password = InputReader.ReadPassword("Password (min 6 chars): ");

        Console.WriteLine("Choose role:");
        Console.WriteLine("1. Customer");
        Console.WriteLine("2. Administrator");
        var roleOption = InputReader.ReadInt("Role: ");
        var role = roleOption == 2 ? Role.Administrator : Role.Customer;

        try
        {
            var createdUser = _authService.Register(name, email, password, role);
            Console.WriteLine();
            Console.WriteLine($"Registered successfully as {createdUser.Role}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"Registration failed: {ex.Message}");
        }

        Console.WriteLine("Press Enter to return.");
        Console.ReadLine();
    }

    private void LoginAs(Role requiredRole)
    {
        Console.Clear();
        Console.WriteLine($"== Login as {requiredRole} ==");

        var email = InputReader.ReadRequired("Email: ");
        var password = InputReader.ReadPassword("Password: ");

        var user = _authService.Login(email, password);

        if (user is null)
        {
            Console.WriteLine("Invalid credentials.");
            Console.WriteLine("Press Enter to return.");
            Console.ReadLine();
            return;
        }

        if (user.Role != requiredRole)
        {
            Console.WriteLine($"Access denied. This account is registered as {user.Role}.");
            Console.WriteLine("Press Enter to return.");
            Console.ReadLine();
            return;
        }

        Console.WriteLine($"Welcome, {user.Name}!");
        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();

        if (requiredRole == Role.Customer)
        {
            _customerMenu.Show(user);
            return;
        }

        _adminMenu.Show();
    }
}
