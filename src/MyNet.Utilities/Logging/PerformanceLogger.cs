// -----------------------------------------------------------------------
// <copyright file="PerformanceLogger.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace MyNet.Utilities.Logging;

/// <summary>
/// A utility class for logging the performance of code blocks. It uses a stack to manage nested performance loggers and allows for registering custom time measurements with associated log levels. When disposed, it logs the elapsed time and any registered times in a structured format.
/// </summary>
public sealed class PerformanceLogger : IDisposable
{
    private static readonly AsyncLocal<Stack<PerformanceLogger>?> CurrentStack = new();

    private static Stack<PerformanceLogger> Stack => CurrentStack.Value ??= new();

    private readonly ILogger _logger;
    private readonly string _title;
    private readonly Stopwatch _stopwatch;
    private readonly IDisposable? _scope;
    private readonly ConcurrentDictionary<string, RegisteredTime> _registeredTimes = new();
    private readonly Func<TimeSpan, LogLevel> _provideLogLevel;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceLogger"/> class with the specified logger, title, log level, and options for showing start and end messages. The log level can be provided as a fixed value or determined dynamically based on the elapsed time using the provided function.
    /// </summary>
    /// <param name="logger">The logger to use for logging performance messages.</param>
    /// <param name="title">The title of the performance logger, used in log messages.</param>
    /// <param name="level">The log level to use for logging performance messages.</param>
    /// <param name="showStartMessage">Whether to show a start message when the performance logger is created.</param>
    /// <param name="showEndMessage">Whether to show an end message when the performance logger is disposed.</param>
    public PerformanceLogger(ILogger logger, string title, LogLevel level = LogLevel.Trace, bool showStartMessage = false, bool showEndMessage = false)
        : this(logger, title, _ => level, showStartMessage, showEndMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceLogger"/> class with the specified logger, title, function to determine log level based on elapsed time, and options for showing start and end messages. The log level is determined dynamically based on the elapsed time using the provided function.
    /// </summary>
    /// <param name="logger">The logger to use for logging performance messages.</param>
    /// <param name="title">The title of the performance logger, used in log messages.</param>
    /// <param name="provideLogLevel">A function that determines the log level based on the elapsed time.</param>
    /// <param name="showStartMessage">Whether to show a start message when the performance logger is created.</param>
    /// <param name="showEndMessage">Whether to show an end message when the performance logger is disposed.</param>
    public PerformanceLogger(ILogger logger, string title, Func<TimeSpan, LogLevel> provideLogLevel, bool showStartMessage = false, bool showEndMessage = false)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _title = title ?? throw new ArgumentNullException(nameof(title));
        _provideLogLevel = provideLogLevel ?? throw new ArgumentNullException(nameof(provideLogLevel));

        Stack.Push(this);

        _scope = _logger.BeginScope(_title);

        if (showStartMessage)
        {
            _logger.LogStart(_title);
        }

        _stopwatch = Stopwatch.StartNew();

        ShowEndMessage = showEndMessage;
    }

    /// <summary>
    /// Gets the current performance logger from the stack. If there are no performance loggers in the stack, it returns null. This allows for accessing the current performance logger in nested scenarios, where multiple performance loggers may be active at the same time.
    /// </summary>
    public static PerformanceLogger? Current => Stack.Count == 0 ? null : Stack.Peek();

    /// <summary>
    /// Gets a value indicating whether to show an end message when the performance logger is disposed. This property is set during the initialization of the performance logger and determines whether an end message should be logged when the logger is disposed, allowing for flexible logging behavior based on the needs of the application.
    /// </summary>
    public bool ShowEndMessage { get; }

    /// <summary>
    /// Gets the elapsed time since the performance logger was created. This property returns the total time that has passed since the performance logger was initialized, allowing for measuring the duration of code execution and logging it when the logger is disposed.
    /// </summary>
    public TimeSpan Elapsed => _stopwatch.Elapsed;

    /// <summary>
    /// Formats a TimeSpan into a human-readable string with appropriate units (days, hours, minutes, seconds, milliseconds, or microseconds) based on the magnitude of the time span. This method checks the total time in various units and formats it accordingly to provide a clear and concise representation of the elapsed time in log messages.
    /// </summary>
    /// <param name="time">The TimeSpan to format.</param>
    /// <returns>A formatted string representing the TimeSpan.</returns>
    private static string FormatTimeSpan(TimeSpan time) =>
        time.TotalDays >= 1
            ? $"{time.TotalDays:F2}d"
            : time.TotalHours >= 1
                ? $"{time.TotalHours:F2}h"
                : time.TotalMinutes >= 1
                    ? $"{time.TotalMinutes:F2}m"
                    : time.TotalSeconds >= 1
                        ? $"{time.TotalSeconds:F2}s"
                        : time.TotalMilliseconds >= 1
                            ? $"{time.TotalMilliseconds:F2}ms"
                            : $"{time.TotalMicroseconds:F2}μs";

