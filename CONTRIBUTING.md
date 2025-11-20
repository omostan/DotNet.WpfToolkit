# Contributing to DotNet.WpfToolKit

Thank you for your interest in contributing to DotNet.WpfToolKit! This document provides guidelines and instructions for contributing.

## ?? Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Process](#development-process)
- [Coding Standards](#coding-standards)
- [Submitting Changes](#submitting-changes)
- [Reporting Issues](#reporting-issues)

## ?? Code of Conduct

### Our Pledge
We are committed to providing a welcoming and inspiring community for everyone. Please be respectful and constructive in all interactions.

### Expected Behavior
- Use welcoming and inclusive language
- Be respectful of differing viewpoints and experiences
- Gracefully accept constructive criticism
- Focus on what is best for the community
- Show empathy towards other community members

## ?? Getting Started

### Prerequisites
- **Visual Studio 2022** (version 17.12 or later)
- **.NET 10.0 SDK** or later
- **Git** for version control
- **Windows OS** (for WPF development)

### Setting Up Your Development Environment

1. **Fork the repository**
   ```bash
   # Fork the repo on GitHub, then clone your fork
   git clone https://github.com/YOUR-USERNAME/DotNet.WpfToolKit.git
   cd DotNet.WpfToolKit
   ```

2. **Add upstream remote**
   ```bash
   git remote add upstream https://github.com/ORIGINAL-OWNER/DotNet.WpfToolKit.git
   ```

3. **Install dependencies**
   ```bash
   dotnet restore
   ```

4. **Build the solution**
   ```bash
   dotnet build
   ```

5. **Run tests** (when available)
   ```bash
   dotnet test
   ```

## ?? Development Process

### Branching Strategy

We follow a simplified Git flow:

- `main` - Production-ready code
- `develop` - Integration branch for features
- `feature/*` - New features
- `bugfix/*` - Bug fixes
- `hotfix/*` - Emergency fixes for production

### Creating a Feature Branch

```bash
# Update your fork
git checkout develop
git pull upstream develop

# Create a feature branch
git checkout -b feature/amazing-feature
```

## ?? Coding Standards

### C# Style Guidelines

Follow the official [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions):

1. **Naming Conventions**
   - PascalCase for class names, method names, and public members
   - camelCase for private fields (with underscore prefix: `_fieldName`)
   - camelCase for parameters and local variables
   - UPPER_CASE for constants

2. **Code Organization**
   - One class per file
   - Files named after the class they contain
   - Organize using statements alphabetically
   - Use file-scoped namespaces (C# 10+)

3. **Documentation**
   - Add XML documentation for all public APIs
   - Include `<summary>`, `<param>`, `<returns>` tags
   - Maintain the existing copyright header format:

   ```csharp
   #region copyright
   /*****************************************************************************************
   *                                     ______________________________________________     *
   *                              o O   |                                              |    *
   *                     (((((  o      <               DotNet WPF Tool Kit             |    *
   *                    ( o o )         |______________________________________________|    *
   * ------------oOOO-----(_)-----OOOo----------------------------------------------------- *
   *             Project: DotNet.WpfToolKit                                                 *
   *            Filename: YourFileName.cs                                                   *
   *              Author: Your Name                                                         *
   *        Created Date: DD.MM.YYYY                                                        *
   *       Modified Date: DD.MM.YYYY                                                        *
   *          Created By: Your Name                                                         *
   *    Last Modified By: Your Name                                                         *
   *           CopyRight: copyright � 2025 Omotech Digital Solutions                        *
   *                  .oooO  Oooo.                                                          *
   *                  (   )  (   )                                                          *
   * ------------------\ (----) /---------------------------------------------------------- *
   *                    \_)  (_/                                                            *
   *****************************************************************************************/
   #endregion copyright
   ```

4. **Modern C# Features**
   - Use nullable reference types
   - Leverage pattern matching
   - Use expression-bodied members where appropriate
   - Prefer `var` for obvious types
   - Use collection expressions (`[]`) for .NET 8+
   - Use primary constructors where applicable

### Code Quality

- **No compiler warnings** in Release builds
- Enable **code analysis** and fix warnings
- Follow **SOLID principles**
- Keep methods **small and focused**
- Write **self-documenting code**

### Example: Good Code Style

```csharp
namespace DotNet.WpfToolKit.MvvM;

/// <summary>
/// Represents a view model with loading state management.
/// </summary>
public class LoadingViewModel : BaseViewModel
{
    private string _statusMessage = string.Empty;

    /// <summary>
    /// Gets or sets the current status message.
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    /// <summary>
    /// Loads data asynchronously while updating the loading state.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task LoadDataAsync()
    {
        IsBusy = true;
        StatusMessage = "Loading...";
        
        try
        {
            // Load data
            await Task.Delay(1000); // Simulated work
            StatusMessage = "Data loaded successfully";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
```

## ?? Submitting Changes

### Commit Messages

Follow the [Conventional Commits](https://www.conventionalcommits.org/) specification:

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `perf`: Performance improvements
- `test`: Adding or updating tests
- `chore`: Maintenance tasks

**Examples:**
```bash
feat(mvvm): add validation support to BaseViewModel
fix(logging): correct line number tracking in LogManager
docs(readme): update installation instructions
```

### Pull Request Process

1. **Update your branch**
   ```bash
   git checkout feature/amazing-feature
   git fetch upstream
   git rebase upstream/develop
   ```

2. **Run tests and build**
   ```bash
   dotnet build
   dotnet test
   ```

3. **Push to your fork**
   ```bash
   git push origin feature/amazing-feature
   ```

4. **Create Pull Request**
   - Go to your fork on GitHub
   - Click "New Pull Request"
   - Select `develop` as the base branch
   - Provide a clear title and description
   - Reference any related issues

5. **PR Description Template**
   ```markdown
   ## Description
   Brief description of changes
   
   ## Type of Change
   - [ ] Bug fix
   - [ ] New feature
   - [ ] Breaking change
   - [ ] Documentation update
   
   ## Testing
   Describe testing done
   
   ## Checklist
   - [ ] Code follows style guidelines
   - [ ] Self-review completed
   - [ ] Comments added for complex code
   - [ ] Documentation updated
   - [ ] No new warnings
   - [ ] Tests added/updated
   ```

6. **Code Review**
   - Address reviewer feedback promptly
   - Make requested changes
   - Push updates to the same branch

## ?? Reporting Issues

### Before Submitting an Issue

1. **Check existing issues** to avoid duplicates
2. **Update to latest version** and test again
3. **Gather information** about your environment

### Issue Template

```markdown
**Description**
Clear description of the issue

**Steps to Reproduce**
1. Step one
2. Step two
3. ...

**Expected Behavior**
What should happen

**Actual Behavior**
What actually happens

**Environment**
- OS: [e.g., Windows 11]
- .NET Version: [e.g., .NET 10.0]
- Visual Studio Version: [e.g., VS 2022 17.12]
- Library Version: [e.g., 1.0.0]

**Additional Context**
Any other relevant information
```

## ?? Checklist for New Features

Before submitting a new feature:

- [ ] Feature aligns with project goals
- [ ] Code follows style guidelines
- [ ] XML documentation added
- [ ] Unit tests written (when applicable)
- [ ] README.md updated
- [ ] CHANGELOG.md updated
- [ ] No breaking changes (or clearly documented)
- [ ] Performance impact considered
- [ ] Cross-platform compatibility maintained (if applicable)

## ?? Learning Resources

### WPF Development
- [Microsoft WPF Documentation](https://docs.microsoft.com/wpf)
- [MVVM Pattern](https://docs.microsoft.com/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern)

### .NET Best Practices
- [.NET Coding Conventions](https://docs.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [Framework Design Guidelines](https://docs.microsoft.com/dotnet/standard/design-guidelines/)

### Git and GitHub
- [GitHub Flow](https://guides.github.com/introduction/flow/)
- [Git Documentation](https://git-scm.com/doc)

## ?? Contact

If you have questions about contributing:
- Open a discussion on GitHub
- Contact the maintainers

---

Thank you for contributing to DotNet.WpfToolKit! ??
