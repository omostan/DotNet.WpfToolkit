# Release Notes - v1.0.6

**Release Date:** April 2026  
**Package:** DotNetTools.Wpfkit  
**Version:** 1.0.6  
**Type:** Patch Release

---

## Highlights

Version 1.0.6 focuses on logging API improvements, helper utility additions, and release hardening.

### Added
- New logging enrichment methods in `LogManager`:
  - `WriteLine(...)`
  - `WithPath(...)`
  - `WithMember(...)`
- New helper utility: `CompareCollections` for collection equality, equivalence, and delta operations.

### Changed
- Updated tests across command, MVVM, logging, and database areas for current API behavior and consistency.
- Improved NuGet restore reliability by adding explicit package source mapping in `nuget.config`.

### Deprecated
- `LogManager.Me(...)` is obsolete and now points to `WriteLine(...)`.
- `LogManager.WithLine(...)` is obsolete and now points to `WriteLine(...)`.

### Quality Gate
- Build: successful in `Release` configuration.
- Tests: `187` passed, `0` failed, `0` skipped.

---

## Migration Guide

### From v1.0.5 to v1.0.6

No breaking changes. Existing code continues to work.

Recommended updates:
1. Keep existing `Me(...)` calls for now (supported but obsolete).
2. Move new/updated logging code to `WriteLine(...)`.
3. Use `WithPath(...)` and `WithMember(...)` where richer diagnostics are needed.

```bash
dotnet add package DotNetTools.Wpfkit --version 1.0.6
```

---

## Package Information

- **NuGet Package ID:** `DotNetTools.Wpfkit`
- **Target Framework:** `.NET 10.0-windows`
- **Dependencies:**
  - `Serilog` (4.3.0+)
  - `Tracetool.DotNet.Api` (14.0.0+)

---

## Links

- NuGet: https://www.nuget.org/packages/DotNetTools.Wpfkit/
- Repository: https://github.com/omostan/DotNetTools.Wpfkit
- Changelog: `CHANGELOG.md`

