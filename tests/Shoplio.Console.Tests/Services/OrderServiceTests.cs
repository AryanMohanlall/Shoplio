using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories;
using Shoplio.ConsoleApp.Services;

namespace Shoplio.Console.Tests.Services;

public sealed class OrderServiceTests
{
    [Fact]
    public void PlaceOrder_UserNotFound_ThrowsInvalidOperationException()
    {
        var orderRepo = new InMemoryOrderRepository();
        var productRepo = new InMemoryProductRepository();
        var userRepo = new InMemoryUserRepository();
        var cartService = new CartService(productRepo);
        var sut = new OrderService(orderRepo, cartService, productRepo, userRepo);

        var action = () => sut.PlaceOrder(55);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void PlaceOrder_EmptyCart_ThrowsInvalidOperationException()
    {
        var (sut, _, _, _, user) = BuildOrderSut();

        var action = () => sut.PlaceOrder(user.Id);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void PlaceOrder_InsufficientStock_ThrowsInvalidOperationException()
    {
        var (sut, cartService, productRepo, _, user) = BuildOrderSut();
        var product = new ProductService(productRepo).AddProduct("Laptop", "Electronics", 100m, 2);
        cartService.AddToCart(user.Id, product.Id, 2);

        // Simulate stock change after cart add but before checkout.
        product.Stock = 1;
        productRepo.Update(product);

        var action = () => sut.PlaceOrder(user.Id);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void PlaceOrder_InsufficientWallet_ThrowsInvalidOperationException()
    {
        var (sut, cartService, productRepo, _, user) = BuildOrderSut();
        var product = new ProductService(productRepo).AddProduct("Laptop", "Electronics", 600m, 2);
        cartService.AddToCart(user.Id, product.Id, 2);

        var action = () => sut.PlaceOrder(user.Id);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void PlaceOrder_ValidRequest_CreatesPaidOrderAndClearsCart()
    {
        var (sut, cartService, productRepo, orderRepo, user) = BuildOrderSut();
        var product = new ProductService(productRepo).AddProduct("Mouse", "Electronics", 50m, 5);
        cartService.AddToCart(user.Id, product.Id, 2);

        var order = sut.PlaceOrder(user.Id);

        Assert.True(order.Id > 0);
        Assert.Equal(OrderStatus.Paid, order.Status);
        Assert.Equal(100m, order.TotalAmount);
        Assert.Empty(cartService.GetCartItems(user.Id));
        Assert.Equal(900m, user.WalletBalance);
        Assert.Equal(3, productRepo.GetById(product.Id)!.Stock);
        Assert.Single(orderRepo.GetAll());
    }

    [Fact]
    public void UpdateOrderStatus_OrderMissing_ReturnsFalse()
    {
        var (sut, _, _, _, _) = BuildOrderSut();

        var result = sut.UpdateOrderStatus(42, OrderStatus.Cancelled);

        Assert.False(result);
    }

    [Fact]
    public void UpdateOrderStatus_OrderExists_ReturnsTrueAndUpdates()
    {
        var (sut, cartService, productRepo, _, user) = BuildOrderSut();
        var product = new ProductService(productRepo).AddProduct("Mouse", "Electronics", 50m, 5);
        cartService.AddToCart(user.Id, product.Id, 1);
        var order = sut.PlaceOrder(user.Id);

        var result = sut.UpdateOrderStatus(order.Id, OrderStatus.Cancelled);

        Assert.True(result);
        Assert.Equal(OrderStatus.Cancelled, sut.GetOrderById(order.Id)!.Status);
    }

    private static (OrderService Sut, CartService CartService, InMemoryProductRepository ProductRepo, InMemoryOrderRepository OrderRepo, User User) BuildOrderSut()
    {
        var orderRepo = new InMemoryOrderRepository();
        var productRepo = new InMemoryProductRepository();
        var userRepo = new InMemoryUserRepository();
        var cartService = new CartService(productRepo);
        var authService = new AuthService(userRepo);
        var user = authService.Register("Buyer", "buyer@example.com", "secret1", Role.Customer);
        var sut = new OrderService(orderRepo, cartService, productRepo, userRepo);

        return (sut, cartService, productRepo, orderRepo, user);
    }
}
