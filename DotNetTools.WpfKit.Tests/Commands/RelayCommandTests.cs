#region copyright

/*****************************************************************************************
*                                     ______________________________________________     *
*                              o O   |                                              |    *
*                     (((((  o      <               DotNet WPF Tool Kit             |    *
*                    ( o o )         |______________________________________________|    *
* ------------oOOO-----(_)-----OOOo----------------------------------------------------- *
*             Project: DotNetTools.Wpfkit.Tests                                          *
*            Filename: RelayCommandTests.cs                                              *
*              Author: Stanley Omoregie                                                  *
*        Created Date: 25.01.2025                                                        *
*       Modified Date: 25.01.2025                                                        *
*          Created By: Stanley Omoregie                                                  *
*    Last Modified By: Stanley Omoregie                                                  *
*           CopyRight: copyright © 2025 Omotech Digital Solutions                        *
*                  .oooO  Oooo.                                                          *
*                  (   )  (   )                                                          *
* ------------------\ (----) /---------------------------------------------------------- *
*                    \_)  (_/                                                            *
*****************************************************************************************/

#endregion copyright

using DotNetTools.Wpfkit.Commands;
using System.Diagnostics;
using System.Windows.Input;

namespace DotNetTools.Wpfkit.Tests.Commands;

/// <summary>
/// Unit tests for the RelayCommand class.
/// RelayCommand extends ActionCommand, so these tests verify the inheritance works correctly.
/// </summary>
public class RelayCommandTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidAction_ShouldCreateCommand()
    {
        // Arrange & Act
        RelayCommand command = new(param => { });

        // Assert
        command.Should().NotBeNull();
        command.Should().BeAssignableTo<ActionCommand>();
        command.Should().BeAssignableTo<ICommand>();
    }

    [Fact]
    public void Constructor_WithNullAction_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new RelayCommand(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithActionAndPredicate_ShouldCreateCommand()
    {
        // Arrange & Act
        RelayCommand command = new(
            param => { },
            param => true
        );

        // Assert
        command.Should().NotBeNull();
    }

    #endregion

    #region Inheritance Tests

    [Fact]
    public void RelayCommand_ShouldInheritFromActionCommand()
    {
        // Arrange & Act
        RelayCommand command = new(param => { });

        // Assert
        command.Should().BeAssignableTo<ActionCommand>();
    }

    [Fact]
    public void RelayCommand_ShouldImplementICommand()
    {
        // Arrange & Act
        RelayCommand command = new(param => { });

        // Assert
        command.Should().BeAssignableTo<ICommand>();
    }

    #endregion

    #region CanExecute Tests

    [Fact]
    public void CanExecute_WithNoPredicate_ShouldReturnTrue()
    {
        // Arrange
        RelayCommand command = new(param => { });

        // Act
        bool result = command.CanExecute(null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WithNullParameter_AndPredicate_ShouldHandleGracefully()
    {
        // Arrange
        RelayCommand command = new(
            param => { },
            param => param != null
        );

        // Act
        bool result = command.CanExecute(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanExecute_WithValidParameter_AndPredicate_ShouldReturnTrue()
    {
        // Arrange
        RelayCommand command = new(
            param => { },
            param => param is string
        );

        // Act
        bool result = command.CanExecute("test");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WhenPredicateThrows_ShouldReturnFalse()
    {
        // Arrange
        RelayCommand command = new(
            param => { },
            param => throw new InvalidOperationException()
        );

        // Act
        bool result = command.CanExecute(null);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Execute Tests

    [Fact]
    public void Execute_ShouldInvokeAction()
    {
        // Arrange
        bool executed = false;
        RelayCommand command = new(param => executed = true);

        // Act
        command.Execute(null);

        // Assert
        executed.Should().BeTrue();
    }

    [Fact]
    public void Execute_WithParameter_ShouldPassToAction()
    {
        // Arrange
        object? capturedParam = null;
        RelayCommand command = new(param => capturedParam = param);

        // Act
        command.Execute("test");

        // Assert
        capturedParam.Should().Be("test");
    }

    [Fact]
    public void Execute_MultipleTimes_ShouldWorkCorrectly()
    {
        // Arrange
        int count = 0;
        RelayCommand command = new(param => count++);

        // Act
        command.Execute(null);
        command.Execute(null);
        command.Execute(null);

        // Assert
        count.Should().Be(3);
    }

    #endregion

    #region Real-World MVVM Scenarios

    [Fact]
    public void MVVM_Scenario_ButtonCommand_ShouldWork()
    {
        // Arrange
        bool clicked = false;
        RelayCommand command = new(
            param => clicked = true,
            param => !clicked // Can only click once
        );

        // Act & Assert
        command.CanExecute(null).Should().BeTrue();
        command.Execute(null);
        clicked.Should().BeTrue();
        command.CanExecute(null).Should().BeFalse();
    }

    [Fact]
    public void MVVM_Scenario_SearchCommand_ShouldWork()
    {
        // Arrange
        string? searchTerm = null;
        RelayCommand command = new(
            param => searchTerm = param as string,
            param => param is string str && !string.IsNullOrWhiteSpace(str)
        );

        // Act & Assert
        command.CanExecute(null).Should().BeFalse();
        command.CanExecute("").Should().BeFalse();
        command.CanExecute("   ").Should().BeFalse();
        command.CanExecute("search").Should().BeTrue();

        command.Execute("search");
        searchTerm.Should().Be("search");
    }

    [Fact]
    public void MVVM_Scenario_DeleteCommand_WithSelectedItem_ShouldWork()
    {
        // Arrange
        object? selectedItem = null;
        List<object> deletedItems = new();

        RelayCommand command = new(
            param =>
            {
                if (param != null)
                    deletedItems.Add(param);
            },
            param => param != null
        );

        // Act & Assert
        command.CanExecute(selectedItem).Should().BeFalse();

        selectedItem = "Item1";
        command.CanExecute(selectedItem).Should().BeTrue();
        command.Execute(selectedItem);

        deletedItems.Should().ContainSingle().Which.Should().Be("Item1");
    }

    [Fact]
    public void MVVM_Scenario_NavigationCommand_ShouldWork()
    {
        // Arrange
        string? currentView = "Home";
        RelayCommand command = new(
            param =>
            {
                if (param is string view)
                    currentView = view;
            },
            param => param is string view && view != currentView
        );

        // Act & Assert
        command.CanExecute("Home").Should().BeFalse(); // Already on Home
        command.CanExecute("Settings").Should().BeTrue();

        command.Execute("Settings");
        currentView.Should().Be("Settings");

        command.CanExecute("Settings").Should().BeFalse(); // Already on Settings
        command.CanExecute("Home").Should().BeTrue();
    }

    #endregion

    #region DataGrid Integration Tests

    [Fact]
    public void DataGrid_ColumnCommand_WithNullRow_ShouldNotCrash()
    {
        // Simulates DataGrid passing null during initialization
        // Arrange
        RelayCommand command = new(
            param => { /* Edit row */ },
            param => param != null
        );

        // Act & Assert
        Action act = () =>
        {
            bool canExecute = command.CanExecute(null);
            canExecute.Should().BeFalse();
        };

        act.Should().NotThrow<NullReferenceException>();
    }

    [Fact]
    public void DataGrid_RowCommand_WithValidRow_ShouldExecute()
    {
        // Arrange
        string editedRow = "";
        RelayCommand command = new(
            param => editedRow = param?.ToString() ?? "",
            param => param != null
        );

        // Act
        command.Execute("Row1");

        // Assert
        editedRow.Should().Be("Row1");
    }

    #endregion

    #region Complex Predicate Tests

    [Fact]
    public void ComplexPredicate_WithMultipleConditions_ShouldEvaluateCorrectly()
    {
        // Arrange
        RelayCommand command = new(
            param => { },
            param =>
            {
                if (param is not string str) return false;
                if (string.IsNullOrWhiteSpace(str)) return false;
                if (str.Length < 3) return false;
                if (!str.Contains("@")) return false;
                return true;
            }
        );

        // Act & Assert
        command.CanExecute(null).Should().BeFalse();
        command.CanExecute("").Should().BeFalse();
        command.CanExecute("ab").Should().BeFalse();
        command.CanExecute("abc").Should().BeFalse();
        command.CanExecute("test@example").Should().BeTrue();
    }

    [Fact]
    public void ComplexPredicate_WithExceptionHandling_ShouldReturnFalse()
    {
        // Arrange
        RelayCommand command = new(
            param => { },
            param =>
            {
                string str = (string)param!; // Will throw on null or non-string
                return str.Length > 0;
            }
        );

        // Act & Assert
        command.CanExecute(null).Should().BeFalse();
        command.CanExecute(123).Should().BeFalse();
        command.CanExecute("test").Should().BeTrue();
    }

    #endregion

    #region Performance Tests

    [Fact]
    public void RelayCommand_MultipleExecutions_ShouldPerformWell()
    {
        // Arrange
        int counter = 0;
        RelayCommand command = new(param => counter++);

        Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 10000; i++)
        {
            command.Execute(null);
        }

        stopwatch.Stop();

        // Assert
        counter.Should().Be(10000);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
    }

    #endregion

    #region CanExecuteChanged Event Tests

    [Fact]
    public void CanExecuteChanged_AddRemoveHandler_ShouldNotThrow()
    {
        // Arrange
        RelayCommand command = new(param => { });
        EventHandler handler = (s, e) => { };

        // Act & Assert
        Action addAct = () => command.CanExecuteChanged += handler;
        Action removeAct = () => command.CanExecuteChanged -= handler;

        addAct.Should().NotThrow();
        removeAct.Should().NotThrow();
    }

    #endregion

    #region Comparison with ActionCommand

    [Fact]
    public void RelayCommand_ShouldBehaveIdenticallyToActionCommand()
    {
        // Arrange
        bool actionCommandExecuted = false;
        bool relayCommandExecuted = false;

        ActionCommand actionCommand = new(
            param => actionCommandExecuted = true,
            param => param is int value && value > 0
        );

        RelayCommand relayCommand = new(
            param => relayCommandExecuted = true,
            param => param is int value && value > 0
        );

        // Act & Assert - Both should behave the same
        actionCommand.CanExecute(null).Should().Be(relayCommand.CanExecute(null));
        actionCommand.CanExecute(0).Should().Be(relayCommand.CanExecute(0));
        actionCommand.CanExecute(5).Should().Be(relayCommand.CanExecute(5));

        actionCommand.Execute(5);
        relayCommand.Execute(5);

        actionCommandExecuted.Should().Be(relayCommandExecuted);
    }

    #endregion
}
