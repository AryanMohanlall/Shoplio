using Shoplio.ConsoleApp.Models;

namespace Shoplio.ConsoleApp.Services.Interfaces;

public interface ICartService
{
    void AddToCart(int userId, int productId, int quantity);
    bool UpdateQuantity(int userId, int productId, int quantity);
    bool RemoveFromCart(int userId, int productId);
    IReadOnlyList<CartItem> GetCartItems(int userId);
    void ClearCart(int userId);
}
