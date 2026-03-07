using Shoplio.ConsoleApp.Repositories;
using Shoplio.ConsoleApp.Services;
using Shoplio.ConsoleApp.UI;
using Shoplio.ConsoleApp.Utils;

// Repositories
var userRepo = new InMemoryUserRepository();
var productRepo = new InMemoryProductRepository();
var orderRepo = new InMemoryOrderRepository();
var reviewRepo = new InMemoryReviewRepository();

// Services
var authService = new AuthService(userRepo);
var productService = new ProductService(productRepo);
var cartService = new CartService(productRepo);
var orderService = new OrderService(orderRepo, productRepo, userRepo, cartService);
var reviewService = new ReviewService(reviewRepo, productRepo);
var reportService = new ReportService(orderRepo, productRepo);

// Seed initial data
SeedData.Seed(authService, productService);

// Launch app
var mainMenu = new MainMenu(authService, productService, cartService, orderService, reviewService, reportService);
mainMenu.Run();
