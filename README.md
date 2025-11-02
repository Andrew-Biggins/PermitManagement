# PermitManagement

## Running the Solution

The repository includes three launch profiles wired up so that the API starts automatically alongside the chosen client.

1. **Console** – launches `PermitManagement.Api` and the console UI (`PermitManagement.Console`).
2. **WPF** – launches `PermitManagement.Api` and the WPF desktop app (`PermitManagement.Desktop`).
3. **MAUI** – launches `PermitManagement.Api` and the cross-platform MAUI client (`PermitManagement.Maui`).

Pick the profile you want in Visual Studio (or `dotnet run --launch-profile "<Name>"`) and the tooling will handle starting both processes.

## Design Philosophy

- **PermitManagement.Core** – domain entities and core logic shared across every layer.
- **PermitManagement.Infrastructure** – data access and API orchestration that drive the core entities.
- **PermitManagement.Shared** – cross-cutting helpers, constants, and validation rules consumed by presentation layers and services.
- **PermitManagement.Presentation** – UI-agnostic view-models (`PermitViewModel`, `RelayCommand`, etc.) consumed by all front-ends.
- **PermitManagement.Api** – ASP.NET Core service exposing the permit endpoints used by each client.
- **PermitManagement.Desktop** – WPF implementation that binds directly to the presentation project via dependency injection.
- **PermitManagement.Console** – lightweight console UI that reuses the same presentation abstractions for quick interactions.
- **PermitManagement.Maui** – cross-platform client that mirrors the WPF experience while sharing presentation logic.
- **Testing Projects** – unit/integration suites target their respective layers (`*.UnitTests`, `PermitManagement.Api.IntegrationTests`, `PermitManagement.Testing.Shared`).

Each UI project references `PermitManagement.Presentation` so view-model behaviour remains identical, while both presentation and infrastructure depend on `PermitManagement.Core` to avoid duplication.

## Testing Philosophy

- **Unit Tests** focus on domain entities, shared utilities, and presentation logic (`*.UnitTests` projects).
- **Integration Tests** (`PermitManagement.Api.IntegrationTests`) exercise API endpoints against realistic data paths.
- **Shared Test Utilities** live in `PermitManagement.Testing.Shared` to keep fixtures and builders consistent.
- Prefer testing behaviour at the boundary of each layer (API contract, view-model commands) so refactors inside remain safe.
- Run `dotnet test` at the solution root to execute the full test suite before merging significant changes.
