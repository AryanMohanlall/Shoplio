using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;

namespace Shoplio.ConsoleApp.Repositories;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public void Add(User user)
    {
        user.Id = _nextId++;
        _users.Add(user);
    }

    public User? GetByEmail(string email) =>
        _users.FirstOrDefault(u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));

    public User? GetById(int id) =>
        _users.FirstOrDefault(u => u.Id == id);

    public IReadOnlyList<User> GetAll() => _users.AsReadOnly();
}
