using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Utils;

public static class SeedData
{
    public static void Seed(IAuthService authService, IProductService productService)
    {
        // Seed admin account
        authService.Register("Admin", "admin@shoplio.com", "admin123", Role.Administrator);

        // Seed a demo customer
        authService.Register("Alice", "alice@shoplio.com", "alice123", Role.Customer);

        // Seed products
        productService.AddProduct("Laptop Pro 15", "Electronics", 12999.99m, 10);
        productService.AddProduct("Wireless Mouse", "Electronics", 299.99m, 50);
        productService.AddProduct("USB-C Hub", "Electronics", 499.99m, 30);
        productService.AddProduct("Mechanical Keyboard", "Electronics", 1299.99m, 20);
        productService.AddProduct("27-inch Monitor", "Electronics", 4999.99m, 8);
        productService.AddProduct("Running Shoes", "Footwear", 899.99m, 40);
        productService.AddProduct("Backpack 30L", "Accessories", 749.99m, 25);
        productService.AddProduct("Water Bottle 1L", "Accessories", 199.99m, 60);
        productService.AddProduct("Fiction Novel – The Lost Code", "Books", 149.99m, 100);
        productService.AddProduct("C# in Depth", "Books", 499.99m, 35);
    }
}
