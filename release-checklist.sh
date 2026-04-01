#!/usr/bin/env bash

# One-command release checklist for DotNetTools.Wpfkit.
# It validates repo readiness and produces package artifacts, but does not publish.

set -u

RED='\033[0;31m'
PASS_COLOR='\033[38;2;2;174;86m'
YELLOW='\033[1;33m'
WARN_COLOR='\033[38;2;255;235;149m'
CYAN='\033[0;36m'
NC='\033[0m'

ALLOW_DIRTY=false
SKIP_PACK=false
CONFIGURATION="Release"

usage() {
  cat <<'EOF'
Release Checklist Script (DotNetTools.Wpfkit)

Usage:
  ./release-checklist.sh [options]

Options:
  --allow-dirty      Continue even if git has uncommitted changes
  --skip-pack        Skip dotnet pack step
  --configuration C  Build configuration (default: Release)
  -h, --help         Show help

Examples:
  ./release-checklist.sh
  ./release-checklist.sh --allow-dirty
  ./release-checklist.sh --configuration Release
EOF
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --allow-dirty) ALLOW_DIRTY=true; shift ;;
    --skip-pack) SKIP_PACK=true; shift ;;
    --configuration)
      CONFIGURATION="${2:-}"
      if [[ -z "$CONFIGURATION" ]]; then
        echo "Missing value for --configuration"
        exit 1
      fi
      shift 2
      ;;
    -h|--help) usage; exit 0 ;;
    *)
      echo "Unknown option: $1"
      usage
      exit 1
      ;;
  esac
done

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$SCRIPT_DIR"
PROJECT_FILE="$REPO_ROOT/DotNetTools.WpfKit/DotNetTools.Wpfkit.csproj"
TEST_PROJECT="$REPO_ROOT/DotNetTools.WpfKit.Tests/DotNetTools.Wpfkit.Tests.csproj"
NUPKG_DIR="$REPO_ROOT/nupkg"

# Checklist tracking
CHECK_NAMES=()
CHECK_STATUS=()
CHECK_DETAILS=()

add_result() {
  CHECK_NAMES+=("$1")
  CHECK_STATUS+=("$2")
  CHECK_DETAILS+=("$3")
}

run_step() {
  local name="$1"
  shift
  local details="$*"

  echo -e "${CYAN}→ ${name}${NC}"
  if eval "$details"; then
    add_result "$name" "PASS" ""
    echo -e "${PASS_COLOR}  ✓ Passed${NC}"
    return 0
  else
    add_result "$name" "FAIL" "Command failed"
    echo -e "${RED}  ✗ Failed${NC}"
    return 1
  fi
}

require_cmd() {
  command -v "$1" >/dev/null 2>&1
}

extract_xml_value() {
  local tag="$1"
  sed -n "s:.*<$tag>\(.*\)</$tag>.*:\1:p" "$PROJECT_FILE" | head -n 1
}

echo -e "${CYAN}==============================================${NC}"
echo -e "${CYAN} DotNetTools.Wpfkit One-Command Release Check ${NC}"
echo -e "${CYAN}==============================================${NC}"
echo

if [[ ! -f "$PROJECT_FILE" ]]; then
  echo -e "${RED}Project file not found: $PROJECT_FILE${NC}"
  exit 1
fi

if [[ ! -f "$TEST_PROJECT" ]]; then
  echo -e "${RED}Test project file not found: $TEST_PROJECT${NC}"
  exit 1
fi

if ! require_cmd dotnet; then
  echo -e "${RED}dotnet CLI is required but was not found in PATH.${NC}"
  exit 1
fi

if ! require_cmd git; then
  echo -e "${RED}git is required but was not found in PATH.${NC}"
  exit 1
fi

PACKAGE_ID="$(extract_xml_value PackageId)"
VERSION="$(extract_xml_value Version)"

if [[ -z "$PACKAGE_ID" || -z "$VERSION" ]]; then
  echo -e "${RED}Could not parse PackageId/Version from $PROJECT_FILE${NC}"
  exit 1
fi

EXPECTED_NUPKG="$NUPKG_DIR/${PACKAGE_ID}.${VERSION}.nupkg"
EXPECTED_SNUPKG="$NUPKG_DIR/${PACKAGE_ID}.${VERSION}.snupkg"
EXPECTED_RELEASE_NOTES="$REPO_ROOT/release-notes-v${VERSION}.md"

echo -e "${YELLOW}Package:${NC} $PACKAGE_ID"
echo -e "${YELLOW}Version:${NC} $VERSION"
echo -e "${YELLOW}Configuration:${NC} $CONFIGURATION"
echo

# 1) Git working tree check
if [[ -n "$(git -C "$REPO_ROOT" status --porcelain)" ]]; then
  if [[ "$ALLOW_DIRTY" == true ]]; then
    add_result "Git working tree clean" "WARN" "Dirty tree allowed by --allow-dirty"
    echo -e "${WARN_COLOR}⚠ Git working tree has uncommitted changes (allowed).${NC}"
  else
    add_result "Git working tree clean" "FAIL" "Uncommitted changes detected"
    echo -e "${RED}✗ Uncommitted changes detected. Re-run with --allow-dirty to override.${NC}"
  fi
