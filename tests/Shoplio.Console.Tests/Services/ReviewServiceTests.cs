using Shoplio.ConsoleApp.Services;

namespace Shoplio.Console.Tests.Services;

public sealed class ReviewServiceTests
{
    [Fact]
    public void AddReview_InvalidRating_ThrowsArgumentException()
    {
        var sut = new ReviewService();

        var action = () => sut.AddReview(1, 1, 0, "bad");

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void AddReview_AssignsIdAndTrimsComment()
    {
        var sut = new ReviewService();

        var review = sut.AddReview(2, 3, 5, "  Great product  ");

        Assert.Equal(1, review.Id);
        Assert.Equal(2, review.UserId);
        Assert.Equal(3, review.ProductId);
        Assert.Equal(5, review.Rating);
        Assert.Equal("Great product", review.Comment);
    }

    [Fact]
    public void GetReviewsByProductId_ReturnsNewestFirst()
    {
        var sut = new ReviewService();
        var first = sut.AddReview(1, 10, 4, "first");
        var second = sut.AddReview(2, 10, 5, "second");
        first.CreatedAt = DateTime.UtcNow.AddMinutes(-10);
        second.CreatedAt = DateTime.UtcNow;

        var reviews = sut.GetReviewsByProductId(10);

        Assert.Equal(2, reviews.Count);
        Assert.Equal(second.Id, reviews[0].Id);
        Assert.Equal(first.Id, reviews[1].Id);
    }
}
