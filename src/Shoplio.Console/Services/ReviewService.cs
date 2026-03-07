using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IProductRepository _productRepository;

    public ReviewService(IReviewRepository reviewRepository, IProductRepository productRepository)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
    }

    public Review AddReview(int userId, int productId, int rating, string comment)
    {
        if (_productRepository.GetById(productId) is null)
            throw new InvalidOperationException("Product not found.");

        if (rating < 1 || rating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.");

        var review = new Review
        {
            UserId = userId,
            ProductId = productId,
            Rating = rating,
            Comment = comment?.Trim() ?? string.Empty
        };

        _reviewRepository.Add(review);
        return review;
    }

    public IReadOnlyList<Review> GetProductReviews(int productId) =>
        _reviewRepository.GetByProductId(productId);

    public double GetAverageRating(int productId)
    {
        var reviews = _reviewRepository.GetByProductId(productId);
        if (!reviews.Any()) return 0;
        return reviews.Average(r => r.Rating);
    }
}
