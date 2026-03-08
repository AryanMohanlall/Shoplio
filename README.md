# Shoplio

Shoplio is a C# console-based online shopping backend simulation project.
It demonstrates role-based workflows, in-memory repositories, layered architecture, and menu-driven backend logic for an e-commerce system.

This project is aligned to a two-stage academic submission:
- Submission 1: core backend functionality
- Submission 2: design patterns and architecture improvements

## Project Objectives

- Build a role-based shopping backend for `Customer` and `Administrator`
- Implement realistic commerce workflows (catalog, cart, orders, payment, inventory)
- Keep business logic cleanly separated from console UI
- Prepare the codebase for extensibility and maintainability

## Implemented Features

- User registration and login with role checks (`Customer`, `Administrator`)
- Password hashing with salt (`SHA256`) in `AuthService`
- Customer workflow:
	- Browse products
	- Search products
	- Add to cart
	- View cart
	- Update cart quantity / remove by setting quantity to `0`
	- Checkout using wallet balance
	- View wallet balance
	- Add wallet funds
	- View order history
	- Track orders
	- Review products
- Administrator workflow:
	- Add product
	- Update product
	- Delete product
	- Restock product
	- View products
	- View orders
	- Update order status
	- View low-stock products
	- Generate sales report
- In-memory repositories for users, products, and orders
- Sales and low-stock reporting with LINQ
- Input validation and user-friendly console prompts

## Current Status

Functional Submission 1 baseline is implemented with in-memory persistence.

- Main menu, customer menu, and administrator menu are wired and working
- Services implemented:
	- `AuthService`
	- `ProductService`
	- `CartService`
	- `OrderService`
	- `ReviewService`
	- `ReportService`
- Repositories implemented:
	- `InMemoryUserRepository`
	- `InMemoryProductRepository`
	- `InMemoryOrderRepository`
- App startup seeds:
	- Default admin account: `admin@shoplio.local` / `admin123`
	- Sample product catalog for testing

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

Runtime flow:

1. User selects operation in `UI`
2. UI invokes `Service` methods
3. Services coordinate rules and validations
4. Services read/write data through `Repository` interfaces
5. Models carry domain state across layers

## Menu Overview

### Main Menu

- Register
- Login as Customer
- Login as Administrator
- Exit

### Customer Menu

- Browse Products
- Search Products
- Add Product to Cart
- View Cart
- Update Cart
- Checkout
- View Wallet Balance
- Add Wallet Funds
- View Order History
- Track Orders
- Review Products
- Logout

### Administrator Menu

- Add Product
- Update Product
- Delete Product
- Restock Product
- View Products
- View Orders
- Update Order Status
- View Low Stock Products
- Generate Sales Report
- Logout

## Design Patterns

### Currently Applied

- Repository Pattern
  - Implemented via repository interfaces and in-memory implementations
- Service Layer Pattern
  - Business logic is isolated from console UI interactions

### Planned for Submission 2

- Strategy Pattern (Payment):
  - Purpose: support extensible payment behaviors beyond wallet simulation
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
- IDE: Visual Studio Code / Visual Studio

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
					InMemoryOrderRepository.cs
					InMemoryProductRepository.cs
					InMemoryUserRepository.cs
				Interfaces/
			Services/
					AuthService.cs
					CartService.cs
					OrderService.cs
					ProductService.cs
					ReportService.cs
					ReviewService.cs
				Interfaces/
			UI/
					AdminMenu.cs
					CustomerMenu.cs
					MainMenu.cs
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

### Quick Test Credentials

- Administrator:
	- Email: `admin@shoplio.local`
	- Password: `admin123`

You can also register customer accounts from the main menu.

## Development Roadmap

1. Improve domain constraints (for example: restrict reviews to purchased products)
2. Introduce persistent storage (file/DB) instead of in-memory repositories
3. Expand order lifecycle and payment strategy abstractions
4. Add automated unit tests and integration tests
5. Refactor for Submission 2 design-pattern depth and maintainability

## LINQ Usage

LINQ is currently used for:
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
- `bin/` and `obj/` build artifacts are intentionally ignored via `.gitignore`.