# Generic Release Script for DotNetTools.Wpfkit
# Run this script from the repository root directory
# Usage: .\Release.ps1 -Version "1.0.3" [-ApiKey YOUR_API_KEY] [-SkipNuGet] [-SkipTag]

param(
    [Parameter(Mandatory=$true)]
    [string]$Version,
    [string]$ApiKey = "",
    [switch]$SkipNuGet = $false,
    [switch]$SkipTag = $false,
    [switch]$Help = $false
)

if ($Help) {
    Write-Host @"
Generic Release Script for DotNetTools.Wpfkit

Usage:
    .\Release.ps1 -Version VERSION [-ApiKey API_KEY] [-SkipNuGet] [-SkipTag] [-Help]

Parameters:
    -Version      Version number (e.g., "1.0.3", "2.0.0") [REQUIRED]
    -ApiKey       Your NuGet API key for publishing
    -SkipNuGet    Skip NuGet package publishing (only create tag and package)
    -SkipTag      Skip Git tag creation
    -Help         Show this help message

Examples:
    # Full release with NuGet publishing
    .\Release.ps1 -Version "1.0.3" -ApiKey "your-api-key-here"
    
    # Create tag and package only (no NuGet publishing)
    .\Release.ps1 -Version "1.0.3" -SkipNuGet
    
    # Build and publish without creating tag (tag already exists)
    .\Release.ps1 -Version "1.0.3" -SkipTag -ApiKey "your-api-key-here"
    
    # Show help
    .\Release.ps1 -Help
"@
    exit 0
}

# Validate version format (basic check)
if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    Write-Host "ERROR: Invalid version format. Expected format: X.Y.Z (e.g., 1.0.3)" -ForegroundColor Red
    exit 1
}

$ProjectName = "DotNetTools.Wpfkit"
$ProjectPath = "DotNetTools.WpfKit"
$ProjectFile = "$ProjectPath\$ProjectName.csproj"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  $ProjectName v$Version Release" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if we're in the right directory
if (-not (Test-Path $ProjectFile)) {
    Write-Host "ERROR: Please run this script from the repository root directory!" -ForegroundColor Red
    Write-Host "Current directory: $(Get-Location)" -ForegroundColor Yellow
    Write-Host "Looking for: $ProjectFile" -ForegroundColor Yellow
    exit 1
}

Write-Host "✓ Project found: $ProjectFile" -ForegroundColor Green
Write-Host ""

# Step 1: Check Git Status
Write-Host "[1/7] Checking Git status..." -ForegroundColor Yellow
$gitStatus = git status --porcelain
if ($gitStatus) {
    Write-Host "WARNING: You have uncommitted changes:" -ForegroundColor Yellow
    git status --short
    $continue = Read-Host "Do you want to continue? (y/n)"
    if ($continue -ne 'y') {
        Write-Host "Release cancelled." -ForegroundColor Red
        exit 1
    }
}
Write-Host "✓ Git status OK" -ForegroundColor Green
Write-Host ""

# Step 2: Create Git Tag
if (-not $SkipTag) {
    Write-Host "[2/7] Creating Git tag v$Version..." -ForegroundColor Yellow
    $tagExists = git tag -l "v$Version"
    if ($tagExists) {
        Write-Host "WARNING: Tag v$Version already exists!" -ForegroundColor Yellow
        $overwrite = Read-Host "Do you want to delete and recreate it? (y/n)"
        if ($overwrite -eq 'y') {
            git tag -d "v$Version"
            git push origin ":refs/tags/v$Version" 2>$null
            Write-Host "✓ Existing tag deleted" -ForegroundColor Green
        } else {
            Write-Host "Skipping tag creation." -ForegroundColor Yellow
        }
    }
    
    if (-not $tagExists -or $overwrite -eq 'y') {
        $tagMessage = Read-Host "Enter tag message (or press Enter for default)"
        if ([string]::IsNullOrWhiteSpace($tagMessage)) {
            $tagMessage = "Release v$Version"
        }
        
        git tag -a "v$Version" -m $tagMessage
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Tag created successfully" -ForegroundColor Green
            
            Write-Host "  Pushing tag to GitHub..." -ForegroundColor Yellow
            git push origin "v$Version"
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✓ Tag pushed to GitHub" -ForegroundColor Green
            } else {
                Write-Host "✗ Failed to push tag" -ForegroundColor Red
            }
        } else {
            Write-Host "✗ Failed to create tag" -ForegroundColor Red
            exit 1
        }
    }
} else {
    Write-Host "[2/7] Skipping Git tag creation (as requested)" -ForegroundColor Yellow
}
Write-Host ""

