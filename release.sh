#!/bin/bash

# Generic Release Script for DotNetTools.Wpfkit
# Run this script from the repository root directory
# Usage: ./release.sh VERSION [OPTIONS]

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
WHITE='\033[1;37m'
NC='\033[0m' # No Color

# Default values
VERSION=""
API_KEY=""
SKIP_NUGET=false
SKIP_TAG=false

# Project configuration
PROJECT_NAME="DotNetTools.Wpfkit"
PROJECT_PATH="DotNetTools.WpfKit"
PROJECT_FILE="$PROJECT_PATH/$PROJECT_NAME.csproj"

# Functions
print_header() {
    echo -e "${CYAN}========================================${NC}"
    echo -e "${CYAN}  $1${NC}"
    echo -e "${CYAN}========================================${NC}"
    echo ""
}

print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

print_info() {
    echo -e "${CYAN}→ $1${NC}"
}

show_help() {
    cat << EOF
Generic Release Script for DotNetTools.Wpfkit

Usage:
    ./release.sh VERSION [OPTIONS]

Arguments:
    VERSION       Version number (e.g., 1.0.3, 2.0.0) [REQUIRED]

Options:
    -k, --api-key KEY     Your NuGet API key for publishing
    -n, --skip-nuget      Skip NuGet package publishing
    -t, --skip-tag        Skip Git tag creation
    -h, --help            Show this help message

Examples:
    # Full release with NuGet publishing
    ./release.sh 1.0.3 --api-key "your-api-key-here"
    
    # Create tag and package only (no NuGet publishing)
    ./release.sh 1.0.3 --skip-nuget
    
    # Build and publish without creating tag (tag already exists)
    ./release.sh 1.0.3 --skip-tag --api-key "your-api-key-here"
    
    # Use environment variable for API key
    export NUGET_API_KEY="your-api-key"
    ./release.sh 1.0.3

EOF
    exit 0
}

# Parse command line arguments
if [ $# -eq 0 ]; then
    print_error "Version number is required!"
    echo ""
    show_help
fi

VERSION=$1
shift

while [[ $# -gt 0 ]]; do
    case $1 in
        -k|--api-key)
            API_KEY="$2"
            shift 2
            ;;
        -n|--skip-nuget)
            SKIP_NUGET=true
            shift
            ;;
        -t|--skip-tag)
            SKIP_TAG=true
            shift
            ;;
        -h|--help)
            show_help
            ;;
        *)
            print_error "Unknown option: $1"
            show_help
            ;;
    esac
done

