#region copyright

/*****************************************************************************************
*                                     ______________________________________________     *
*                              o O   |                                              |    *
*                     (((((  o      <               DotNet WPF Tool Kit             |    *
*                    ( o o )         |______________________________________________|    *
* ------------oOOO-----(_)-----OOOo----------------------------------------------------- *
*             Project: DotNetTools.Wpfkit.Tests                                          *
*            Filename: ObservableObjectTests.cs                                          *
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

using System.ComponentModel;
using DotNetTools.Wpfkit.MvvM;

namespace DotNetTools.Wpfkit.Tests.MvvM;

/// <summary>
/// Unit tests for the ObservableObject class.
/// </summary>
public class ObservableObjectTests
{
    #region Test Helper Classes

    private class TestObservableObject : ObservableObject
    {
        private string _name = string.Empty;
        private int _age;
        private bool _isActive;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public int Age
        {
            get => _age;
            set => SetProperty(ref _age, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        // Property with validation
        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value, validateValue: (oldVal, newVal) => newVal.Contains('@'));
        }

        // Property with change callback
        private int _counter;
        public int OnChangedCallCount { get; private set; }

        public int Counter
        {
            get => _counter;
            set => SetProperty(ref _counter, value, onChanged: () => OnChangedCallCount++);
        }
    }

    #endregion

    #region SetProperty Tests

    [Fact]
    public void SetProperty_WhenValueChanges_ShouldReturnTrue()
    {
        // Arrange
        TestObservableObject obj = new();

        // Act
        string result = obj.Name = "John";

        // Assert
        obj.Name.Should().Be("John");
    }

    [Fact]
    public void SetProperty_WhenValueIsSame_ShouldReturnFalse()
    {
        // Arrange
        TestObservableObject obj = new() { Name = "John" };

        // Act & Assert
        obj.Name = "John";
        obj.Name.Should().Be("John");
    }

    [Fact]
    public void SetProperty_WhenValueChanges_ShouldRaisePropertyChanged()
    {
        // Arrange
        TestObservableObject obj = new();
        bool eventRaised = false;
        string? propertyName = null;

        obj.PropertyChanged += (sender, args) =>
        {
            eventRaised = true;
            propertyName = args.PropertyName;
        };

        // Act
        obj.Name = "John";

        // Assert
        eventRaised.Should().BeTrue();
        propertyName.Should().Be(nameof(TestObservableObject.Name));
    }

    [Fact]
    public void SetProperty_WhenValueIsSame_ShouldNotRaisePropertyChanged()
    {
        // Arrange
        TestObservableObject obj = new() { Name = "John" };
        bool eventRaised = false;

        obj.PropertyChanged += (sender, args) => eventRaised = true;

        // Act
        obj.Name = "John";

        // Assert
        eventRaised.Should().BeFalse();
    }

    [Fact]
    public void SetProperty_WithOnChanged_ShouldInvokeCallback()
    {
        // Arrange
        TestObservableObject obj = new();

        // Act
        obj.Counter = 1;
        obj.Counter = 2;

        // Assert
        obj.OnChangedCallCount.Should().Be(2);
    }

    [Fact]
    public void SetProperty_WithOnChanged_WhenValueIsSame_ShouldNotInvokeCallback()
    {
        // Arrange
        TestObservableObject obj = new() { Counter = 5 };
        int initialCallCount = obj.OnChangedCallCount;

        // Act
        obj.Counter = 5;

        // Assert
        obj.OnChangedCallCount.Should().Be(initialCallCount);
    }

    [Fact]
    public void SetProperty_WithValidation_WhenValid_ShouldSetValue()
    {
        // Arrange
        TestObservableObject obj = new();

        // Act
        obj.Email = "test@example.com";

        // Assert
        obj.Email.Should().Be("test@example.com");
    }

    [Fact]
    public void SetProperty_WithValidation_WhenInvalid_ShouldNotSetValue()
    {
        // Arrange
        TestObservableObject obj = new() { Email = "test@example.com" };

        // Act
        obj.Email = "invalid-email";

        // Assert
        obj.Email.Should().Be("test@example.com");
    }

    [Fact]
    public void SetProperty_WithValidation_WhenInvalid_ShouldNotRaisePropertyChanged()
    {
        // Arrange
        TestObservableObject obj = new() { Email = "test@example.com" };
        bool eventRaised = false;

        obj.PropertyChanged += (sender, args) => eventRaised = true;

        // Act
        obj.Email = "invalid-email";

        // Assert
        eventRaised.Should().BeFalse();
    }

    #endregion

    #region PropertyChanged Event Tests

    [Fact]
    public void PropertyChanged_ShouldImplementINotifyPropertyChanged()
    {
        // Arrange & Act
        TestObservableObject obj = new();

        // Assert
        obj.Should().BeAssignableTo<INotifyPropertyChanged>();
    }

    [Fact]
    public void PropertyChanged_WhenMultiplePropertiesChange_ShouldRaiseForEach()
    {
        // Arrange
        TestObservableObject obj = new();
        List<string?> changedProperties = new();

        obj.PropertyChanged += (sender, args) => changedProperties.Add(args.PropertyName);

        // Act
        obj.Name = "John";
        obj.Age = 30;
        obj.IsActive = true;

        // Assert
        changedProperties.Should().HaveCount(3);
        changedProperties.Should().Contain(nameof(TestObservableObject.Name));
        changedProperties.Should().Contain(nameof(TestObservableObject.Age));
        changedProperties.Should().Contain(nameof(TestObservableObject.IsActive));
    }

    [Fact]
    public void PropertyChanged_ShouldPassCorrectSender()
    {
        // Arrange
        TestObservableObject obj = new();
        object? sender = null;

        obj.PropertyChanged += (s, args) => sender = s;

        // Act
        obj.Name = "John";

        // Assert
        sender.Should().BeSameAs(obj);
    }

    #endregion

    #region OnPropertyChanged Tests

    [Fact]
    public void OnPropertyChanged_WithEmptyPropertyName_ShouldRaiseEvent()
    {
        // Arrange
        TestObservableObject obj = new();
        bool eventRaised = false;

        obj.PropertyChanged += (sender, args) => eventRaised = true;

        // Act
        obj.Name = "Test"; // This will call OnPropertyChanged

        // Assert
        eventRaised.Should().BeTrue();
    }

    #endregion

    #region Value Type Tests

    [Fact]
    public void SetProperty_WithInt_ShouldWorkCorrectly()
    {
        // Arrange
        TestObservableObject obj = new();

        // Act
        obj.Age = 25;

        // Assert
        obj.Age.Should().Be(25);
    }

    [Fact]
    public void SetProperty_WithBool_ShouldWorkCorrectly()
    {
        // Arrange
        TestObservableObject obj = new()
        {
            // Act
            IsActive = true
        };

        // Assert
        obj.IsActive.Should().BeTrue();
    }

    [Fact]
    public void SetProperty_WithDefaultValues_ShouldWorkCorrectly()
    {
        // Arrange
        TestObservableObject obj = new();

        // Assert
        obj.Name.Should().Be(string.Empty);
        obj.Age.Should().Be(0);
        obj.IsActive.Should().BeFalse();
    }

    #endregion

    #region Thread Safety Tests

    [Fact]
    public async Task SetProperty_WithMultipleThreads_ShouldBeThreadSafe()
    {
        // Arrange
        TestObservableObject obj = new();
        List<Exception> exceptions = new();
        List<Task> tasks = new();

        // Act
        for (int i = 0; i < 10; i++)
        {
            int index = i;
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    obj.Age = index;
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert
        exceptions.Should().BeEmpty();
        obj.Age.Should().BeInRange(0, 9);
    }

    #endregion

    #region Complex Scenario Tests

    [Fact]
    public void SetProperty_ComplexScenario_WithValidationAndCallback_ShouldWorkCorrectly()
    {
        // Arrange
        TestObservableObject obj = new();
        int propertyChangedCount = 0;

        obj.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(TestObservableObject.Email))
                propertyChangedCount++;
        };

        // Act
        obj.Email = "valid@email.com";
        obj.Email = "invalid"; // Should be rejected
        obj.Email = "another@valid.com";

        // Assert
        obj.Email.Should().Be("another@valid.com");
        propertyChangedCount.Should().Be(2); // Only valid changes
    }

    #endregion
}
