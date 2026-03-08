using Shoplio.ConsoleApp.Models;

namespace Shoplio.Console.Tests;

public sealed class ModelTests
{
    [Fact]
    public void CartItem_LineTotal_ComputesUnitPriceTimesQuantity()
    {
        var item = new CartItem { UnitPrice = 12.5m, Quantity = 4 };

        Assert.Equal(50m, item.LineTotal);
    }

    [Fact]
    public void OrderItem_LineTotal_ComputesUnitPriceTimesQuantity()
    {
        var item = new OrderItem { UnitPrice = 9.99m, Quantity = 3 };

        Assert.Equal(29.97m, item.LineTotal);
    }
}
