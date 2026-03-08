using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class OrderService(IOrderRepository orderRepository) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;

    public Order PlaceOrder(int userId)
    {
        // Placeholder implementation for now - actual cart integration will come later
        throw new NotImplementedException("Order placement will be implemented with cart functionality.");
    }

    public IReadOnlyList<Order> GetUserOrders(int userId)
    {
        return _orderRepository.GetByUserId(userId);
    }

    public IReadOnlyList<Order> GetAllOrders()
    {
        return _orderRepository.GetAll();
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
