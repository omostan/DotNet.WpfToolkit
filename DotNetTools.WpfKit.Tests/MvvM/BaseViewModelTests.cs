#region copyright

/*****************************************************************************************
*                                     ______________________________________________     *
*                              o O   |                                              |    *
*                     (((((  o      <               DotNet WPF Tool Kit             |    *
*                    ( o o )         |______________________________________________|    *
* ------------oOOO-----(_)-----OOOo----------------------------------------------------- *
*             Project: DotNetTools.Wpfkit.Tests                                          *
*            Filename: BaseViewModelTests.cs                                             *
*              Author: Stanley Omoregie                                                  *
*        Created Date: 20.11.2025                                                        *
*       Modified Date: 21.11.2025                                                        *
*          Created By: Stanley Omoregie                                                  *
*    Last Modified By: Stanley Omoregie                                                  *
*           CopyRight: copyright © 2025 Omotech Digital Solutions                        *
*                  .oooO  Oooo.                                                          *
*                  (   )  (   )                                                          *
* ------------------\ (----) /---------------------------------------------------------- *
*                    \_)  (_/                                                            *
*****************************************************************************************/

#endregion copyright

using DotNetTools.Wpfkit.MvvM;

namespace DotNetTools.Wpfkit.Tests.MvvM;

/// <summary>
/// Unit tests for the BaseViewModel class.
/// </summary>
public class BaseViewModelTests
{
    #region Test Helper Classes

    private class TestViewModel : BaseViewModel
    {
        public int LoadDataCallCount { get; private set; }

        public async Task LoadDataAsync()
        {
            IsBusy = true;
            try
            {
                await Task.Delay(10);
                LoadDataCallCount++;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    #endregion

    #region Title Property Tests

    [Fact]
    public void Title_DefaultValue_ShouldBeEmpty()
    {
        // Arrange & Act
        BaseViewModel viewModel = new();

        // Assert
        viewModel.Title.Should().Be(string.Empty);
    }

    [Fact]
    public void Title_WhenSet_ShouldUpdateValue()
    {
        // Arrange
        BaseViewModel viewModel = new();

        // Act
        viewModel.Title = "Test Title";

        // Assert
        viewModel.Title.Should().Be("Test Title");
    }

    [Fact]
    public void Title_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        BaseViewModel viewModel = new();
        bool eventRaised = false;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(BaseViewModel.Title))
                eventRaised = true;
        };

        // Act
        viewModel.Title = "New Title";

        // Assert
        eventRaised.Should().BeTrue();
    }

    #endregion

    #region Subtitle Property Tests

    [Fact]
    public void Subtitle_DefaultValue_ShouldBeEmpty()
    {
        // Arrange & Act
        BaseViewModel viewModel = new();

        // Assert
        viewModel.Subtitle.Should().Be(string.Empty);
    }

    [Fact]
    public void Subtitle_WhenSet_ShouldUpdateValue()
    {
        // Arrange
        BaseViewModel viewModel = new();

        // Act
        viewModel.Subtitle = "Test Subtitle";

        // Assert
        viewModel.Subtitle.Should().Be("Test Subtitle");
    }

