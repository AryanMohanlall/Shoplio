using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class ReviewService : IReviewService
{
    private readonly List<Review> _reviews = [];
    private int _nextId = 1;

    public Review AddReview(int userId, int productId, int rating, string comment)
    {
        if (rating is < 1 or > 5)
        {
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));
        }

        var review = new Review
        {
            Id = _nextId++,
            UserId = userId,
            ProductId = productId,
            Rating = rating,
            Comment = comment?.Trim() ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        _reviews.Add(review);
        return review;
    }

    public IReadOnlyList<Review> GetReviewsByProductId(int productId)
    {
        return _reviews
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }
}