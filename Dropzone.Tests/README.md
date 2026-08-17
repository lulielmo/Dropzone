# Dropzone Tests

This directory contains unit tests for the Dropzone application.

## Test Structure

The test project mirrors the structure of the main Dropzone project:

- `Models/` - Tests for model classes (RowModel, JobConfig, JobResult)
- `Services/` - Tests for service classes (DownloadService, TempFileService, PythonProcessService)
- `Forms/` - Tests for main window behaviour (Done action / idle reset)
- `Views/` - Tests for result views (GridAndCommentView diagnostics)
- `Handlers/` - Tests for handler classes (AteaInvoiceHandler)
- `Config/` - Tests for configuration loading (ConfigLoader)

## Test Frameworks

- **xUnit** - Primary test framework
- **FluentAssertions** - Fluent assertion library for readable test assertions
- **Moq** - Mocking framework (for future use with more complex dependencies)
- **Coverlet** - Code coverage collection (already included)

## Running Tests

### From Command Line

```bash
# Run all tests
dotnet test

# Run with code coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test class
dotnet test --filter "FullyQualifiedName~RowModelTests"
```

### From Visual Studio

Use the Test Explorer (Test → Test Explorer) or right-click on the test project and select "Run Tests".

## Code Coverage

To generate code coverage reports:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./TestResults/
```

Then open the coverage report in Visual Studio Code using extensions like "Coverage Gutters" or view it in Visual Studio's Code Coverage Results window.

## Test Coverage Goals

We aim for high code coverage, with special focus on:

- ✅ **Models** - 100% coverage (simple data classes)
- ✅ **Services** - High coverage with mocking of external dependencies
- ✅ **Handlers** - High coverage with integration-style tests where needed
- ✅ **Config** - 100% coverage (configuration loading logic)

## Notes

- Some services (like DownloadService and PythonProcessService) require external dependencies (HTTP endpoints, Python installation). Consider refactoring to inject dependencies for better testability.
- Integration tests may be added in a separate test project for end-to-end scenarios.

