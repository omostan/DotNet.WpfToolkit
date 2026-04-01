#region copyright

/*****************************************************************************************
*                                     ______________________________________________     *
*                              o O   |                                              |    *
*                     (((((  o      <               DotNet WPF Tool Kit             |    *
*                    ( o o )         |______________________________________________|    *
* ------------oOOO-----(_)-----OOOo----------------------------------------------------- *
*             Project: DotNetTools.Wpfkit.Tests                                          *
*            Filename: ActionCommandTests.cs                                             *
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
/// Comprehensive unit tests for the ActionCommand class.
/// Tests cover null handling, exception handling, predicates, and WPF integration.
/// </summary>
public class ActionCommandTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithValidAction_ShouldCreateCommand()
    {
        // Arrange & Act
        ActionCommand command = new(param => { });

        // Assert
        command.Should().NotBeNull();
        command.Should().BeAssignableTo<ICommand>();
    }

    [Fact]
    public void Constructor_WithNullAction_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () =>
        {
            ActionCommand actionCommand = new(null!);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("action");
    }

    [Fact]
    public void Constructor_WithNullPredicate_ShouldCreateCommand()
    {
        // Arrange & Act
        ActionCommand command = new(param => { }, null);

        // Assert
        command.Should().NotBeNull();
    }

    #endregion

    #region CanExecute Tests - No Predicate

    [Fact]
    public void CanExecute_WithNoPredicate_ShouldReturnTrue()
    {
        // Arrange
        ActionCommand command = new(param => { });

        // Act
        bool result = command.CanExecute(null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WithNoPredicateAndNullParameter_ShouldReturnTrue()
    {
        // Arrange
        ActionCommand command = new(param => { });

        // Act
        bool result = command.CanExecute(null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WithNoPredicateAndNonNullParameter_ShouldReturnTrue()
    {
        // Arrange
        ActionCommand command = new(param => { });

        // Act
        bool result = command.CanExecute("test");

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region CanExecute Tests - With Predicate

    [Fact]
    public void CanExecute_WithPredicateReturningTrue_ShouldReturnTrue()
    {
        // Arrange
        ActionCommand command = new(
            param => { },
            param => true
        );

        // Act
        bool result = command.CanExecute(null);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WithPredicateReturningFalse_ShouldReturnFalse()
    {
        // Arrange
        ActionCommand command = new(
            param => { },
            param => false
        );

        // Act
        bool result = command.CanExecute(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanExecute_WithPredicateCheckingNull_AndNullParameter_ShouldWork()
    {
        // Arrange
        ActionCommand command = new(
            param => { },
            param => param != null
        );

        // Act
        bool result = command.CanExecute(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanExecute_WithPredicateCheckingNull_AndNonNullParameter_ShouldWork()
    {
        // Arrange
        ActionCommand command = new(
            param => { },
            param => param != null
        );

        // Act
        bool result = command.CanExecute("test");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WithPredicateAccessingParameter_AndNullParameter_ShouldReturnFalse()
    {
        // Arrange - This simulates the bug scenario
        ActionCommand command = new(
            param => { },
            param => param!.ToString()!.Length > 0  // Will throw NullReferenceException if not handled
        );

        // Act
        bool result = command.CanExecute(null);

        // Assert
        result.Should().BeFalse(); // Should return false instead of crashing
    }

    [Fact]
    public void CanExecute_WithTypeCheckPredicate_AndCorrectType_ShouldReturnTrue()
    {
        // Arrange
        ActionCommand command = new(
            param => { },
            param => param is string
        );

        // Act
        bool result = command.CanExecute("test");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WithTypeCheckPredicate_AndIncorrectType_ShouldReturnFalse()
    {
        // Arrange
        ActionCommand command = new(
            param => { },
            param => param is string
        );

        // Act
        bool result = command.CanExecute(123);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanExecute_WithComplexPredicate_ShouldEvaluateCorrectly()
    {
        // Arrange
        ActionCommand command = new(
            param => { },
            param => param is string str && !string.IsNullOrEmpty(str) && str.Length > 3
        );

        // Act & Assert
        command.CanExecute(null).Should().BeFalse();
        command.CanExecute("").Should().BeFalse();
        command.CanExecute("ab").Should().BeFalse();
        command.CanExecute("test").Should().BeTrue();
        command.CanExecute("hello").Should().BeTrue();
    }

    #endregion

    #region CanExecute Tests - Exception Handling

    [Fact]
    public void CanExecute_WhenPredicateThrowsException_ShouldReturnFalse()
    {
        // Arrange
        ActionCommand command = new(
            param => { },
            param => throw new InvalidOperationException("Test exception")
        );

        // Act
        bool result = command.CanExecute(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanExecute_WhenPredicateThrowsNullReferenceException_ShouldReturnFalse()
    {
        // Arrange
        string? nullString = null;
        ActionCommand command = new(
            param => { },
            param => nullString!.Length > 0  // Will throw NullReferenceException
        );

        // Act
        bool result = command.CanExecute(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanExecute_WhenPredicateThrowsInvalidCastException_ShouldReturnFalse()
    {
        // Arrange
        ActionCommand command = new(
            param => { },
            param => ((string)param!).Length > 0  // Will throw InvalidCastException with int
        );

        // Act
        bool result = command.CanExecute(123);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Execute Tests

    [Fact]
    public void Execute_ShouldInvokeAction()
    {
        // Arrange
        bool actionInvoked = false;
        ActionCommand command = new(param => actionInvoked = true);

        // Act
        command.Execute(null);

        // Assert
        actionInvoked.Should().BeTrue();
    }

    [Fact]
    public void Execute_WithParameter_ShouldPassParameterToAction()
    {
        // Arrange
        object? capturedParameter = null;
        ActionCommand command = new(param => capturedParameter = param);

        // Act
        command.Execute("test");

        // Assert
        capturedParameter.Should().Be("test");
    }

    [Fact]
    public void Execute_WithNullParameter_ShouldPassNullToAction()
    {
        // Arrange
        object? capturedParameter = "initial";
        ActionCommand command = new(param => capturedParameter = param);

        // Act
        command.Execute(null);

        // Assert
        capturedParameter.Should().BeNull();
    }

    [Fact]
    public void Execute_MultipleTimes_ShouldInvokeActionEachTime()
    {
        // Arrange
        int invokeCount = 0;
        ActionCommand command = new(param => invokeCount++);

        // Act
        command.Execute(null);
        command.Execute(null);
        command.Execute(null);

        // Assert
        invokeCount.Should().Be(3);
    }

    [Fact]
    public void Execute_WithDifferentParameters_ShouldPassEachCorrectly()
    {
        // Arrange
        List<object?> parameters = new();
        ActionCommand command = new(param => parameters.Add(param));

        // Act
        command.Execute("first");
        command.Execute(123);
        command.Execute(null);

        // Assert
        parameters.Should().HaveCount(3);
        parameters[0].Should().Be("first");
        parameters[1].Should().Be(123);
        parameters[2].Should().BeNull();
    }

    #endregion

    #region CanExecuteChanged Event Tests

    [Fact]
    public void CanExecuteChanged_ShouldNotBeNull()
    {
        // Arrange
        ActionCommand command = new(param => { });
        EventHandler? handler = (sender, args) => { };

        // Act
        command.CanExecuteChanged += handler;

        // Assert - No exception should be thrown
        command.CanExecuteChanged -= handler;
    }

    [Fact]
    public void CanExecuteChanged_Subscribe_ShouldNotThrow()
    {
        // Arrange
        ActionCommand command = new(param => { });

        // Act
        Action act = () => command.CanExecuteChanged += (sender, args) => { };

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void CanExecuteChanged_Unsubscribe_ShouldNotThrow()
    {
        // Arrange
        ActionCommand command = new(param => { });
        EventHandler handler = (sender, args) => { };
        command.CanExecuteChanged += handler;

        // Act
        Action act = () => command.CanExecuteChanged -= handler;

        // Assert
        act.Should().NotThrow();
    }

    #endregion

    #region DataGrid Scenario Tests (Critical Bug Fix)

    [Fact]
    public void DataGrid_Scenario_NullParameterWithPredicate_ShouldNotCrash()
    {
        // This test simulates the exact scenario that caused the production crash
        // Arrange
        ActionCommand command = new(
            param => { /* DataGrid action */ },
            param => param != null && param is string str && !string.IsNullOrEmpty(str)
        );

        // Act & Assert - Should not throw exception
        Action act = () =>
        {
            bool canExecute = command.CanExecute(null); // DataGrid often passes null
            canExecute.Should().BeFalse();
        };

        act.Should().NotThrow<NullReferenceException>();
    }

    [Fact]
    public void DataGrid_Scenario_MultipleNullChecks_ShouldWork()
    {
        // Arrange
        List<string> executionLog = new();
        ActionCommand command = new(
            param =>
            {
                if (param != null)
                    executionLog.Add(param.ToString()!);
            },
            param => param is int value && value > 0
        );

        // Act
        command.CanExecute(null).Should().BeFalse();
        command.CanExecute(0).Should().BeFalse();
        command.CanExecute(-1).Should().BeFalse();
        command.CanExecute(5).Should().BeTrue();

        command.Execute(5);

        // Assert
        executionLog.Should().ContainSingle()
            .Which.Should().Be("5");
    }

    #endregion

    #region Complex Real-World Scenarios

    [Fact]
    public void RealWorld_SaveCommand_WithValidation_ShouldWork()
    {
        // Arrange
        bool saveExecuted = false;
        ActionCommand command = new(
            param =>
            {
                string? data = param as string;
                // Save logic
                saveExecuted = true;
            },
            param =>
            {
                // Validation logic
                if (param == null) return false;
                if (param is not string str) return false;
                return !string.IsNullOrWhiteSpace(str) && str.Length >= 3;
            }
        );

        // Act & Assert
        command.CanExecute(null).Should().BeFalse();
        command.CanExecute("").Should().BeFalse();
        command.CanExecute("ab").Should().BeFalse();
        command.CanExecute("valid").Should().BeTrue();

        command.Execute("valid");
        saveExecuted.Should().BeTrue();
    }

    [Fact]
    public void RealWorld_DeleteCommand_WithEntityCheck_ShouldWork()
    {
        // Arrange
        List<int> deletedIds = new();
        ActionCommand command = new(
            param =>
            {
                if (param is int id)
                    deletedIds.Add(id);
            },
            param => param is int id && id > 0
        );

        // Act & Assert
        command.CanExecute(null).Should().BeFalse();
        command.CanExecute("not an int").Should().BeFalse();
        command.CanExecute(0).Should().BeFalse();
        command.CanExecute(-1).Should().BeFalse();
        command.CanExecute(42).Should().BeTrue();

        command.Execute(42);
        deletedIds.Should().ContainSingle().Which.Should().Be(42);
    }

    #endregion

    #region Thread Safety Tests

    [Fact]
    public async Task Execute_FromMultipleThreads_ShouldBeThreadSafe()
    {
        // Arrange
        int counter = 0;
        object lockObj = new();
        ActionCommand command = new(param =>
        {
            lock (lockObj)
            {
                counter++;
            }
        });

        // Act
        Task[] tasks = Enumerable.Range(0, 100)
            .Select(_ => Task.Run(() => command.Execute(null)))
            .ToArray();

        await Task.WhenAll(tasks);

        // Assert
        counter.Should().Be(100);
    }

    [Fact]
    public async Task CanExecute_FromMultipleThreads_ShouldNotThrow()
    {
        // Arrange
        ActionCommand command = new(
            param => { },
            param => param is int value && value > 0
        );

        // Act
        Task<bool>[] tasks = Enumerable.Range(0, 100)
            .Select(i => Task.Run(() => command.CanExecute(i)))
            .ToArray();

        // Assert
        Func<Task> act = async () => await Task.WhenAll(tasks);
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region Performance Tests

    [Fact]
    public void CanExecute_CalledManyTimes_ShouldPerformWell()
    {
        // Arrange
        ActionCommand command = new(
            param => { },
            param => param is string { Length: > 5 });

        Stopwatch stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 10000; i++)
        {
            command.CanExecute("test string");
        }

        stopwatch.Stop();

        // Assert - Should complete in reasonable time (< 100ms for 10k calls)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
    }

    #endregion
}
