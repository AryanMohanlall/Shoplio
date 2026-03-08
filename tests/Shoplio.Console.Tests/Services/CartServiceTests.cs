using Shoplio.ConsoleApp.Repositories;
using Shoplio.ConsoleApp.Services;

namespace Shoplio.Console.Tests.Services;

public sealed class CartServiceTests
{
    [Fact]
    public void AddToCart_UnknownProduct_ThrowsInvalidOperationException()
    {
        var productRepo = new InMemoryProductRepository();
        var sut = new CartService(productRepo);

        var action = () => sut.AddToCart(1, 10, 1);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void AddToCart_SameProduct_CombinesQuantity()
    {
        var productRepo = new InMemoryProductRepository();
        var product = new ProductService(productRepo).AddProduct("Mouse", "Electronics", 10m, 10);
        var sut = new CartService(productRepo);

        sut.AddToCart(1, product.Id, 2);
        sut.AddToCart(1, product.Id, 3);

        var items = sut.GetCartItems(1);
        Assert.Single(items);
        Assert.Equal(5, items[0].Quantity);
    }

    [Fact]
    public void AddToCart_ExceedingStock_ThrowsInvalidOperationException()
    {
        var productRepo = new InMemoryProductRepository();
        var product = new ProductService(productRepo).AddProduct("Mouse", "Electronics", 10m, 4);
        var sut = new CartService(productRepo);
        sut.AddToCart(1, product.Id, 3);

        var action = () => sut.AddToCart(1, product.Id, 2);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void UpdateQuantity_ZeroOrLess_RemovesItem()
    {
        var productRepo = new InMemoryProductRepository();
        var product = new ProductService(productRepo).AddProduct("Mouse", "Electronics", 10m, 4);
        var sut = new CartService(productRepo);
        sut.AddToCart(1, product.Id, 1);

        var success = sut.UpdateQuantity(1, product.Id, 0);

        Assert.True(success);
        Assert.Empty(sut.GetCartItems(1));
    }

    [Fact]
    public void UpdateQuantity_TooHigh_ReturnsFalse()
    {
        var productRepo = new InMemoryProductRepository();
        var product = new ProductService(productRepo).AddProduct("Mouse", "Electronics", 10m, 4);
        var sut = new CartService(productRepo);
        sut.AddToCart(1, product.Id, 1);

        var success = sut.UpdateQuantity(1, product.Id, 10);

        Assert.False(success);
    }

    [Fact]
    public void GetCartItems_ReturnsCopy_NotMutableInternalState()
    {
        var productRepo = new InMemoryProductRepository();
        var product = new ProductService(productRepo).AddProduct("Mouse", "Electronics", 10m, 4);
        var sut = new CartService(productRepo);
        sut.AddToCart(1, product.Id, 1);

        var snapshot = sut.GetCartItems(1);
        snapshot[0].Quantity = 99;

        var current = sut.GetCartItems(1);
        Assert.Equal(1, current[0].Quantity);
    }
}