    /// <summary>
    /// Disposes the performance logger, stopping the stopwatch and logging the elapsed time along with any registered times. It also manages the stack of performance loggers to ensure that nested loggers are handled correctly. If there are registered times, it logs them in a structured format, showing the hierarchy of logged times and their associated log levels. Finally, it disposes of the logging scope if it was created.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _stopwatch.Stop();

        var elapsed = _stopwatch.Elapsed;

        if (ShowEndMessage)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogEnd(_title, FormatTimeSpan(elapsed));
            }
        }

        PerformanceLogger? parent = null;

        if (Stack.Count > 0 && ReferenceEquals(Stack.Peek(), this))
        {
            Stack.Pop();
        }

        if (Stack.Count > 0)
        {
            parent = Stack.Peek();
        }

        if (parent is null)
        {
            TraceTimes();
        }
        else
        {
            var level = _provideLogLevel(elapsed);

            parent.AddTime(_title, elapsed, level);
            parent.MergeTimes(this);
        }

        _scope?.Dispose();
    }

    /// <summary>
    /// Adds a registered time to the performance logger with the specified key, time, and log level. This method allows for adding custom time measurements to the performance logger, which can be logged along with the total elapsed time when the logger is disposed. The registered times are stored in a concurrent dictionary, allowing for thread-safe updates and retrievals of registered times in nested scenarios.
    /// </summary>
    /// <param name="key">The key associated with the registered time.</param>
    /// <param name="time">The time span to register.</param>
    /// <param name="level">The log level for the registered time.</param>
    public void AddTime(string key, TimeSpan time, LogLevel level)
        => _registeredTimes.AddOrUpdate(key, _ => new(level, time), (_, existing) => new(level, existing.Time + time));

    /// <summary>
    /// Merges the registered times from a child performance logger into the current performance logger. This method is used when disposing of a child performance logger to ensure that any registered times from the child are added to the parent logger, allowing for accurate logging of nested performance measurements. The method iterates through the registered times in the child logger and adds them to the current logger using the AddTime method, preserving the hierarchy of logged times and their associated log levels.
    /// </summary>
    /// <param name="child">The child performance logger whose registered times are to be merged.</param>
    private void MergeTimes(PerformanceLogger child)
    {
        foreach (var item in child._registeredTimes)
        {
            AddTime(
                $"{child._title} -> {item.Key}",
                item.Value.Time,
                item.Value.Level);
        }
    }

    /// <summary>
    /// Logs the total elapsed time and any registered times in a structured format. If there are no registered times, it simply logs the total elapsed time. If there are registered times, it logs them in descending order of their time values, showing the key and elapsed time for each registered time, followed by the total elapsed time. The log messages are formatted with separators to clearly indicate the hierarchy of logged times and their associated log levels. This method is called when disposing of a performance logger that has no parent logger, ensuring that all logged times are outputted to the logger in a clear and organized manner.
    /// </summary>
    [SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "The number of messages is dynamic and does not justify the creation of static delegates.")]
    [SuppressMessage("ReSharper", "DuplicateItemInLoggerTemplate", Justification = "The log messages are dynamic and do not justify the creation of static delegates.")]
    private void TraceTimes()
    {
        var totalLevel = _provideLogLevel(Elapsed);

        if (_registeredTimes.IsEmpty)
        {
            if (_logger.IsEnabled(totalLevel))
                _logger.Log(totalLevel, "{Title} completed in {Elapsed}", _title, FormatTimeSpan(Elapsed));

            return;
        }

        const string separator = "********************";

        if (_logger.IsEnabled(totalLevel))
            _logger.Log(totalLevel, "{} {Title} {Separator}", separator, _title, separator);

        foreach (var item in _registeredTimes.OrderByDescending(x => x.Value.Time))
        {
            if (_logger.IsEnabled(item.Value.Level))
                _logger.Log(item.Value.Level, "{Key} - {Elapsed}", item.Key, FormatTimeSpan(item.Value.Time));
        }

        if (_logger.IsEnabled(totalLevel))
            _logger.Log(totalLevel, "Total Time : {Elapsed}", FormatTimeSpan(Elapsed));

        if (_logger.IsEnabled(totalLevel))
            _logger.Log(totalLevel, "{Separator} {Title} {Separator}", separator, _title, separator);
    }

    /// <summary>
    /// A record representing a registered time measurement with an associated log level. This record is used to store the log level and time span for each registered time in the performance logger, allowing for structured logging of custom time measurements along with the total elapsed time when the logger is disposed.
    /// </summary>
    /// <param name="Level">The log level associated with the registered time.</param>
    /// <param name="Time">The time span of the registered time.</param>
    private sealed record RegisteredTime(LogLevel Level, TimeSpan Time);
}
