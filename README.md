# Fixy API — Home Maintenance Service Marketplace

**Fixy** is a modern, scalable, and enterprise-ready ASP.NET Core Web API built to power a service-matching platform connecting customers with home maintenance technicians.

---

## 🏗 Architectural Overview

The project follows **Clean Architecture** combined with **Domain-Driven Design (DDD)** principles to ensure maximum decoupling, testability, and scalability. The solution is divided into four main layers:

* **`Fixy.Domain`**: Contains all entities, enums, exceptions, interfaces, types, and logic specific to the domain layer.
* **`Fixy.Application`**: Contains business logic, MediatR commands/queries, validations, DTOs, and mapping profiles.
* **`Fixy.Infrastructure`**: Contains database persistence (EF Core Context, Migrations), generic repositories, Unit of Work, identity management, and external service implementations (Stripe, Hangfire, Firebase).
* **`Fixy.Api`**: The presentation layer containing RESTful controllers, minimal configurations, middlewares, and routing.

---

## 🛠 Core Tech Stack & Patterns

- **Framework**: .NET 10 / ASP.NET Core Web API
- **CQRS Implementation**: **MediatR** handles the decoupling of Commands (write operations) and Queries (read operations).
- **Data Access**: **Entity Framework Core (SQL Server)** with a robust **Repository Pattern** and **Unit of Work** for transaction management.
- **Functional Flow**: Employs the **Result Pattern** to standardize API responses without relying on exception throwing for business flow, coupled with **FluentValidation** for strongly-typed request validation.
- **Object Mapping**: **AutoMapper** for mapping Domain models to DTOs and vice versa.

---

## ✨ Key Features & Implementations

### Security & Authentication
- **JWT Bearer Tokens**: Stateless authentication for both Customers and Technicians.
- **OTP Verification**: Registration and mobile verifications bypass traditional email links in favor of SMS/Mobile One-Time Passwords.
- **Google External Login**: Seamless OAuth integration.

### Real-Time Communication
- **SignalR**: Utilized to facilitate live interactions. 
  - `ChatHub`: Dedicated customer-to-technician booking chat.
  - `NotificationHub`: Real-time push notifications across clients.

### Background Processing
- **Hangfire**: Orchestrates scheduled and background jobs (e.g., balance clearance, delayed notifications, state management transitions) complete with a dedicated monitoring dashboard.

### Payments
- **Stripe Integration**: Connects technicians and customers via secure Payment Intents, webhook event handlers, and platform commission splits.

### AI Integration
- **Fixy Assistant (Chatbot)**: Integrates with an external Python Flask AI microservice via `IChatbotService`. It supports dynamic AI prompting for conversational user guidance and intelligent service categorizations.

### Cross-Cutting Concerns
- **Global Exception Handling**: Centralized middleware to trap exceptions and return consistent error envelopes.
- **Structured Logging**: Powered by **Serilog** configured in `appsettings.json`.
- **Localization**: Full multi-language support (English and Egyptian Arabic) via built-in .NET `IStringLocalizer`.
- **API Versioning**: Enforced to support safe, backward-compatible iterations.
- **Rate Limiting & Pagination**: Built-in limits on endpoints and generalized pagination models for heavy lists.

---

## 🚀 Getting Started & Setup

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or Docker instance)
- Access to Firebase, Stripe, and Google Cloud Console (for external API keys).

### Configuration (`appsettings.Development.json` / Environment Variables)
Populate the following sections in your `appsettings.json` or as environment variables before running:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=FixyDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "HangfireConnection": "Server=...;Database=FixyHangfireDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "jwtSettings": {
    "Secret": "your-256-bit-secret-key-here",
    "Issuer": "fixy-api",
    "Audience": "fixy-clients"
  },
  "flaskApiSettings": {
    "ChatbotUrl": "http://localhost:5000/api/chatbot"
  },
  "stripeSettings": {
    "SecretKey": "sk_test_...",
    "PublishableKey": "pk_test_...",
    "WebhookSecret": "whsec_..."
  },
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id.apps.googleusercontent.com",
      "ClientSecret": "your-google-client-secret"
    }
  }
}
```

*Note: Ensure your `firebase-service-account.json` is properly referenced at the root of the API for push notifications to function.*

### Database Migration & Run
Open your terminal at the root of the solution and execute:
```bash
# Apply EF Core Migrations
dotnet ef database update --project src/Fixy.Infrastructure --startup-project src/Fixy.Api

# Run the Application
dotnet run --project src/Fixy.Api
```

---

## 📖 API Documentation & Testing

- **Swagger UI**: Accessible by navigating to `https://fixy-dwbmggafbkd0enbb.germanywestcentral-01.azurewebsites.net/swagger/index.html` when running in the Development environment. Swagger includes built-in JWT authorization buttons for easy route testing.
- **Postman**: A dedicated Postman Collection file is available in the documentation folder for rapid endpoint importing and automated local environment testing.
