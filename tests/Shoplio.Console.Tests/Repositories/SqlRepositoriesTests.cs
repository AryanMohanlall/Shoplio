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
}
