namespace Shoplio.ConsoleApp.UI;

public sealed class CustomerMenu
{
    public void Show()
    {
        Console.Clear();
        Console.WriteLine("== Customer Menu ==");
        Console.WriteLine("1. Browse Products");
        Console.WriteLine("2. Search Products");
        Console.WriteLine("3. View Cart");
        Console.WriteLine("4. Checkout");
        Console.WriteLine("5. Order History");
        Console.WriteLine("6. Add Review");
        Console.WriteLine("0. Back");
        Console.WriteLine();
        Console.WriteLine("Feature wiring pending: implement services and connect actions.");
        Console.WriteLine("Press Enter to return.");
        Console.ReadLine();
    }
}
