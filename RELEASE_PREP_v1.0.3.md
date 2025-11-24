# Release Preparation for v1.0.3

## Changes Made

### 1. Version Bump
- Updated version from **1.0.2** to **1.0.3** in `DotNetTools.Wpfkit.csproj`
- Updated AssemblyVersion and FileVersion to 1.0.3.0
- Updated PackageReleaseNotes to reference v1.0.3 changes

### 2. Documentation Updates

#### Main README.md
- Added "What's New in v1.0.3" section
- Highlighted RelayCommand visibility fix
- Moved v1.0.2 features to "Previous Updates" section

#### Library README (DotNetTools.WpfKit\README.md)
- Added "What's New" section with version history
- Updated RelayCommand section to emphasize it's now public
- Added ✨ emoji indicators for the new accessibility

#### Created release-notes-v1.0.3.md
- Comprehensive release notes documenting the bug fix
- Usage examples showing proper RelayCommand usage
- Migration notes for developers
- Package information and links

### 3. Files Modified
- `DotNetTools.WpfKit\DotNetTools.Wpfkit.csproj` - Version bump and release notes
- `README.md` - Updated to reflect v1.0.3
- `DotNetTools.WpfKit\README.md` - Added What's New section and updated RelayCommand docs
- `release-notes-v1.0.3.md` - New file with detailed release notes

## Git Commit Message

```
Release v1.0.3 - Fix RelayCommand Visibility

- Fixed: RelayCommand is now properly public and accessible to developers
- Updated: Package version from 1.0.2 to 1.0.3
- Updated: Documentation to reflect RelayCommand accessibility
- Added: Comprehensive release notes for v1.0.3
- Added: Usage examples for public RelayCommand implementation

This release addresses the issue where RelayCommand was not properly
accessible due to incorrect visibility modifiers, making it fully
usable in MVVM applications.

Closes: Issue with RelayCommand accessibility
```

## Next Steps for Release

### 1. Commit Changes
```bash
git add .
git commit -m "Release v1.0.3 - Fix RelayCommand Visibility

- Fixed: RelayCommand is now properly public and accessible to developers
- Updated: Package version from 1.0.2 to 1.0.3
- Updated: Documentation to reflect RelayCommand accessibility
- Added: Comprehensive release notes for v1.0.3
- Added: Usage examples for public RelayCommand implementation"
```

### 2. Tag the Release
```bash
git tag -a v1.0.3 -m "Version 1.0.3 - RelayCommand Visibility Fix"
```

### 3. Push to GitHub
```bash
git push origin main
git push origin v1.0.3
```

### 4. Build NuGet Package
```bash
cd DotNetTools.WpfKit
dotnet pack -c Release
```

### 5. Publish to NuGet
```bash
dotnet nuget push bin\Release\DotNetTools.Wpfkit.1.0.3.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
```

### 6. Create GitHub Release
- Go to: https://github.com/omostan/DotNetTools.Wpfkit/releases/new
- Tag: v1.0.3
- Title: "v1.0.3 - RelayCommand Visibility Fix"
- Description: Use content from release-notes-v1.0.3.md
- Attach: DotNetTools.Wpfkit.1.0.3.nupkg and DotNetTools.Wpfkit.1.0.3.snupkg

## Verification Steps

### Before Publishing
- [x] Build successful (confirmed)
- [x] Version numbers updated in csproj
- [x] Release notes created
- [x] Documentation updated
- [ ] Manual testing of RelayCommand accessibility
- [ ] Review all changes in git diff

### After Publishing
- [ ] Verify package appears on NuGet.org
- [ ] Test installation: `dotnet add package DotNetTools.Wpfkit --version 1.0.3`
- [ ] Verify documentation renders correctly on NuGet
- [ ] Verify GitHub release is created
- [ ] Announce release (if applicable)

## Breaking Changes
**None** - This is a bug fix that only improves accessibility.

## Impact
- Developers can now properly use RelayCommand in their applications
- No breaking changes to existing code
- Better alignment with intended API design
