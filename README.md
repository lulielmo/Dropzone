# Dropzone

Windows desktop app that routes drag-and-drop input (URLs or files) to configurable Python-backed jobs, then shows the result in a matching view.

The goal is a **generic host**: add support for new Python solutions primarily through configuration. When config is not enough, extend with a new job handler (`IJobHandler`) and/or a new result view.

## Architecture overview

```
Drop / URL or file
        │
        ▼
  Job matching (dropzone.config.json)
        │
        ▼
  Handler (IJobHandler)  →  Python script / processing
        │
        ▼
  View (e.g. GridAndCommentView)  ←  JobResult
```

| Layer | Responsibility | Extends via |
|-------|----------------|-------------|
| **Job** | Business case: matching rules + display name | Config entry (user picks if several match) |
| **Handler** | Execution strategy: how input is processed | New `IJobHandler` when needed |
| **View** | Presentation: how `JobResult` is shown | New view control when needed |

See [DEVELOPMENT.md](DEVELOPMENT.md) for naming conventions and how to add jobs, handlers, and views.

## Requirements

- .NET 9 SDK
- Windows (WinForms)
- Python available on `PATH` (or configured per job via `pythonExe`) when running Python-backed jobs

## Quick start

1. Open `Dropzone.sln` in Visual Studio / Cursor, or build from the command line:

```bash
dotnet build Dropzone.sln
```

2. Edit `Dropzone/Config/dropzone.config.json` and point `handlerConfig.pythonScript` (and optionally `pythonExe`) at your script.

3. Run the app:

```bash
dotnet run --project Dropzone
```

4. Drag a matching URL or file onto the window. Idle is a compact drop target; a result grows the window. When you are finished, click **Done** to clear it and return to idle. Minimize to send Dropzone to the system tray; double-click the tray icon to bring it back (always on top while visible).

## Configuration

Jobs live in `Dropzone/Config/dropzone.config.json` (copied to the output directory on build). Click **Configuration** in the app to open the file the running instance loads — the project file when you F5 from this repo, otherwise the output copy. The next drop re-reads it; no restart needed.

Each job can define:

- `name` — human-readable business case label
- `urlRegex` / `domainName` / `fileNameRegex` / `fileExtension` / `textRegex` — matching rules
- `handlerType` — registered handler class name (currently `PythonScriptHandler`)
- `viewType` — result view to show
- `handlerConfig` — handler-specific settings (e.g. `pythonScript`, `pythonExe`, `workingDirectory`, `inputKind`)

For uv-based Python projects, set `pythonExe` to the project’s `.venv\Scripts\python.exe` and `workingDirectory` to the project root. See [DEVELOPMENT.md](DEVELOPMENT.md).

## Tests

```bash
dotnet test
```

More detail: [Dropzone.Tests/README.md](Dropzone.Tests/README.md).

## Project docs

| File | Purpose |
|------|---------|
| [DEVELOPMENT.md](DEVELOPMENT.md) | Conventions, architecture extension guide |
| [TODO.md](TODO.md) | Shared backlog |
| `.cursor/rules/` | Cursor agent rules |
