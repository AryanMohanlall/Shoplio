using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class ProductService(IProductRepository productRepository) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;

    public IReadOnlyList<Product> GetAllProducts()
    {
        return _productRepository.GetAll();
    }

    public IReadOnlyList<Product> SearchProducts(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAllProducts();
        }

        var normalizedQuery = query.Trim().ToLowerInvariant();
        return _productRepository.GetAll()
            .Where(p => p.Name.ToLowerInvariant().Contains(normalizedQuery) ||
                       p.Category.ToLowerInvariant().Contains(normalizedQuery))
            .ToList();
    }

    public Product AddProduct(string name, string category, decimal price, int stock)
    {
        var normalizedName = name?.Trim() ?? string.Empty;
        var normalizedCategory = category?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ArgumentException("Product name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(normalizedCategory))
        {
            throw new ArgumentException("Product category is required.", nameof(category));
        }

        if (price <= 0)
        {
            throw new ArgumentException("Price must be greater than zero.", nameof(price));
        }

        if (stock < 0)
        {
            throw new ArgumentException("Stock cannot be negative.", nameof(stock));
        }

        var product = new Product
        {
            Name = normalizedName,
            Category = normalizedCategory,
            Price = price,
            Stock = stock
        };

        _productRepository.Add(product);
        return product;
    }

    public bool UpdateProduct(Product product)
    {
        if (product is null)
        {
            throw new ArgumentNullException(nameof(product));
        }

        if (string.IsNullOrWhiteSpace(product.Name))
        {
            throw new ArgumentException("Product name is required.");
        }

        if (string.IsNullOrWhiteSpace(product.Category))
        {
            throw new ArgumentException("Product category is required.");
        }

        if (product.Price <= 0)
        {
            throw new ArgumentException("Price must be greater than zero.");
        }

        if (product.Stock < 0)
        {
            throw new ArgumentException("Stock cannot be negative.");
        }

        try
        {
            _productRepository.Update(product);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    public bool DeleteProduct(int productId)
    {
        return _productRepository.Delete(productId);
    }

    public bool RestockProduct(int productId, int additionalStock)
    {
        if (additionalStock <= 0)
        {
            throw new ArgumentException("Additional stock must be greater than zero.", nameof(additionalStock));
        }

        var product = _productRepository.GetById(productId);
        if (product is null)
        {
            return false;
        }

        product.Stock += additionalStock;
        _productRepository.Update(product);
        return true;
    }

    public IReadOnlyList<Product> GetLowStockProducts(int threshold)
    {
        return _productRepository.GetAll()
            .Where(p => p.Stock <= threshold)
            .OrderBy(p => p.Stock)
            .ToList();
    }
}
