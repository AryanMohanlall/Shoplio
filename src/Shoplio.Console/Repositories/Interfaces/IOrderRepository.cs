using Shoplio.ConsoleApp.Models;

namespace Shoplio.ConsoleApp.Repositories.Interfaces;

public interface IOrderRepository
{
    void Add(Order order);
    Order? GetById(int orderId);
    IReadOnlyList<Order> GetByUserId(int userId);
    IReadOnlyList<Order> GetAll();
}
