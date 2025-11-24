# Release Notes - v1.0.3

**Release Date:** January 2025  
**Package:** DotNetTools.Wpfkit  
**Version:** 1.0.3

## 🐛 Bug Fixes

### RelayCommand Visibility Fixed

**Issue:** The `RelayCommand` class was not properly accessible to developers due to incorrect visibility modifiers.

**Resolution:** `RelayCommand` has been made `public`, allowing developers to properly implement and use this command type in their MVVM applications.

### What Changed

- **RelayCommand** is now `public` and fully accessible
- Developers can now directly instantiate and use `RelayCommand` in their view models
- Maintains the same functionality as before: synchronous command with action and predicate support

### Usage Example

```csharp
using DotNetTools.Wpfkit.Commands;
using System.Windows.Input;

public class MyViewModel : BaseViewModel
{
    public ICommand SaveCommand { get; }
    
    public MyViewModel()
    {
        // Now works correctly - RelayCommand is public
        SaveCommand = new RelayCommand(
            action: param => SaveData(param),
            predicate: param => CanSave()
        );
    }
    
    private void SaveData(object parameter)
    {
        // Save logic
    }
    
    private bool CanSave()
    {
        return !string.IsNullOrEmpty(DataToSave);
    }
}
```

## 📝 Migration Notes

If you were working around this issue in v1.0.2, you can now:
- Replace any workarounds with direct `RelayCommand` usage
- Use `RelayCommand` as intended for synchronous command scenarios
- Continue using `ActionCommand` as an alternative (both are valid)

## 🔄 Breaking Changes

**None** - This is a bug fix that only improves accessibility. No breaking changes to existing code.

## 📦 Package Information

- **NuGet Package:** DotNetTools.Wpfkit
- **Target Framework:** .NET 10.0
- **Dependencies:**
  - Serilog 4.3.0+
  - Tracetool.DotNet.Api 14.0.0+

## 🔗 Links

- [GitHub Repository](https://github.com/omostan/DotNetTools.Wpfkit)
- [NuGet Package](https://www.nuget.org/packages/DotNetTools.Wpfkit/)
- [Documentation](https://github.com/omostan/DotNetTools.Wpfkit/blob/main/DotNetTools.WpfKit/README.md)
- [Report Issues](https://github.com/omostan/DotNetTools.Wpfkit/issues)

## 🙏 Acknowledgments

Thank you to the community for reporting this issue and helping us improve the toolkit!

---

**Copyright © 2025 Omotech Digital Solutions**  
**Author:** Stanley Omoregie
