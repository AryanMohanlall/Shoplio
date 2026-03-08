using Microsoft.EntityFrameworkCore;
using Shoplio.ConsoleApp.Data;
using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;

namespace Shoplio.ConsoleApp.Repositories;

public sealed class SqlOrderRepository : IOrderRepository
{
    private readonly ShoplioDbContext _context;

    public SqlOrderRepository(ShoplioDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Add(Order order)
    {
        if (order is null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        _context.Orders.Add(order);
        _context.SaveChanges();
    }

    public Order? GetById(int orderId)
    {
        return _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefault(o => o.Id == orderId);
    }

    public IReadOnlyList<Order> GetByUserId(int userId)
    {
        return _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .ToList();
    }

    public IReadOnlyList<Order> GetAll()
    {
        return _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .ToList();
    }
}
