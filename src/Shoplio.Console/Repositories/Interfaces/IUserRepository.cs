using Shoplio.ConsoleApp.Models;

namespace Shoplio.ConsoleApp.Repositories.Interfaces;

public interface IUserRepository
{
    void Add(User user);
    User? GetByEmail(string email);
    User? GetById(int id);
    IReadOnlyList<User> GetAll();
}
