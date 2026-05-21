// -----------------------------------------------------------------------
// <copyright file="LoggerExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using MyNet.Utilities.Logging;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class LoggerExtensions
{
    extension(ILogger logger)
    {
        /// <summary>
        /// Measures the execution time of a code block and logs it at the specified log level. The log entry will include the provided title or the caller member name if no title is specified.
        /// </summary>
        /// <param name="level">The log level at which to log the execution time.</param>
        /// <param name="title">The title to include in the log entry. If null, the caller member name will be used.</param>
        /// <param name="memberName">The name of the calling member. This is automatically provided by the compiler.</param>
        /// <returns>An IDisposable that, when disposed, logs the elapsed time.</returns>
        public IDisposable MeasureTime(LogLevel level = LogLevel.Trace, string? title = null, [CallerMemberName] string memberName = "")
            => new PerformanceLogger(logger, title ?? memberName, level);

        /// <summary>
        /// Measures the execution time of a code block and logs it. The log entry will include the provided title.
        /// </summary>
        /// <param name="title">The title to include in the log entry.</param>
        /// <param name="level">The log level at which to log the execution time.</param>
        /// <returns>An IDisposable that, when disposed, logs the elapsed time.</returns>
        public IDisposable MeasureTime(string title, LogLevel level = LogLevel.Trace)
            => new PerformanceLogger(logger, title, level);

        /// <summary>
        /// Measures the execution time of a code block and logs it with a dynamic log level based on elapsed time.
        /// </summary>
        /// <param name="provideLogLevel">A function that determines the log level based on the elapsed time.</param>
        /// <param name="title">The title to include in the log entry.</param>
        /// <returns>An IDisposable that, when disposed, logs the elapsed time.</returns>
        public IDisposable MeasureTime(Func<TimeSpan, LogLevel> provideLogLevel, string title)
            => new PerformanceLogger(logger, title, provideLogLevel);

        /// <summary>
        /// Measures the execution time of a code block and logs it with custom settings.
        /// </summary>
        /// <param name="title">The title to include in the log entry.</param>
        /// <param name="showStartMessage">Whether to show a start message when the performance logger is created.</param>
        /// <param name="showEndMessage">Whether to show an end message when the performance logger is disposed.</param>
        /// <param name="level">The log level at which to log the execution time.</param>
        /// <returns>An IDisposable that, when disposed, logs the elapsed time.</returns>
        public IDisposable MeasureTime(string title, bool showStartMessage, bool showEndMessage, LogLevel level = LogLevel.Trace)
            => new PerformanceLogger(logger, title, level, showStartMessage, showEndMessage);

        /// <summary>
        /// Measures the execution time of a code block and logs it with a dynamic log level based on elapsed time and custom settings.
        /// </summary>
        /// <param name="title">The title to include in the log entry.</param>
        /// <param name="showStartMessage">Whether to show a start message when the performance logger is created.</param>
        /// <param name="showEndMessage">Whether to show an end message when the performance logger is disposed.</param>
        /// <param name="provideLogLevel">A function that determines the log level based on the elapsed time.</param>
        /// <returns>An IDisposable that, when disposed, logs the elapsed time.</returns>
        public IDisposable MeasureTime(string title, bool showStartMessage, bool showEndMessage, Func<TimeSpan, LogLevel> provideLogLevel)
            => new PerformanceLogger(logger, title, provideLogLevel, showStartMessage, showEndMessage);
    }
}