else
  add_result "Git working tree clean" "PASS" ""
  echo -e "${PASS_COLOR}✓ Git working tree is clean.${NC}"
fi

# 2) Release notes existence check
if [[ -f "$EXPECTED_RELEASE_NOTES" ]]; then
  add_result "Release notes file exists" "PASS" ""
  echo -e "${PASS_COLOR}✓ Found release notes: $(basename "$EXPECTED_RELEASE_NOTES")${NC}"
else
  add_result "Release notes file exists" "WARN" "Missing $(basename "$EXPECTED_RELEASE_NOTES")"
  echo -e "${WARN_COLOR}⚠ Missing release notes file: $(basename "$EXPECTED_RELEASE_NOTES")${NC}"
fi

echo

# Stop early only for hard failures so far
for i in "${!CHECK_STATUS[@]}"; do
  if [[ "${CHECK_STATUS[$i]}" == "FAIL" && "${CHECK_NAMES[$i]}" == "Git working tree clean" ]]; then
    echo -e "${RED}Checklist stopped due to hard precondition failure.${NC}"
    break
  fi
done

# Continue only if we do not have hard-fail from clean tree requirement
HARD_FAIL=false
for i in "${!CHECK_STATUS[@]}"; do
  if [[ "${CHECK_STATUS[$i]}" == "FAIL" && "${CHECK_NAMES[$i]}" == "Git working tree clean" ]]; then
    HARD_FAIL=true
  fi
done

if [[ "$HARD_FAIL" == false ]]; then
  run_step "dotnet restore" "dotnet restore \"$REPO_ROOT/DotNetTools.slnx\" -v minimal" || true
  run_step "dotnet build ($CONFIGURATION)" "dotnet build \"$REPO_ROOT/DotNetTools.slnx\" -c \"$CONFIGURATION\" --no-restore -v minimal" || true
  run_step "dotnet test ($CONFIGURATION)" "dotnet test \"$REPO_ROOT/DotNetTools.slnx\" -c \"$CONFIGURATION\" --no-build -v minimal" || true

  if [[ "$SKIP_PACK" == true ]]; then
    add_result "dotnet pack ($CONFIGURATION)" "WARN" "Skipped by --skip-pack"
    echo -e "${WARN_COLOR}⚠ Skipping pack step (--skip-pack).${NC}"
  else
    run_step "dotnet pack ($CONFIGURATION)" "dotnet pack \"$PROJECT_FILE\" -c \"$CONFIGURATION\" --no-build -o \"$NUPKG_DIR\"" || true
  fi

  if [[ "$SKIP_PACK" == true ]]; then
    add_result "Package artifacts generated" "WARN" "Not checked because pack was skipped"
  else
    if [[ -f "$EXPECTED_NUPKG" && -f "$EXPECTED_SNUPKG" ]]; then
      add_result "Package artifacts generated" "PASS" ""
      echo -e "${PASS_COLOR}✓ Found artifacts:${NC}"
      echo "  - $(basename "$EXPECTED_NUPKG")"
      echo "  - $(basename "$EXPECTED_SNUPKG")"
    else
      add_result "Package artifacts generated" "FAIL" "Expected package files not found"
      echo -e "${RED}✗ Missing expected package artifacts for $PACKAGE_ID $VERSION${NC}"
    fi
  fi
fi

echo
echo -e "${CYAN}Release Checklist Summary${NC}"
echo "----------------------------------------------"
PASS_COUNT=0
WARN_COUNT=0
FAIL_COUNT=0
for i in "${!CHECK_NAMES[@]}"; do
  name="${CHECK_NAMES[$i]}"
  status="${CHECK_STATUS[$i]}"
  detail="${CHECK_DETAILS[$i]}"
  case "$status" in
    PASS)
      PASS_COUNT=$((PASS_COUNT + 1))
      printf "${PASS_COLOR}[PASS]${NC} %s\n" "$name"
      ;;
    WARN)
      WARN_COUNT=$((WARN_COUNT + 1))
      printf "${WARN_COLOR}[WARN]${NC} %s - %s\n" "$name" "$detail"
      ;;
    FAIL)
      FAIL_COUNT=$((FAIL_COUNT + 1))
      printf "${RED}[FAIL]${NC} %s - %s\n" "$name" "$detail"
      ;;
  esac
done

echo "----------------------------------------------"
printf "${PASS_COLOR}PASS:${NC} %d  ${WARN_COLOR}WARN:${NC} %d  ${RED}FAIL:${NC} %d\n" "$PASS_COUNT" "$WARN_COUNT" "$FAIL_COUNT"

if [[ $FAIL_COUNT -gt 0 ]]; then
  echo -e "${RED}Release checklist failed.${NC}"
  exit 1
fi

echo -e "${PASS_COLOR}Release checklist passed.${NC}"
exit 0

