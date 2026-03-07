using Shoplio.ConsoleApp.Models;

namespace Shoplio.ConsoleApp.Repositories.Interfaces;

public interface IProductRepository
{
    void Add(Product product);
    void Update(Product product);
    bool Delete(int productId);
    Product? GetById(int productId);
    IReadOnlyList<Product> GetAll();
}
