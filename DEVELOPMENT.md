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
- Optional local notes (e.g. a gitignored `dev-notes.md`)

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

- `PythonScriptHandler` — run a configured Python script with one CLI argument (file path or token)
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

Prefer taking display title / type from the **job config** `name`. The host sets `JobResult.Title` and `JobResult.Type` after the handler returns; generic handlers must not hard-code vendor strings.

## How to add a new job (config only)

1. Add an entry under `jobs` in `Dropzone/Config/dropzone.config.json`.
2. Set matching rules (`urlRegex`, `domainName`, `fileNameRegex`, `fileExtension`, and/or `textRegex`).
3. Set `handlerType` and `viewType` to existing registered types.
4. Fill `handlerConfig` (script path, exe, working directory, etc.).
5. Run and drop a matching URL, file, or text snippet.

No C# change is required if handler and view already exist and are registered.

`fileNameRegex` is matched against the **file name only** (not the full path), for both local file drops and the file segment of a URL. Use it for system-specific attachment names (e.g. Medius `einvoicecapture-embedded-attachment`) without tying jobs to a Downloads folder.

`textRegex` is matched against **dropped plain text** (not URLs). Use it when the trigger is selected PDF text rather than a file, e.g. Medius line `AZURECONS`.

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
- Dropzone invokes: `pythonExe "pythonScript" "inputArgument"` with that working directory.
- For file jobs, `inputArgument` is the input file path. For `handlerConfig.inputKind` = `cliArgument`, it is a token such as a billing period `YYYYMM` (no file is created or required).
- Stdout/stderr are read as **UTF-8** (`PYTHONIOENCODING` / `PYTHONUTF8` are also set for the child process). Scripts should emit UTF-8 JSON (`ensure_ascii=False` in Python is fine).

**Script contract (implemented in the Python project, not in Dropzone):** accept the CLI argument (file path **or** period token) and write a single JSON object to stdout. Log to stderr or files so stdout stays valid JSON. Interactive `input()` menus must not run in this mode.

### Dropped text and billing period

Plain text that is not an `http(s)`/`file` URL is matched with `textRegex`. When the job uses `inputKind: cliArgument`, Dropzone parses a billing period from the text and passes `YYYYMM`:

- `Period 2026-06-01 -- 2026-06-30` → `202606` (start month)
- `Period 2026-06` → `202606`
- `202606` → `202606`

The job picker is still used if more than one job matches. A text-only Azure job should not share ACP file matchers.

Row objects follow Medius Excel column order (A–J):

