# AGENTS.md

.NET 10 Minimal API URL shortener. Clean Architecture, no tests / lint / CI in repo.

## Structure

- `UrlShortener.API/` — sole entrypoint (`Program.cs`). Thin Minimal API endpoints in `Endpoints/` (`AuthEndpoints.cs`, `UrlShortenerEndpoints.cs`), DI in `DependencyInjection.cs`, `Usings.cs` (global usings). No controllers.
- `UrlShortener.Application/` — `Abstractions/` (service interfaces) + `Services/` implementations, `DTOs/`, `Settings/`. Registered via `AddApplication()`.
- `UrlShortener.Domain/` — `Entities/` (`User`, `ShortenedUrl`, `VisitedUrl`), `Contracts/` (repository/unit-of-work interfaces). No dependencies.
- `UrlShortener.Infrastructure/` — EF Core `Data/ApplicationDbContext.cs` + `Data/Configurations/`, `Migrations/`, `Repositories/`, JWT auth, HybridCache/Redis. Registered via `AddInfrastructure(configuration)`.
- Dependency direction: API → Application → Domain ← Infrastructure.

## Run

Requires .NET 10 SDK (pinned via `global.json`, `rollForward: latestFeature`). Infra (SQL Server `:1433`, Redis `:6379`, Seq `:5341`) must be up first:

```bash
docker compose up -d
dotnet build UrlShortener.slnx
dotnet run --project UrlShortener.API          # http://localhost:5139, Swagger only in Development
```

`docker compose up -d` also builds/runs the API on `:8080` (`ASPNETCORE_ENVIRONMENT=Development`).

## Config gotchas

- `appsettings.json` ships with empty `ConnectionStrings`, `JwtSettings:Secret`, and Seq `serverUrl`. Real dev values live in `appsettings.Development.json` (SA password `MyStrongPassword@123`, JWT secret, `localhost:6379` / `localhost:5341`). Always run with `ASPNETCORE_ENVIRONMENT=Development` or override via env (`ConnectionStrings__Default`, `ConnectionStrings__Redis`, `JwtSettings__Secret`, `Serilog__WriteTo__1__Args__serverUrl`) as in `compose.yaml`.
- Migrations auto-apply **only** in Development (`Program.cs` → `ApplyMigrations()`). Outside Development, apply manually.
- EF tooling: DbContext is in Infrastructure, startup is API. From repo root:
  ```bash
  dotnet ef migrations add <Name> --project UrlShortener.Infrastructure --startup-project UrlShortener.API
  dotnet ef database update --project UrlShortener.Infrastructure --startup-project UrlShortener.API
  ```

## Conventions

- Add routes as Minimal API extension methods on `IEndpointRouteBuilder` in `UrlShortener.API/Endpoints/`, grouped via `app.MapGroup("/api")` in `Program.cs`. Keep handlers thin: validate input in the endpoint, delegate to `Application/Services/`.
- DI lives in three `DependencyInjection.cs` files (API / Application / Infrastructure) — register in the layer that owns the implementation.
- `GET /{code}` at root is a redirect shim (excluded from OpenAPI) that forwards to `/api/url-shortener/{code}`. Don't reorder it above `/api` mapping carelessly.
- `url-shortener` group is `AllowAnonymous`; only `GET /api/auth/user/{id}` requires the `admin` role.
- JWT validation intentionally sets `ValidateIssuer=false, ValidateAudience=false` with lifetime + signing-key validation (`Infrastructure/DependencyInjection.cs`). Do not "fix" without confirmation.
- Serilog is configured via `Serilog` appsettings section + `UseSerilogRequestLogging()`; Seq sink fails silently if Seq is down, but `serverUrl: ""` (non-Development) means no Seq logging.
