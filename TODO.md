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

## Next up (priority order)

### 1. Done action

- [ ] Implement **Done**: clear latest result, cleanup temp file(s), restore idle GUI (“Släpp något här”)
- [ ] Align link state: Done enabled only when a result is shown; disable again after reset

### 2. Window behavior (desktop utility)

- [ ] **Always on top** when the window is visible (not minimized / not in tray)
- [ ] Minimize to **system tray / hidden icons**; restore on double-click (NotifyIcon)
- [ ] Optional: global **hotkey** to show/hide from tray
- [ ] Optional: start with Windows / single-instance guard

### 3. Code quality / hygiene (good “waiting for invoice” work)

- [ ] Enable Visual Studio / .NET **code analysis** on rebuild (e.g. analyzers + `.editorconfig`, treat warnings as agreed)
- [ ] Rename `AteaInvoiceHandler` → `PythonScriptHandler` (or equivalent strategy name)
  - [ ] Update class, tests, and `handlerType` registration in `MainForm`
  - [ ] Update `dropzone.config.json` samples
  - [ ] Move hard-coded `JobResult.Type` / `Title` off the handler onto job config where possible
- [ ] Remove leftover `Form1` scaffolding if unused
- [ ] Prefer dependency injection for services used by handlers (`PythonProcessService`, etc.) to improve testability
- [ ] Confirm view registration pattern matches handler registration (document any gaps)
- [ ] Temp-file cleanup strategy (timing and failure paths) — partly overlaps Done

### 4. Idle UX polish

- [ ] **Mouse dodge** in idle only: when the pointer approaches, move the window aside so it does not obscure content underneath; **do not** dodge during an active drag-and-drop onto Dropzone; **never** dodge while a result is displayed
- [ ] Clarify idle / processing / result states in the main window
- [ ] Improve “no matching job” feedback for dropped URLs/files
- [ ] Detect HTML / login-page download (auth missing) and show a clear message instead of letting Python fail on a non-PDF
- [ ] Reject or warn early when the input file is not a usable PDF (before calling the script)
- [ ] Optional: visible **Copy comment** button (textarea copy already works)

### 5. Configuration UI

- [ ] Implement **Configuration** menu: start simple (open/`reveal` `dropzone.config.json`, reload config) before a full editor
- [ ] Later: graphical UI on top of `dropzone.config.json` (view/edit jobs, paths, matchers)
- [ ] Validate config on load and surface errors in the UI

### 6. Smarter routing / more jobs

- [ ] Optional content-based job suggestion (keywords / PDF text) before or instead of manual choice
- [ ] Second real job via config only (e.g. Azure Consumption) when its script exists — validates config-first
- [ ] Additional view type when a new result shape appears
- [ ] Additional handler only when execution strategy diverges from “single file → Python script”

## Testing

- [ ] Increase coverage around config matching edge cases
- [ ] Consider a separate integration test project for real Python script runs
- [ ] Verify Medius paste end-to-end when the next ACP invoice arrives (Excel paste already OK)

## Notes

- Prefer file-drop from Edge downloads while Medius URL auth is unresolved.
- Script JSON contract and column layout: see `DEVELOPMENT.md`.