```json
{
  "success": true,
  "comment": "Medius comment text with \\n line breaks",
  "messages": [
    { "level": "error", "text": "Totalsumman stämmer inte med fakturan." },
    { "level": "warning", "text": "Rad 2 saknar aktivitet." }
  ],
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

### Script diagnostics

`comment` is Medius paste text only. Validation and other script notes belong in `messages` so Dropzone can show them in a distinct panel above the grid.

| JSON | Meaning |
|------|---------|
| `messages` | Array of diagnostics (preferred) |
| `warnings` | Alias used only when `messages` is omitted |
| `level` / `severity` | `error`, `warning` (or `warn`), `info` (or `note`). Default: `warning` |
| `text` / `message` | Human-readable text |

A `warnings` item may also be a plain string; it is treated as `{ "level": "warning", "text": "..." }`. Empty items are ignored.

Emit `messages` only in Dropzone / CLI JSON mode (stdout). Interactive console runs should keep printing to stderr or the terminal as today. Do not put diagnostics into `comment`.

`GridAndCommentView` hides the panel when there are no messages. Errors are red, warnings amber, info blue. Host failures (`JobResult.ErrorMessage` when `success` is false) appear in the same panel.

### Copying grid data into Medius

`GridAndCommentView` supports Excel-like copy:

- Cell multi-select (`CellSelect`)
- After results load, all data cells are selected
- **Ctrl+A** — select all cells
- **Ctrl+C** — copy selection as tab-separated values (TSV) with CRLF, **without** column headers; empty columns are preserved as empty fields between tabs

Paste into Medius (or Excel) should behave like copying the corresponding range from a spreadsheet.

**Copy comment** copies the Medius comment textarea (CRLF) to the clipboard. The button is enabled only when the comment is non-empty. Selecting the textarea and using Ctrl+C still works.

**Copy grid** copies every data cell as the same Excel-like TSV, without requiring a selection. Ctrl+A / Ctrl+C on a selected range still copies only that range.

### Multiple matching jobs

If more than one job matches the same URL or file, Dropzone shows a selection dialog and the user picks which job to run. A single match runs immediately. Zero matches shows the existing “no handler” message.

This is the first step toward content-based routing: matching can stay broad (e.g. Medius attachment URLs), while the user disambiguates until automatic classification exists.

## Done action

The main window has three content states:

- **Idle** — prompt “Släpp något här”. **Done** is disabled.
- **Processing** — “Bearbetar...”. **Done** is disabled.
- **Result** — the job’s view (e.g. `GridAndCommentView`). **Done** is enabled.

**Done** is an action, not a tab. Clicking it disposes the result view, deletes Dropzone-owned temp files (URL downloads under `%LocalAppData%\Dropzone\temp`), restores idle, and disables itself. Files the user dropped from disk are not deleted. Starting a new drop, or closing the window, also cleans owned temps. Files older than 24 hours in the temp folder are removed on close.

Idle and processing use a compact host size (~320×180). Showing a result grows to ~1000×700; **Done** (and a new processing cycle) shrinks back. The top-right corner stays put so the title bar, **Done**, and caption buttons remain on screen. Remembering last user-resized size per state is a later enhancement.

## Window behavior

While Dropzone is visible it stays **always on top** so it remains available next to Medius. Minimizing hides the window to the system tray (including the hidden-icons overflow). Double-click the tray icon, or choose **Show**, to restore it on top. **Exit** on the tray menu (or the window Close button) quits the app and runs temp-file cleanup.

Modal UI (job picker, error `MessageBox`) temporarily turns off always-on-top so the dialog is not hidden behind the main window.

**Configuration** opens the JSON file the host loads (the project `Config/dropzone.config.json` when running from this repo’s build output; otherwise the copy next to the exe). The next drop re-reads it.

File jobs are checked before the Python script runs: the input must look like a PDF (`%PDF`). HTML (often a Medius login page after a URL drop) is rejected with a clear message. When nothing matches, the message names the drop and hints at file vs URL vs `AZURECONS` text.

## How to add a new handler

1. Implement `IJobHandler` in `Dropzone/Handlers/`.
2. Register the type in `MainForm`’s `_handlerTypes` dictionary (key = `handlerType` string in config).
3. Add or extend tests under `Dropzone.Tests/Handlers/`.
4. Point one or more jobs at the new `handlerType`.

## How to add a new view

1. Create a WinForms `UserControl` under `Dropzone/Views/` that implements `IJobResultView`.
2. Register the type in `MainForm`’s `_viewTypes` dictionary (key = `viewType` string in config).
3. Add or extend tests under `Dropzone.Tests/Views/`.
4. Point one or more jobs at the new `viewType`.

Handler and view registration use the same pattern: a dictionary on `MainForm` keyed by the config type name.

## Code analysis

Rebuild runs the .NET recommended analyzers. Warnings are treated as errors (`TreatWarningsAsErrors` in `Directory.Build.props`). Shared style lives in `.editorconfig`. Designer files are marked `generated_code`.

Agreed exceptions (see `.editorconfig`):

- Tests: **CA1707** (underscores in xUnit names), **CA1816** (`IDisposable` fixtures), **CA1861** (inline arrays in assertions).
- WinForms Forms/Views: **IDE1006** (designer event handlers use `controlName_EventName`).

## Solution layout

```
Dropzone/                 WinForms host
  Config/                 JSON config + loader
  Forms/                  Main UI shell
  Handlers/               IJobHandler implementations
  Models/                 JobConfig, JobResult, RowModel
  Services/               Download, temp files, Python process
  Views/                  IJobResultView + result UI
Dropzone.Tests/           Unit tests (mirrors production folders)
```

## Testing

- Prefer unit tests with clear Arrange–Act–Assert.
- One test class per production type when practical.
- See `Dropzone.Tests/README.md` for runners and coverage notes.

External dependencies (HTTP download, real Python) should stay injectable or integration-scoped where possible so unit tests remain reliable.
