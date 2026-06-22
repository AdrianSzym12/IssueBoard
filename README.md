# IssueBoard

IssueBoard is a portfolio-oriented full-stack web application built with C# and .NET.

The goal of the project is to demonstrate clean backend and full-stack development practices using Onion Architecture, REST API design, authentication, persistence, testing, Docker, and a structured Git workflow.

## Project Status

Current stage:

```txt
Commit 1: Initial solution structure
The application currently contains the base solution layout and project structure. Business features will be added step by step in separate branches and pull requests.

Planned Features
User registration and login
JWT authentication
Workspace management
Project management
Issue tracking
Issue comments
Issue status changes
Issue assignment
Role-based workspace access
Project dashboard
Filtering and pagination
Blazor WebAssembly frontend
Docker Compose local environment
Unit and integration tests
Tech Stack
C#
.NET 8
ASP.NET Core Web API
Blazor WebAssembly
Entity Framework Core
PostgreSQL
JWT Authentication
MediatR
FluentValidation
Serilog
Docker
Docker Compose
xUnit
FluentAssertions
Testcontainers
Architecture

The project follows Onion Architecture.

Current solution structure:
IssueBoard/
  src/
    IssueBoard.Domain/
    IssueBoard.Application/
    IssueBoard.Infrastructure/
    IssueBoard.Api/
    IssueBoard.Web/

  tests/
    IssueBoard.UnitTests/
    IssueBoard.IntegrationTests/

  docs/

    Dependency rules:

Domain -> no dependencies
Application -> Domain
Infrastructure -> Application, Domain
Api -> Application, Infrastructure, Domain
Web -> communicates with Api through HTTP

Running Locally

At this stage, only the solution structure and a minimal API are available.

Visual Studio 2022
Open IssueBoard.sln.
Set IssueBoard.Api as the Startup Project.
Press F5.
Open the root endpoint or health endpoint.

Available endpoints:
GET /
GET /health

Terminal
dotnet build
dotnet run --project src/IssueBoard.Api/IssueBoard.Api.csproj
Tests

Test projects are already created:

tests/IssueBoard.UnitTests
tests/IssueBoard.IntegrationTests

Actual test cases will be added in later commits.

Git Workflow

The main branch should always contain a stable version.

Each feature or setup step is developed on a separate branch and merged through a Pull Request.

Current branch:

chore/initialize-solution
What I Learned

This section will be expanded as the project grows.

Future Improvements

This section will be expanded after the MVP is completed.

---

# 12. Dodaj folder docs

W folderze root solution utwórz fizyczny folder:

```txt
docs

Najprościej:

Kliknij prawym na solution:

IssueBoard

Wybierz:

Open Folder in File Explorer

Utwórz folder:

docs

W folderze docs utwórz plik:

docs/README.md

Wklej:

# IssueBoard Documentation

This folder will contain project documentation.

Planned documents:

- `architecture.md`
- `api-endpoints.md`
- `database-schema.md`

The documentation will be expanded in later commits.