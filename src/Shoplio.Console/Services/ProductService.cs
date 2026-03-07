using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public IReadOnlyList<Product> GetAllProducts() =>
        _productRepository.GetAll();

    public IReadOnlyList<Product> SearchProducts(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return _productRepository.GetAll();

        var lower = query.Trim().ToLowerInvariant();
        return _productRepository.GetAll()
            .Where(p => p.Name.ToLowerInvariant().Contains(lower)
                     || p.Category.ToLowerInvariant().Contains(lower))
            .ToList()
            .AsReadOnly();
    }

    public Product AddProduct(string name, string category, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category is required.");
        if (price < 0)
            throw new ArgumentException("Price must be non-negative.");
        if (stock < 0)
            throw new ArgumentException("Stock must be non-negative.");

        var product = new Product
        {
            Name = name.Trim(),
            Category = category.Trim(),
            Price = price,
            Stock = stock
        };

        _productRepository.Add(product);
        return product;
    }

    public bool UpdateProduct(Product product)
    {
        if (_productRepository.GetById(product.Id) is null)
            return false;

        _productRepository.Update(product);
        return true;
    }

    public bool DeleteProduct(int productId)
    {
        return _productRepository.Delete(productId);
    }
}