    [Fact]
    public void Subtitle_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        BaseViewModel viewModel = new();
        bool eventRaised = false;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(BaseViewModel.Subtitle))
                eventRaised = true;
        };

        // Act
        viewModel.Subtitle = "New Subtitle";

        // Assert
        eventRaised.Should().BeTrue();
    }

    #endregion

    #region Icon Property Tests

    [Fact]
    public void Icon_DefaultValue_ShouldBeEmpty()
    {
        // Arrange & Act
        BaseViewModel viewModel = new();

        // Assert
        viewModel.Icon.Should().Be(string.Empty);
    }

    [Fact]
    public void Icon_WhenSet_ShouldUpdateValue()
    {
        // Arrange
        BaseViewModel viewModel = new();

        // Act
        viewModel.Icon = "icon.png";

        // Assert
        viewModel.Icon.Should().Be("icon.png");
    }

    [Fact]
    public void Icon_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        BaseViewModel viewModel = new();
        bool eventRaised = false;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(BaseViewModel.Icon))
                eventRaised = true;
        };

        // Act
        viewModel.Icon = "new-icon.png";

        // Assert
        eventRaised.Should().BeTrue();
    }

    #endregion

    #region IsBusy Property Tests

    [Fact]
    public void IsBusy_DefaultValue_ShouldBeFalse()
    {
        // Arrange & Act
        BaseViewModel viewModel = new();

        // Assert
        viewModel.IsBusy.Should().BeFalse();
    }

    [Fact]
    public void IsBusy_WhenSetToTrue_ShouldUpdateValue()
    {
        // Arrange
        BaseViewModel viewModel = new();

        // Act
        viewModel.IsBusy = true;

        // Assert
        viewModel.IsBusy.Should().BeTrue();
    }

    [Fact]
    public void IsBusy_WhenSetToTrue_ShouldSetIsNotBusyToFalse()
    {
        // Arrange
        BaseViewModel viewModel = new();

        // Act
        viewModel.IsBusy = true;

        // Assert
        viewModel.IsBusy.Should().BeTrue();
        viewModel.IsNotBusy.Should().BeFalse();
    }

    [Fact]
    public void IsBusy_WhenSet_ShouldRaisePropertyChangedForBothProperties()
    {
        // Arrange
        BaseViewModel viewModel = new();
        bool isBusyRaised = false;
        bool isNotBusyRaised = false;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(BaseViewModel.IsBusy))
                isBusyRaised = true;
            if (args.PropertyName == nameof(BaseViewModel.IsNotBusy))
                isNotBusyRaised = true;
        };

        // Act
        viewModel.IsBusy = true;

        // Assert
        isBusyRaised.Should().BeTrue();
        isNotBusyRaised.Should().BeTrue();
    }

    #endregion

    #region IsNotBusy Property Tests

    [Fact]
    public void IsNotBusy_DefaultValue_ShouldBeTrue()
    {
        // Arrange & Act
        BaseViewModel viewModel = new();

        // Assert
        viewModel.IsNotBusy.Should().BeTrue();
    }

    [Fact]
    public void IsNotBusy_WhenSetToFalse_ShouldSetIsBusyToTrue()
    {
        // Arrange
        BaseViewModel viewModel = new();

        // Act
        viewModel.IsNotBusy = false;

        // Assert
        viewModel.IsNotBusy.Should().BeFalse();
        viewModel.IsBusy.Should().BeTrue();
    }

    [Fact]
    public void IsNotBusy_WhenSet_ShouldRaisePropertyChangedForBothProperties()
    {
        // Arrange
        BaseViewModel viewModel = new();
        bool isBusyRaised = false;
        bool isNotBusyRaised = false;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(BaseViewModel.IsBusy))
                isBusyRaised = true;
            if (args.PropertyName == nameof(BaseViewModel.IsNotBusy))
                isNotBusyRaised = true;
        };

        // Act
        viewModel.IsNotBusy = false;

        // Assert
        isBusyRaised.Should().BeTrue();
        isNotBusyRaised.Should().BeTrue();
    }

    [Fact]
    public void IsBusy_And_IsNotBusy_ShouldBeInverse()
    {
        // Arrange
        BaseViewModel viewModel = new();

        // Act & Assert - Initial state
        viewModel.IsBusy.Should().BeFalse();
        viewModel.IsNotBusy.Should().BeTrue();

        // Act & Assert - Set IsBusy
        viewModel.IsBusy = true;
        viewModel.IsBusy.Should().BeTrue();
        viewModel.IsNotBusy.Should().BeFalse();

        // Act & Assert - Set IsNotBusy
        viewModel.IsNotBusy = true;
        viewModel.IsBusy.Should().BeFalse();
        viewModel.IsNotBusy.Should().BeTrue();
    }

    #endregion

    #region CanLoadMore Property Tests

    [Fact]
    public void CanLoadMore_DefaultValue_ShouldBeTrue()
    {
        // Arrange & Act
        BaseViewModel viewModel = new();

        // Assert
        viewModel.CanLoadMore.Should().BeTrue();
    }

    [Fact]
    public void CanLoadMore_WhenSet_ShouldUpdateValue()
    {
        // Arrange
        BaseViewModel viewModel = new();

        // Act
        viewModel.CanLoadMore = false;

        // Assert
        viewModel.CanLoadMore.Should().BeFalse();
    }

    [Fact]
    public void CanLoadMore_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        BaseViewModel viewModel = new();
        bool eventRaised = false;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(BaseViewModel.CanLoadMore))
                eventRaised = true;
        };

        // Act
        viewModel.CanLoadMore = false;

        // Assert
        eventRaised.Should().BeTrue();
    }

    #endregion

    #region Header Property Tests

    [Fact]
    public void Header_DefaultValue_ShouldBeEmpty()
    {
        // Arrange & Act
        BaseViewModel viewModel = new();

        // Assert
        viewModel.Header.Should().Be(string.Empty);
    }

    [Fact]
    public void Header_WhenSet_ShouldUpdateValue()
    {
        // Arrange
        BaseViewModel viewModel = new();

        // Act
        viewModel.Header = "Test Header";

        // Assert
        viewModel.Header.Should().Be("Test Header");
    }

    [Fact]
    public void Header_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        BaseViewModel viewModel = new();
        bool eventRaised = false;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(BaseViewModel.Header))
                eventRaised = true;
        };

        // Act
        viewModel.Header = "New Header";

        // Assert
        eventRaised.Should().BeTrue();
    }

    #endregion

    #region Footer Property Tests

    [Fact]
    public void Footer_DefaultValue_ShouldBeEmpty()
    {
        // Arrange & Act
        BaseViewModel viewModel = new();

        // Assert
        viewModel.Footer.Should().Be(string.Empty);
    }

    [Fact]
    public void Footer_WhenSet_ShouldUpdateValue()
    {
        // Arrange
        BaseViewModel viewModel = new();

        // Act
        viewModel.Footer = "Test Footer";

        // Assert
        viewModel.Footer.Should().Be("Test Footer");
    }

    [Fact]
    public void Footer_WhenSet_ShouldRaisePropertyChanged()
    {
        // Arrange
        BaseViewModel viewModel = new();
        bool eventRaised = false;

        viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(BaseViewModel.Footer))
                eventRaised = true;
        };

        // Act
        viewModel.Footer = "New Footer";

        // Assert
        eventRaised.Should().BeTrue();
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void BaseViewModel_ShouldInheritFromObservableObject()
    {
        // Arrange & Act
        BaseViewModel viewModel = new();

        // Assert
        viewModel.Should().BeAssignableTo<ObservableObject>();
    }

    [Fact]
    public async Task BaseViewModel_AsyncLoadingScenario_ShouldWorkCorrectly()
    {
        // Arrange
        TestViewModel viewModel = new();

        // Act
        Task loadTask = viewModel.LoadDataAsync();

        // Assert - During loading
        await Task.Delay(5); // Give it a moment to start
        // Note: IsBusy state might already be false if task completes quickly

        await loadTask;

        // Assert - After loading
        viewModel.IsBusy.Should().BeFalse();
        viewModel.IsNotBusy.Should().BeTrue();
        viewModel.LoadDataCallCount.Should().Be(1);
    }

    [Fact]
    public void BaseViewModel_MultiplePropertiesSet_ShouldAllUpdate()
    {
        // Arrange
        BaseViewModel viewModel = new();

        // Act
        viewModel.Title = "My App";
        viewModel.Subtitle = "Welcome";
        viewModel.Icon = "app-icon.png";
        viewModel.Header = "Dashboard";
        viewModel.Footer = "© 2025";
        viewModel.IsBusy = true;
        viewModel.CanLoadMore = false;

        // Assert
        viewModel.Title.Should().Be("My App");
        viewModel.Subtitle.Should().Be("Welcome");
        viewModel.Icon.Should().Be("app-icon.png");
        viewModel.Header.Should().Be("Dashboard");
        viewModel.Footer.Should().Be("© 2025");
        viewModel.IsBusy.Should().BeTrue();
        viewModel.IsNotBusy.Should().BeFalse();
        viewModel.CanLoadMore.Should().BeFalse();
    }

    [Fact]
    public void BaseViewModel_AllProperties_ShouldRaisePropertyChanged()
    {
        // Arrange
        BaseViewModel viewModel = new();
        List<string?> changedProperties = new();

        viewModel.PropertyChanged += (sender, args) => changedProperties.Add(args.PropertyName);

        // Act
        viewModel.Title = "Title";
        viewModel.Subtitle = "Subtitle";
        viewModel.Icon = "Icon";
        viewModel.Header = "Header";
        viewModel.Footer = "Footer";
        viewModel.IsBusy = true;
        viewModel.CanLoadMore = false;

        // Assert
        changedProperties.Should().Contain(nameof(BaseViewModel.Title));
        changedProperties.Should().Contain(nameof(BaseViewModel.Subtitle));
        changedProperties.Should().Contain(nameof(BaseViewModel.Icon));
        changedProperties.Should().Contain(nameof(BaseViewModel.Header));
        changedProperties.Should().Contain(nameof(BaseViewModel.Footer));
        changedProperties.Should().Contain(nameof(BaseViewModel.IsBusy));
        changedProperties.Should().Contain(nameof(BaseViewModel.IsNotBusy));
        changedProperties.Should().Contain(nameof(BaseViewModel.CanLoadMore));
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void BaseViewModel_SetSameValue_ShouldNotRaisePropertyChanged()
    {
        // Arrange
        BaseViewModel viewModel = new() { Title = "Test" };
        bool eventRaised = false;

        viewModel.PropertyChanged += (sender, args) => eventRaised = true;

        // Act
        viewModel.Title = "Test";

        // Assert
        eventRaised.Should().BeFalse();
    }

    [Fact]
    public void BaseViewModel_SetNullValues_ShouldHandleGracefully()
    {
        // Arrange
        BaseViewModel viewModel = new();

        // Act
        viewModel.Title = null!;
        viewModel.Subtitle = null!;
        viewModel.Icon = null!;

        // Assert
        viewModel.Title.Should().BeNull();
        viewModel.Subtitle.Should().BeNull();
        viewModel.Icon.Should().BeNull();
    }

    #endregion
}
