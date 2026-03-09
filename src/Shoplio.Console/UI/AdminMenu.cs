using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Services.Interfaces;
using Shoplio.ConsoleApp.Utils;

namespace Shoplio.ConsoleApp.UI;

public sealed class AdminMenu(
    IProductService productService,
    IOrderService orderService,
    IReportService reportService,
    IReviewService reviewService)
{
    private readonly IProductService _productService = productService;
    private readonly IOrderService _orderService = orderService;
    private readonly IReportService _reportService = reportService;
    private readonly IReviewService _reviewService = reviewService;

    public void Show()
    {
        var running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine("== Administrator Menu ==");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. Update Product");
            Console.WriteLine("3. Delete Product");
            Console.WriteLine("4. Restock Product");
            Console.WriteLine("5. View Products");
            Console.WriteLine("6. View Orders");
            Console.WriteLine("7. Update Order Status");
            Console.WriteLine("8. View Low Stock Products");
            Console.WriteLine("9. Generate Sales Report");
            Console.WriteLine("10. View Product Reviews");
            Console.WriteLine("0. Logout");

            var choice = InputReader.ReadInt("Select an option: ");

            switch (choice)
            {
                case 1:
                    AddProduct();
                    break;
                case 2:
                    UpdateProduct();
                    break;
                case 3:
                    DeleteProduct();
                    break;
                case 4:
                    RestockProduct();
                    break;
                case 5:
                    ViewProducts();
                    break;
                case 6:
                    ViewOrders();
                    break;
                case 7:
                    UpdateOrderStatus();
                    break;
                case 8:
                    ViewLowStockProducts();
                    break;
                case 9:
                    GenerateSalesReport();
                    break;
                case 10:
                    ViewProductReviews();
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

    private void AddProduct()
    {
        Console.Clear();
        Console.WriteLine("== Add Product ==");

        var name = InputReader.ReadRequired("Product Name: ");
        var category = InputReader.ReadRequired("Category: ");
        var price = InputReader.ReadDecimal("Price: ");
        var stock = InputReader.ReadInt("Initial Stock: ");

        try
        {
            var product = _productService.AddProduct(name, category, price, stock);
            Console.WriteLine();
            Console.WriteLine($"Product added successfully! ID: {product.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"Failed to add product: {ex.Message}");
        }

        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    private void UpdateProduct()
    {
        Console.Clear();
        Console.WriteLine("== Update Product ==");
        Console.WriteLine();

        var products = _productService.GetAllProducts();

        if (!products.Any())
        {
            Console.WriteLine("No products available to update.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("Available Products:");
        for (int i = 0; i < products.Count; i++)
        {
            var p = products[i];
            Console.WriteLine($"{i + 1}. {p.Name} ({p.Category}) | ${p.Price:F2} | Stock: {p.Stock}");
        }

        Console.WriteLine();
        Console.WriteLine("0. Cancel");
        Console.WriteLine();

        var choice = InputReader.ReadInt("Select product number to update: ");

        if (choice == 0)
        {
            return;
        }

        if (choice < 1 || choice > products.Count)
        {
            Console.WriteLine("Invalid selection.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        var existingProduct = products[choice - 1];

        Console.WriteLine();
        Console.WriteLine($"Current: {existingProduct.Name} | {existingProduct.Category} | ${existingProduct.Price} | Stock: {existingProduct.Stock}");
        Console.WriteLine();

        var name = InputReader.ReadRequired("New Name: ");
        var category = InputReader.ReadRequired("New Category: ");
        var price = InputReader.ReadDecimal("New Price: ");
        var stock = InputReader.ReadInt("New Stock: ");

        var updatedProduct = new Product
        {
            Id = existingProduct.Id,
            Name = name,
            Category = category,
            Price = price,
            Stock = stock
        };

        try
        {
            var success = _productService.UpdateProduct(updatedProduct);
            Console.WriteLine();
            Console.WriteLine(success
                ? "Product updated successfully!"
                : "Failed to update product.");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"Failed to update product: {ex.Message}");
        }

        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    private void DeleteProduct()
    {
        Console.Clear();
        Console.WriteLine("== Delete Product ==");
        Console.WriteLine();

        var products = _productService.GetAllProducts();

        if (!products.Any())
        {
            Console.WriteLine("No products available to delete.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("Available Products:");
        for (int i = 0; i < products.Count; i++)
        {
            var p = products[i];
            Console.WriteLine($"{i + 1}. {p.Name} ({p.Category}) | ${p.Price:F2} | Stock: {p.Stock}");
        }

        Console.WriteLine();
        Console.WriteLine("0. Cancel");
        Console.WriteLine();

        var choice = InputReader.ReadInt("Select product number to delete: ");

        if (choice == 0)
        {
            return;
        }

        if (choice < 1 || choice > products.Count)
        {
            Console.WriteLine("Invalid selection.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        var existingProduct = products[choice - 1];

        Console.WriteLine();
        Console.WriteLine($"Product: {existingProduct.Name} | {existingProduct.Category} | ${existingProduct.Price:F2}");
        Console.Write("Are you sure you want to delete this product? (y/n): ");
        var confirmation = Console.ReadLine()?.Trim().ToLowerInvariant();

        if (confirmation != "y")
        {
            Console.WriteLine("Deletion cancelled.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        var success = _productService.DeleteProduct(existingProduct.Id);
        Console.WriteLine();
        Console.WriteLine(success
            ? "Product deleted successfully!"
            : "Failed to delete product.");

        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    private void RestockProduct()
    {
        Console.Clear();
        Console.WriteLine("== Restock Product ==");
        Console.WriteLine();

        var products = _productService.GetAllProducts();

        if (!products.Any())
        {
            Console.WriteLine("No products available to restock.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        Console.WriteLine("Available Products:");
        for (int i = 0; i < products.Count; i++)
        {
            var p = products[i];
            Console.WriteLine($"{i + 1}. {p.Name} ({p.Category}) | Current Stock: {p.Stock}");
        }

        Console.WriteLine();
        Console.WriteLine("0. Cancel");
        Console.WriteLine();

        var choice = InputReader.ReadInt("Select product number to restock: ");

        if (choice == 0)
        {
            return;
        }

        if (choice < 1 || choice > products.Count)
        {
            Console.WriteLine("Invalid selection.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        var existingProduct = products[choice - 1];

        Console.WriteLine();
        Console.WriteLine($"Product: {existingProduct.Name} | Current Stock: {existingProduct.Stock}");

        var additionalStock = InputReader.ReadInt("Additional Stock to Add: ");

        try
        {
            var success = _productService.RestockProduct(existingProduct.Id, additionalStock);
            Console.WriteLine();
            if (success)
            {
                var updatedStock = existingProduct.Stock + additionalStock;
                Console.WriteLine($"Product restocked successfully! New stock: {updatedStock}");
            }
            else
            {
                Console.WriteLine("Failed to restock product.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"Failed to restock product: {ex.Message}");
        }

        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    private void ViewProducts()
    {
        Console.Clear();
        Console.WriteLine("== All Products ==");
        Console.WriteLine();

        var products = _productService.GetAllProducts();

        if (!products.Any())
        {
            Console.WriteLine("No products found.");
        }
        else
        {
            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.Id} | {product.Name} ({product.Category}) | ${product.Price:F2} | Stock: {product.Stock}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    private void ViewOrders()
    {
        Console.Clear();
        Console.WriteLine("== All Orders ==");
        Console.WriteLine();

        var orders = _orderService.GetAllOrders();

        if (!orders.Any())
        {
            Console.WriteLine("No orders found.");
        }
        else
        {
            foreach (var order in orders)
            {
                Console.WriteLine($"Order ID: {order.Id} | User ID: {order.UserId} | Total: ${order.TotalAmount:F2} | Status: {order.Status} | Date: {order.CreatedAt:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"  Items: {order.Items.Count} item(s)");
                foreach (var item in order.Items)
                {
                    Console.WriteLine($"    - Product ID: {item.ProductId} | Qty: {item.Quantity} | Unit Price: ${item.UnitPrice:F2} | Line Total: ${item.LineTotal:F2}");
                }
                Console.WriteLine();
            }
        }

        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    private void UpdateOrderStatus()
    {
        Console.Clear();
        Console.WriteLine("== Update Order Status ==");

        var orderId = InputReader.ReadInt("Order ID: ");
        var existingOrder = _orderService.GetAllOrders()
            .FirstOrDefault(o => o.Id == orderId);

        if (existingOrder is null)
        {
            Console.WriteLine($"Order with ID {orderId} not found.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"Order ID: {existingOrder.Id} | Current Status: {existingOrder.Status}");
        Console.WriteLine();
        Console.WriteLine("Select new status:");
        Console.WriteLine("1. Placed");
        Console.WriteLine("2. Paid");
        Console.WriteLine("3. Cancelled");

        var statusChoice = InputReader.ReadInt("Status: ");
        var newStatus = statusChoice switch
        {
            1 => OrderStatus.Placed,
            2 => OrderStatus.Paid,
            3 => OrderStatus.Cancelled,
            _ => existingOrder.Status
        };

        if (newStatus == existingOrder.Status)
        {
            Console.WriteLine("Status unchanged.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        var success = _orderService.UpdateOrderStatus(orderId, newStatus);
        Console.WriteLine();
        Console.WriteLine(success
            ? $"Order status updated to {newStatus}!"
            : "Failed to update order status.");

        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    private void ViewLowStockProducts()
    {
        Console.Clear();
        Console.WriteLine("== Low Stock Products ==");

        var threshold = InputReader.ReadInt("Stock Threshold: ");
        Console.WriteLine();

        var report = _reportService.GenerateLowStockReport(threshold);

        foreach (var line in report)
        {
            Console.WriteLine(line);
        }

        Console.WriteLine();
        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    private void GenerateSalesReport()
    {
        Console.Clear();
        Console.WriteLine("== Sales Report ==");
        Console.WriteLine();

        var salesSummary = _reportService.GenerateSalesSummary();

        foreach (var line in salesSummary)
        {
            Console.WriteLine(line);
        }

        Console.WriteLine();
        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }

    private void ViewProductReviews()
    {
        Console.Clear();
        Console.WriteLine("== View Product Reviews ==");
        Console.WriteLine();

        var products = _productService.GetAllProducts().ToList();
        if (!products.Any())
        {
            Console.WriteLine("No products available.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        for (var i = 0; i < products.Count; i++)
        {
            var p = products[i];
            var reviews = _reviewService.GetReviewsByProductId(p.Id);
            var avgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            var reviewCount = reviews.Count;
            
            Console.WriteLine($"{i + 1}. {p.Name} ({p.Category}) - Avg Rating: {avgRating:F1}/5.0 ({reviewCount} reviews)");
        }

        Console.WriteLine();
        Console.WriteLine("0. Back");
        var choice = InputReader.ReadInt("Select product to view reviews: ");
        if (choice == 0)
        {
            return;
        }

        if (choice < 1 || choice > products.Count)
        {
            Console.WriteLine("Invalid selection.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        var selectedProduct = products[choice - 1];
        var productReviews = _reviewService.GetReviewsByProductId(selectedProduct.Id).ToList();

        Console.Clear();
        Console.WriteLine($"== Reviews for {selectedProduct.Name} ==");
        Console.WriteLine();

        if (!productReviews.Any())
        {
            Console.WriteLine("No reviews yet.");
            Console.WriteLine("Press Enter to continue.");
            Console.ReadLine();
            return;
        }

        var totalRating = productReviews.Average(r => r.Rating);
        Console.WriteLine($"Average Rating: {totalRating:F1}/5.0 ({productReviews.Count} reviews)");
        Console.WriteLine(new string('=', 60));
        Console.WriteLine();

        foreach (var review in productReviews)
        {
            Console.WriteLine($"Rating: [{new string('*', review.Rating)}{new string('-', 5 - review.Rating)}] {review.Rating}/5");
            Console.WriteLine($"Date: {review.CreatedAt:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"Comment: {review.Comment}");
            Console.WriteLine(new string('-', 60));
        }

        Console.WriteLine();
        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }
}
