# Modernization Notes

This repository is being modernized in phases before any Avalonia migration.

## Current direction

- Runtime target: `.NET 10`
- Current UI shell during refactor: `WPF`
- Packaging direction: official modern .NET publish, not `Costura.Fody`
- Single-file release default:
  - `RuntimeIdentifier=win-x64`
  - `PublishSingleFile=true`
  - `SelfContained=true`
- Updater direction:
  - keep the current update XML compatibility during refactor
  - expose updates through `IUpdateService`
  - keep a legacy-compatible updater implementation first
  - prepare for a later Velopack-backed implementation

## Commands

- Build launcher: `dotnet build src/UminekoLauncher/UminekoLauncher.csproj`
- Build extractor: `dotnet build src/ZipExtractor/ZipExtractor.csproj`
- Run core tests: `dotnet run --project tests/UminekoLauncher.Core.Tests/UminekoLauncher.Core.Tests.csproj`
- Publish launcher: `dotnet publish src/UminekoLauncher/UminekoLauncher.csproj -p:PublishProfile=FolderProfile`

## Architecture direction

- `UminekoLauncher` stays as the temporary WPF shell.
- `UminekoLauncher.Core` contains config, manifest, checksum, and launch logic.
- `UminekoLauncher.Application` contains service contracts and application orchestration.
- UI-specific window and dialog code stays in the WPF project behind service adapters.
