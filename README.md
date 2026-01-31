# MotorStores Cheque System

## 🏗️ Architecture Overview

This project follows **Clean Architecture** principles with a layered approach, ensuring separation of concerns, maintainability, and testability.

## 📁 Project Structure

```
MotorStores.ChequeSystem/
│
├── MotorStores.Domain/              # Core Layer (No Dependencies)
│   ├── Common/                      # Base classes and abstractions
│   │   ├── BaseEntity.cs
│   │   └── AuditableEntity.cs
│   ├── Entities/                    # Domain entities (Cheque, Vendor, etc.)
│   └── Enums/                       # Domain enumerations (ChequeStatus, etc.)
│
├── MotorStores.Application/         # Business Logic Layer
│   ├── Interfaces/                  # Service contracts and interfaces
│   ├── Features/                    # CQRS Commands & Queries
│   │   ├── Cheques/
│   │   ├── Vendors/
│   │   └── BankAccounts/
│   ├── DTOs/                        # Data Transfer Objects
│   ├── Behaviors/                   # MediatR pipeline behaviors
│   ├── Mappings/                    # AutoMapper profiles
│   └── DependencyInjection.cs       # Service registration
│
├── MotorStores.Infrastructure/      # Data Access & External Services
│   ├── Persistence/                 # Database context & configurations
│   ├── Repositories/                # Repository implementations
│   ├── Services/                    # Infrastructure services (Print, Email)
│   └── DependencyInjection.cs       # Service registration
│
└── MotorStores.Api/                 # Presentation Layer
    ├── Controllers/                 # REST API controllers
    ├── Hubs/                        # SignalR hubs for real-time updates
    ├── Middlewares/                 # Custom middleware
    ├── Properties/
    │   └── launchSettings.json      # Launch profiles (HTTPS default)
    ├── appsettings.json
    └── Program.cs                   # Application entry point
```

## 🛠️ Technology Stack

- **.NET 8.0** - Latest LTS framework
- **ASP.NET Core Web API** - RESTful API
- **SignalR** - Real-time communication
- **Entity Framework Core** - ORM (to be added)
- **PostgreSQL/SQL Server** - Database (to be configured)
- **MediatR** - CQRS pattern (to be added)
- **AutoMapper** - Object mapping (to be added)
- **FluentValidation** - Input validation (to be added)

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 (17.8+) or Visual Studio Code
- PostgreSQL or SQL Server (for production)

### Running the Application

1. **Open the solution in Visual Studio:**
   ```
   Double-click: MotorStores.ChequeSystem.sln
   ```

2. **Set MotorStores.Api as startup project** (if not already set)

3. **Press F5 or click the green arrow (https)** to run
   - The application will launch at: `https://localhost:7196`
   - Swagger UI will open automatically at: `https://localhost:7196/swagger`

### Using dotnet CLI

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run the API
dotnet run --project MotorStores.Api
```

## 📋 Layer Dependencies

```
MotorStores.Api
    ↓ depends on
MotorStores.Application + MotorStores.Infrastructure
    ↓ depends on
MotorStores.Domain (Core - No dependencies)
```

## 🔧 Configuration

### Launch Settings

The project is configured to start with **HTTPS by default**. You can find the configuration in:
- `MotorStores.Api/Properties/launchSettings.json`

Available profiles:
- **https** (default): `https://localhost:7196`
- **http**: `http://localhost:5136`
- **IIS Express**: Uses IIS Express with SSL

### CORS Policy

CORS is enabled for React frontend with policy name: `AllowReactApp`
- Configured in `Program.cs`
- Allows all origins, methods, and headers (development mode)
- ⚠️ Restrict in production


## 🏢 Enterprise Patterns Used

✅ **Clean Architecture** - Dependency inversion and separation of concerns  
✅ **CQRS Pattern** - Command Query Responsibility Segregation  
✅ **Repository Pattern** - Data access abstraction  
✅ **Dependency Injection** - Loose coupling and testability  
✅ **Mediator Pattern** - Decoupled request/response handling  

## 📝 License

This project is proprietary software for Motor Stores.

## 👥 Team

- **Project**: Motor Stores Cheque Management System
- **Architecture**: Clean Architecture / Onion Architecture
- **Target Framework**: .NET 8.0

"# cheque-backend" 
