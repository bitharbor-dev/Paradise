# Paradise - .NET back-end template

## Overview

An open-source back-end template designed to accelerate development
by providing a structured foundation with prebuilt core features.
It allows developers to focus on business logic instead of repetitive infrastructure concerns.

The template includes ready-to-use implementations for authentication, user and role management,
and system bootstrapping.

It also provides a full set of unit tests, giving 90%+ code coverage.

## Features

- JWT-based authentication
- Token refresh flow
- User management
- Role management
- Domain events (in-memory, extensible to external message bus)
- Automatic database migrations
- Smart data seeding with update support

## Architecture

The project follows a hybrid architecture combining best of:

- Domain-Driven Design (DDD)
- Clean Architecture principles

This provides strong boundaries and maintainability without
introducing distributed system complexity.

## Tech stack

- .NET 10
- ASP.NET Core
- Entity Framework Core
- MS SQL Server
- In-memory domain events (channel-based)
- Azure Functions (included project)

## Getting started

### Prerequisites

- .NET 10 SDK
- MS SQL Server
- Docker (optional)

### Configuration

Configuration is split into:

- `appsettings.json` — application-level configuration
- `options.json` — business logic and infrastructure configuration

Update connection strings and required settings before running.

### Database

Migrations are executed automatically on startup. Data is seeded from the `SeedData` directory.
If seed files change, existing records are updated accordingly instead of duplicated.

### Project structure

```
/Source
  /Code
    /Core - core application components. 
    /Intermediate - prebuilt .NET API client.
    /Shared - shared application components.
    /Web - executable applications.
  /Tests
    /Integration - integration tests (WIP).
    /Supplemental - tests infrastructure and doubles.
    /Unit - unit tests.
```

### Deployment

#### Supported environments:

- Local development
- Docker-based environments
- Cloud (Azure-oriented setup)

#### Target Audience

- Developers building back-end systems from scratch
- Teams standardizing architecture across services
- Projects requiring a production-ready starting point

#### License

MIT. Read LICENSE.txt