using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories;

namespace Shoplio.Console.Tests.Repositories;

public sealed class InMemoryRepositoriesTests
{
    [Fact]
    public void InMemoryUserRepository_GetByEmail_IsCaseInsensitive()
    {
        var repo = new InMemoryUserRepository();
        repo.Add(new User { Name = "User", Email = "user@example.com", Password = "x" });

        var found = repo.GetByEmail("USER@example.com");

        Assert.NotNull(found);
        Assert.Equal("user@example.com", found!.Email);
    }

    [Fact]
    public void InMemoryProductRepository_UpdateMissing_ThrowsInvalidOperationException()
    {
        var repo = new InMemoryProductRepository();

        var action = () => repo.Update(new Product
        {
            Id = 123,
            Name = "Missing",
            Category = "Cat",
            Price = 10m,
            Stock = 1
        });

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void InMemoryProductRepository_DeleteMissing_ReturnsFalse()
    {
        var repo = new InMemoryProductRepository();

        var result = repo.Delete(404);

        Assert.False(result);
    }

    [Fact]
    public void InMemoryOrderRepository_GetByUserId_FiltersOrders()
    {
        var repo = new InMemoryOrderRepository();
        repo.Add(new Order { UserId = 1, TotalAmount = 10m });
        repo.Add(new Order { UserId = 2, TotalAmount = 20m });
        repo.Add(new Order { UserId = 1, TotalAmount = 30m });

        var userOrders = repo.GetByUserId(1);

        Assert.Equal(2, userOrders.Count);
        Assert.All(userOrders, o => Assert.Equal(1, o.UserId));
    }

    [Fact]
    public void InMemoryReviewRepository_Add_AssignsIncrementingIds()
    {
        var repo = new InMemoryReviewRepository();

        var first = new Review { UserId = 1, ProductId = 10, Rating = 4, Comment = "Good" };
        var second = new Review { UserId = 2, ProductId = 10, Rating = 5, Comment = "Great" };

        repo.Add(first);
        repo.Add(second);

        Assert.Equal(1, first.Id);
        Assert.Equal(2, second.Id);
    }

    [Fact]
    public void InMemoryReviewRepository_GetByProductId_FiltersAndSortsNewestFirst()
    {
        var repo = new InMemoryReviewRepository();

        var older = new Review
        {
            UserId = 1,
            ProductId = 5,
            Rating = 3,
            Comment = "older",
            CreatedAt = DateTime.UtcNow.AddHours(-1)
        };
        var newer = new Review
        {
            UserId = 2,
            ProductId = 5,
            Rating = 5,
            Comment = "newer",
            CreatedAt = DateTime.UtcNow
        };
        var otherProduct = new Review
        {
            UserId = 3,
            ProductId = 9,
            Rating = 4,
            Comment = "other"
        };

        repo.Add(older);
        repo.Add(newer);
        repo.Add(otherProduct);

        var reviews = repo.GetByProductId(5);

        Assert.Equal(2, reviews.Count);
        Assert.Equal("newer", reviews[0].Comment);
        Assert.Equal("older", reviews[1].Comment);
    }
}
