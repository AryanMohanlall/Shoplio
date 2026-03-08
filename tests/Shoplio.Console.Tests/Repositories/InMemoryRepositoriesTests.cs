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
}
