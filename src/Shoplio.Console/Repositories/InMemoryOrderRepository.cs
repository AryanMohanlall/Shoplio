using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;

namespace Shoplio.ConsoleApp.Repositories;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = [];
    private int _nextId = 1;

    public void Add(Order order)
    {
        if (order is null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        order.Id = _nextId++;
        _orders.Add(order);
    }

    public Order? GetById(int orderId)
    {
        return _orders.FirstOrDefault(o => o.Id == orderId);
    }

    public IReadOnlyList<Order> GetByUserId(int userId)
    {
        return _orders.Where(o => o.UserId == userId).ToList();
    }

    public IReadOnlyList<Order> GetAll()
    {
        return _orders;
    }
}
