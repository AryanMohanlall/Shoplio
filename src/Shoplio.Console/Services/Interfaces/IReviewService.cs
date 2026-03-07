using Shoplio.ConsoleApp.Models;

namespace Shoplio.ConsoleApp.Services.Interfaces;

public interface IReviewService
{
    Review AddReview(int userId, int productId, int rating, string comment);
    IReadOnlyList<Review> GetProductReviews(int productId);
    double GetAverageRating(int productId);
}
