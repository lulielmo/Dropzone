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

## Next up (priority order)

### 1. Window behavior (optional follow-ups)

- [ ] Optional: global **hotkey** to show/hide from tray
- [ ] Optional: start with Windows / single-instance guard

### 2. Code quality / hygiene (good “waiting for invoice” work)

- [ ] Enable Visual Studio / .NET **code analysis** on rebuild (e.g. analyzers + `.editorconfig`, treat warnings as agreed)
- [ ] Rename `AteaInvoiceHandler` → `PythonScriptHandler` (or equivalent strategy name)
  - [ ] Update class, tests, and `handlerType` registration in `MainForm`
  - [ ] Update `dropzone.config.json` samples
  - [ ] Move hard-coded `JobResult.Type` / `Title` off the handler onto job config where possible
- [ ] Remove leftover `Form1` scaffolding if unused
- [ ] Prefer dependency injection for services used by handlers (`PythonProcessService`, etc.) to improve testability
- [ ] Confirm view registration pattern matches handler registration (document any gaps)

### 3. Idle UX polish

- [ ] **Window size per state** (idle compact, result large)
  - [ ] v1: fixed sizes — idle/drop target much smaller than today; result (and maybe processing→result) uses the current ~1000×700; **Done** shrinks back to idle. Lower `MinimumSize` (today 800×500) so idle can be compact
  - [ ] Later: remember last user-resized size per state (defaults = the v1 sizes)
  - Keep a stable corner when growing/shrinking so the drop target does not jump
  - Processing can stay at idle size until a result is shown
- [ ] **Idle concealment** (pick one; idle only — never while a result is shown; do not hide/move during an active drag-and-drop onto Dropzone)
  - [ ] Option A — **Edge dock / auto-hide** (ICQ-style): user docks the compact idle window to a screen edge; when the pointer is away it slides mostly off-screen (leave a thin peek); when the pointer nears the peek it slides back in. Delay + hysteresis so it does not flicker
  - [ ] Option B — **Mouse dodge**: when the pointer approaches, move the window aside so it does not obscure content underneath
  - Compact idle size (above) makes either option much more usable; prefer A if we only implement one
- [ ] Clarify idle / processing / result states in the main window
- [ ] Improve “no matching job” feedback for dropped URLs/files
- [ ] Detect HTML / login-page download (auth missing) and show a clear message instead of letting Python fail on a non-PDF
- [ ] Reject or warn early when the input file is not a usable PDF (before calling the script)
- [ ] Optional: visible **Copy comment** button (textarea copy already works)

### 4. Configuration UI

- [ ] Implement **Configuration** menu: start simple (open/`reveal` `dropzone.config.json`, reload config) before a full editor
- [ ] Later: graphical UI on top of `dropzone.config.json` (view/edit jobs, paths, matchers)
- [ ] Validate config on load and surface errors in the UI

### 5. Smarter routing / more jobs

- [ ] Optional content-based job suggestion (keywords / PDF text) before or instead of manual choice
- [ ] Additional view type when a new result shape appears
- [ ] Additional handler only when execution strategy diverges from “single file → Python script”

## Testing

- [ ] Increase coverage around config matching edge cases
- [ ] Consider a separate integration test project for real Python script runs
- [x] Verify Medius paste end-to-end when the next ACP invoice arrives (Excel paste already OK)

## Notes

- Prefer file-drop from Edge downloads while Medius URL auth is unresolved.
- Script JSON contract (rows, comment, `messages`) and column layout: see `DEVELOPMENT.md`.
- InvoiceHelper (and later scripts) should emit `messages` only in Dropzone/CLI-JSON mode; interactive console output stays as-is.
- Optional later: collapsible raw stderr log for deep troubleshooting.