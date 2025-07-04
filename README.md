
# Final Year Project - Modern E-commerce Backend

This repository contains the source code for the backend of a feature-rich, modern e-commerce platform. Built with ASP.NET Core Web API, it provides a robust, scalable, and secure foundation for the corresponding [Next.js Frontend](https://github.com/unsignedbuntu/FinalYearProject_Frontend).

## 🚀 Key Features

This backend powers the entire e-commerce experience, from user management to advanced AI-driven features.

### Core E-commerce Functionality

-   **RESTful API Design:** A complete set of endpoints for managing Products, Categories, Suppliers, and Stores.
-   **Order Processing:** Transactional order creation to ensure data integrity, including stock quantity checks, invoice generation, and order history tracking.
-   **Shopping Cart & Favorites:** Endpoints to manage a persistent shopping cart and multiple user-specific favorite lists.
-   **Product Review System:** Allows authenticated users to submit ratings and written reviews for products they have purchased.
-   **Search Functionality:** A dedicated search endpoint to query products across the catalog.

### User Management & Security

-   **ASP.NET Core Identity:** Handles all aspects of user authentication and management, including registration, login, and password handling.
-   **JWT Authentication:** Secure, stateless authentication using JSON Web Tokens, delivered to the client via `HttpOnly` cookies for enhanced security.
-   **User Profile Management:** Comprehensive endpoints for users to manage their personal information, shipping addresses, and to follow/unfollow suppliers.
-   **Support System:** A support ticket system for users to communicate with administrators.

### Advanced & Decoupled Features

-   **AI Image Generation Caching:** A sophisticated multi-layer caching strategy for AI-generated images. It checks a high-speed **Redis cache** first, then a persistent **database cache**, before invoking the costly image generation model. This is managed by the `ImageCacheController`.
-   **Python Microservice Integration:** The backend acts as a gateway for a **Python-based gamified loyalty program**. It receives requests from the frontend and routes them to the separate Python service, promoting a decoupled and scalable architecture.

### System & Architecture

-   **Entity Framework Core:** Utilizes a Code-First approach with `KTUN_DbContext` to define the database schema, relationships, and constraints.
-   **Clean Architecture Principles:** The project is structured with a clear separation of concerns, using DTOs (Data Transfer Objects) for API contracts and entities for the database model.
-   **AutoMapper:** Streamlines the mapping process between DTOs and database entities.
-   **Centralized Configuration:** Manages database connection strings, JWT secrets, and other settings via `appsettings.json`.
-   **API Documentation:** Integrated **Swagger (OpenAPI)** for interactive API documentation and easy testing of all endpoints.
-   **HTTPS & Certificate Management:** Custom service for loading SSL certificates to enable secure HTTPS communication in development and production.

## 🛠️ Tech Stack

| Category                 | Technology / Library                                 |
| ------------------------ | ---------------------------------------------------- |
| **Framework**            | ASP.NET Core Web API                                 |
| **Language**             | C#                                                   |
| **Database**             | Microsoft SQL Server                                 |
| **ORM**                  | Entity Framework Core                                |
| **Authentication**       | ASP.NET Core Identity, JWT (JSON Web Tokens)         |
| **API Documentation**    | Swagger (Swashbuckle)                                |
| **Caching**              | Redis (`IDistributedCache`)                          |
| **Mapping**              | AutoMapper                                           |
| **Microservice Communication** | `HttpClient` (for Python service)                    |
| **Web Server**           | Kestrel                                              |

## 📂 Project Structure

The project follows a standard, scalable ASP.NET Core directory structure.

```
/
├── Controllers/        # API endpoints (e.g., AuthController, ProductsController)
├── DTOs/               # Data Transfer Objects for API requests
├── ResponseDTOs/       # Data Transfer Objects for API responses
├── Entities/           # EF Core entity classes representing DB tables
├── Services/           # Business logic services (e.g., RedisService)
├── Mapper/             # AutoMapper profiles
├── Migrations/         # EF Core database migrations
├── Properties/         # Launch settings for development (launchSettings.json)
├── KTUN_DbContext.cs   # The main database context for EF Core
├── Program.cs          # Main application entry point, service configuration
└── appsettings.json    # Application configuration
```

## 🖼️ Project Gallery

*This section is ready for you to add screenshots of the Swagger UI, database diagrams, or key API responses.*

---
*(Example: A screenshot of the Swagger UI showing the Products endpoints)*
`![Swagger UI](./docs/swagger-products.png)`

---

## 📜 API Endpoints

Here is a comprehensive list of all available API endpoints, grouped by their respective controllers.

### Addresses
- `GET /api/Addresses`
- `POST /api/Addresses`
- `GET /api/Addresses/{id}`
- `PUT /api/Addresses/{id}`
- `DELETE /api/Addresses/{id}`

### Auth
- `POST /api/Auth/register`
- `POST /api/Auth/login`
- `GET /api/Auth/me`
- `POST /api/Auth/logout`

### Cart
- `GET /api/Cart`
- `POST /api/Cart`
- `DELETE /api/Cart/{productId}`
- `DELETE /api/Cart/clear`

### Categories
- `GET /api/Categories`
- `POST /api/Categories`
- `GET /api/Categories/{id}`
- `PUT /api/Categories/{id}`
- `GET /api/Categories/ByStore/{storeId}`
- `DELETE /api/Categories/SoftDelete_Status/{id}`

### FavoriteLists
- `GET /api/FavoriteLists/users/{userId}`
- `POST /api/FavoriteLists/users/{userId}`
- `GET /api/FavoriteLists/public`
- `GET /api/FavoriteLists/{listId}`
- `PUT /api/FavoriteLists/{listId}`
- `DELETE /api/FavoriteLists/{listId}`
- `GET /api/FavoriteLists/{listId}/products`
- `POST /api/FavoriteLists/{listId}/products`
- `DELETE /api/FavoriteLists/{listId}/products/{productId}`

### Favorites
- `GET /api/Favorites`
- `POST /api/Favorites`
- `DELETE /api/Favorites/{productId}`

### ImageCache
- `GET /api/ImageCache/{id}`
- `GET /api/ImageCache/prompt/{prompt}`
- `POST /api/ImageCache`
- `GET /api/ImageCache/products`
- `GET /api/ImageCache/product/{productId}`
- `GET /api/ImageCache/supplier/{supplierId}`
- `DELETE /api/ImageCache/{id}`

### ImageServe
- `GET /api/ImageServe/{productId}`

### Inventory
- `GET /api/Inventory`
- `POST /api/Inventory`
- `GET /api/Inventory/{id}`
- `PUT /api/Inventory/{id}`
- `DELETE /api/Inventory/SoftDelete_Status/{id}`

### LoyaltyPrograms
- `GET /api/LoyaltyPrograms`
- `POST /api/LoyaltyPrograms`
- `GET /api/LoyaltyPrograms/{id}`
- `PUT /api/LoyaltyPrograms/{id}`
- `DELETE /api/LoyaltyPrograms/SoftDelete_Status/{id}`

### OrderItems
- `GET /api/OrderItems`
- `POST /api/OrderItems`
- `GET /api/OrderItems/{id}`
- `PUT /api/OrderItems/{id}`
- `DELETE /api/OrderItems/SoftDelete_Status/{id}`
- `GET /api/OrderItems/ByOrder/{orderId}`
- `GET /api/OrderItems/ByProduct/{productId}`

### Orders
- `GET /api/Orders`
- `POST /api/Orders`
- `GET /api/Orders/{id}`
- `PUT /api/Orders/{id}`
- `GET /api/Orders/ByUser/{userId}`
- `GET /api/Orders/ByStatus/{status}`
- `DELETE /api/Orders/SoftDelete_Status/{id}`
- `PUT /api/Orders/cancel/{id}`

### Products
- `GET /api/Products`
- `POST /api/Products`
- `GET /api/Products/{id}`
- `PUT /api/Products/{id}`
- `GET /api/Products/ByCategory/{categoryId}`
- `GET /api/Products/ByStore/{storeId}`
- `DELETE /api/Products/SoftDelete_Status/{id}`
- `GET /api/products/top-reviewed`

### ProductRecommendations
- `GET /api/ProductRecommendations`
- `POST /api/ProductRecommendations`
- `GET /api/ProductRecommendations/{id}`
- `PUT /api/ProductRecommendations/{id}`
- `DELETE /api/ProductRecommendations/SoftDelete_Status/{id}`

### ProductSuppliers
- `GET /api/ProductSuppliers`
- `POST /api/ProductSuppliers`
- `GET /api/ProductSuppliers/{id}`
- `PUT /api/ProductSuppliers/{id}`
- `DELETE /api/ProductSuppliers/SoftDelete_Status/{id}`

### Reviews
- `GET /api/Reviews`
- `POST /api/Reviews`
- `GET /api/Reviews/ByProduct/{productId}`
- `GET /api/Reviews/ByUser/{userId}`
- `GET /api/Reviews/ByStore/{storeId}`
- `PUT /api/Reviews/{id}`
- `DELETE /api/Reviews/{id}`
- `GET /api/Reviews/me/reviewable-order-items`
- `GET /api/Reviews/details/{id}`

### Sales
- `GET /api/Sales`
- `POST /api/Sales`
- `GET /api/Sales/{id}`
- `PUT /api/Sales/{id}`
- `DELETE /api/Sales/SoftDelete_Status/{id}`

### SalesDetails
- `GET /api/SalesDetails`
- `POST /api/SalesDetails`
- `GET /api/SalesDetails/{id}`
- `PUT /api/SalesDetails/{id}`
- `DELETE /api/SalesDetails/SoftDelete_Status/{id}`

### Search
- `GET /api/Search`

### Stores
- `GET /api/Stores`
- `POST /api/Stores`
- `GET /api/Stores/{id}`
- `PUT /api/Stores/{id}`
- `DELETE /api/Stores/SoftDelete_Status/{id}`

### Suppliers
- `GET /api/Suppliers`
- `POST /api/Suppliers`
- `GET /api/Suppliers/{id}`
- `PUT /api/Suppliers/{id}`
- `DELETE /api/Suppliers/SoftDelete_Status/{id}`

### SupportMessages
- `GET /api/SupportMessages`
- `POST /api/SupportMessages`
- `GET /api/SupportMessages/{id}`
- `PUT /api/SupportMessages/{id}`
- `GET /api/SupportMessages/User/{userId}`
- `GET /api/SupportMessages/Open`
- `DELETE /api/SupportMessages/SoftDelete_Status/{id}`

### Test
- `GET /api/test/test-redis`

### UserFollowedSuppliers
- `GET /api/users/{userId}/followed-suppliers`
- `POST /api/users/{userId}/followed-suppliers/{supplierId}`
- `DELETE /api/users/{userId}/followed-suppliers/{supplierId}`

### UserInformation
- `GET /api/UserInformation`
- `PUT /api/UserInformation`

### UserLoyalty
- `GET /api/userLoyalty`
- `POST /api/userLoyalty`
- `GET /api/userLoyalty/{id}`
- `PUT /api/userLoyalty/{id}`
- `DELETE /api/userLoyalty/SoftDelete_Status/{id}`

### Users
- `GET /api/Users`
- `POST /api/Users`
- `GET /api/Users/{id}`
- `PUT /api/Users/{id}`
- `DELETE /api/Users/SoftDelete_Status/{id}`

### UserStore
- `GET /api/UserStore`
- `POST /api/UserStore`
- `GET /api/UserStore/{id}`
- `PUT /api/UserStore/{id}`
- `DELETE /api/UserStore/{id}`


## ⚙️ Getting Started

Follow these instructions to get the backend up and running on your local machine.

### Prerequisites

-   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (or newer)
-   [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express, Developer, or full edition)
-   An IDE like [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with the C# Dev Kit.

### Installation

1.  **Clone the repository:**
    ```sh
    git clone https://github.com/your-username/your-backend-repo.git
    cd your-backend-repo
    ```

2.  **Configure User Secrets or `appsettings.Development.json`:**
    This project requires configuration for the database connection and JWT. The recommended approach is to use .NET User Secrets.

    a. **Initialize User Secrets:**
    ```sh
    dotnet user-secrets init
    ```

    b. **Set the Database Connection String:**
    Replace the server, database name, user, and password with your SQL Server details.
    ```sh
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=FinalYearProjectDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True;"
    ```

    c. **Set the JWT Configuration:**
    Use strong, randomly generated keys for a real application.
    ```sh
    dotnet user-secrets set "Jwt:Key" "A_VERY_LONG_AND_SECURE_SECRET_KEY_GOES_HERE"
    dotnet user-secrets set "Jwt:Issuer" "https://your-domain.com"
    dotnet user-secrets set "Jwt:Audience" "https://your-domain.com"
    ```

    d. **(Optional) Set Redis Configuration:**
    If you are running Redis on a different port or server.
    ```sh
    dotnet user-secrets set "Redis:ConnectionString" "localhost:6379"
    ```

    > **Alternative:** You can also paste these settings directly into the `appsettings.Development.json` file, but using User Secrets is safer as it keeps sensitive data out of the project directory.

3.  **Setup the Database:**
    Apply the Entity Framework Core migrations to create the database schema.
    ```sh
    dotnet ef database update
    ```
    This command will read the connection string you set and build the database and all its tables.

4.  **Run the Application:**
    You can run the project from your IDE (e.g., by pressing F5 in Visual Studio) or by using the command line:
    ```sh
    dotnet run
    ```

The API should now be running. You can access the Swagger UI at the URL specified in `Properties/launchSettings.json` (e.g., `https://localhost:7199/swagger`).
