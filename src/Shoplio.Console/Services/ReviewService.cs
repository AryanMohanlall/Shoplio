using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class ReviewService(IReviewRepository reviewRepository) : IReviewService
{
    private readonly IReviewRepository _reviewRepository = reviewRepository;

    public Review AddReview(int userId, int productId, int rating, string comment)
    {
        if (rating is < 1 or > 5)
        {
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(rating));
        }

        var review = new Review
        {
            UserId = userId,
            ProductId = productId,
            Rating = rating,
            Comment = comment?.Trim() ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        _reviewRepository.Add(review);
        return review;
    }

    public IReadOnlyList<Review> GetReviewsByProductId(int productId)
    {
        return _reviewRepository.GetByProductId(productId);
    }
}