using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class CartService : ICartService
{
    private readonly IProductRepository _productRepository;
    private readonly Dictionary<int, List<CartItem>> _carts = new();

    public CartService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public void AddToCart(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var product = _productRepository.GetById(productId)
            ?? throw new InvalidOperationException("Product not found.");

        if (product.Stock < quantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {product.Stock}.");

        var cart = GetOrCreateCart(userId);
        var existing = cart.FirstOrDefault(i => i.ProductId == productId);

        if (existing is not null)
        {
            var newQty = existing.Quantity + quantity;
            if (product.Stock < newQty)
                throw new InvalidOperationException($"Insufficient stock. Available: {product.Stock}.");
            existing.Quantity = newQty;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price
            });
        }
    }

    public bool UpdateQuantity(int userId, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var product = _productRepository.GetById(productId);
        if (product is null) return false;

        if (product.Stock < quantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {product.Stock}.");

        var cart = GetOrCreateCart(userId);
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item is null) return false;

        item.Quantity = quantity;
        return true;
    }

    public bool RemoveFromCart(int userId, int productId)
    {
        var cart = GetOrCreateCart(userId);
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item is null) return false;

        cart.Remove(item);
        return true;
    }

    public IReadOnlyList<CartItem> GetCartItems(int userId) =>
        GetOrCreateCart(userId).AsReadOnly();

    public void ClearCart(int userId)
    {
        if (_carts.ContainsKey(userId))
            _carts[userId].Clear();
    }

    private List<CartItem> GetOrCreateCart(int userId)
    {
        if (!_carts.TryGetValue(userId, out var cart))
        {
            cart = new List<CartItem>();
            _carts[userId] = cart;
        }

        return cart;
    }
}