# Step 3: Clean
Write-Host "[3/7] Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean --configuration Release --verbosity quiet
Write-Host "✓ Clean completed" -ForegroundColor Green
Write-Host ""

# Step 4: Restore
Write-Host "[4/7] Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Restore completed" -ForegroundColor Green
} else {
    Write-Host "✗ Restore failed" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 5: Build
Write-Host "[5/7] Building in Release configuration..." -ForegroundColor Yellow
dotnet build --configuration Release --no-restore --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Build completed successfully" -ForegroundColor Green
} else {
    Write-Host "✗ Build failed" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 6: Create Package
Write-Host "[6/7] Creating NuGet package..." -ForegroundColor Yellow
if (-not (Test-Path "nupkg")) {
    New-Item -ItemType Directory -Path "nupkg" | Out-Null
}
dotnet pack $ProjectFile --configuration Release --no-build --output nupkg --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Package created successfully" -ForegroundColor Green
    Write-Host "  Package: nupkg\$ProjectName.$Version.nupkg" -ForegroundColor Cyan
    Write-Host "  Symbols: nupkg\$ProjectName.$Version.snupkg" -ForegroundColor Cyan
} else {
    Write-Host "✗ Package creation failed" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 7: Publish to NuGet
if (-not $SkipNuGet) {
    Write-Host "[7/7] Publishing to NuGet.org..." -ForegroundColor Yellow
    
    if ([string]::IsNullOrEmpty($ApiKey)) {
        Write-Host "WARNING: No API key provided!" -ForegroundColor Yellow
        $ApiKey = Read-Host "Enter your NuGet API key (or press Enter to skip)"
    }
    
    if (-not [string]::IsNullOrEmpty($ApiKey)) {
        Write-Host "  Publishing package..." -ForegroundColor Yellow
        dotnet nuget push "nupkg\$ProjectName.$Version.nupkg" --api-key $ApiKey --source https://api.nuget.org/v3/index.json --skip-duplicate
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Package published successfully!" -ForegroundColor Green
            Write-Host ""
            Write-Host "  Package will be available at:" -ForegroundColor Cyan
            Write-Host "  https://www.nuget.org/packages/$ProjectName/$Version" -ForegroundColor Cyan
            Write-Host ""
            Write-Host "  Note: It may take 5-10 minutes to appear in search results." -ForegroundColor Yellow
        } else {
            Write-Host "✗ Package publication failed" -ForegroundColor Red
            Write-Host "  You can publish manually later using:" -ForegroundColor Yellow
            Write-Host "  dotnet nuget push nupkg\$ProjectName.$Version.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json" -ForegroundColor Cyan
        }
    } else {
        Write-Host "⊘ Skipping NuGet publishing (no API key provided)" -ForegroundColor Yellow
    }
} else {
    Write-Host "[7/7] Skipping NuGet publishing (as requested)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Release Process Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Release Summary:" -ForegroundColor Yellow
Write-Host "  Version: v$Version" -ForegroundColor White
Write-Host "  Package: $ProjectName.$Version.nupkg" -ForegroundColor White
Write-Host "  Location: $(Join-Path (Get-Location) "nupkg")" -ForegroundColor White
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Create GitHub Release at: https://github.com/omostan/$ProjectName/releases/new" -ForegroundColor White
Write-Host "   - Select tag: v$Version" -ForegroundColor White
Write-Host "   - Title: v$Version - [Feature Name]" -ForegroundColor White
Write-Host "   - Copy description from: release-notes-v$Version.md" -ForegroundColor White
Write-Host ""
Write-Host "2. Verify NuGet package at: https://www.nuget.org/packages/$ProjectName/" -ForegroundColor White
Write-Host ""
Write-Host "3. Test installation:" -ForegroundColor White
Write-Host "   dotnet add package $ProjectName --version $Version" -ForegroundColor Cyan
Write-Host ""
