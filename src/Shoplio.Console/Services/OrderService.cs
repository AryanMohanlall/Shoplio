using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICartService _cartService;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IUserRepository userRepository,
        ICartService cartService)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _cartService = cartService;
    }

    public Order PlaceOrder(int userId)
    {
        var user = _userRepository.GetById(userId)
            ?? throw new InvalidOperationException("User not found.");

        var cartItems = _cartService.GetCartItems(userId);
        if (cartItems.Count == 0)
            throw new InvalidOperationException("Your cart is empty.");

        // Validate stock and gather snapshot items
        var orderItems = new List<CartItem>();
        foreach (var item in cartItems)
        {
            var product = _productRepository.GetById(item.ProductId)
                ?? throw new InvalidOperationException($"Product {item.ProductId} no longer exists.");

            if (product.Stock < item.Quantity)
                throw new InvalidOperationException(
                    $"Insufficient stock for '{product.Name}'. Available: {product.Stock}.");

            orderItems.Add(new CartItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        var total = orderItems.Sum(i => i.LineTotal);

        if (user.WalletBalance < total)
            throw new InvalidOperationException(
                $"Insufficient wallet balance. Required: {total:C2}, Available: {user.WalletBalance:C2}.");

        // Deduct stock
        foreach (var item in orderItems)
        {
            var product = _productRepository.GetById(item.ProductId)!;
            product.Stock -= item.Quantity;
            _productRepository.Update(product);
        }

        // Deduct wallet
        user.WalletBalance -= total;

        var order = new Order
        {
            UserId = userId,
            Items = orderItems,
            TotalAmount = total,
            Status = OrderStatus.Paid
        };

        _orderRepository.Add(order);
        _cartService.ClearCart(userId);

        return order;
    }

    public IReadOnlyList<Order> GetUserOrders(int userId) =>
        _orderRepository.GetByUserId(userId);

    public IReadOnlyList<Order> GetAllOrders() =>
        _orderRepository.GetAll();
}