# Validate version format
if ! [[ $VERSION =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
    print_error "Invalid version format. Expected format: X.Y.Z (e.g., 1.0.3)"
    exit 1
fi

# Use environment variable if API key not provided
if [ -z "$API_KEY" ]; then
    API_KEY="${NUGET_API_KEY:-}"
fi

print_header "$PROJECT_NAME v$VERSION Release"

# Check if we're in the right directory
if [ ! -f "$PROJECT_FILE" ]; then
    print_error "Please run this script from the repository root directory!"
    echo -e "${YELLOW}Current directory: $(pwd)${NC}"
    echo -e "${YELLOW}Looking for: $PROJECT_FILE${NC}"
    exit 1
fi

print_success "Project found: $PROJECT_FILE"
echo ""

# Step 1: Check Git Status
echo -e "${YELLOW}[1/7] Checking Git status...${NC}"
if [ -n "$(git status --porcelain)" ]; then
    print_warning "You have uncommitted changes:"
    git status --short
    read -p "Do you want to continue? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        print_error "Release cancelled."
        exit 1
    fi
fi
print_success "Git status OK"
echo ""

# Step 2: Create Git Tag
if [ "$SKIP_TAG" = false ]; then
    echo -e "${YELLOW}[2/7] Creating Git tag v$VERSION...${NC}"
    
    if git rev-parse "v$VERSION" >/dev/null 2>&1; then
        print_warning "Tag v$VERSION already exists!"
        read -p "Do you want to delete and recreate it? (y/n) " -n 1 -r
        echo
        if [[ $REPLY =~ ^[Yy]$ ]]; then
            git tag -d "v$VERSION"
            git push origin ":refs/tags/v$VERSION" 2>/dev/null || true
            print_success "Existing tag deleted"
        else
            print_warning "Skipping tag creation."
            SKIP_TAG=true
        fi
    fi
    
    if [ "$SKIP_TAG" = false ]; then
        read -p "Enter tag message (or press Enter for default): " TAG_MESSAGE
        if [ -z "$TAG_MESSAGE" ]; then
            TAG_MESSAGE="Release v$VERSION"
        fi
        
        if git tag -a "v$VERSION" -m "$TAG_MESSAGE"; then
            print_success "Tag created successfully"
            
            echo -e "${YELLOW}  Pushing tag to GitHub...${NC}"
            if git push origin "v$VERSION"; then
                print_success "Tag pushed to GitHub"
            else
                print_error "Failed to push tag"
            fi
        else
            print_error "Failed to create tag"
            exit 1
        fi
    fi
else
    echo -e "${YELLOW}[2/7] Skipping Git tag creation (as requested)${NC}"
fi
echo ""

# Step 3: Clean
echo -e "${YELLOW}[3/7] Cleaning previous builds...${NC}"
dotnet clean --configuration Release --verbosity quiet
print_success "Clean completed"
echo ""

# Step 4: Restore
echo -e "${YELLOW}[4/7] Restoring NuGet packages...${NC}"
if dotnet restore --verbosity quiet; then
    print_success "Restore completed"
else
    print_error "Restore failed"
    exit 1
fi
echo ""

# Step 5: Build
echo -e "${YELLOW}[5/7] Building in Release configuration...${NC}"
if dotnet build --configuration Release --no-restore --verbosity quiet; then
    print_success "Build completed successfully"
else
    print_error "Build failed"
    exit 1
fi
echo ""

# Step 6: Create Package
echo -e "${YELLOW}[6/7] Creating NuGet package...${NC}"
mkdir -p nupkg
if dotnet pack "$PROJECT_FILE" --configuration Release --no-build --output nupkg --verbosity quiet; then
    print_success "Package created successfully"
    echo -e "${CYAN}  Package: nupkg/$PROJECT_NAME.$VERSION.nupkg${NC}"
    echo -e "${CYAN}  Symbols: nupkg/$PROJECT_NAME.$VERSION.snupkg${NC}"
else
    print_error "Package creation failed"
    exit 1
fi
echo ""

# Step 7: Publish to NuGet
if [ "$SKIP_NUGET" = false ]; then
    echo -e "${YELLOW}[7/7] Publishing to NuGet.org...${NC}"
    
    if [ -z "$API_KEY" ]; then
        print_warning "No API key provided!"
        read -p "Enter your NuGet API key (or press Enter to skip): " API_KEY
    fi
    
    if [ -n "$API_KEY" ]; then
        echo -e "${YELLOW}  Publishing package...${NC}"
        if dotnet nuget push "nupkg/$PROJECT_NAME.$VERSION.nupkg" --api-key "$API_KEY" --source https://api.nuget.org/v3/index.json --skip-duplicate; then
            print_success "Package published successfully!"
            echo ""
            echo -e "${CYAN}  Package will be available at:${NC}"
            echo -e "${CYAN}  https://www.nuget.org/packages/$PROJECT_NAME/$VERSION${NC}"
            echo ""
            print_warning "Note: It may take 5-10 minutes to appear in search results."
        else
            print_error "Package publication failed"
            echo -e "${YELLOW}  You can publish manually later using:${NC}"
            echo -e "${CYAN}  dotnet nuget push nupkg/$PROJECT_NAME.$VERSION.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json${NC}"
        fi
    else
        print_warning "Skipping NuGet publishing (no API key provided)"
    fi
else
    echo -e "${YELLOW}[7/7] Skipping NuGet publishing (as requested)${NC}"
fi

echo ""
print_header "Release Process Complete!"
echo ""
echo -e "${YELLOW}Release Summary:${NC}"
echo -e "${WHITE}  Version: v$VERSION${NC}"
echo -e "${WHITE}  Package: $PROJECT_NAME.$VERSION.nupkg${NC}"
echo -e "${WHITE}  Location: $(pwd)/nupkg${NC}"
echo ""
echo -e "${YELLOW}Next steps:${NC}"
echo -e "${WHITE}1. Create GitHub Release at: https://github.com/omostan/$PROJECT_NAME/releases/new${NC}"
echo -e "${WHITE}   - Select tag: v$VERSION${NC}"
echo -e "${WHITE}   - Title: v$VERSION - [Feature Name]${NC}"
echo -e "${WHITE}   - Copy description from: release-notes-v$VERSION.md${NC}"
echo ""
echo -e "${WHITE}2. Verify NuGet package at: https://www.nuget.org/packages/$PROJECT_NAME/${NC}"
echo ""
echo -e "${WHITE}3. Test installation:${NC}"
echo -e "${CYAN}   dotnet add package $PROJECT_NAME --version $VERSION${NC}"
echo ""
