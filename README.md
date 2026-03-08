# Shoplio

Shoplio is a C# console-based shopping backend simulation with role-based workflows for customers and administrators.
The project now uses SQL Server persistence via Entity Framework Core (instead of only in-memory repositories).

## Highlights

- Role-based authentication and authorization (`Customer`, `Administrator`)
- Product catalog, cart, checkout, wallet, order tracking, and reviews
- Admin operations for product/order management and reporting
- Layered architecture (`UI` -> `Services` -> `Repositories` -> `Data`)
- SQL Server persistence with EF Core migrations

## Technology Stack

- C#
- .NET 10 (`net10.0`)
- Entity Framework Core 10
- SQL Server (Docker supported)

## Design Patterns

This project implements several software design patterns for maintainability, testability, and scalability:

### Repository Pattern
- **Interfaces**: `IUserRepository`, `IProductRepository`, `IOrderRepository`
- **Implementations**: Both in-memory (`InMemory*Repository`) and SQL-backed (`Sql*Repository`) variants
- **Benefits**: Abstracts data access logic, enables easy swapping between data sources, simplifies testing

### Dependency Injection (DI)
- **Usage**: Constructor injection throughout services and repositories
- **Example**: `AuthService(IUserRepository userRepository)`, `OrderService(IOrderRepository, ICartService, IProductRepository, IUserRepository)`
- **Benefits**: Loose coupling, easier unit testing with mock dependencies, improved maintainability

### Service Layer Pattern
- **Structure**: Clear separation of concerns across layers:
  - `UI` → User interaction and menu flows
  - `Services` → Business logic (AuthService, OrderService, CartService, etc.)
  - `Repositories` → Data access abstraction
  - `Data` → Entity Framework DbContext and models
- **Benefits**: Single Responsibility Principle, easier to test business logic in isolation

### Factory Pattern
- **Implementation**: `ShoplioDbContextFactory` implements `IDesignTimeDbContextFactory<ShoplioDbContext>`
- **Usage**: Creates DbContext instances for EF Core design-time tools (migrations)
- **Benefits**: Centralizes configuration, supports tooling scenarios

### Strategy Pattern
- **Implementation**: Interchangeable repository implementations (in-memory vs. SQL)
- **Usage**: `Program.cs` can switch between `InMemoryUserRepository` and `SqlUserProvider` without changing service code
- **Benefits**: Runtime flexibility, supports multiple storage strategies

### Primary Constructor Pattern (C# 12+)
- **Usage**: Simplified constructor syntax with automatic field initialization
- **Example**: `public sealed class AuthService(IUserRepository userRepository) : IAuthService`
- **Benefits**: Reduces boilerplate code, clearer intent

## Project Structure

```text
Shoplio/
  src/
    Shoplio.Console/
      Data/
        ShoplioDbContext.cs
        ShoplioDbContextFactory.cs
      Migrations/
      Models/
      Repositories/
        Interfaces/
        InMemory*.cs
        Sql*.cs
      Services/
      UI/
      Program.cs
      appsettings.json
      Shoplio.Console.csproj
  docs/
  Shoplio.slnx
  README.md
```

## Database Setup

### 1. Start SQL Server (Docker example)

```powershell
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name shoplio-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Configure connection string

Update `src/Shoplio.Console/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ShoplioDb": "Server=localhost,1433;Database=ShoplioDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;"
  }
}
```

## Setup and Run

### Prerequisites

- .NET SDK 10+
- SQL Server running (local instance or Docker)

### 1. Restore and build

```powershell
dotnet restore Shoplio.slnx
dotnet build Shoplio.slnx
```

### 2. Run tests

From repo root:

```powershell
dotnet test Shoplio.slnx
```

Run only the test project:

```powershell
dotnet test tests/Shoplio.Console.Tests/Shoplio.Console.Tests.csproj
```

Collect coverage:

```powershell
dotnet test Shoplio.slnx --collect:"XPlat Code Coverage"
```

### 3. Apply migrations (create/update database schema)

From the project folder:

```powershell
cd src/Shoplio.Console
dotnet ef database update
```

If `dotnet ef` is not installed:

```powershell
dotnet tool install --global dotnet-ef
```

### 4. Run the app

From repo root:

```powershell
dotnet run --project src/Shoplio.Console/Shoplio.Console.csproj
```

## Default Seed Data

On startup, the app seeds data only when missing:

- Admin user: `admin@shoplio.local` / `admin123`
- Sample products: seeded when product table is empty

## Notes

- `bin/` and `obj/` are build artifacts and should not be tracked in Git.
- The repository contains both in-memory and SQL repository implementations; `Program.cs` is currently wired to SQL repositories.
- For schema evolution, create migrations with:

```powershell
cd src/Shoplio.Console
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## Troubleshooting

### SQL Server connection failed

**Error:** `Login failed for user 'sa'` or `A network-related or instance-specific error occurred`

**Solutions:**
- Verify SQL Server is running: `docker ps` (should show your container)
- Check connection string password matches your Docker `SA_PASSWORD`
- Ensure port 1433 is not blocked by firewall
- For Docker: verify container is healthy with `docker logs shoplio-sql`

### Port 1433 already in use

**Error:** `Bind for 0.0.0.0:1433 failed: port is already allocated`

**Solutions:**
- Check if SQL Server is already running locally: stop it or use a different port
- Use a different port in Docker: `-p 1434:1433` and update connection string to `localhost,1434`
- Stop conflicting container: `docker stop <container-name>`

### dotnet ef command not found

**Error:** `Could not execute because the specified command or file was not found`

**Solution:**
```powershell
dotnet tool install --global dotnet-ef
```

After installation, restart your terminal or IDE.

### Build errors with Console namespace

**Error:** `The type or namespace name 'WriteLine' does not exist in the namespace 'Shoplio.Console'`

**Solution:**
This was resolved by adding `GlobalUsings.cs` with a `Console` alias. If you encounter this:
- Clean the build: `dotnet clean`
- Rebuild: `dotnet build`
- Ensure `GlobalUsings.cs` exists in the project root

### Database already exists but schema is outdated

**Solution:**
Delete and recreate the database:
```powershell
cd src/Shoplio.Console
dotnet ef database drop
dotnet ef database update
```

Or apply pending migrations:
```powershell
dotnet ef database update
```
