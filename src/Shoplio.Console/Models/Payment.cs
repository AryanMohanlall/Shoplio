namespace Shoplio.ConsoleApp.Models;

public sealed class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
