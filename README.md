# Paradise - .NET Back-End Template with 3 Layer Architecture and DDD "Lite" Pattern
This is a template for creating .NET back-end solutions using a 3-layer architecture and Domain-Driven Design (DDD) pattern. The template is built with Visual Studio and targets .NET 9.

## Getting Started
To use this template, you can either download the source code or create a new project based on this template using Visual Studio.

## Architecture
The template is designed with a 3 layer architecture:

- `ApplicationLogic` layer: This layer contains the application services and any other logic specific to the application.
- `DataAccess` layer: This layer contains the repositories, which handle data access to the database or any other data source.
- `Domain` layer: This layer contains the entities, value objects, and domain services, which model the business concepts and logic.

---

In addition to the `ApplicationLogic`, `DataAccess`, and `Domain` layer structure, this template also incorporates the DDD pattern, which emphasizes the importance of the domain layer and its models.

## Configuration

This template uses the `options.json` file for configuration. You can modify this file to configure the connection string for the database and other settings.

## Dependencies

This template depends on the following libraries:

- Swashbuckle.AspNetCore
- Microsoft.ApplicationInsights.AspNetCore
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.AspNetCore.Mvc.Core
- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.Extensions.Configuration.Json
- Microsoft.Extensions.Hosting
- Microsoft.Extensions.Identity.Core
- Microsoft.Extensions.Identity.Stores
- Microsoft.Extensions.Options
- Microsoft.VisualStudio.Azure.Containers.Tools.Targets

## License
This template is licensed under the MIT License. See the LICENSE file for details.