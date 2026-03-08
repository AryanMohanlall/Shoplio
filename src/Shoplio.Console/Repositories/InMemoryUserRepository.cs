using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;

namespace Shoplio.ConsoleApp.Repositories;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = [];
    private int _nextId = 1;

    public void Add(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        user.Id = _nextId++;
        _users.Add(user);
    }

    public User? GetByEmail(string email)
    {
        return _users.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
    }

    public User? GetById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public IReadOnlyList<User> GetAll()
    {
        return _users;
    }
}