# Shoplio Architecture and Design

This document describes the architecture, design decisions, and key patterns used in the Shoplio console application.

## 1. System Overview

Shoplio is a role-based shopping console system with two personas:

- Customer: browse products, manage cart, place orders, and add reviews.
- Administrator: manage catalog/order status and generate operational reports.

The application follows a layered architecture and uses Entity Framework Core with SQL Server for persistence.

## 2. High-Level Architecture

[Flowchat by Mermaid](https://mermaid.ai/app/projects/2a1a2059-5976-443a-bb91-543292d5c2b6/diagrams/7748094c-e043-4768-8f77-d328e9c958a8/share/invite/eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJkb2N1bWVudElEIjoiNzc0ODA5NGMtZTA0My00NzY4LThmNzctZDMyOGU5Yzk1OGE4IiwiYWNjZXNzIjoiVmlldyIsImlhdCI6MTc3MzA1NTU3M30.p62D_cYZ60im1pSumFQWumgm8GCsGOFtFFTzdB3uLls)

![Architecture](/docs//Layered%20Service%20Architecture-2026-03-09-112653.png)

### Layer Responsibilities

- UI Layer:
	- Handles interaction flow and menu orchestration.
	- Delegates all business behavior to service interfaces.
- Service Layer:
	- Implements business rules (validation, checkout logic, stock and wallet handling, reporting).
	- Coordinates multiple repositories where workflows span entities.
- Repository Layer:
	- Encapsulates data-access operations behind stable interfaces.
	- Supports interchangeable storage implementations.
- Data Layer:
	- Defines entity mappings and persistence rules in `ShoplioDbContext`.
	- Owns schema evolution via EF migrations.

## 3. Key Design Patterns

### Repository Pattern

- Interfaces: `IUserRepository`, `IProductRepository`, `IOrderRepository`.
- Implementations:
	- In-memory: `InMemoryUserRepository`, `InMemoryProductRepository`, `InMemoryOrderRepository`.
	- SQL: `SqlUserRepository`, `SqlProductRepository`, `SqlOrderRepository`.
- Value:
	- Isolates persistence details from business logic.
	- Improves testability and implementation swapping.

### Service Layer Pattern

- Business logic is centralized in:
	- `AuthService`, `ProductService`, `CartService`, `OrderService`, `ReviewService`, `ReportService`.
- Value:
	- Prevents UI from embedding business rules.
	- Keeps use-case logic cohesive and easier to test.

### Dependency Injection (Constructor Injection)

- Dependencies are supplied through constructors (including primary constructors).
- Examples:
	- `AuthService(IUserRepository userRepository)`
	- `OrderService(IOrderRepository, ICartService, IProductRepository, IUserRepository)`
- Value:
	- Loose coupling and easier mocking/substitution in tests.

### Strategy-Style Repository Selection

- App can select storage strategy by wiring different repository implementations in `Program.cs`.
- Current runtime wiring uses SQL repositories.
- Value:
	- Flexible environment setup and easier local experimentation.

### Factory Pattern (Design-Time)

- `ShoplioDbContextFactory` implements EF design-time creation for tooling and migrations.
- Value:
	- Stable migration workflow independent from runtime composition.

## 4. Domain Model Notes

Core entities:

- `User`, `Product`, `Order`, `OrderItem`, `Review`, `CartItem`, `Payment`.

Enums:

- `Role`: `Customer`, `Administrator`
- `OrderStatus`: `Placed`, `Paid`, `Cancelled`
- `PaymentStatus`: `Pending`, `Success`, `Failed`

Important relationships from EF mapping:

- `Order` -> `User` (many-to-one)
- `Order` -> `OrderItem` (one-to-many, cascade delete)
- `OrderItem` -> `Product` (many-to-one)
- `Review` -> `User` and `Product` (many-to-one)

Current scope note:

- `Payment` exists as a domain model but is not yet mapped as a `DbSet` in `ShoplioDbContext`.

## 5. Runtime Composition

`Program.cs` acts as the composition root:

- Builds configuration and SQL connection.
- Constructs `ShoplioDbContext`.
- Wires SQL repositories.
- Wires services with interface-based dependencies.
- Seeds bootstrap data when required.
- Starts the main UI loop.

## 6. Main Business Flows

### Checkout / Place Order

1. `OrderService.PlaceOrder(userId)` loads user and cart.
2. Validates stock and wallet balance.
3. Updates product stock through `IProductRepository`.
4. Creates and persists `Order`/`OrderItem` via `IOrderRepository`.
5. Clears user cart through `ICartService`.

### Product Management

1. UI invokes `IProductService` operations.
2. Service validates input and delegates persistence via `IProductRepository`.

### Reporting

1. `ReportService` reads orders/products from repositories.
2. Produces sales summary, low stock, and top-product projections.

## 7. Quality Attributes and Trade-Offs

- Maintainability:
	- Clear separation of concerns and interface boundaries.
- Testability:
	- Service behavior can be validated with repository substitutions.
- Extensibility:
	- New data sources or service capabilities can be introduced with minimal UI impact.
- Trade-off:
	- Manual composition in `Program.cs` is explicit and simple but not as scalable as container-managed DI for larger systems.

## 8. Extension Guidelines

When adding new features:

1. Add/extend model and EF mapping in `ShoplioDbContext`.
2. Define repository interface operation(s), then implement SQL and in-memory variants.
3. Add/extend service interface and service implementation.
4. Wire dependencies in `Program.cs`.
5. Add tests in `tests/Shoplio.Console.Tests` for service and repository behavior.

## 9. Diagrams

- Domain model (Draw.io):
	- [Domain Model by Draw.io](https://drive.google.com/file/d/1cpYraw73o2i_LQCraEeQmU9R6QZ9AsaV/view?usp=sharing)
	- ![Domain Model](/docs/Shoplio%20Domain%20Model.jpg)
- Class diagram ([Mermaid](https://mermaid.ai/app/projects/2a1a2059-5976-443a-bb91-543292d5c2b6/diagrams/e7e930e9-d704-41f0-aba3-01c22613dec9/share/invite/eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJkb2N1bWVudElEIjoiZTdlOTMwZTktZDcwNC00MWYwLWFiYTMtMDFjMjI2MTNkZWM5IiwiYWNjZXNzIjoiVmlldyIsImlhdCI6MTc3MzA1NTcyNn0.HhJv0CEhOBHAlIEt2sFkrsr2IMbcNwJ3FZf8KFM7X6w)):
	- Source: `docs/class-diagram.md`
	- ![Class diagram](/docs/Shoplio%20Class%20Diagram.png)