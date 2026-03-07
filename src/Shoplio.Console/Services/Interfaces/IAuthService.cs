using Shoplio.ConsoleApp.Models;

namespace Shoplio.ConsoleApp.Services.Interfaces;

public interface IAuthService
{
    User Register(string name, string email, string password, Role role);
    User? Login(string email, string password);
}
