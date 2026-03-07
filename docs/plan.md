# Shoplio Project Plan

## Goal
Build a C# console-based online shopping backend system that satisfies Submission 1 (core backend) and Submission 2 (design patterns and architecture improvements).

## Timeline (March 7-9)

### Day 1: Foundation + Core Models
- [ ] Set up solution and project structure
- [ ] Implement domain models (`User`, `Product`, `CartItem`, `Order`, `Payment`, `Review`)
- [ ] Add role enum (`Customer`, `Administrator`)
- [ ] Build menu skeleton (Main, Customer, Admin)
- [ ] Seed initial test data

### Day 2: Core Submission 1 Features
- [ ] Registration/login
- [ ] Product browsing and search
- [ ] Cart add/update/remove/view
- [ ] Checkout and order creation
- [ ] Wallet payment simulation
- [ ] Stock decrement/restock rules
- [ ] Order tracking/history
- [ ] Product reviews and ratings

### Day 3 (Morning): Hardening + Rubric Coverage
- [ ] Input validation for all menu actions
- [ ] Exception handling around all service operations
- [ ] LINQ-based queries for search/reporting
- [ ] Admin reporting (sales, low stock, top-selling products)
- [ ] Clean up code structure and naming

### Day 3 (Afternoon): Submission 2 Upgrade
- [ ] Introduce repository pattern consistently
- [ ] Introduce strategy pattern for payment processing
- [ ] Introduce factory for object creation/seeding
- [ ] Refactor for interfaces and separation of concerns
- [ ] Add documentation of design decisions

## Rubric Alignment
- System Functionality: implement all listed user/admin workflows.
- OOP Design: model entities + service layer + role-based behavior.
- Console Interface: clear numbered menus and navigation.
- LINQ Usage: search/filter/group/sum/reporting operations.
- Exception Handling: guard inputs and business rule checks.
- Code Quality: modular folders, interfaces, short methods.

## Definition Of Done (Submission 1)
- [ ] App runs end-to-end via console menus.
- [ ] Customer can register, login, browse, cart, checkout, pay, review.
- [ ] Admin can manage products, stock, orders, and view reports.
- [ ] Invalid input is handled without crashing.
- [ ] LINQ is used in at least search and reporting.

## Definition Of Done (Submission 2)
- [ ] At least 2-3 design patterns are correctly implemented.
- [ ] Architecture has clear separation: UI, services, repositories, models.
- [ ] Code is maintainable and interfaces are used.
- [ ] Documentation explains design choices and trade-offs.
