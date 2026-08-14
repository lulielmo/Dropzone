# TODO List

## Documentation

- [x] Add project starter docs (`README.md`, `DEVELOPMENT.md`, `TODO.md`)
- [x] Add Cursor rules under `.cursor/rules/`
- [ ] Keep docs in sync when renaming handlers or adding extension points

## Naming / architecture cleanup

- [ ] Rename `AteaInvoiceHandler` → `PythonScriptHandler` (or equivalent strategy name)
  - [ ] Update class, tests, and `handlerType` registration in `MainForm`
  - [ ] Update `dropzone.config.json` samples
  - [ ] Move hard-coded `JobResult.Type` / `Title` off the handler onto job config where possible
- [ ] Confirm view registration pattern matches handler registration (document any gaps)

## Product / UX

- [x] Expand grid/JSON row contract to full Medius columns (incl. empty spacers, Netto, Godkänt av)
- [x] Excel-compatible multi-cell copy from the result grid (Ctrl+A / Ctrl+C, TSV without headers)
- [ ] Clarify idle / processing / result states in the main window
- [ ] Improve “no matching job” feedback for dropped URLs/files
- [ ] Optional content-based job suggestion (keywords / PDF text) before or instead of manual choice
- [ ] Explicit “done” action: cleanup temp file and return to idle

## Code quality

- [x] Support `workingDirectory` + uv `.venv` pythonExe in handler config
- [ ] Prefer dependency injection for services used by handlers (`PythonProcessService`, etc.) to improve testability
- [ ] Review leftover `Form1` scaffolding if unused
- [ ] Temp-file cleanup strategy (timing and failure paths)

## Testing

- [ ] Increase coverage around config matching edge cases
- [ ] Consider a separate integration test project for real Python script runs

## Future extensibility

- [ ] Second real job via config only (validates the “config-first” goal)
- [ ] Additional view type when a new result shape appears
- [ ] Additional handler only when execution strategy diverges from “single file → Python script”
