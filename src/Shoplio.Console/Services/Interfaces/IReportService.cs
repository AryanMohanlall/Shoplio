namespace Shoplio.ConsoleApp.Services.Interfaces;

public interface IReportService
{
    IEnumerable<string> GenerateSalesSummary();
    IEnumerable<string> GenerateLowStockReport(int threshold);
    IEnumerable<string> GenerateTopProductsReport(int takeCount);
}
