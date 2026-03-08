using Shoplio.ConsoleApp.Models;

namespace Shoplio.ConsoleApp.Services.Interfaces;

public interface IOrderService
{
    Order PlaceOrder(int userId);
    IReadOnlyList<Order> GetUserOrders(int userId);
    IReadOnlyList<Order> GetAllOrders();
    Order? GetOrderById(int orderId);
    bool UpdateOrderStatus(int orderId, OrderStatus newStatus);
}
