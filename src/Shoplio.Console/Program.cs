using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shoplio.ConsoleApp.Data;
using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories;
using Shoplio.ConsoleApp.Services;
using Shoplio.ConsoleApp.UI;

// Load configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Get connection string
var connectionString = configuration.GetConnectionString("ShoplioDb")
    ?? throw new InvalidOperationException("Connection string 'ShoplioDb' not found.");

// Create DbContext options
var optionsBuilder = new DbContextOptionsBuilder<ShoplioDbContext>();
optionsBuilder.UseSqlServer(connectionString);

// Create DbContext and ensure database is created
using var dbContext = new ShoplioDbContext(optionsBuilder.Options);
dbContext.Database.EnsureCreated();

// Initialize repositories
var userRepository = new SqlUserRepository(dbContext);
var productRepository = new SqlProductRepository(dbContext);
var orderRepository = new SqlOrderRepository(dbContext);
var reviewRepository = new SqlReviewRepository(dbContext);

// Initialize services
var authService = new AuthService(userRepository);
var productService = new ProductService(productRepository);
var cartService = new CartService(productRepository);
var reviewService = new ReviewService(reviewRepository);
var orderService = new OrderService(orderRepository, cartService, productRepository, userRepository);
var reportService = new ReportService(orderRepository, productRepository);

// Seed a default admin for immediate role-specific login testing (only if not exists)
if (userRepository.GetByEmail("admin@shoplio.local") is null)
{
    authService.Register("System Admin", "admin@shoplio.local", "admin123", Role.Administrator);
}

// Seed sample products for testing (only if products table is empty)
if (productRepository.GetAll().Count == 0)
{
    productService.AddProduct("Laptop", "Electronics", 999.99m, 10);
    productService.AddProduct("Mouse", "Electronics", 29.99m, 50);
    productService.AddProduct("Keyboard", "Electronics", 79.99m, 30);
    productService.AddProduct("Monitor", "Electronics", 299.99m, 3);
    productService.AddProduct("Desk Chair", "Furniture", 199.99m, 1);
    productService.AddProduct("Coffee Mug", "Home", 9.99m, 100);
}

var mainMenu = new MainMenu(authService, productService, cartService, orderService, reviewService, reportService);
mainMenu.Run();
