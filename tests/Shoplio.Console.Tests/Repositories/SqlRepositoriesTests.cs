using Shoplio.Console.Tests.TestHelpers;
using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories;

namespace Shoplio.Console.Tests.Repositories;

public sealed class SqlRepositoriesTests
{
    [Fact]
    public void SqlUserRepository_AddAndGetByEmail_Works()
    {
        using var context = DbContextTestFactory.Create();
        var repo = new SqlUserRepository(context);

        repo.Add(new User
        {
            Name = "User",
            Email = "user@example.com",
            Password = "hash",
            Role = Role.Customer,
            WalletBalance = 100m
        });

        var found = repo.GetByEmail("USER@example.com");

        Assert.NotNull(found);
        Assert.Equal("User", found!.Name);
    }

    [Fact]
    public void SqlProductRepository_UpdateMissing_ThrowsInvalidOperationException()
    {
        using var context = DbContextTestFactory.Create();
        var repo = new SqlProductRepository(context);

        var action = () => repo.Update(new Product
        {
            Id = 777,
            Name = "Missing",
            Category = "Cat",
            Price = 10m,
            Stock = 1
        });

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void SqlOrderRepository_GetById_LoadsItemsAndProducts()
    {
        using var context = DbContextTestFactory.Create();
        var user = new User
        {
            Name = "Buyer",
            Email = "buyer@example.com",
            Password = "hash",
            Role = Role.Customer,
            WalletBalance = 100m
        };
        var product = new Product
        {
            Name = "Mouse",
            Category = "Electronics",
            Price = 25m,
            Stock = 10
        };
        context.Users.Add(user);
        context.Products.Add(product);
        context.SaveChanges();

        var order = new Order
        {
            UserId = user.Id,
            Status = OrderStatus.Paid,
            TotalAmount = 50m,
            Items =
            [
                new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = 2,
                    UnitPrice = 25m
                }
            ]
        };

        var repo = new SqlOrderRepository(context);
        repo.Add(order);

        var loaded = repo.GetById(order.Id);

        Assert.NotNull(loaded);
        Assert.Single(loaded!.Items);
        Assert.Equal("Mouse", loaded.Items[0].Product.Name);
        Assert.Equal(50m, loaded.Items[0].LineTotal);
    }

    [Fact]
    public void SqlReviewRepository_AddAndGetByProductId_Works()
    {
        using var context = DbContextTestFactory.Create();

        var user = new User
        {
            Name = "Reviewer",
            Email = "reviewer@example.com",
            Password = "hash",
            Role = Role.Customer,
            WalletBalance = 0m
        };
        var product = new Product
        {
            Name = "Keyboard",
            Category = "Electronics",
            Price = 79m,
            Stock = 20
        };
        context.Users.Add(user);
        context.Products.Add(product);
        context.SaveChanges();

        var repo = new SqlReviewRepository(context);
        repo.Add(new Review
        {
            UserId = user.Id,
            ProductId = product.Id,
            Rating = 5,
            Comment = "Excellent",
            CreatedAt = DateTime.UtcNow
        });

        var reviews = repo.GetByProductId(product.Id);

        Assert.Single(reviews);
        Assert.Equal("Excellent", reviews[0].Comment);
        Assert.Equal(5, reviews[0].Rating);
    }

    [Fact]
    public void SqlReviewRepository_GetByProductId_ReturnsNewestFirstAndFilters()
    {
        using var context = DbContextTestFactory.Create();

        var user = new User
        {
            Name = "Reviewer",
            Email = "reviewer2@example.com",
            Password = "hash",
            Role = Role.Customer,
            WalletBalance = 0m
        };
        var productA = new Product { Name = "A", Category = "Cat", Price = 10m, Stock = 10 };
        var productB = new Product { Name = "B", Category = "Cat", Price = 20m, Stock = 10 };
        context.Users.Add(user);
        context.Products.AddRange(productA, productB);
        context.SaveChanges();

        var repo = new SqlReviewRepository(context);
        repo.Add(new Review
        {
            UserId = user.Id,
            ProductId = productA.Id,
            Rating = 3,
            Comment = "older",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5)
        });
        repo.Add(new Review
        {
            UserId = user.Id,
            ProductId = productA.Id,
            Rating = 4,
            Comment = "newer",
            CreatedAt = DateTime.UtcNow
        });
        repo.Add(new Review
        {
            UserId = user.Id,
            ProductId = productB.Id,
            Rating = 5,
            Comment = "other product",
            CreatedAt = DateTime.UtcNow
        });

        var reviews = repo.GetByProductId(productA.Id);

        Assert.Equal(2, reviews.Count);
        Assert.Equal("newer", reviews[0].Comment);
        Assert.Equal("older", reviews[1].Comment);
    }
}
