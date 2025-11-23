# 🔧 Quick Fix: NuGet API Key 403 Error

## Your Error
```
error: Response status code does not indicate success: 403 
(The specified API key is invalid, has expired, or does not 
have permission to access the specified package.)
```

## 3-Step Quick Fix

### Step 1: Create New API Key (2 minutes)

1. **Open NuGet API Keys page**:
   ```
   https://www.nuget.org/account/apikeys
   ```

2. **Click "Create" button** (blue button, top right)

3. **Fill in the form**:
   ```
   Key Name:     DotNet.WpfToolkit-Publish
   Expiration:   365 days
   Owner:        [Your username]
   
   Select Packages:  ✅ All packages (select this!)
   
   Scopes:
   ✅ Push new packages and package versions
   
   Glob Pattern:  * (leave as is)
   ```

4. **Click "Create"**

5. **COPY THE KEY IMMEDIATELY** - it looks like:
   ```
   oy2abcdefghijklmnopqrstuvwxyz1234567890abcdefghijk
   ```
   
   ⚠️ **You'll only see it once!**

### Step 2: Store the Key (30 seconds)

**Git Bash:**
```bash
export NUGET_API_KEY="paste-your-key-here"
```

**PowerShell:**
```powershell
$env:NUGET_API_KEY = "paste-your-key-here"
```

**Verify it's stored:**
```bash
# Git Bash
echo $NUGET_API_KEY

# PowerShell  
echo $env:NUGET_API_KEY
```

You should see your key printed out.

### Step 3: Publish Again (1 minute)

```bash
cd D:/Tutorials/DotNet/DotNet.WpfToolkit

dotnet nuget push nupkg/DotNet.WpfToolKit.1.0.0.nupkg \
  --api-key $NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json \
  --skip-duplicate
```

## Success Looks Like This:

```
Pushing DotNet.WpfToolKit.1.0.0.nupkg to 'https://api.nuget.org/v3/index.json'...
  PUT https://api.nuget.org/v3/index.json
  Created https://api.nuget.org/v3/index.json 5678ms
Your package was pushed.
```

## Still Getting 403? Try This:

### Option A: Use Key Directly (Testing)
```bash
dotnet nuget push nupkg/DotNet.WpfToolKit.1.0.0.nupkg \
  --api-key "paste-actual-key-here" \
  --source https://api.nuget.org/v3/index.json
```

### Option B: Web Upload
1. Go to: https://www.nuget.org/packages/manage/upload
2. Click "Browse"
3. Select: `nupkg/DotNet.WpfToolKit.1.0.0.nupkg`
4. Click "Upload"
5. Verify and publish

## API Key Checklist

Before trying again, verify:

| Check | Status |
|-------|--------|
| ✅ Logged into NuGet.org | [ ] |
| ✅ API key not expired | [ ] |
| ✅ "Push" permission enabled | [ ] |
| ✅ "All packages" selected | [ ] |
| ✅ Key copied correctly (no spaces) | [ ] |
| ✅ Using correct source URL | [ ] |
| ✅ Package file exists | [ ] |

## Screenshot Guide

### Creating API Key - What to Look For:

```
┌─────────────────────────────────────────────┐
│  Create API Key                             │
├─────────────────────────────────────────────┤
│  Key Name:  [DotNet.WpfToolkit-Publish]     │
│                                             │
│  Expiration:  [365 days ▼]                  │
│                                             │
│  Select Packages:                           │
│  ✅ All packages         ← IMPORTANT!	      │
│  ⭕ Only selected packages		      │
│                                             │
│  Glob Pattern: [*]                          │
│                                             │
│  Scopes:                                    │
│  ✅ Push new packages and package versions  │
│  ⭕ Push only new package versions	      │
│  ⭕ Unlist			              │
│                                             │
│         [Create]  [Cancel]                  │
└─────────────────────────────────────────────┘
```

### After Clicking Create:

```
┌──────────────────────────────────────────┐
│  ⚠️ Save your API key!		   |
│                                          │
│  Copy and save this key now. You won't be│
│  able to see it again.                   │
│                                          │
│  oy2abc...xyz123                         │
│                   [Copy] ← CLICK THIS!   │
│                                          │
│  [I have saved my key]                   │
└──────────────────────────────────────────┘
```

## Common Mistakes

### ❌ Wrong: Restricted to specific package
```
Select Packages: ⭕ Only selected packages
Glob Pattern: [MyOtherPackage*]
```
This won't work for first-time publishing `DotNet.WpfToolKit`!

### ✅ Correct: All packages
```
Select Packages: ✅ All packages
Glob Pattern: [*]
```

### ❌ Wrong: No push permission
```
Scopes:
⭕ Push new packages and package versions
✅ Unlist
```

### ✅ Correct: Push permission enabled
```
Scopes:
✅ Push new packages and package versions
```

## Need More Help?

📚 **Detailed Guide**: `NUGET_API_KEY_TROUBLESHOOTING.md`
📖 **Publishing Guide**: `NUGET_PUBLISHING_GUIDE.md`

## Quick Commands Reference

```bash
# Git Bash - Complete sequence
export NUGET_API_KEY="your-key-here"
cd D:/Tutorials/DotNet/DotNet.WpfToolkit
dotnet nuget push nupkg/DotNet.WpfToolKit.1.0.0.nupkg \
  --api-key $NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json

# PowerShell - Complete sequence
$env:NUGET_API_KEY = "your-key-here"
cd D:\Tutorials\DotNet\DotNet.WpfToolkit
dotnet nuget push nupkg/DotNet.WpfToolKit.1.0.0.nupkg `
  --api-key $env:NUGET_API_KEY `
  --source https://api.nuget.org/v3/index.json
```

---

**Most likely fix**: Create a fresh API key with "All packages" and "Push" permission! ✅
