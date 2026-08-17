# Development Guidelines

## Language policy

### Documentation (English)

All shared documentation must be in English, including:

- XML documentation comments
- README and DEVELOPMENT files
- API / architecture notes
- Code comments

### Communication (Swedish allowed)

Team communication can be in Swedish, including:

- Pull request discussions
- Code review comments
- Meetings
- Local notes (`dev-notes.md`)

### Code (English)

All code-related content must be in English, including:

- Namespaces, types, members
- Error messages and log messages
- Config keys that are part of the public contract (`handlerType`, `viewType`, etc.)

## Commit message conventions

All commit messages are in English.

We follow Conventional Commits:

- `feat:` — new functionality
- `fix:` — bug fixes
- `refactor:` — restructuring without changing behaviour
- `docs:` — documentation only
- `style:` — formatting only
- `test:` — tests
- `chore:` — maintenance
- `perf:` — performance
- `ci:` / `build:` — CI or build system

Optional scope:

- `feat(handler): Add PythonScriptHandler`
- `docs(dev): Document job/handler/view naming`

## Naming conventions: Job, Handler, View

Dropzone separates **what** the user cares about from **how** the host runs and shows it.

| Layer | What it names | Where | Example |
|-------|---------------|--------|---------|
| **Job** | Business case | `name` (and matching fields) in config | `"Atea Invoice License"` |
| **Handler** | Execution strategy | Class + `handlerType` | `PythonScriptHandler` |
| **View** | UI contract for `JobResult` | Control + `viewType` | `GridAndCommentView` |

Company and product names belong in the **job**, not in handler or view type names.

### Handlers — name by strategy, not customer

**Pattern:** `{Strategy}Handler`

Good:

- `PythonScriptHandler` — run a configured Python script on one input file
- `BatchFolderHandler` — process a folder (future)
- `HttpApiHandler` — call an HTTP API (future)

Avoid:

- Customer/vendor names in the type (`AteaInvoiceHandler`)
- Narrow domain names if the same code path is reusable (`InvoiceHandler` when it is really “run Python + return rows”)

**When to add a new `IJobHandler`:** only when the execution path or result contract cannot be expressed with config alone.

**When to add a new job only:** new business case that reuses an existing handler and view — add a config entry.

### Views — name by presentation contract

**Pattern:** `{WhatIsShown}View`

Examples: `GridAndCommentView`, `PdfPreviewView`, `ImagePreviewView`.

Handler and view should align on the **shape of `JobResult`** (rows + comment, file path, etc.), not on a vendor name.

### Result metadata

Prefer taking display title / type from the **job config** (e.g. `name`) rather than hard-coding vendor strings inside a generic handler.

## Current handler note

The first handler implementation is still named `AteaInvoiceHandler`. Its processing logic is already generic (config-driven Python execution). A rename to something like `PythonScriptHandler` is planned; until then, treat the class as the default Python script strategy despite the name.

## How to add a new job (config only)

1. Add an entry under `jobs` in `Dropzone/Config/dropzone.config.json`.
2. Set matching rules (`urlRegex`, `domainName`, `fileNameRegex`, and/or `fileExtension`).
3. Set `handlerType` and `viewType` to existing registered types.
4. Fill `handlerConfig` (script path, exe, working directory, etc.).
5. Run and drop a matching URL or file.

No C# change is required if handler and view already exist and are registered.

`fileNameRegex` is matched against the **file name only** (not the full path), for both local file drops and the file segment of a URL. Use it for system-specific attachment names (e.g. Medius `einvoicecapture-embedded-attachment`) without tying jobs to a Downloads folder.

### Python / uv projects

