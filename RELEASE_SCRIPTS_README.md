# Release Scripts Documentation

## Overview

This directory contains generic release scripts for automating the release process of DotNetTools.Wpfkit. These scripts handle version tagging, building, packaging, and publishing to NuGet.org.

## Available Scripts

### 1. `Release.ps1` (PowerShell - Windows)
- **Platform**: Windows PowerShell 5.1+ or PowerShell Core 7+
- **Best for**: Windows development environments, Visual Studio

### 2. `release.sh` (Bash - Cross-platform)
- **Platform**: Linux, macOS, Git Bash (Windows), WSL
- **Best for**: Unix-like systems, CI/CD pipelines

## Prerequisites

### All Platforms
- **.NET 10.0 SDK** or later
- **Git** installed and configured
- **NuGet account** with valid API key (for publishing)

### PowerShell Script (Windows)
- PowerShell 5.1 or later
- Execution policy allowing script execution

### Bash Script (Linux/macOS/WSL)
- Bash 4.0 or later
- Make script executable: `chmod +x release.sh`

## Quick Start

### PowerShell (Windows)

```powershell
# Full release with NuGet publishing
.\Release.ps1 -Version "1.0.3" -ApiKey "your-nuget-api-key"

# Create tag and package only (no NuGet publishing)
.\Release.ps1 -Version "1.0.3" -SkipNuGet

# Build and publish without creating tag
.\Release.ps1 -Version "1.0.3" -SkipTag -ApiKey "your-api-key"
```

### Bash (Linux/macOS/WSL)

```bash
# Make script executable (first time only)
chmod +x release.sh

# Full release with NuGet publishing
./release.sh 1.0.3 --api-key "your-nuget-api-key"

# Create tag and package only (no NuGet publishing)
./release.sh 1.0.3 --skip-nuget

# Use environment variable for API key
export NUGET_API_KEY="your-api-key"
./release.sh 1.0.3
```

## Usage

### PowerShell Script

```powershell
.\Release.ps1 -Version VERSION [-ApiKey API_KEY] [-SkipNuGet] [-SkipTag] [-Help]
```

**Parameters:**
- `-Version` (Required): Version number (e.g., "1.0.3", "2.0.0")
- `-ApiKey`: Your NuGet API key for publishing
- `-SkipNuGet`: Skip NuGet package publishing
- `-SkipTag`: Skip Git tag creation
- `-Help`: Show help message

### Bash Script

```bash
./release.sh VERSION [OPTIONS]
```

**Arguments:**
- `VERSION` (Required): Version number (e.g., 1.0.3, 2.0.0)

**Options:**
- `-k, --api-key KEY`: Your NuGet API key for publishing
- `-n, --skip-nuget`: Skip NuGet package publishing
- `-t, --skip-tag`: Skip Git tag creation
- `-h, --help`: Show help message

## Release Process

Both scripts perform the following steps:

1. **Git Status Check**: Verifies no uncommitted changes (with override option)
2. **Git Tag Creation**: Creates annotated tag `vX.Y.Z` and pushes to GitHub
3. **Clean**: Removes previous build artifacts
4. **Restore**: Restores NuGet package dependencies
5. **Build**: Builds project in Release configuration
6. **Package**: Creates NuGet package (.nupkg) and symbols (.snupkg)
7. **Publish**: Publishes package to NuGet.org (if not skipped)

## Examples

### Example 1: Standard Release

**PowerShell:**
```powershell
.\Release.ps1 -Version "1.0.3" -ApiKey "oy2abc...xyz"
```

**Bash:**
```bash
./release.sh 1.0.3 --api-key "oy2abc...xyz"
```

**What it does:**
- Creates and pushes tag `v1.0.3`
- Builds Release configuration
- Creates NuGet package
- Publishes to NuGet.org

---

### Example 2: Create Package Without Publishing

**PowerShell:**
```powershell
.\Release.ps1 -Version "1.0.3" -SkipNuGet
```

**Bash:**
```bash
./release.sh 1.0.3 --skip-nuget
```

**What it does:**
- Creates and pushes tag `v1.0.3`
- Builds Release configuration
- Creates NuGet package
- Does NOT publish (you can publish manually later)

**When to use:** When you want to review the package before publishing

---

### Example 3: Publish Existing Version

**PowerShell:**
```powershell
.\Release.ps1 -Version "1.0.3" -SkipTag -ApiKey "oy2abc...xyz"
```

**Bash:**
```bash
./release.sh 1.0.3 --skip-tag --api-key "oy2abc...xyz"
```

**What it does:**
- Skips tag creation (assumes tag already exists)
- Builds Release configuration
- Creates NuGet package
- Publishes to NuGet.org

**When to use:** When tag already exists but package wasn't published, or to rebuild and republish

---

### Example 4: Using Environment Variable for API Key

**PowerShell:**
```powershell
$env:NUGET_API_KEY = "oy2abc...xyz"
.\Release.ps1 -Version "1.0.3" -ApiKey $env:NUGET_API_KEY
```

**Bash:**
```bash
export NUGET_API_KEY="oy2abc...xyz"
./release.sh 1.0.3
```

