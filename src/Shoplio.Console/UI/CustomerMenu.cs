using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Services.Interfaces;
using Shoplio.ConsoleApp.Utils;

namespace Shoplio.ConsoleApp.UI;

public sealed class CustomerMenu(
    IProductService productService,
    ICartService cartService,
    IOrderService orderService,
    IReviewService reviewService)
{
    private readonly IProductService _productService = productService;
    private readonly ICartService _cartService = cartService;
    private readonly IOrderService _orderService = orderService;
    private readonly IReviewService _reviewService = reviewService;

    public void Show(User user)
    {
        var running = true;

        while (running)
        {
            Console.Clear();
            Console.WriteLine($"== Customer Menu ({user.Name}) ==");
            Console.WriteLine("1. Browse Products");
            Console.WriteLine("2. Search Products");
            Console.WriteLine("3. Add Product to Cart");
            Console.WriteLine("4. View Cart");
            Console.WriteLine("5. Update Cart");
            Console.WriteLine("6. Checkout");
            Console.WriteLine("7. View Wallet Balance");
            Console.WriteLine("8. Add Wallet Funds");
            Console.WriteLine("9. View Order History");
            Console.WriteLine("10. Track Orders");
            Console.WriteLine("11. Add Product Review");
            Console.WriteLine("12. View Product Reviews");
            Console.WriteLine("0. Logout");

            var choice = InputReader.ReadInt("Select an option: ");

            switch (choice)
            {
                case 1:
                    BrowseProducts();
                    break;
                case 2:
                    SearchProducts();
                    break;
                case 3:
                    AddProductToCart(user.Id);
                    break;
                case 4:
                    ViewCart(user.Id);
                    break;
                case 5:
                    UpdateCart(user.Id);
                    break;
                case 6:
                    Checkout(user);
                    break;
                case 7:
                    ViewWalletBalance(user);
                    break;
                case 8:
                    AddWalletFunds(user);
                    break;
                case 9:
                    ViewOrderHistory(user.Id);
                    break;
                case 10:
                    TrackOrders(user.Id);
                    break;
                case 11:
                    AddProductReview(user.Id);
                    break;
                case 12:
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

    private void BrowseProducts()
    {
        Console.Clear();
        Console.WriteLine("== Browse Products ==");
        Console.WriteLine();

        var products = _productService.GetAllProducts();
        if (!products.Any())
        {
            Console.WriteLine("No products available.");
            Pause();
            return;
        }

        foreach (var p in products)
        {
            Console.WriteLine($"ID: {p.Id} | {p.Name} ({p.Category}) | ${p.Price:F2} | Stock: {p.Stock}");
        }

        Pause();
    }

    private void SearchProducts()
    {
        Console.Clear();
        Console.WriteLine("== Search Products ==");
        var query = InputReader.ReadRequired("Search by name/category: ");
        Console.WriteLine();

        var results = _productService.SearchProducts(query);
        if (!results.Any())
        {
            Console.WriteLine("No matching products found.");
            Pause();
            return;
        }

        foreach (var p in results)
        {
            Console.WriteLine($"ID: {p.Id} | {p.Name} ({p.Category}) | ${p.Price:F2} | Stock: {p.Stock}");
        }

        Pause();
    }

    private void AddProductToCart(int userId)
    {
        Console.Clear();
        Console.WriteLine("== Add Product to Cart ==");
        Console.WriteLine();

        var products = _productService.GetAllProducts().Where(p => p.Stock > 0).ToList();
        if (!products.Any())
        {
            Console.WriteLine("No in-stock products available.");
            Pause();
            return;
        }

        for (var i = 0; i < products.Count; i++)
        {
            var p = products[i];
            Console.WriteLine($"{i + 1}. {p.Name} ({p.Category}) | ${p.Price:F2} | Stock: {p.Stock}");
        }

        Console.WriteLine();
        Console.WriteLine("0. Cancel");
        var choice = InputReader.ReadInt("Select product number: ");
        if (choice == 0)
        {
            return;
        }

        if (choice < 1 || choice > products.Count)
        {
            Console.WriteLine("Invalid selection.");
            Pause();
            return;
        }

        var selected = products[choice - 1];
        var quantity = InputReader.ReadInt("Quantity: ");

        try
        {
            _cartService.AddToCart(userId, selected.Id, quantity);
            Console.WriteLine("Added to cart successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not add to cart: {ex.Message}");
        }

        Pause();
    }

    private void ViewCart(int userId)
    {
        Console.Clear();
        Console.WriteLine("== View Cart ==");
        Console.WriteLine();

        var items = _cartService.GetCartItems(userId);
        if (!items.Any())
        {
            Console.WriteLine("Your cart is empty.");
            Pause();
            return;
        }

        var products = _productService.GetAllProducts().ToDictionary(p => p.Id);
        decimal total = 0;

        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var name = products.TryGetValue(item.ProductId, out var p) ? p.Name : "Unknown Product";
            total += item.LineTotal;
            Console.WriteLine($"{i + 1}. {name} | Qty: {item.Quantity} | Unit: ${item.UnitPrice:F2} | Line: ${item.LineTotal:F2}");
        }

        Console.WriteLine();
        Console.WriteLine($"Cart Total: ${total:F2}");
        Pause();
    }

    private void UpdateCart(int userId)
    {
        Console.Clear();
        Console.WriteLine("== Update Cart ==");
        Console.WriteLine();

        var items = _cartService.GetCartItems(userId).ToList();
        if (!items.Any())
        {
            Console.WriteLine("Your cart is empty.");
            Pause();
            return;
        }

        var products = _productService.GetAllProducts().ToDictionary(p => p.Id);
        for (var i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var name = products.TryGetValue(item.ProductId, out var p) ? p.Name : "Unknown Product";
            Console.WriteLine($"{i + 1}. {name} | Current Qty: {item.Quantity}");
        }

        Console.WriteLine();
        Console.WriteLine("0. Cancel");
        var choice = InputReader.ReadInt("Select cart item number: ");
        if (choice == 0)
        {
            return;
        }

        if (choice < 1 || choice > items.Count)
        {
            Console.WriteLine("Invalid selection.");
            Pause();
            return;
        }

        var selectedItem = items[choice - 1];
        Console.WriteLine("Enter new quantity (0 to remove item).\n");
        var newQuantity = InputReader.ReadInt("New quantity: ");

        var updated = _cartService.UpdateQuantity(userId, selectedItem.ProductId, newQuantity);
        Console.WriteLine(updated ? "Cart updated successfully." : "Unable to update cart item.");
        Pause();
    }

    private void Checkout(User user)
    {
        Console.Clear();
        Console.WriteLine("== Checkout ==");
        Console.WriteLine();

        var cartItems = _cartService.GetCartItems(user.Id);
        if (!cartItems.Any())
        {
            Console.WriteLine("Your cart is empty.");
            Pause();
            return;
        }

        var total = cartItems.Sum(i => i.LineTotal);
        Console.WriteLine($"Order Total: ${total:F2}");
        Console.WriteLine($"Wallet Balance: ${user.WalletBalance:F2}");

        if (user.WalletBalance < total)
        {
            Console.WriteLine("Insufficient wallet balance. Add funds and try again.");
            Pause();
            return;
        }

        Console.Write("Confirm checkout? (y/n): ");
        var confirmation = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (confirmation != "y")
        {
            Console.WriteLine("Checkout canceled.");
            Pause();
            return;
        }

        try
        {
            var order = _orderService.PlaceOrder(user.Id);
            Console.WriteLine($"Order placed successfully. Order ID: {order.Id}");
            Console.WriteLine($"Remaining Wallet Balance: ${user.WalletBalance:F2}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Checkout failed: {ex.Message}");
        }

        Pause();
    }

    private static void ViewWalletBalance(User user)
    {
        Console.Clear();
        Console.WriteLine("== Wallet Balance ==");
        Console.WriteLine($"Current Wallet Balance: ${user.WalletBalance:F2}");
        Pause();
    }

    private static void AddWalletFunds(User user)
    {
        Console.Clear();
        Console.WriteLine("== Add Wallet Funds ==");
        var amount = InputReader.ReadDecimal("Amount to add: ");

        if (amount <= 0)
        {
            Console.WriteLine("Amount must be greater than zero.");
            Pause();
            return;
        }

        user.WalletBalance += amount;
        Console.WriteLine($"Funds added successfully. New balance: ${user.WalletBalance:F2}");
        Pause();
    }

    private void ViewOrderHistory(int userId)
    {
        Console.Clear();
        Console.WriteLine("== Order History ==");
        Console.WriteLine();

        var orders = _orderService.GetUserOrders(userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToList();

        if (!orders.Any())
        {
            Console.WriteLine("No orders found.");
            Pause();
            return;
        }

        foreach (var order in orders)
        {
            Console.WriteLine(
                $"Order #{order.Id} | {order.CreatedAt:yyyy-MM-dd HH:mm} | Status: {order.Status} | Total: ${order.TotalAmount:F2}");
        }

        Pause();
    }

    private void TrackOrders(int userId)
    {
        Console.Clear();
        Console.WriteLine("== Track Orders ==");
        Console.WriteLine();

        var orders = _orderService.GetUserOrders(userId).ToList();
        if (!orders.Any())
        {
            Console.WriteLine("No orders found.");
            Pause();
            return;
        }

        for (var i = 0; i < orders.Count; i++)
        {
            var order = orders[i];
            Console.WriteLine($"{i + 1}. Order #{order.Id} | Status: {order.Status} | Date: {order.CreatedAt:yyyy-MM-dd HH:mm}");
        }

        Console.WriteLine();
        Console.WriteLine("0. Back");
        var choice = InputReader.ReadInt("Select order number to track: ");
        if (choice == 0)
        {
            return;
        }

        if (choice < 1 || choice > orders.Count)
        {
            Console.WriteLine("Invalid selection.");
            Pause();
            return;
        }

        var selected = orders[choice - 1];
        Console.WriteLine();
        Console.WriteLine($"Order ID: {selected.Id}");
        Console.WriteLine($"Status: {selected.Status}");
        Console.WriteLine($"Placed On: {selected.CreatedAt:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"Items: {selected.Items.Count}");
        Console.WriteLine($"Total Amount: ${selected.TotalAmount:F2}");
        Pause();
    }

    private void AddProductReview(int userId)
    {
        Console.Clear();
        Console.WriteLine("== Add Product Review ==");
        Console.WriteLine();

        var products = _productService.GetAllProducts().ToList();
        if (!products.Any())
        {
            Console.WriteLine("No products available.");
            Pause();
            return;
        }

        for (var i = 0; i < products.Count; i++)
        {
            var p = products[i];
            Console.WriteLine($"{i + 1}. {p.Name} ({p.Category})");
        }

        Console.WriteLine();
        Console.WriteLine("0. Cancel");
        var choice = InputReader.ReadInt("Select product number to review: ");
        if (choice == 0)
        {
            return;
        }

        if (choice < 1 || choice > products.Count)
        {
            Console.WriteLine("Invalid selection.");
            Pause();
            return;
        }

        var product = products[choice - 1];
        var rating = InputReader.ReadInt("Rating (1-5): ");
        var comment = InputReader.ReadRequired("Comment: ");

        try
        {
            _reviewService.AddReview(userId, product.Id, rating, comment);
            Console.WriteLine("Review submitted successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Could not submit review: {ex.Message}");
        }

        Pause();
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
            Pause();
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
            Pause();
            return;
        }

        var selectedProduct = products[choice - 1];
        var productReviews = _reviewService.GetReviewsByProductId(selectedProduct.Id).ToList();

        Console.Clear();
        Console.WriteLine($"== Reviews for {selectedProduct.Name} ==");
        Console.WriteLine();

        if (!productReviews.Any())
        {
            Console.WriteLine("No reviews yet. Be the first to review this product!");
            Pause();
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

        Pause();
    }

    private static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press Enter to continue.");
        Console.ReadLine();
    }
}
