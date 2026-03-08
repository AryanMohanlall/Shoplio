using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories;
using Shoplio.ConsoleApp.Services;
using Shoplio.ConsoleApp.UI;

var userRepository = new InMemoryUserRepository();
var authService = new AuthService(userRepository);

// Seed a default admin for immediate role-specific login testing.
authService.Register("System Admin", "admin@shoplio.local", "admin123", Role.Administrator);

var mainMenu = new MainMenu(authService);
mainMenu.Run();