**What it does:**
- Uses API key from environment variable
- More secure than hardcoding in script

**When to use:** In CI/CD pipelines or when you don't want to expose API key in command history

---

### Example 5: Interactive Mode (No API Key Provided)

**PowerShell:**
```powershell
.\Release.ps1 -Version "1.0.3"
```

**Bash:**
```bash
./release.sh 1.0.3
```

**What it does:**
- Script will prompt for API key during execution
- You can press Enter to skip publishing

**When to use:** For one-off releases or when you don't have API key readily available

## Security Best Practices

### Storing API Keys

**DON'T:**
- ❌ Hardcode API keys in scripts
- ❌ Commit API keys to version control
- ❌ Share API keys in plain text

**DO:**
- ✅ Use environment variables
- ✅ Use secure password managers
- ✅ Rotate keys regularly
- ✅ Use scoped keys with minimal permissions

### Setting Environment Variables

**PowerShell (Session):**
```powershell
$env:NUGET_API_KEY = "your-api-key"
```

**PowerShell (Permanent - User):**
```powershell
[System.Environment]::SetEnvironmentVariable('NUGET_API_KEY', 'your-api-key', 'User')
```

**Bash (Session):**
```bash
export NUGET_API_KEY="your-api-key"
```

**Bash (Permanent - Add to ~/.bashrc or ~/.zshrc):**
```bash
echo 'export NUGET_API_KEY="your-api-key"' >> ~/.bashrc
source ~/.bashrc
```

## Post-Release Checklist

After running the release script:

- [ ] Verify tag on GitHub: `https://github.com/omostan/DotNetTools.Wpfkit/tags`
- [ ] Create GitHub Release with release notes
- [ ] Verify package on NuGet.org: `https://www.nuget.org/packages/DotNetTools.Wpfkit/`
- [ ] Test package installation in a test project
- [ ] Update documentation if needed
- [ ] Announce release (social media, blog, etc.)

## Troubleshooting

### Issue: "Execution policy" error (PowerShell)

**Error:**
```
File cannot be loaded because running scripts is disabled on this system
```

**Solution:**
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

---

### Issue: "Permission denied" (Bash)

**Error:**
```
bash: ./release.sh: Permission denied
```

**Solution:**
```bash
chmod +x release.sh
```

---

### Issue: "Invalid version format"

**Error:**
```
ERROR: Invalid version format. Expected format: X.Y.Z
```

**Solution:** Use semantic versioning format (e.g., 1.0.3, 2.0.0, 1.2.5)

---

### Issue: "Package already exists" on NuGet

**Solution:** The scripts use `--skip-duplicate` flag, so this should be handled automatically. If you need to unpublish a version, do it through NuGet.org website (note: unlist, not delete).

---

### Issue: Tag already exists

**Solution:** The scripts will prompt you to delete and recreate the tag. Choose 'y' to proceed or 'n' to skip tag creation.

---

### Issue: Uncommitted changes

**Solution:** The scripts will warn you and ask if you want to continue. Either:
1. Commit your changes first (recommended)
2. Stash your changes: `git stash`
3. Continue anyway (not recommended)

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Release

on:
  workflow_dispatch:
    inputs:
      version:
        description: 'Version to release (e.g., 1.0.3)'
        required: true
        type: string

jobs:
  release:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '10.0.x'
      
      - name: Make script executable
        run: chmod +x release.sh
      
      - name: Run release script
        env:
          NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
        run: ./release.sh ${{ inputs.version }}
```

### Azure DevOps Example

```yaml
trigger: none

parameters:
  - name: version
    displayName: 'Version to release'
    type: string

jobs:
  - job: Release
    pool:
      vmImage: 'ubuntu-latest'
    steps:
      - task: UseDotNet@2
        inputs:
          version: '10.0.x'
      
      - bash: chmod +x release.sh
        displayName: 'Make script executable'
      
      - bash: ./release.sh ${{ parameters.version }} --api-key $(NUGET_API_KEY)
        displayName: 'Run release script'
        env:
          NUGET_API_KEY: $(NUGET_API_KEY)
```

## Customization

### Modifying for Your Project

To adapt these scripts for a different project:

1. **Update project variables** (at the top of each script):
   ```powershell
   # PowerShell
   $ProjectName = "YourProject.Name"
   $ProjectPath = "YourProject"
   $ProjectFile = "$ProjectPath\$ProjectName.csproj"
   ```
   
   ```bash
   # Bash
   PROJECT_NAME="YourProject.Name"
   PROJECT_PATH="YourProject"
   PROJECT_FILE="$PROJECT_PATH/$PROJECT_NAME.csproj"
   ```

2. **Update repository URL** in the "Next steps" section

3. **Modify tag message format** if needed

4. **Add custom build steps** (e.g., running tests, code analysis)

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2025-11-24 | Initial release scripts created |

## Support

For issues or questions:
- **GitHub Issues**: https://github.com/omostan/DotNetTools.Wpfkit/issues
- **Email**: stan@omotech.com

## License

These scripts are part of the DotNetTools.Wpfkit project and are licensed under the MIT License.

---

**Happy Releasing! 🚀**
