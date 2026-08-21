# TODO List

Suggested implementation order for upcoming work is under **Next up (priority order)**. Revisit when picking up after a break.

## Documentation

- [x] Add project starter docs (`README.md`, `DEVELOPMENT.md`, `TODO.md`)
- [x] Add Cursor rules under `.cursor/rules/`
- [ ] Keep docs in sync when renaming handlers or adding extension points

## Completed recently

- [x] Manual job selection when multiple jobs match the same URL/file
- [x] Support `workingDirectory` + uv `.venv` pythonExe in handler config
- [x] Expand grid/JSON row contract to full Medius columns (incl. empty spacers, Netto, Godkänt av)
- [x] Excel-compatible multi-cell copy from the result grid (Ctrl+A / Ctrl+C, TSV without headers)
- [x] **Done** action: clear result, cleanup owned temp files, restore idle; enabled only while a result is shown
- [x] Script diagnostics in the result UI (`messages` JSON → `JobResult` → prominent panel in `GridAndCommentView`)
- [x] Always on top while visible; minimize to system tray (restore on double-click / Show)
- [x] Azure Consumption job via dropped Medius text (`AZURECONS` + period → `YYYYMM`)
- [x] Rename `AteaInvoiceHandler` → `PythonScriptHandler`; result title/type from job `name`
- [x] Enable .NET recommended analyzers on rebuild (warnings as errors); remove `Form1`; register views like handlers
- [x] Compact idle/processing window; result grows to ~1000×700 with a stable top-right corner
- [x] **Configuration** opens the JSON the running app loads (project file when F5 from this repo)

## Next up (priority order)

While we are in active feature development (many F5 / restart / drop cycles), prefer work that is cheapest before the codebase grows, or that makes testing easier. Defer daily-driver install behaviour and anything that adds extra clicks on every restart.

### 1. Clearer drop failures (when testing real invoices)

Saves time on bad inputs; not needed for every UX restart.

- [ ] Improve “no matching job” feedback for dropped URLs/files
- [ ] Detect HTML / login-page download (auth missing) and show a clear message instead of letting Python fail on a non-PDF
- [ ] Reject or warn early when the input file is not a usable PDF (before calling the script)

### 2. Idle concealment (after compact idle feels good)

Useful in daily use. Auto-hide can add friction while drop-testing (find the peek, wait for slide-in). Idle only — never while a result is shown; do not hide/move during an active drag-and-drop onto Dropzone.

- [ ] Option A — **Edge dock / auto-hide** (ICQ-style): user docks the compact idle window to a screen edge; when the pointer is away it slides mostly off-screen (leave a thin peek); when the pointer nears the peek it slides back in. Delay + hysteresis so it does not flicker
- [ ] Option B — **Mouse dodge**: when the pointer approaches, move the window aside so it does not obscure content underneath
- Compact idle size (above) makes either option much more usable; prefer A if we only implement one
- [ ] Clarify idle / processing / result states in the main window
- [ ] Optional: visible **Copy comment** button (textarea copy already works)

### 3. Daily-driver window behaviour (wait until Dropzone sits in the tray all day)

Do **not** enable these during the F5-heavy phase.

- [ ] Optional: global **hotkey** to show/hide from tray
- [ ] **Single-instance** guard — fights “start a new build while the old one is still in the tray”
- [ ] **Start with Windows** — easy to launch a stale copy while iterating; better once there is a stable daily build

### 4. Smarter routing / more jobs (when a new case appears)

- [ ] Optional content-based job suggestion (keywords / PDF text) before or instead of manual choice
- [ ] Additional view type when a new result shape appears
- [ ] Additional handler only when execution strategy diverges from “single file → Python script”
- [ ] Prefer dependency injection for services used by handlers (`PythonProcessService`, etc.) to improve testability
- [ ] Later: graphical UI on top of `dropzone.config.json` (view/edit jobs, paths, matchers)
- [ ] Validate config on load and surface errors in the UI

## Testing

- [ ] Increase coverage around config matching edge cases
- [ ] Consider a separate integration test project for real Python script runs
- [x] Verify Medius paste end-to-end when the next ACP invoice arrives (Excel paste already OK)

## Notes

- Prefer file-drop from Edge downloads while Medius URL auth is unresolved.
- Script JSON contract (rows, comment, `messages`) and column layout: see `DEVELOPMENT.md`.
- InvoiceHelper (and later scripts) should emit `messages` only in Dropzone/CLI-JSON mode; interactive console output stays as-is.
- Optional later: collapsible raw stderr log for deep troubleshooting.