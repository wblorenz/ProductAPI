# ProductAPI

A product management system built with .NET 10 and .NET Aspire.

### Projects

- **`AspireApp.AppHost`**: Orchestrates services with .NET Aspire, provisioning the PostgreSQL database and application services.
- **`AspireApp.Server`**: ASP.NET Core Web API exposing HTTP endpoints, Swagger UI, and request validation.
  - On a real system would likely separate those concerns into more projects. And also would likely have added some mapping library like AutoMapper.

- **`App.Database`**: Data access layer using Entity Framework Core, managing PostgreSQL migrations and repository implementations.
- **`App.Model`**: Core domain layer defining business entities, domain rules, exceptions, and repository interfaces.
  - On a real system I would separate this into different domains, but for a small project I kept it simple.

- **`App.Test`**: Integration and unit test suite running against isolated PostgreSQL containers using Testcontainers.
  - Never had used Testcontainers before. It seems really powerful.