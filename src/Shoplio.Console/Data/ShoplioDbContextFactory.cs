using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Shoplio.ConsoleApp.Data;

public class ShoplioDbContextFactory : IDesignTimeDbContextFactory<ShoplioDbContext>
{
    public ShoplioDbContext CreateDbContext(string[] args)
    {
        // Load configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // Get connection string
        var connectionString = configuration.GetConnectionString("ShoplioDb")
            ?? throw new InvalidOperationException("Connection string 'ShoplioDb' not found.");

        // Create options
        var optionsBuilder = new DbContextOptionsBuilder<ShoplioDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ShoplioDbContext(optionsBuilder.Options);
    }
}
