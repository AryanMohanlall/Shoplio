using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class OrderService(
    IOrderRepository orderRepository,
    ICartService cartService,
    IProductRepository productRepository,
    IUserRepository userRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ICartService _cartService = cartService;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IUserRepository _userRepository = userRepository;

    public Order PlaceOrder(int userId)
    {
        var user = _userRepository.GetById(userId)
                   ?? throw new InvalidOperationException("User not found.");

        var cartItems = _cartService.GetCartItems(userId);
        if (cartItems.Count == 0)
        {
            throw new InvalidOperationException("Your cart is empty.");
        }

        foreach (var item in cartItems)
        {
            var product = _productRepository.GetById(item.ProductId)
                          ?? throw new InvalidOperationException("One or more products are unavailable.");

            if (item.Quantity > product.Stock)
            {
                throw new InvalidOperationException(
                    $"Insufficient stock for '{product.Name}'. Available: {product.Stock}.");
            }
        }

        var totalAmount = cartItems.Sum(i => i.LineTotal);
        if (user.WalletBalance < totalAmount)
        {
            throw new InvalidOperationException(
                $"Insufficient wallet balance. Required: {totalAmount:F2}, Available: {user.WalletBalance:F2}.");
        }

        foreach (var item in cartItems)
        {
            var product = _productRepository.GetById(item.ProductId)!;
            product.Stock -= item.Quantity;
            _productRepository.Update(product);
        }

        user.WalletBalance -= totalAmount;

        var order = new Order
        {
            UserId = userId,
            Items = cartItems
                .Select(i => new CartItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                })
                .ToList(),
            TotalAmount = totalAmount,
            Status = OrderStatus.Paid,
            CreatedAt = DateTime.UtcNow
        };

        _orderRepository.Add(order);
        _cartService.ClearCart(userId);
        return order;
    }

    public IReadOnlyList<Order> GetUserOrders(int userId)
    {
        return _orderRepository.GetByUserId(userId);
    }

    public IReadOnlyList<Order> GetAllOrders()
    {
        return _orderRepository.GetAll();
    }

    public Order? GetOrderById(int orderId)
    {
        return _orderRepository.GetById(orderId);
    }

    public bool UpdateOrderStatus(int orderId, OrderStatus newStatus)
    {
        var order = _orderRepository.GetById(orderId);
        if (order is null)
        {
            return false;
        }

        order.Status = newStatus;
        return true;
    }
}
