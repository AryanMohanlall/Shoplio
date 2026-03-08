using Microsoft.EntityFrameworkCore;
using Shoplio.ConsoleApp.Data;

namespace Shoplio.Console.Tests.TestHelpers;

internal static class DbContextTestFactory
{
    public static ShoplioDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ShoplioDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString("N"))
            .Options;

        return new ShoplioDbContext(options);
    }
}