For Python jobs managed with [uv](https://docs.astral.sh/uv/), point config at the project’s synced virtualenv interpreter and set the project root as working directory:

```json
"handlerConfig": {
  "pythonExe": "C:\\path\\to\\Project\\.venv\\Scripts\\python.exe",
  "pythonScript": "C:\\path\\to\\Project\\src\\main.py",
  "workingDirectory": "C:\\path\\to\\Project"
}
```

- Create/update the env with `uv sync` in the Python project (outside Dropzone).
- `workingDirectory` ensures relative paths inside the script (`data/`, `logs/`, `output/`, etc.) resolve correctly.
- Dropzone invokes: `pythonExe "pythonScript" "inputFile"` with that working directory.
- Stdout/stderr are read as **UTF-8** (`PYTHONIOENCODING` / `PYTHONUTF8` are also set for the child process). Scripts should emit UTF-8 JSON (`ensure_ascii=False` in Python is fine).

**Script contract (implemented in the Python project, not in Dropzone):** accept the input file path as a CLI argument and write a single JSON object to stdout. Log to stderr or files so stdout stays valid JSON.

Row objects follow Medius Excel column order (A–J):

```json
{
  "success": true,
  "comment": "Medius comment text with \\n line breaks",
  "rows": [
    {
      "konProj": "5420",
      "empty1": "",
      "rg": "10200",
      "aktivitet": "738",
      "projAkt": "",
      "ean": "",
      "projKat": "",
      "empty2": "",
      "netto": "144,21",
      "godkantAv": "John Munthe"
    }
  ]
}
```

| JSON | Grid / Excel |
|------|----------------|
| `konProj` | Kon/Proj |
| `empty1` | (empty column B) |
| `rg` | RG |
| `aktivitet` | Aktivitet |
| `projAkt` | ProjAkt |
| `ean` | EAN |
| `projKat` | ProjKat (`projKa` still accepted as legacy alias) |
| `empty2` | (empty column H) |
| `netto` | Netto (string; Swedish decimal comma is fine) |
| `godkantAv` | Godkänt av |

Omitted optional fields are treated as empty.

### Copying grid data into Medius

`GridAndCommentView` supports Excel-like copy:

- Cell multi-select (`CellSelect`)
- After results load, all data cells are selected
- **Ctrl+A** — select all cells
- **Ctrl+C** — copy selection as tab-separated values (TSV) with CRLF, **without** column headers; empty columns are preserved as empty fields between tabs

Paste into Medius (or Excel) should behave like copying the corresponding range from a spreadsheet.

### Multiple matching jobs

If more than one job matches the same URL or file, Dropzone shows a selection dialog and the user picks which job to run. A single match runs immediately. Zero matches shows the existing “no handler” message.

This is the first step toward content-based routing: matching can stay broad (e.g. Medius attachment URLs), while the user disambiguates until automatic classification exists.

## Done action

The main window has three content states:

- **Idle** — prompt “Släpp något här”. **Done** is disabled.
- **Processing** — “Bearbetar...”. **Done** is disabled.
- **Result** — the job’s view (e.g. `GridAndCommentView`). **Done** is enabled.

**Done** is an action, not a tab. Clicking it disposes the result view, deletes Dropzone-owned temp files (URL downloads under `%LocalAppData%\Dropzone\temp`), restores idle, and disables itself. Files the user dropped from disk are not deleted. Starting a new drop, or closing the window, also cleans owned temps. Files older than 24 hours in the temp folder are removed on close.

## How to add a new handler

1. Implement `IJobHandler` in `Dropzone/Handlers/`.
2. Register the type in `MainForm`’s `_handlerTypes` dictionary (key = `handlerType` string in config).
3. Add or extend tests under `Dropzone.Tests/Handlers/`.
4. Point one or more jobs at the new `handlerType`.

## How to add a new view

1. Create a WinForms `UserControl` under `Dropzone/Views/` that can bind to `JobResult`.
2. Wire it in `MainForm` (same pattern as `GridAndCommentView`).
3. Use the new `viewType` from job config.

## Solution layout

```
Dropzone/                 WinForms host
  Config/                 JSON config + loader
  Forms/                  Main UI shell
  Handlers/               IJobHandler implementations
  Models/                 JobConfig, JobResult, RowModel
  Services/               Download, temp files, Python process
  Views/                  Result UI
Dropzone.Tests/           Unit tests (mirrors production folders)
```

## Testing

- Prefer unit tests with clear Arrange–Act–Assert.
- One test class per production type when practical.
- See `Dropzone.Tests/README.md` for runners and coverage notes.

External dependencies (HTTP download, real Python) should stay injectable or integration-scoped where possible so unit tests remain reliable.
