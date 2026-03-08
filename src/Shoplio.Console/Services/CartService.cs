using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class CartService(IProductRepository productRepository) : ICartService
{
    private readonly Dictionary<int, List<CartItem>> _carts = [];
    private readonly IProductRepository _productRepository = productRepository;

    public void AddToCart(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }

        var product = _productRepository.GetById(productId)
                      ?? throw new InvalidOperationException("Product not found.");

        var cart = GetOrCreateCart(userId);
        var existing = cart.FirstOrDefault(c => c.ProductId == productId);
        var existingQty = existing?.Quantity ?? 0;
        var newQty = existingQty + quantity;

        if (newQty > product.Stock)
        {
            throw new InvalidOperationException($"Only {product.Stock} unit(s) available in stock.");
        }

        if (existing is null)
        {
            cart.Add(new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price
            });
            return;
        }

        existing.Quantity = newQty;
        existing.UnitPrice = product.Price;
    }

    public bool UpdateQuantity(int userId, int productId, int quantity)
    {
        if (!_carts.TryGetValue(userId, out var cart))
        {
            return false;
        }

        var item = cart.FirstOrDefault(c => c.ProductId == productId);
        if (item is null)
        {
            return false;
        }

        if (quantity <= 0)
        {
            cart.Remove(item);
            return true;
        }

        var product = _productRepository.GetById(productId);
        if (product is null || quantity > product.Stock)
        {
            return false;
        }

        item.Quantity = quantity;
        item.UnitPrice = product.Price;
        return true;
    }

    public bool RemoveFromCart(int userId, int productId)
    {
        if (!_carts.TryGetValue(userId, out var cart))
        {
            return false;
        }

        var item = cart.FirstOrDefault(c => c.ProductId == productId);
        if (item is null)
        {
            return false;
        }

        cart.Remove(item);
        return true;
    }

    public IReadOnlyList<CartItem> GetCartItems(int userId)
    {
        if (!_carts.TryGetValue(userId, out var cart))
        {
            return [];
        }

        return cart
            .Select(c => new CartItem
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                UnitPrice = c.UnitPrice
            })
            .ToList();
    }

    public void ClearCart(int userId)
    {
        _carts.Remove(userId);
    }

    private List<CartItem> GetOrCreateCart(int userId)
    {
        if (_carts.TryGetValue(userId, out var cart))
        {
            return cart;
        }

        cart = [];
        _carts[userId] = cart;
        return cart;
    }
}