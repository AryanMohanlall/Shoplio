using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories;
using Shoplio.ConsoleApp.Services;

namespace Shoplio.Console.Tests.Services;

public sealed class ProductServiceTests
{
    [Fact]
    public void SearchProducts_BlankQuery_ReturnsAll()
    {
        var repo = new InMemoryProductRepository();
        var sut = new ProductService(repo);
        sut.AddProduct("Laptop", "Electronics", 1000m, 10);
        sut.AddProduct("Desk", "Furniture", 250m, 4);

        var results = sut.SearchProducts("   ");

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void SearchProducts_MatchesNameAndCategory_CaseInsensitive()
    {
        var repo = new InMemoryProductRepository();
        var sut = new ProductService(repo);
        sut.AddProduct("Laptop", "Electronics", 1000m, 10);
        sut.AddProduct("Desk", "Furniture", 250m, 4);

        var byName = sut.SearchProducts("lap");
        var byCategory = sut.SearchProducts("FURN");

        Assert.Single(byName);
        Assert.Equal("Laptop", byName[0].Name);
        Assert.Single(byCategory);
        Assert.Equal("Desk", byCategory[0].Name);
    }

    [Fact]
    public void AddProduct_TrimsValues_AndPersists()
    {
        var repo = new InMemoryProductRepository();
        var sut = new ProductService(repo);

        var product = sut.AddProduct("  Mouse  ", "  Electronics  ", 25m, 9);

        Assert.Equal(1, product.Id);
        Assert.Equal("Mouse", product.Name);
        Assert.Equal("Electronics", product.Category);
        Assert.Equal(25m, product.Price);
    }

    [Fact]
    public void UpdateProduct_WhenProductNotFound_ReturnsFalse()
    {
        var repo = new InMemoryProductRepository();
        var sut = new ProductService(repo);

        var result = sut.UpdateProduct(new Product
        {
            Id = 999,
            Name = "Missing",
            Category = "Category",
            Price = 10m,
            Stock = 1
        });

        Assert.False(result);
    }

    [Fact]
    public void RestockProduct_ExistingProduct_IncreasesStock()
    {
        var repo = new InMemoryProductRepository();
        var sut = new ProductService(repo);
        var product = sut.AddProduct("Keyboard", "Electronics", 75m, 3);

        var success = sut.RestockProduct(product.Id, 7);

        Assert.True(success);
        Assert.Equal(10, repo.GetById(product.Id)!.Stock);
    }

    [Fact]
    public void GetLowStockProducts_ReturnsSortedProductsWithinThreshold()
    {
        var repo = new InMemoryProductRepository();
        var sut = new ProductService(repo);
        sut.AddProduct("A", "Cat", 1m, 5);
        sut.AddProduct("B", "Cat", 1m, 2);
        sut.AddProduct("C", "Cat", 1m, 7);

        var lowStock = sut.GetLowStockProducts(5);

        Assert.Equal(2, lowStock.Count);
        Assert.Equal("B", lowStock[0].Name);
        Assert.Equal("A", lowStock[1].Name);
    }
}
