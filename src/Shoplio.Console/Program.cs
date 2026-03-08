using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories;
using Shoplio.ConsoleApp.Services;
using Shoplio.ConsoleApp.UI;

// Initialize repositories
var userRepository = new InMemoryUserRepository();
var productRepository = new InMemoryProductRepository();
var orderRepository = new InMemoryOrderRepository();

// Initialize services
var authService = new AuthService(userRepository);
var productService = new ProductService(productRepository);
var cartService = new CartService(productRepository);
var reviewService = new ReviewService();
var orderService = new OrderService(orderRepository, cartService, productRepository, userRepository);
var reportService = new ReportService(orderRepository, productRepository);

// Seed a default admin for immediate role-specific login testing
authService.Register("System Admin", "admin@shoplio.local", "admin123", Role.Administrator);

// Seed sample products for testing
productService.AddProduct("Laptop", "Electronics", 999.99m, 10);
productService.AddProduct("Mouse", "Electronics", 29.99m, 50);
productService.AddProduct("Keyboard", "Electronics", 79.99m, 30);
productService.AddProduct("Monitor", "Electronics", 299.99m, 3);
productService.AddProduct("Desk Chair", "Furniture", 199.99m, 1);
productService.AddProduct("Coffee Mug", "Home", 9.99m, 100);

var mainMenu = new MainMenu(authService, productService, cartService, orderService, reviewService, reportService);
mainMenu.Run();
