# Todo Application

A simple Todo application built with ASP.NET Core that demonstrates CRUD operations, logging, and different environment configurations.

## Features

- Create, Read, Update, and Delete Todo items
- RESTful API with Swagger documentation
- Logging with ILogger and HttpLogging
- Different environment configurations (Development, Staging, Production)
- Azure SQL Database support
- Modern UI with Bootstrap

## Prerequisites

- .NET 9.0 SDK or later
- SQL Server (LocalDB for development)
- Visual Studio 2022 or VS Code (optional)

## Getting Started

1. Clone the repository
2. Navigate to the project directory
3. Run the application:
   ```bash
   dotnet run
   ```
4. Access the application:
   - Web UI: https://localhost:5001
   - Swagger UI: https://localhost:5001/swagger

## Environment Configuration

The application supports three environments:

- Development (appsettings.Development.json)
- Staging (appsettings.Staging.json)
- Production (appsettings.Production.json)

To switch environments:

```bash
set ASPNETCORE_ENVIRONMENT=Development
# or
set ASPNETCORE_ENVIRONMENT=Staging
# or
set ASPNETCORE_ENVIRONMENT=Production
```

## Database Configuration

Update the connection string in the appropriate appsettings.{Environment}.json file:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your-Connection-String"
  }
}
```

## Azure Deployment

1. Create an Azure SQL Database
2. Update the connection string in appsettings.Production.json
3. Deploy to Azure App Service:
   ```bash
   az webapp up --name your-app-name --resource-group your-resource-group
   ```

## API Endpoints

- GET /api/todo - Get all todos
- GET /api/todo/{id} - Get a specific todo
- POST /api/todo - Create a new todo
- PUT /api/todo/{id} - Update a todo
- DELETE /api/todo/{id} - Delete a todo

## Logging

The application uses:
- ILogger for application logging
- HttpLogging middleware for HTTP request/response logging

Logs are configured differently for each environment:
- Development: Detailed logging
- Staging: Information level
- Production: Warning and Error only

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request 