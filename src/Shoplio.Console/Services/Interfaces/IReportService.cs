namespace Shoplio.ConsoleApp.Services.Interfaces;

public interface IReportService
{
    IEnumerable<string> GenerateSalesSummary();
    IEnumerable<string> GenerateLowStockReport(int threshold);
    IEnumerable<string> GenerateTopProductsReport(int takeCount);
    
    // Chart-friendly data methods
    (decimal TotalRevenue, int TotalOrders, decimal AverageOrderValue) GetSalesSummaryData();
    IEnumerable<(string Status, int Count)> GetOrdersByStatus();
    IEnumerable<(string ProductName, decimal Revenue, int Quantity)> GetTopProductsData(int takeCount);
    IEnumerable<(int ProductId, string ProductName, int Stock, string Category)> GetLowStockData(int threshold);
}
