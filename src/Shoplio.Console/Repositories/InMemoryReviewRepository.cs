using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;

namespace Shoplio.ConsoleApp.Repositories;

public sealed class InMemoryReviewRepository : IReviewRepository
{
    private readonly List<Review> _reviews = new();
    private int _nextId = 1;

    public void Add(Review review)
    {
        review.Id = _nextId++;
        _reviews.Add(review);
    }

    public IReadOnlyList<Review> GetByProductId(int productId) =>
        _reviews.Where(r => r.ProductId == productId).ToList().AsReadOnly();

    public IReadOnlyList<Review> GetAll() => _reviews.AsReadOnly();
}
