# Employee Management API

## Overview
An ASP.NET Core Web API for managing employee records using Entity Framework Core and SQL Server.

## Technologies
- C#
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SQL Server LocalDB
- JWT Bearer Authentication
- Swagger / OpenAPI

## Features
- Create, read, update and delete employee records
- Employee input validation
- Unique email constraint
- Pagination for employee listing
- Department-wise employee count
- JWT protection for create, update and delete endpoints
- Global exception handling and logging
- Swagger API documentation

## Database Setup
1. Configure `DefaultConnection` in `appsettings.json`.
2. Open Visual Studio Package Manager Console.
3. Run `Update-Database` to apply the existing EF Core migrations.

## Run the Application
1. Open the project in Visual Studio.
2. Restore NuGet packages.
3. Build and run the application using F5.
4. Open the Swagger URL shown by Visual Studio.

## Authentication
For local testing, generate a JWT using the configured authentication endpoint and authorize protected requests in Swagger.

**Security note:** The current test-token endpoint is for local assignment testing only. Replace it with real credential verification before production deployment. Store signing keys securely.

## SQL Injection Prevention
EF Core LINQ queries use parameterized database operations. Avoid concatenating untrusted input into raw SQL queries.

## Error Handling and Logging
Unhandled exceptions are logged on the server, while API clients receive a generic error response.
