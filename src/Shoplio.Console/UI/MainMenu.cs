using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Services.Interfaces;
using Shoplio.ConsoleApp.Utils;

namespace Shoplio.ConsoleApp.UI;

public sealed class MainMenu
{
    private readonly IAuthService _authService;
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly IReviewService _reviewService;
    private readonly IReportService _reportService;

    public MainMenu(
        IAuthService authService,
        IProductService productService,
        ICartService cartService,
        IOrderService orderService,
        IReviewService reviewService,
        IReportService reportService)
    {
        _authService = authService;
        _productService = productService;
        _cartService = cartService;
        _orderService = orderService;
        _reviewService = reviewService;
        _reportService = reportService;
    }

    public void Run()
    {
        var running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("           Welcome to Shoplio           ");
            Console.WriteLine("========================================");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("0. Exit");
            Console.WriteLine();

            var choice = InputReader.ReadInt("Select an option: ");

            switch (choice)
            {
                case 1:
                    Register();
                    break;
                case 2:
                    Login();
                    break;
                case 0:
                    running = false;
                    Console.WriteLine("Thank you for using Shoplio. Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid option. Press Enter to continue.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private void Register()
    {
        Console.Clear();
        Console.WriteLine("=== Register ===");

        try
        {
            var name = InputReader.ReadRequired("Full name: ");
            var email = InputReader.ReadRequired("Email: ");
            var password = InputReader.ReadRequired("Password: ");

            Console.WriteLine("Role: 1. Customer  2. Administrator");
            int roleChoice;
            while (true)
            {
                roleChoice = InputReader.ReadInt("Select role (1 or 2): ");
                if (roleChoice == 1 || roleChoice == 2) break;
                Console.WriteLine("Please enter 1 for Customer or 2 for Administrator.");
            }

            var role = roleChoice == 2 ? Role.Administrator : Role.Customer;

            var user = _authService.Register(name, email, password, role);
            Console.WriteLine($"\nRegistration successful! Welcome, {user.Name}.");
            if (role == Role.Customer)
                Console.WriteLine($"Your starting wallet balance is {user.WalletBalance:C2}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }

        Console.WriteLine("\nPress Enter to continue.");
        Console.ReadLine();
    }

    private void Login()
    {
        Console.Clear();
        Console.WriteLine("=== Login ===");

        var email = InputReader.ReadRequired("Email: ");
        var password = InputReader.ReadRequired("Password: ");

        var user = _authService.Login(email, password);

        if (user is null)
        {
            Console.WriteLine("\nInvalid email or password. Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        Console.WriteLine($"\nWelcome back, {user.Name}!");
        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();

        if (user.Role == Role.Administrator)
        {
            var adminMenu = new AdminMenu(user, _productService, _orderService, _reportService);
            adminMenu.Run();
        }
        else
        {
            var customerMenu = new CustomerMenu(user, _productService, _cartService, _orderService, _reviewService);
            customerMenu.Run();
        }
    }
}
