# Shoplio

Shoplio is a C# console-based online shopping backend simulation project.
It is designed to demonstrate backend system design, object-oriented programming, LINQ usage, error handling, and clean architecture in a menu-driven console environment.

This project is aligned to a two-stage academic submission:
- Submission 1: core backend functionality
- Submission 2: design patterns and architecture improvements

## Project Objectives

- Build a role-based shopping backend for `Customer` and `Administrator`
- Implement realistic commerce workflows (catalog, cart, orders, payment, inventory)
- Keep business logic cleanly separated from console UI
- Prepare the codebase for extensibility and maintainability

## Core Features (Specification)

- User registration and login
- Role separation: Customer and Administrator
- Product catalog management
- Shopping cart operations
- Order placement and tracking
- Wallet-based payment simulation
- Inventory and stock management
- Administrative reporting and analytics
- Product review and rating
- Input validation and exception handling

## Current Status

Scaffolded and ready for feature implementation:

- Solution and project setup complete (`Shoplio.slnx`, `src/Shoplio.Console`)
- Domain models created:
	- `User`, `Product`, `CartItem`, `Order`, `Payment`, `Review`
	- `Role`, `OrderStatus`, `PaymentStatus`
- Contracts created:
	- Repository interfaces (`IUserRepository`, `IProductRepository`, `IOrderRepository`)
	- Service interfaces (`IAuthService`, `IProductService`, `ICartService`, `IOrderService`, `IReportService`)
- Menu skeletons created:
	- Main menu, customer menu, admin menu
- Input helper utility added (`InputReader`)

Tracking documents:
- `docs/plan.md`
- `docs/spec-checklist.md`

## Architecture

The codebase follows a layered, clean structure:

- `UI/`: console menu and user interaction layer
- `Services/`: business rules and use-case logic
- `Repositories/`: data access abstractions
- `Models/`: domain entities and enums
- `Utils/`: shared helper utilities

Proposed flow:

1. User selects operation in `UI`
2. UI invokes `Service` methods
3. Services coordinate rules and validations
4. Services read/write data through `Repository` interfaces
5. Models carry domain state across layers

## Design Patterns

### Planned for Submission 2

- Repository Pattern:
	- Purpose: isolate data storage and retrieval logic from business logic
	- Location: `Repositories/` and repository interfaces

- Strategy Pattern (Payment):
	- Purpose: support extensible payment behaviors (starting with wallet)
	- Example: `IPaymentStrategy` with `WalletPaymentStrategy`

- Factory Pattern:
	- Purpose: centralize creation of domain objects and seed data
	- Example: user/product/order factories for controlled object construction

- Optional Singleton (Session/App Context):
	- Purpose: hold runtime session state in a controlled single instance

## Technology Stack

- Language: C#
- Runtime: .NET 10 (`net10.0`)
- App type: Console application
- IDE: Visual Studio Code (or Visual Studio)

## Project Structure

```text
Shoplio/
	docs/
		plan.md
		spec-checklist.md
	src/
		Shoplio.Console/
			Models/
			Repositories/
				Interfaces/
			Services/
				Interfaces/
			UI/
			Utils/
			Program.cs
			Shoplio.Console.csproj
	Shoplio.slnx
	spec.md
```

## Setup and Run

### Prerequisites

- .NET SDK 10+

### Build

```powershell
dotnet build Shoplio.slnx
```

### Run

```powershell
dotnet run --project src/Shoplio.Console/Shoplio.Console.csproj
```

## Development Roadmap

1. Implement in-memory repositories and seed data
2. Implement `AuthService` and complete register/login flows
3. Implement customer features: browse/search/cart/checkout/order history/reviews
4. Implement admin features: product CRUD, restock, order view, reports
5. Add robust validation and exception handling
6. Introduce design patterns and refactor for Submission 2

## LINQ Usage Plan

LINQ will be used for:
- Product search and filtering
- Cart total and order total calculations
- Sales aggregation and analytics
- Low-stock and top-selling reports

## Quality and Validation

- Guard all menu input using safe parsing (`TryParse` patterns)
- Enforce business rules with clear exception messages
- Keep methods focused and modular
- Use interfaces for testability and maintainability

## Notes

- `spec.md` is used as the active requirement reference for this repository.
- Some OCR artifacts may exist in the source spec text; use `docs/spec-checklist.md` as the implementation checklist of record.