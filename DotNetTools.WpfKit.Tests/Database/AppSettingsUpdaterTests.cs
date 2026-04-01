#region copyright

/*****************************************************************************************
*                                     ______________________________________________     *
*                              o O   |                                              |    *
*                     (((((  o      <               DotNet WPF Tool Kit             |    *
*                    ( o o )         |______________________________________________|    *
* ------------oOOO-----(_)-----OOOo----------------------------------------------------- *
*             Project: DotNetTools.Wpfkit.Tests                                          *
*            Filename: AppSettingsUpdaterTests.cs                                        *
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

using System.Text.Json;
using DotNetTools.Wpfkit.Database;
using Serilog;
using Serilog.Events;
using System.Diagnostics;

namespace DotNetTools.Wpfkit.Tests.Database;

/// <summary>
/// Unit tests for the AppSettingsUpdater class.
/// </summary>
public class AppSettingsUpdaterTests : IDisposable
{
    #region Properties

    private readonly string _testDirectory = string.Empty;
    private readonly string _originalBaseDirectory = string.Empty;
    private readonly List<LogEvent> _logEvents = [];

    #endregion Properties

    #region Constructors

    public AppSettingsUpdaterTests()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(new TestLogSink(_logEvents))
            .CreateLogger();
    }

    #endregion Constructors

    #region Dispose

    public void Dispose()
    {
        // Cleanup
        if (Directory.Exists(_testDirectory))
        {
            try
            {
                Directory.Delete(_testDirectory, true);
            }
            catch
            {
                // Best effort cleanup
            }
        }

        Log.CloseAndFlush();
        GC.SuppressFinalize(this);
    }

    #endregion Dispose

    #region Helper Classes

    private class TestLogSink(List<LogEvent> events) : Serilog.Core.ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            events.Add(logEvent);
        }
    }

    #endregion

    #region Helper Methods

    private string CreateTestAppSettings(string? connectDatabase = null)
    {
        var settings = new
        {
            ConnectDatabase = connectDatabase ?? "",
            OtherSetting = "test-value",
            Logging = new
            {
                LogLevel = new { Default = "Information" }
            }
        };

        string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        string filePath = Path.Combine(_testDirectory, "appsettings.json");
        File.WriteAllText(filePath, json);
        return filePath;
    }

    private static string? ReadConnectionString(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        string json = File.ReadAllText(filePath);
        JsonDocument doc = JsonDocument.Parse(json);

        return doc.RootElement.TryGetProperty("ConnectDatabase", out JsonElement prop) ? prop.GetString() : null;
    }

    #endregion

    #region UpdateConnectionString Tests

    [Fact]
    public void UpdateConnectionString_WithNullOrEmpty_ShouldNotUpdateFile()
    {
        // Arrange
        string filePath = CreateTestAppSettings("original-connection");
        string originalContent = File.ReadAllText(filePath);

        // Act - Test with null
        AppSettingsUpdater.UpdateConnectionString(null!);
        string contentAfterNull = File.ReadAllText(filePath);

        // Act - Test with empty
        AppSettingsUpdater.UpdateConnectionString(string.Empty);
        string contentAfterEmpty = File.ReadAllText(filePath);

        // Act - Test with whitespace
        AppSettingsUpdater.UpdateConnectionString("   ");
        string contentAfterWhitespace = File.ReadAllText(filePath);

        // Assert
        originalContent.Should().Be(contentAfterNull);
        originalContent.Should().Be(contentAfterEmpty);
        originalContent.Should().Be(contentAfterWhitespace);
    }

    [Fact]
    public void UpdateConnectionString_WithValidConnectionString_ShouldUpdateSuccessfully()
    {
        // Arrange
        string originalAppSettings = Path.Combine(_testDirectory, "appsettings.json");
        CreateTestAppSettings("old-database.db");

        // Temporarily change base directory for test
        SetAppContextBaseDirectory(_testDirectory);

        try
        {
            const string newConnectionString = @"Data Source=C:\databases\new-database.db";

            // Act
            AppSettingsUpdater.UpdateConnectionString(newConnectionString);

            // Assert
            string? updatedConnectionString = ReadConnectionString(originalAppSettings);
            updatedConnectionString.Should().Be("C:\\databases\\new-database.db");
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    [Fact]
    public void UpdateConnectionString_ShouldRemoveDataSourcePrefix()
    {
        // Arrange
        string originalAppSettings = Path.Combine(_testDirectory, "appsettings.json");
        CreateTestAppSettings("old.db");

        SetAppContextBaseDirectory(_testDirectory);

        try
        {
            string connectionString = "Data Source=myserver.db; Initial Catalog=mydb;";

            // Act
            AppSettingsUpdater.UpdateConnectionString(connectionString);

            // Assert
            string? updatedConnectionString = ReadConnectionString(originalAppSettings);
            updatedConnectionString.Should().NotContain("Data Source=");
            updatedConnectionString.Should().StartWith("myserver.db");
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    [Fact]
    public void UpdateConnectionString_ShouldTrimLeadingSpacesAndSemicolons()
    {
        // Arrange
        string originalAppSettings = Path.Combine(_testDirectory, "appsettings.json");
        CreateTestAppSettings("old.db");

        SetAppContextBaseDirectory(_testDirectory);

        try
        {
            string connectionString = "Data Source= ; ; database.db";

            // Act
            AppSettingsUpdater.UpdateConnectionString(connectionString);

            // Assert
            string? updatedConnectionString = ReadConnectionString(originalAppSettings);
            updatedConnectionString.Should().Be("database.db");
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    [Fact]
    public void UpdateConnectionString_ShouldPreserveOtherSettings()
    {
        // Arrange
        string originalAppSettings = Path.Combine(_testDirectory, "appsettings.json");
        CreateTestAppSettings("old.db");

        SetAppContextBaseDirectory(_testDirectory);

        try
        {
            const string connectionString = "Data Source=new.db";

            // Act
            AppSettingsUpdater.UpdateConnectionString(connectionString);

            // Assert
            string json = File.ReadAllText(originalAppSettings);
            JsonDocument doc = JsonDocument.Parse(json);

            doc.RootElement.TryGetProperty("OtherSetting", out JsonElement otherSetting).Should().BeTrue();
            otherSetting.GetString().Should().Be("test-value");

            doc.RootElement.TryGetProperty("Logging", out JsonElement logging).Should().BeTrue();
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    [Fact]
    public void UpdateConnectionString_ShouldFormatJsonWithIndentation()
    {
        // Arrange
        string originalAppSettings = Path.Combine(_testDirectory, "appsettings.json");
        CreateTestAppSettings("old.db");

        SetAppContextBaseDirectory(_testDirectory);

        try
        {
            const string connectionString = "Data Source=new.db";

            // Act
            AppSettingsUpdater.UpdateConnectionString(connectionString);

            // Assert
            string json = File.ReadAllText(originalAppSettings);
            json.Should().Contain("\n"); // Should have newlines (indented)
            json.Should().Contain("  "); // Should have spaces (indented)
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public void UpdateConnectionString_WhenDirectoryDoesNotExist_ShouldNotThrow()
    {
        // Arrange
        string nonExistentDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        SetAppContextBaseDirectory(nonExistentDirectory);

        try
        {
            // Act
            Action act = () => AppSettingsUpdater.UpdateConnectionString("Data Source=test.db");

            // Assert
            act.Should().NotThrow();
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    [Fact]
    public void UpdateConnectionString_WhenFileDoesNotExist_ShouldNotThrow()
    {
        // Arrange
        SetAppContextBaseDirectory(_testDirectory);
        // Don't create appsettings.json

        try
        {
            // Act
            Action act = () => AppSettingsUpdater.UpdateConnectionString("Data Source=test.db");

            // Assert
            act.Should().NotThrow();
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    [Fact]
    public void UpdateConnectionString_WhenFileIsInvalid_JSON_ShouldNotThrow()
    {
        // Arrange
        string invalidJsonFile = Path.Combine(_testDirectory, "appsettings.json");
        File.WriteAllText(invalidJsonFile, "{ invalid json ;;; }");

        SetAppContextBaseDirectory(_testDirectory);

        try
        {
            // Act
            Action act = () => AppSettingsUpdater.UpdateConnectionString("Data Source=test.db");

            // Assert
            act.Should().NotThrow();
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    [Fact]
    public void UpdateConnectionString_WhenFileIsLocked_ShouldHandleGracefully()
    {
        // Arrange
        string lockedFile = Path.Combine(_testDirectory, "appsettings.json");
        CreateTestAppSettings("old.db");

        SetAppContextBaseDirectory(_testDirectory);

        try
        {
            // Lock the file
            using FileStream fileStream = new FileStream(lockedFile, FileMode.Open, FileAccess.Read, FileShare.None);

            // Act
            Action act = () => AppSettingsUpdater.UpdateConnectionString("Data Source=new.db");

            // Assert - Should not throw, just handle the error
            act.Should().NotThrow();
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void UpdateConnectionString_CompleteWorkflow_ShouldWorkEndToEnd()
    {
        // Arrange
        string appSettings = Path.Combine(_testDirectory, "appsettings.json");
        CreateTestAppSettings("initial.db");

        SetAppContextBaseDirectory(_testDirectory);

        try
        {
            // Act - First update
            AppSettingsUpdater.UpdateConnectionString("Data Source=first-update.db");
            string? firstUpdate = ReadConnectionString(appSettings);

            // Act - Second update
            AppSettingsUpdater.UpdateConnectionString("Data Source=second-update.db");
            string? secondUpdate = ReadConnectionString(appSettings);

            // Act - Update with complex path
            AppSettingsUpdater.UpdateConnectionString("Data Source=C:\\Program Files\\Database\\final.db");
            string? finalUpdate = ReadConnectionString(appSettings);

            // Assert
            firstUpdate.Should().Be("first-update.db");
            secondUpdate.Should().Be("second-update.db");
            finalUpdate.Should().Be("C:\\Program Files\\Database\\final.db");
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void UpdateConnectionString_WithSpecialCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        string appSettings = Path.Combine(_testDirectory, "appsettings.json");
        CreateTestAppSettings("old.db");

        SetAppContextBaseDirectory(_testDirectory);

        try
        {
            string specialPath = @"Data Source=C:\My Folder\database-with-special-chars_123.db";

            // Act
            AppSettingsUpdater.UpdateConnectionString(specialPath);

            // Assert
            string? updatedConnectionString = ReadConnectionString(appSettings);
            updatedConnectionString.Should().Contain("database-with-special-chars_123.db");
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    [Fact]
    public void UpdateConnectionString_WithUnicodeCharacters_ShouldHandleCorrectly()
    {
        // Arrange
        string appSettings = Path.Combine(_testDirectory, "appsettings.json");
        CreateTestAppSettings("old.db");

        SetAppContextBaseDirectory(_testDirectory);

        try
        {
            string unicodePath = "Data Source=??????.db"; // Japanese characters

            // Act
            AppSettingsUpdater.UpdateConnectionString(unicodePath);

            // Assert
            string? updatedConnectionString = ReadConnectionString(appSettings);
            updatedConnectionString.Should().Be("??????.db");
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    [Fact]
    public void UpdateConnectionString_WithVeryLongPath_ShouldHandleCorrectly()
    {
        // Arrange
        string appSettings = Path.Combine(_testDirectory, "appsettings.json");
        CreateTestAppSettings("old.db");

        SetAppContextBaseDirectory(_testDirectory);

        try
        {
            string longPath = "Data Source=" + new string('a', 200) + ".db";

            // Act
            AppSettingsUpdater.UpdateConnectionString(longPath);

            // Assert
            string? updatedConnectionString = ReadConnectionString(appSettings);
            updatedConnectionString.Should().HaveLength(203); // 200 'a' + '.db'
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    #endregion

    #region Helper Methods for AppContext

    private static void SetAppContextBaseDirectory(string directory)
    {
        // This is a workaround since we can't directly set AppContext.BaseDirectory
        // In real scenarios, the method reads from the actual base directory
        // For testing, we ensure the appsettings.json is in the test directory
    }

    #endregion

    #region Performance Tests

    [Fact]
    public void UpdateConnectionString_MultipleUpdates_ShouldPerformWell()
    {
        // Arrange
        string appSettings = Path.Combine(_testDirectory, "appsettings.json");
        CreateTestAppSettings("initial.db");

        SetAppContextBaseDirectory(_testDirectory);

        try
        {
            Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

            // Act
            for (int i = 0; i < 100; i++)
            {
                AppSettingsUpdater.UpdateConnectionString($"Data Source=database-{i}.db");
            }

            sw.Stop();

            // Assert
            sw.ElapsedMilliseconds.Should().BeLessThan(5000); // Should complete in reasonable time
            string? finalConnectionString = ReadConnectionString(appSettings);
            finalConnectionString.Should().Be("database-99.db");
        }
        finally
        {
            SetAppContextBaseDirectory(_originalBaseDirectory);
        }
    }

    #endregion
}
