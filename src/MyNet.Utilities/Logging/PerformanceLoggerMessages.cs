// -----------------------------------------------------------------------
// <copyright file="PerformanceLoggerMessages.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.Logging;

namespace MyNet.Utilities.Logging;

/// <summary>
/// Defines strongly-typed logging messages for performance logging. These messages are used by the <see cref="PerformanceLogger"/> to log the start, end, completion, and other relevant information about performance measurements in a consistent and structured way.
/// </summary>
internal static partial class PerformanceLoggerMessages
{
    private static readonly Func<ILogger, string, IDisposable?> Scope = LoggerMessage.DefineScope<string>("{PerformanceScope}");

    [LoggerMessage(EventId = 1000, Level = LogLevel.Trace, Message = "START - {Title}")]
    public static partial void LogStart(this ILogger logger, string title);

    [LoggerMessage(EventId = 1001, Level = LogLevel.Trace, Message = "END - {Title} : {Elapsed}")]
    public static partial void LogEnd(this ILogger logger, string title, string elapsed);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Information, Message = "{Title} completed in {Elapsed}")]
    public static partial void LogCompleted(this ILogger logger, string title, string elapsed);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Information, Message = "{Separator} {Title} {Separator}")]
    public static partial void LogHeader(this ILogger logger, string separator, string title);

    [LoggerMessage(EventId = 1004, Level = LogLevel.Information, Message = "{Key} - {Elapsed}")]
    public static partial void LogEntry(this ILogger logger, string key, string elapsed);

    [LoggerMessage(EventId = 1005, Level = LogLevel.Information, Message = "Total Time : {Elapsed}")]
    public static partial void LogTotal(this ILogger logger, string elapsed);

    public static IDisposable? BeginScope(this ILogger logger, string message) => Scope(logger, message);
}
