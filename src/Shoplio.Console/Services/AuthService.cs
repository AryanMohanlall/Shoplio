using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User Register(string name, string email, string password, Role role)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.");

        if (_userRepository.GetByEmail(email) is not null)
            throw new InvalidOperationException("An account with that email already exists.");

        var user = new User
        {
            Name = name.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Password = password,
            Role = role,
            WalletBalance = role == Role.Customer ? 500m : 0m
        };

        _userRepository.Add(user);
        return user;
    }

    public User? Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return null;

        var user = _userRepository.GetByEmail(email.Trim());
        // Plain-text comparison is intentional for this in-memory console simulation.
        if (user is null || user.Password != password)
            return null;

        return user;
    }
}
