namespace Shoplio.ConsoleApp.UI;

public sealed class AdminMenu
{
    public void Show()
    {
        Console.Clear();
        Console.WriteLine("== Administrator Menu ==");
        Console.WriteLine("1. Add Product");
        Console.WriteLine("2. Update Product");
        Console.WriteLine("3. Delete Product");
        Console.WriteLine("4. Restock Inventory");
        Console.WriteLine("5. View Orders");
        Console.WriteLine("6. View Reports");
        Console.WriteLine("0. Back");
        Console.WriteLine();
        Console.WriteLine("Feature wiring pending: implement services and connect actions.");
        Console.WriteLine("Press Enter to return.");
        Console.ReadLine();
    }
}
