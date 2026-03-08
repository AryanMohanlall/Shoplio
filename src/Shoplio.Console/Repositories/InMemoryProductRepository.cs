using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;

namespace Shoplio.ConsoleApp.Repositories;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = [];
    private int _nextId = 1;

    public void Add(Product product)
    {
        if (product is null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        product.Id = _nextId++;
        _products.Add(product);
    }

    public void Update(Product product)
    {
        if (product is null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        var existing = _products.FirstOrDefault(p => p.Id == product.Id);
        if (existing is null)
        {
            throw new InvalidOperationException($"Product with ID {product.Id} not found.");
        }

        existing.Name = product.Name;
        existing.Category = product.Category;
        existing.Price = product.Price;
        existing.Stock = product.Stock;
    }

    public bool Delete(int productId)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId);
        if (product is null)
        {
            return false;
        }

        _products.Remove(product);
        return true;
    }

    public Product? GetById(int productId)
    {
        return _products.FirstOrDefault(p => p.Id == productId);
    }

    public IReadOnlyList<Product> GetAll()
    {
        return _products;
    }
}
