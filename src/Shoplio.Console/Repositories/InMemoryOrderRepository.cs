using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;

namespace Shoplio.ConsoleApp.Repositories;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new();
    private int _nextId = 1;

    public void Add(Order order)
    {
        order.Id = _nextId++;
        _orders.Add(order);
    }

    public Order? GetById(int orderId) =>
        _orders.FirstOrDefault(o => o.Id == orderId);

    public IReadOnlyList<Order> GetByUserId(int userId) =>
        _orders.Where(o => o.UserId == userId).ToList().AsReadOnly();

    public IReadOnlyList<Order> GetAll() => _orders.AsReadOnly();
}
