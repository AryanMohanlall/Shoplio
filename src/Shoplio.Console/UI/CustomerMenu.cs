using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Services.Interfaces;
using Shoplio.ConsoleApp.Utils;

namespace Shoplio.ConsoleApp.UI;

public sealed class CustomerMenu
{
    private readonly User _user;
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly IReviewService _reviewService;

    public CustomerMenu(
        User user,
        IProductService productService,
        ICartService cartService,
        IOrderService orderService,
        IReviewService reviewService)
    {
        _user = user;
        _productService = productService;
        _cartService = cartService;
        _orderService = orderService;
        _reviewService = reviewService;
    }

    public void Run()
    {
        var running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine($"=== Customer Menu — {_user.Name} | Wallet: {_user.WalletBalance:C2} ===");
            Console.WriteLine("1. Browse Products");
            Console.WriteLine("2. Search Products");
            Console.WriteLine("3. Add Product to Cart");
            Console.WriteLine("4. View Cart");
            Console.WriteLine("5. Update Cart Item Quantity");
            Console.WriteLine("6. Remove Item from Cart");
            Console.WriteLine("7. Checkout");
            Console.WriteLine("8. Order History");
            Console.WriteLine("9. View Product Reviews");
            Console.WriteLine("10. Add Review");
            Console.WriteLine("0. Logout");
            Console.WriteLine();

            var choice = InputReader.ReadInt("Select an option: ");

            switch (choice)
            {
                case 1: BrowseProducts(); break;
                case 2: SearchProducts(); break;
                case 3: AddToCart(); break;
                case 4: ViewCart(); break;
                case 5: UpdateCartItem(); break;
                case 6: RemoveCartItem(); break;
                case 7: Checkout(); break;
                case 8: OrderHistory(); break;
                case 9: ViewProductReviews(); break;
                case 10: AddReview(); break;
                case 0: running = false; break;
                default:
                    Console.WriteLine("Invalid option. Press Enter to continue.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private void BrowseProducts()
    {
        Console.Clear();
        Console.WriteLine("=== Product Catalog ===");
        PrintProducts(_productService.GetAllProducts());
        Pause();
    }

    private void SearchProducts()
    {
        Console.Clear();
        Console.WriteLine("=== Search Products ===");
        var query = InputReader.ReadRequired("Search (name or category): ");
        var results = _productService.SearchProducts(query);
        Console.WriteLine($"\nFound {results.Count} result(s):");
        PrintProducts(results);
        Pause();
    }

    private void AddToCart()
    {
        Console.Clear();
        Console.WriteLine("=== Add to Cart ===");
        PrintProducts(_productService.GetAllProducts());

        var productId = InputReader.ReadInt("\nProduct ID: ");
        var quantity = InputReader.ReadInt("Quantity: ");

        try
        {
            _cartService.AddToCart(_user.Id, productId, quantity);
            Console.WriteLine("Item added to cart.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Pause();
    }

    private void ViewCart()
    {
        Console.Clear();
        Console.WriteLine("=== Your Cart ===");
        var items = _cartService.GetCartItems(_user.Id);

        if (items.Count == 0)
        {
            Console.WriteLine("Your cart is empty.");
        }
        else
        {
            Console.WriteLine($"{"ID",-5} {"Name",-30} {"Qty",5} {"Unit Price",12} {"Line Total",12}");
            Console.WriteLine(new string('-', 68));

            foreach (var item in items)
            {
                var product = _productService.GetAllProducts().FirstOrDefault(p => p.Id == item.ProductId);
                var name = product?.Name ?? $"Product #{item.ProductId}";
                Console.WriteLine($"{item.ProductId,-5} {name,-30} {item.Quantity,5} {item.UnitPrice,12:C2} {item.LineTotal,12:C2}");
            }

            Console.WriteLine(new string('-', 68));
            var total = items.Sum(i => i.LineTotal);
            Console.WriteLine($"{"Total:",-43} {total,12:C2}");
        }

        Pause();
    }

    private void UpdateCartItem()
    {
        Console.Clear();
        Console.WriteLine("=== Update Cart Item ===");
        var productId = InputReader.ReadInt("Product ID to update: ");
        var quantity = InputReader.ReadInt("New quantity: ");

        try
        {
            var updated = _cartService.UpdateQuantity(_user.Id, productId, quantity);
            Console.WriteLine(updated ? "Quantity updated." : "Item not found in cart.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Pause();
    }

    private void RemoveCartItem()
    {
        Console.Clear();
        Console.WriteLine("=== Remove Cart Item ===");
        var productId = InputReader.ReadInt("Product ID to remove: ");

        var removed = _cartService.RemoveFromCart(_user.Id, productId);
        Console.WriteLine(removed ? "Item removed." : "Item not found in cart.");
        Pause();
    }

    private void Checkout()
    {
        Console.Clear();
        Console.WriteLine("=== Checkout ===");

        var items = _cartService.GetCartItems(_user.Id);
        if (items.Count == 0)
        {
            Console.WriteLine("Your cart is empty.");
            Pause();
            return;
        }

        var total = items.Sum(i => i.LineTotal);
        Console.WriteLine($"Order total: {total:C2}");
        Console.WriteLine($"Wallet balance: {_user.WalletBalance:C2}");
        Console.Write("Confirm purchase? (y/n): ");
        var confirm = Console.ReadLine();

        if (!string.Equals(confirm, "y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Checkout cancelled.");
            Pause();
            return;
        }

        try
        {
            var order = _orderService.PlaceOrder(_user.Id);
            Console.WriteLine($"\nOrder #{order.Id} placed successfully!");
            Console.WriteLine($"Amount charged: {order.TotalAmount:C2}");
            Console.WriteLine($"Remaining wallet: {_user.WalletBalance:C2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Pause();
    }

    private void OrderHistory()
    {
        Console.Clear();
        Console.WriteLine("=== Order History ===");
        var orders = _orderService.GetUserOrders(_user.Id);

        if (orders.Count == 0)
        {
            Console.WriteLine("You have no orders.");
        }
        else
        {
            foreach (var order in orders.OrderByDescending(o => o.CreatedAt))
            {
                Console.WriteLine($"\nOrder #{order.Id} — {order.CreatedAt:yyyy-MM-dd HH:mm} — Status: {order.Status} — Total: {order.TotalAmount:C2}");
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

    private void ViewProductReviews()
    {
        Console.Clear();
        Console.WriteLine("=== View Product Reviews ===");
        PrintProducts(_productService.GetAllProducts());

        var productId = InputReader.ReadInt("\nProduct ID: ");
        var product = _productService.GetAllProducts().FirstOrDefault(p => p.Id == productId);
        if (product is null)
        {
            Console.WriteLine("Product not found.");
            Pause();
            return;
        }

        var reviews = _reviewService.GetProductReviews(productId);
        var avg = _reviewService.GetAverageRating(productId);
        Console.WriteLine($"\nReviews for '{product.Name}' — Average Rating: {(reviews.Count > 0 ? $"{avg:F1}/5" : "No reviews yet")}");

        if (reviews.Count == 0)
        {
            Console.WriteLine("No reviews yet.");
        }
        else
        {
            foreach (var review in reviews.OrderByDescending(r => r.CreatedAt))
            {
                Console.WriteLine($"\n  Rating: {new string('★', review.Rating)}{new string('☆', 5 - review.Rating)} ({review.Rating}/5)");
                if (!string.IsNullOrWhiteSpace(review.Comment))
                    Console.WriteLine($"  Comment: {review.Comment}");
                Console.WriteLine($"  Date: {review.CreatedAt:yyyy-MM-dd}");
            }
        }

        Pause();
    }

    private void AddReview()
    {
        Console.Clear();
        Console.WriteLine("=== Add Review ===");
        PrintProducts(_productService.GetAllProducts());

        var productId = InputReader.ReadInt("\nProduct ID: ");
        var rating = InputReader.ReadInt("Rating (1–5): ");

        Console.Write("Comment (optional): ");
        var comment = Console.ReadLine() ?? string.Empty;

        try
        {
            _reviewService.AddReview(_user.Id, productId, rating, comment);
            Console.WriteLine("Review submitted. Thank you!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

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

    private static void Pause()
    {
        Console.WriteLine("\nPress Enter to continue.");
        Console.ReadLine();
    }
}
