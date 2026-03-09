classDiagram
    class User {
        +int Id
        +string Name
        +string Email
        +string Password
        +Role Role
        +decimal WalletBalance
    }

    class Product {
        +int Id
        +string Name
        +string Category
        +decimal Price
        +int Stock
    }

    class CartItem {
        +int ProductId
        +int Quantity
        +decimal UnitPrice
        +decimal LineTotal
    }

    class Order {
        +int Id
        +int UserId
        +List<OrderItem> Items
        +decimal TotalAmount
        +OrderStatus Status
        +DateTime CreatedAt
        +User User
    }

    class OrderItem {
        +int Id
        +int OrderId
        +int ProductId
        +int Quantity
        +decimal UnitPrice
        +decimal LineTotal
        +Order Order
        +Product Product
    }

    class Review {
        +int Id
        +int ProductId
        +int UserId
        +int Rating
        +string Comment
        +DateTime CreatedAt
        +Product Product
        +User User
    }

    class Payment {
        +int Id
        +int OrderId
        +decimal Amount
        +PaymentStatus Status
        +DateTime Timestamp
    }

    class Role {
        <<enumeration>>
        Customer
        Administrator
    }

    class OrderStatus {
        <<enumeration>>
        Placed
        Paid
        Cancelled
    }

    class PaymentStatus {
        <<enumeration>>
        Pending
        Success
        Failed
    }

    class ShoplioDbContext {
        +DbSet<User> Users
        +DbSet<Product> Products
        +DbSet<Order> Orders
        +DbSet<OrderItem> OrderItems
        +DbSet<Review> Reviews
    }

    Order "1" *-- "0..*" OrderItem : items
    OrderItem "*" --> "1" Product
    Review "*" --> "1" Product
    Review "*" --> "1" User
    Order "*" --> "1" User
    CartItem "*" --> "1" Product

    ShoplioDbContext --> User
    ShoplioDbContext --> Product
    ShoplioDbContext --> Order
    ShoplioDbContext --> OrderItem
    ShoplioDbContext --> Review