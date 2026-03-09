using Shoplio.ConsoleApp.Models;

namespace Shoplio.ConsoleApp.Repositories.Interfaces;

public interface IReviewRepository
{
    void Add(Review review);
    IReadOnlyList<Review> GetByProductId(int productId);
}
