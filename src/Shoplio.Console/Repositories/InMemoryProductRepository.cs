using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;

namespace Shoplio.ConsoleApp.Repositories;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;

    public void Add(Product product)
    {
        product.Id = _nextId++;
        _products.Add(product);
    }

    public void Update(Product product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index >= 0)
        {
            _products[index] = product;
        }
    }

    public bool Delete(int productId)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId);
        if (product is null) return false;
        _products.Remove(product);
        return true;
    }

    public Product? GetById(int productId) =>
        _products.FirstOrDefault(p => p.Id == productId);

    public IReadOnlyList<Product> GetAll() => _products.AsReadOnly();
}
