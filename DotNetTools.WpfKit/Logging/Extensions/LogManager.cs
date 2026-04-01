#region copyright

/*****************************************************************************************
*                                     ______________________________________________     *
*                              o O   |                                              |    *
*                     (((((  o      <               DotNet WPF Tool Kit             |    *
*                    ( o o )         |______________________________________________|    *
* ------------oOOO-----(_)-----OOOo----------------------------------------------------- *
*             Project: DotNetTools.Wpfkit                                                *
*            Filename: LogManager.cs                                                     *
*              Author: Stanley Omoregie                                                  *
*        Created Date: 20.11.2025                                                        *
*       Modified Date: 27.01.2026                                                        *
*          Created By: Stanley Omoregie                                                  *
*    Last Modified By: Stanley Omoregie                                                  *
*           CopyRight: copyright © 2025 Omotech Digital Solutions                        *
*                  .oooO  Oooo.                                                          *
*                  (   )  (   )                                                          *
* ------------------\ (----) /---------------------------------------------------------- *
*                    \_)  (_/                                                            *
*****************************************************************************************/

#endregion copyright

using Serilog;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DotNetTools.Wpfkit.Logging.Extensions;

/// <summary>
/// Provides methods for obtaining and enriching logger instances with contextual information
/// such as class, line number, file path, and member name. Intended for use with Serilog.
/// </summary>
public static class LogManager
{
    /// <summary>
    /// Gets a logger instance for the calling class. Ensure this is set to a static field on the class.
    /// </summary>
    /// <ref>https://github.com/serilog/serilog/issues/886#issuecomment-265063611</ref>
    /// <remarks>
    /// Uses <see cref="MethodImplOptions.NoInlining"/> to ensure the correct calling class is captured.
    /// </remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static ILogger GetCurrentClassLogger()
    {
        return Log.ForContext(new StackFrame(skipFrames: 1, needFileInfo: false).GetMethod()?.DeclaringType!);
    }

    /// <summary>
    /// Extension methods for enriching <see cref="ILogger"/> with additional context information.
    /// </summary>
    /// <param name="logger">The logger instance to enrich.</param>
    extension(ILogger logger)
    {
        /// <summary>
        /// Enriches the logger with the source line number where the log call was made.
        /// </summary>
        /// <param name="sourceLineNumber">The line number in the source file at the call site. Automatically provided by the compiler.</param>
        /// <returns>An enriched <see cref="ILogger"/> with the line number context.</returns>
        [Obsolete("This method is deprecated. Use WithLine instead.", false)]
        public ILogger Me([CallerLineNumber] int sourceLineNumber = default)
        {
            return logger.ForContext(propertyName: "LineNumber", sourceLineNumber);
        }

        /// <summary>
        /// Enriches the logger with the source line number where the log call was made.
        /// </summary>
        /// <param name="sourceLineNumber">The line number in the source file at the call site. Automatically provided by the compiler.</param>
        /// <returns>An enriched <see cref="ILogger"/> with the line number context.</returns>
        [Obsolete("This method is deprecated. Use WriteLine instead.", false)]
        public ILogger WithLine([CallerLineNumber] int sourceLineNumber = default)
        {
            return logger.ForContext(propertyName: "LineNumber", sourceLineNumber);
        }

        /// <summary>
        /// Enriches the logger with the source line number where the log call was made.
        /// </summary>
        /// <param name="sourceLineNumber">The line number in the source file at the call site. Automatically provided by the compiler.</param>
        /// <returns>An enriched <see cref="ILogger"/> with the line number context.</returns>
        public ILogger WriteLine([CallerLineNumber] int sourceLineNumber = default)
        {
            return logger.ForContext(propertyName: "LineNumber", sourceLineNumber);
        }

        /// <summary>
        /// Enriches the logger with the source file path where the log call was made.
        /// </summary>
        /// <param name="sourceFilePath">
        /// The full path of the source file at the call site. Automatically provided by the compiler.
        /// </param>
        public ILogger WithPath([CallerFilePath] string? sourceFilePath = default)
        {
            return logger.ForContext(propertyName: "FilePath", sourceFilePath);
        }

        /// <summary>
        /// Enriches the logger with the member name where the log call was made.
        /// </summary>
        /// <param name="memberName">
        /// The name of the method or property at the call site. Automatically provided by the compiler.
        /// </param>
        public ILogger WithMember([CallerMemberName] string? memberName = default)
        {
            return logger.ForContext(propertyName: "MemberName", memberName);
        }
    }
}
