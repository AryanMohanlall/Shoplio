using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;

namespace Shoplio.ConsoleApp.Repositories;

public sealed class InMemoryReviewRepository : IReviewRepository
{
    private readonly List<Review> _reviews = [];
    private int _nextId = 1;

    public void Add(Review review)
    {
        if (review is null)
        {
            throw new ArgumentNullException(nameof(review));
        }

        review.Id = _nextId++;
        _reviews.Add(review);
    }

    public IReadOnlyList<Review> GetByProductId(int productId)
    {
        return _reviews
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }
}
