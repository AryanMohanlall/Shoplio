using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories;
using Shoplio.ConsoleApp.Services;

namespace Shoplio.Console.Tests.Services;

public sealed class ReportServiceTests
{
    [Fact]
    public void GenerateSalesSummary_NoOrders_ReturnsNoOrdersMessage()
    {
        var sut = new ReportService(new InMemoryOrderRepository(), new InMemoryProductRepository());

        var lines = sut.GenerateSalesSummary().ToList();

        Assert.Single(lines);
        Assert.Equal("No orders found.", lines[0]);
    }

    [Fact]
    public void GenerateSalesSummary_WithOrders_ReturnsTotalsAndStatuses()
    {
        var orderRepo = new InMemoryOrderRepository();
        var productRepo = new InMemoryProductRepository();
        orderRepo.Add(new Order { UserId = 1, TotalAmount = 50m, Status = OrderStatus.Paid });
        orderRepo.Add(new Order { UserId = 1, TotalAmount = 100m, Status = OrderStatus.Cancelled });
        orderRepo.Add(new Order { UserId = 1, TotalAmount = 150m, Status = OrderStatus.Cancelled });
        var sut = new ReportService(orderRepo, productRepo);

        var text = string.Join('\n', sut.GenerateSalesSummary());

        Assert.Contains("Total Orders: 3", text);
        Assert.Contains("Total Revenue: $300", text);
        Assert.Contains("Average Order Value: $100", text);
        Assert.Contains("Paid: 1", text);
        Assert.Contains("Cancelled: 2", text);
    }

    [Fact]
    public void GenerateLowStockReport_NoMatches_ReturnsNoProductsMessage()
    {
        var orderRepo = new InMemoryOrderRepository();
        var productRepo = new InMemoryProductRepository();
        productRepo.Add(new Product { Name = "A", Category = "C", Price = 1m, Stock = 10 });
        var sut = new ReportService(orderRepo, productRepo);

        var lines = sut.GenerateLowStockReport(2).ToList();

        Assert.Single(lines);
        Assert.Equal("No products with stock <= 2.", lines[0]);
    }

    [Fact]
    public void GenerateTopProductsReport_WithoutOrders_ReturnsNoOrderDataMessage()
    {
        var sut = new ReportService(new InMemoryOrderRepository(), new InMemoryProductRepository());

        var lines = sut.GenerateTopProductsReport(3).ToList();

        Assert.Single(lines);
        Assert.Equal("No order data available.", lines[0]);
    }

    [Fact]
    public void GenerateTopProductsReport_WithNoOrderItems_ReturnsNoProductSalesDataMessage()
    {
        var orderRepo = new InMemoryOrderRepository();
        orderRepo.Add(new Order { UserId = 1, TotalAmount = 10m, Status = OrderStatus.Paid });
        var sut = new ReportService(orderRepo, new InMemoryProductRepository());

        var lines = sut.GenerateTopProductsReport(3).ToList();

        Assert.Single(lines);
        Assert.Equal("No product sales data available.", lines[0]);
    }

    [Fact]
    public void GenerateTopProductsReport_UsesUnknownProductFallback()
    {
        var orderRepo = new InMemoryOrderRepository();
        var productRepo = new InMemoryProductRepository();
        var known = new Product { Name = "Known", Category = "Cat", Price = 10m, Stock = 10 };
        productRepo.Add(known);

        orderRepo.Add(new Order
        {
            UserId = 1,
            Status = OrderStatus.Paid,
            Items =
            [
                new OrderItem { ProductId = known.Id, Quantity = 2, UnitPrice = 10m },
                new OrderItem { ProductId = 999, Quantity = 1, UnitPrice = 30m }
            ],
            TotalAmount = 50m
        });

        var sut = new ReportService(orderRepo, productRepo);

        var text = string.Join('\n', sut.GenerateTopProductsReport(5));

        Assert.Contains("Known", text);
        Assert.Contains("Unknown Product", text);
    }
}
