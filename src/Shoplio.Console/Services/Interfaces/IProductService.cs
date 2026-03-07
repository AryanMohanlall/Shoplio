using Shoplio.ConsoleApp.Models;

namespace Shoplio.ConsoleApp.Services.Interfaces;

public interface IProductService
{
    IReadOnlyList<Product> GetAllProducts();
    IReadOnlyList<Product> SearchProducts(string query);
    Product AddProduct(string name, string category, decimal price, int stock);
    bool UpdateProduct(Product product);
    bool DeleteProduct(int productId);
}
