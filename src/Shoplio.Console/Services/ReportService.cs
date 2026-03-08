using Shoplio.ConsoleApp.Repositories.Interfaces;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class ReportService(IOrderRepository orderRepository, IProductRepository productRepository) : IReportService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IProductRepository _productRepository = productRepository;

    public IEnumerable<string> GenerateSalesSummary()
    {
        var orders = _orderRepository.GetAll();
        
        if (!orders.Any())
        {
            yield return "No orders found.";
            yield break;
        }

        var totalRevenue = orders.Sum(o => o.TotalAmount);
        var totalOrders = orders.Count;
        var ordersByStatus = orders.GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() });

        yield return "=== Sales Summary ===";
        yield return $"Total Orders: {totalOrders}";
        yield return $"Total Revenue: ${totalRevenue:F2}";
        yield return $"Average Order Value: ${(totalOrders > 0 ? totalRevenue / totalOrders : 0):F2}";
        yield return "";
        yield return "Orders by Status:";
        foreach (var status in ordersByStatus)
        {
            yield return $"  {status.Status}: {status.Count}";
        }
    }

    public IEnumerable<string> GenerateLowStockReport(int threshold)
    {
        var lowStockProducts = _productRepository.GetAll()
            .Where(p => p.Stock <= threshold)
            .OrderBy(p => p.Stock)
            .ToList();

        if (!lowStockProducts.Any())
        {
            yield return $"No products with stock <= {threshold}.";
            yield break;
        }

        yield return $"=== Low Stock Report (Threshold: {threshold}) ===";
        foreach (var product in lowStockProducts)
        {
            yield return $"ID: {product.Id} | {product.Name} ({product.Category}) | Stock: {product.Stock}";
        }
    }

    public IEnumerable<string> GenerateTopProductsReport(int takeCount)
    {
        var orders = _orderRepository.GetAll();
        
        if (!orders.Any())
        {
            yield return "No order data available.";
            yield break;
        }

        var productSales = orders
            .SelectMany(o => o.Items)
            .GroupBy(item => item.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalQuantity = g.Sum(item => item.Quantity),
                TotalRevenue = g.Sum(item => item.LineTotal)
            })
            .OrderByDescending(p => p.TotalRevenue)
            .Take(takeCount)
            .ToList();

        if (!productSales.Any())
        {
            yield return "No product sales data available.";
            yield break;
        }

        yield return $"=== Top {takeCount} Products by Revenue ===";
        var rank = 1;
        foreach (var sale in productSales)
        {
            var product = _productRepository.GetById(sale.ProductId);
            var productName = product?.Name ?? "Unknown Product";
            yield return $"{rank}. {productName} | Qty Sold: {sale.TotalQuantity} | Revenue: ${sale.TotalRevenue:F2}";
            rank++;
        }
    }
}
