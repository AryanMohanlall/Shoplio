using Shoplio.ConsoleApp.Models;
using Shoplio.ConsoleApp.Repositories;
using Shoplio.ConsoleApp.Services;

namespace Shoplio.Console.Tests.Services;

public sealed class AuthServiceTests
{
    [Fact]
    public void Register_Customer_StoresHashedPasswordAndWalletBalance()
    {
        var repo = new InMemoryUserRepository();
        var sut = new AuthService(repo);

        var user = sut.Register("  Aryan  ", "  aryan@example.com  ", "secret1", Role.Customer);

        Assert.Equal(1, user.Id);
        Assert.Equal("Aryan", user.Name);
        Assert.Equal("aryan@example.com", user.Email);
        Assert.Equal(Role.Customer, user.Role);
        Assert.Equal(1000m, user.WalletBalance);
        Assert.NotEqual("secret1", user.Password);
        Assert.Contains(':', user.Password);
    }

    [Fact]
    public void Register_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var repo = new InMemoryUserRepository();
        var sut = new AuthService(repo);
        sut.Register("User One", "same@example.com", "secret1", Role.Customer);

        var action = () => sut.Register("User Two", "SAME@example.com", "secret2", Role.Customer);

        Assert.Throws<InvalidOperationException>(action);
    }

    [Fact]
    public void Register_ShortPassword_ThrowsArgumentException()
    {
        var repo = new InMemoryUserRepository();
        var sut = new AuthService(repo);

        var action = () => sut.Register("Name", "email@example.com", "123", Role.Customer);

        var ex = Assert.Throws<ArgumentException>(action);
        Assert.Equal("password", ex.ParamName);
    }

    [Fact]
    public void Login_WithValidCredentials_ReturnsUser()
    {
        var repo = new InMemoryUserRepository();
        var sut = new AuthService(repo);
        var registered = sut.Register("Name", "name@example.com", "secret1", Role.Administrator);

        var user = sut.Login(" name@example.com ", "secret1");

        Assert.NotNull(user);
        Assert.Equal(registered.Id, user!.Id);
    }

    [Fact]
    public void Login_WithWrongPassword_ReturnsNull()
    {
        var repo = new InMemoryUserRepository();
        var sut = new AuthService(repo);
        sut.Register("Name", "name@example.com", "secret1", Role.Customer);

        var user = sut.Login("name@example.com", "wrong-password");

        Assert.Null(user);
    }

    [Fact]
    public void Login_WithMalformedStoredPassword_ReturnsNull()
    {
        var repo = new InMemoryUserRepository();
        repo.Add(new User
        {
            Name = "Name",
            Email = "name@example.com",
            Password = "not-a-valid-hash"
        });
        var sut = new AuthService(repo);

        var user = sut.Login("name@example.com", "secret1");

        Assert.Null(user);
    }
}
