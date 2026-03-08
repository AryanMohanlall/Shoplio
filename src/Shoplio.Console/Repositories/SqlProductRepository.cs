using Microsoft.EntityFrameworkCore;
using Shoplio.ConsoleApp.Data;
using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;

namespace Shoplio.ConsoleApp.Repositories;

public sealed class SqlProductRepository : IProductRepository
{
    private readonly ShoplioDbContext _context;

    public SqlProductRepository(ShoplioDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Add(Product product)
    {
        if (product is null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public void Update(Product product)
    {
        if (product is null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        var existing = _context.Products.Find(product.Id);
        if (existing is null)
        {
            throw new InvalidOperationException($"Product with ID {product.Id} not found.");
        }

        existing.Name = product.Name;
        existing.Category = product.Category;
        existing.Price = product.Price;
        existing.Stock = product.Stock;
        
        _context.SaveChanges();
    }

    public bool Delete(int productId)
    {
        var product = _context.Products.Find(productId);
        if (product is null)
        {
            return false;
        }

        _context.Products.Remove(product);
        _context.SaveChanges();
        return true;
    }

    public Product? GetById(int productId)
    {
        return _context.Products.Find(productId);
    }

    public IReadOnlyList<Product> GetAll()
    {
        return _context.Products.ToList();
    }
}
