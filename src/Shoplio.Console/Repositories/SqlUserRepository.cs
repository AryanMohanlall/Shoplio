using Microsoft.EntityFrameworkCore;
using Shoplio.ConsoleApp.Data;
using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;

namespace Shoplio.ConsoleApp.Repositories;

public sealed class SqlUserRepository : IUserRepository
{
    private readonly ShoplioDbContext _context;

    public SqlUserRepository(ShoplioDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Add(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public User? GetByEmail(string email)
    {
        return _context.Users
            .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
    }

    public User? GetById(int id)
    {
        return _context.Users.Find(id);
    }

    public IReadOnlyList<User> GetAll()
    {
        return _context.Users.ToList();
    }
}
