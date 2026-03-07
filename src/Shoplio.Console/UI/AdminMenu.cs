using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Services.Interfaces;
using Shoplio.ConsoleApp.Utils;

namespace Shoplio.ConsoleApp.UI;

public sealed class AdminMenu
{
    private readonly User _user;
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;
    private readonly IReportService _reportService;

    public AdminMenu(
        User user,
        IProductService productService,
        IOrderService orderService,
        IReportService reportService)
    {
        _user = user;
        _productService = productService;
        _orderService = orderService;
        _reportService = reportService;
    }

    public void Run()
    {
        var running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine($"=== Administrator Menu — {_user.Name} ===");
            Console.WriteLine("1.  Add Product");
            Console.WriteLine("2.  Update Product");
            Console.WriteLine("3.  Delete Product");
            Console.WriteLine("4.  Restock Product");
            Console.WriteLine("5.  View All Products");
            Console.WriteLine("6.  View All Orders");
            Console.WriteLine("7.  Sales Summary Report");
            Console.WriteLine("8.  Low Stock Report");
            Console.WriteLine("9.  Top-Selling Products Report");
            Console.WriteLine("0.  Logout");
            Console.WriteLine();

            var choice = InputReader.ReadInt("Select an option: ");

            switch (choice)
            {
                case 1: AddProduct(); break;
                case 2: UpdateProduct(); break;
                case 3: DeleteProduct(); break;
                case 4: RestockProduct(); break;
                case 5: ViewAllProducts(); break;
                case 6: ViewAllOrders(); break;
                case 7: SalesSummaryReport(); break;
                case 8: LowStockReport(); break;
                case 9: TopProductsReport(); break;
                case 0: running = false; break;
                default:
                    Console.WriteLine("Invalid option. Press Enter to continue.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private void AddProduct()
    {
        Console.Clear();
        Console.WriteLine("=== Add Product ===");

        try
        {
            var name = InputReader.ReadRequired("Product name: ");
            var category = InputReader.ReadRequired("Category: ");
            var price = ReadDecimal("Price: ");
            var stock = InputReader.ReadInt("Initial stock: ");

            var product = _productService.AddProduct(name, category, price, stock);
            Console.WriteLine($"\nProduct '{product.Name}' added with ID {product.Id}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Pause();
    }

    private void UpdateProduct()
    {
        Console.Clear();
        Console.WriteLine("=== Update Product ===");
        PrintProducts(_productService.GetAllProducts());

        var id = InputReader.ReadInt("\nProduct ID to update: ");
        var products = _productService.GetAllProducts();
        var product = products.FirstOrDefault(p => p.Id == id);

        if (product is null)
        {
            Console.WriteLine("Product not found.");
            Pause();
            return;
        }

        try
        {
            Console.WriteLine($"Current name: {product.Name}");
            var name = InputReader.ReadRequired("New name (or same): ");

            Console.WriteLine($"Current category: {product.Category}");
            var category = InputReader.ReadRequired("New category (or same): ");

            Console.WriteLine($"Current price: {product.Price:C2}");
            var price = ReadDecimal("New price: ");

            var updated = new Product
            {
                Id = product.Id,
                Name = name,
                Category = category,
                Price = price,
                Stock = product.Stock
            };

            _productService.UpdateProduct(updated);
            Console.WriteLine("Product updated.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Pause();
    }

    private void DeleteProduct()
    {
        Console.Clear();
        Console.WriteLine("=== Delete Product ===");
        PrintProducts(_productService.GetAllProducts());

        var id = InputReader.ReadInt("\nProduct ID to delete: ");
        var deleted = _productService.DeleteProduct(id);
        Console.WriteLine(deleted ? "Product deleted." : "Product not found.");
        Pause();
    }

    private void RestockProduct()
    {
        Console.Clear();
        Console.WriteLine("=== Restock Product ===");
        PrintProducts(_productService.GetAllProducts());

        var id = InputReader.ReadInt("\nProduct ID to restock: ");
        var products = _productService.GetAllProducts();
        var product = products.FirstOrDefault(p => p.Id == id);

        if (product is null)
        {
            Console.WriteLine("Product not found.");
            Pause();
            return;
        }

        var quantity = InputReader.ReadInt($"Amount to add to current stock ({product.Stock}): ");

        if (quantity <= 0)
        {
            Console.WriteLine("Restock quantity must be greater than zero.");
            Pause();
            return;
        }

        var restocked = new Product
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Price = product.Price,
            Stock = product.Stock + quantity
        };

        _productService.UpdateProduct(restocked);
        Console.WriteLine($"Stock updated to {restocked.Stock}.");
        Pause();
    }

    private void ViewAllProducts()
    {
        Console.Clear();
        Console.WriteLine("=== All Products ===");
        PrintProducts(_productService.GetAllProducts());
        Pause();
    }

    private void ViewAllOrders()
    {
        Console.Clear();
        Console.WriteLine("=== All Orders ===");
        var orders = _orderService.GetAllOrders();

        if (orders.Count == 0)
        {
            Console.WriteLine("No orders found.");
        }
        else
        {
            foreach (var order in orders.OrderByDescending(o => o.CreatedAt))
            {
                Console.WriteLine($"\nOrder #{order.Id} — User {order.UserId} — {order.CreatedAt:yyyy-MM-dd HH:mm} — Status: {order.Status} — Total: {order.TotalAmount:C2}");
                foreach (var item in order.Items)
                {
                    var product = _productService.GetAllProducts().FirstOrDefault(p => p.Id == item.ProductId);
                    var name = product?.Name ?? $"Product #{item.ProductId}";
                    Console.WriteLine($"  {name,-30} x{item.Quantity}  @{item.UnitPrice:C2}  = {item.LineTotal:C2}");
                }
            }
        }

        Pause();
    }

    private void SalesSummaryReport()
    {
        Console.Clear();
        Console.WriteLine("=== Sales Summary Report ===\n");
        foreach (var line in _reportService.GenerateSalesSummary())
            Console.WriteLine(line);
        Pause();
    }

    private void LowStockReport()
    {
        Console.Clear();
        Console.WriteLine("=== Low Stock Report ===");
        var threshold = InputReader.ReadInt("Low-stock threshold (press 0 for default of 10): ");
        if (threshold <= 0) threshold = 10;
        Console.WriteLine();
        foreach (var line in _reportService.GenerateLowStockReport(threshold))
            Console.WriteLine(line);
        Pause();
    }

    private void TopProductsReport()
    {
        Console.Clear();
        Console.WriteLine("=== Top-Selling Products ===");
        var count = InputReader.ReadInt("How many top products to show: ");
        Console.WriteLine();
        foreach (var line in _reportService.GenerateTopProductsReport(count))
            Console.WriteLine(line);
        Pause();
    }

    private static void PrintProducts(IReadOnlyList<Product> products)
    {
        if (products.Count == 0)
        {
            Console.WriteLine("No products found.");
            return;
        }

        Console.WriteLine($"\n{"ID",-5} {"Name",-32} {"Category",-15} {"Price",10} {"Stock",6}");
        Console.WriteLine(new string('-', 72));
        foreach (var p in products)
        {
            Console.WriteLine($"{p.Id,-5} {p.Name,-32} {p.Category,-15} {p.Price,10:C2} {p.Stock,6}");
        }
    }

    private static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = Console.ReadLine();
            if (decimal.TryParse(input, out var value) && value >= 0)
                return value;
            Console.WriteLine("Invalid amount. Enter a non-negative number.");
        }
    }

    private static void Pause()
    {
        Console.WriteLine("\nPress Enter to continue.");
        Console.ReadLine();
    }
}
