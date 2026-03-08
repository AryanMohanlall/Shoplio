using System.Security.Cryptography;
using System.Text;
using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories.Interfaces;
using Shoplio.ConsoleApp.Services.Interfaces;

namespace Shoplio.ConsoleApp.Services;

public sealed class AuthService(IUserRepository userRepository) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;

    public User Register(string name, string email, string password, Role role)
    {
        var normalizedName = name?.Trim() ?? string.Empty;
        var normalizedEmail = email?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            throw new ArgumentException("Password must be at least 6 characters.", nameof(password));
        }

        if (_userRepository.GetByEmail(normalizedEmail) is not null)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        var user = new User
        {
            Name = normalizedName,
            Email = normalizedEmail,
            Password = HashPassword(password),
            Role = role,
            WalletBalance = role == Role.Customer ? 1000m : 0m
        };

        _userRepository.Add(user);
        return user;
    }

    public User? Login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var user = _userRepository.GetByEmail(email.Trim());
        if (user is null)
        {
            return null;
        }

        return VerifyPassword(password, user.Password) ? user : null;
    }

    private static string HashPassword(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        var inputBytes = new byte[saltBytes.Length + passwordBytes.Length];
        Buffer.BlockCopy(saltBytes, 0, inputBytes, 0, saltBytes.Length);
        Buffer.BlockCopy(passwordBytes, 0, inputBytes, saltBytes.Length, passwordBytes.Length);

        var hashBytes = SHA256.HashData(inputBytes);
        return $"{Convert.ToBase64String(saltBytes)}:{Convert.ToBase64String(hashBytes)}";
    }

    private static bool VerifyPassword(string providedPassword, string storedPassword)
    {
        var passwordParts = storedPassword.Split(':', 2);
        if (passwordParts.Length != 2)
        {
            return false;
        }

        byte[] saltBytes;
        byte[] storedHashBytes;

        try
        {
            saltBytes = Convert.FromBase64String(passwordParts[0]);
            storedHashBytes = Convert.FromBase64String(passwordParts[1]);
        }
        catch (FormatException)
        {
            return false;
        }

        var passwordBytes = Encoding.UTF8.GetBytes(providedPassword);
        var inputBytes = new byte[saltBytes.Length + passwordBytes.Length];
        Buffer.BlockCopy(saltBytes, 0, inputBytes, 0, saltBytes.Length);
        Buffer.BlockCopy(passwordBytes, 0, inputBytes, saltBytes.Length, passwordBytes.Length);

        var providedHash = SHA256.HashData(inputBytes);
        return CryptographicOperations.FixedTimeEquals(providedHash, storedHashBytes);
    }
}