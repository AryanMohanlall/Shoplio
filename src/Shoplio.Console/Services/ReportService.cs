using Shoplio.ConsoleApp.Repositories.Interfaces;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class ReportService : IReportService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public ReportService(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public IEnumerable<string> GenerateSalesSummary()
    {
        var orders = _orderRepository.GetAll();

        if (!orders.Any())
        {
            yield return "No orders have been placed yet.";
            yield break;
        }

        var totalRevenue = orders.Sum(o => o.TotalAmount);
        var totalOrders = orders.Count;
        var paidOrders = orders.Count(o => o.Status == Models.OrderStatus.Paid);

        yield return $"Total Orders   : {totalOrders}";
        yield return $"Paid Orders    : {paidOrders}";
        yield return $"Total Revenue  : {totalRevenue:C2}";

        var topProducts = orders
            .SelectMany(o => o.Items)
            .GroupBy(i => i.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Revenue = g.Sum(i => i.LineTotal),
                UnitsSold = g.Sum(i => i.Quantity)
            })
            .OrderByDescending(x => x.Revenue)
            .Take(5);

        yield return string.Empty;
        yield return "Top 5 Products by Revenue:";
        foreach (var entry in topProducts)
        {
            var product = _productRepository.GetById(entry.ProductId);
            var name = product?.Name ?? $"Product #{entry.ProductId}";
            yield return $"  {name,-30} Units Sold: {entry.UnitsSold,4}   Revenue: {entry.Revenue:C2}";
        }
    }

    public IEnumerable<string> GenerateLowStockReport(int threshold)
    {
        var lowStock = _productRepository.GetAll()
            .Where(p => p.Stock <= threshold)
            .OrderBy(p => p.Stock);

        if (!lowStock.Any())
        {
            yield return $"No products are at or below the threshold of {threshold}.";
            yield break;
        }

        yield return $"Products with stock <= {threshold}:";
        foreach (var product in lowStock)
        {
            yield return $"  [{product.Id}] {product.Name,-30} Stock: {product.Stock}";
        }
    }

    public IEnumerable<string> GenerateTopProductsReport(int takeCount)
    {
        var orders = _orderRepository.GetAll();

        if (!orders.Any())
        {
            yield return "No orders found.";
            yield break;
        }

        var top = orders
            .SelectMany(o => o.Items)
            .GroupBy(i => i.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                UnitsSold = g.Sum(i => i.Quantity)
            })
            .OrderByDescending(x => x.UnitsSold)
            .Take(takeCount);

        yield return $"Top {takeCount} Best-Selling Products:";
        var rank = 1;
        foreach (var entry in top)
        {
            var product = _productRepository.GetById(entry.ProductId);
            var name = product?.Name ?? $"Product #{entry.ProductId}";
            yield return $"  {rank++,2}. {name,-30} Units Sold: {entry.UnitsSold}";
        }
    }
}
