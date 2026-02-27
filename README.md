# E-Commerce Web Application

A full-featured e-commerce web application built with ASP.NET Core MVC, Entity Framework Core, and modern web technologies.

## About The Project

This project is a fully functional online shop (TechShop) that demonstrates professional web development practices including clean architecture, design patterns, and modern UI/UX implementations. The application supports user authentication, role-based authorization, shopping cart management, order processing, and an administrative dashboard.

## Features

### Customer Features
- Browse products with pagination and category filtering
- Search products by name or description
- View detailed product information with images and pricing
- Add items to shopping cart with stock validation
- Apply discount coupons at checkout
- Complete orders with real-time inventory updates
- View order history

### Admin Features
- Manage products (Create, Read, Update, Delete)
- Manage categories
- Track inventory levels
- Update product discounts
- View all customer orders
- Manage user accounts

## Built With

- ASP.NET Core 8.0 MVC
- Entity Framework Core
- ASP.NET Core Identity
- SQL Server
- Bootstrap 5
- jQuery & AJAX
- Toastr.js & SweetAlert2

## Architecture & Design Patterns

- **MVC Pattern**: Clear separation of concerns between Models, Views, and Controllers
- **Repository Pattern**: Abstraction layer for data access operations
- **Generic Repository**: Base repository implementation for common CRUD operations
- **Dependency Injection**: Built-in ASP.NET Core DI container for service management
- **Async/Await**: Asynchronous programming throughout for better scalability
- **ViewModels**: Dedicated classes for data transfer between controllers and views
- **ViewComponents**: Reusable UI components (shopping cart summary, category dropdown)

## Database Schema

The application uses Entity Framework Core with the following main entities:
- **ApplicationUser**: Extended ASP.NET Identity user
- **Product**: Product information with pricing and inventory
- **Category**: Product categorization
- **Order**: Customer orders
- **OrderItem**: Individual items within orders
- **ShoppingCartItem**: Temporary cart storage
- **Coupon**: Discount codes

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022, VS Code, or JetBrains Rider

### Installation

1. Clone the repository
   ```bash
   git clone https://github.com/yourusername/webshop-owp.git
   ```

2. Navigate to the project directory
   ```bash
   cd webshop-owp
   ```

3. Update the connection string in `appsettings.json`
   ```json
   "ConnectionStrings": {
     "DefaultConnectionString": "Server=YOUR_SERVER;Database=webshop-db;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

4. Apply database migrations
   ```bash
   dotnet ef database update
   ```

5. Run the application
   ```bash
   dotnet run
   ```

6. Access the application at `https://localhost:5001` or the URL shown in terminal

### Default Login Credentials

**Admin Account:**
- Email: admin@techshop.com
- Password: Coding@1234?

**User Account:**
- Email: user@techshop.com
- Password: Coding@1234?

## Project Structure

```
webshop-owp/
├── Controllers/          # MVC Controllers
├── Data/
│   ├── Base/            # Generic repository and base classes
│   ├── Cart/            # Shopping cart logic
│   ├── Services/        # Business logic services
│   ├── Static/          # Constants and enums
│   ├── ViewComponents/  # Reusable UI components
│   └── ViewModels/      # Data transfer objects
├── Models/              # Domain entities
├── Views/               # Razor views
└── wwwroot/             # Static files (CSS, JS, images)
```

## Key Implementation Details

### Pagination
Custom `PaginatedList<T>` implementation with efficient database queries using `Skip()` and `Take()` to minimize memory usage.

### Shopping Cart
Session-based cart management with persistent storage in database. Cart items are linked to a unique session ID stored as a browser cookie.

### Authentication & Authorization
ASP.NET Core Identity with custom `ApplicationUser` model. Role-based access control with Admin and User roles. Middleware pipeline configured for proper authentication flow.

### AJAX Integration
Dynamic cart operations without page refreshes. Real-time feedback using Toastr notifications and SweetAlert2 confirmations.

## License

This project was developed for educational purposes as part of a university web programming course.

## Contact

Your Name - dejan.zegarac0@gmail.com

Project Link: (https://github.com/dejan3dejan/webshop-owp)
