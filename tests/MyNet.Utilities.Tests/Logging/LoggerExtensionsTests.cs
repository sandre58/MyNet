// -----------------------------------------------------------------------
// <copyright file="LoggerExtensionsTests.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MyNet.Utilities.Logging;
using Xunit;

namespace MyNet.Utilities.Tests.Logging;

public class LoggerExtensionsTests
{
    [Fact]
    public void MeasureTime_WithLevelAndTitle_ReturnsPerformanceLogger()
    {
        var logger = new CaptureLogger();

        using var perf = logger.MeasureTime(LogLevel.Information, "operation");

        Assert.IsType<PerformanceLogger>(perf);
    }

    [Fact]
    public void MeasureTime_WithTitleOverload_ReturnsPerformanceLogger()
    {
        var logger = new CaptureLogger();

        using var perf = logger.MeasureTime("operation", LogLevel.Warning);

        Assert.IsType<PerformanceLogger>(perf);
    }

    [Fact]
    public void MeasureTime_WithDynamicLevel_ReturnsPerformanceLogger()
    {
        var logger = new CaptureLogger();

        using var perf = logger.MeasureTime(_ => LogLevel.Debug, "operation");

        Assert.IsType<PerformanceLogger>(perf);
    }

    [Fact]
    public void MeasureTime_WithStartAndEndMessages_LogsStartAndEnd()
    {
        var logger = new CaptureLogger();

        using (logger.MeasureTime("operation", showStartMessage: true, showEndMessage: true, level: LogLevel.Trace))
        {
        }

        Assert.Contains(logger.Entries, e => e.Message.Contains("START", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(logger.Entries, e => e.Message.Contains("END", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void MeasureTime_NestedLoggers_ProduceSummaryEntries()
    {
        var logger = new CaptureLogger();

        using (logger.MeasureTime("root", showStartMessage: false, showEndMessage: false, level: LogLevel.Information))
        {
            using (logger.MeasureTime("child", showStartMessage: false, showEndMessage: false, level: LogLevel.Information))
            {
            }
        }

        Assert.Contains(logger.Entries, e => e.Message.Contains("Total Time", StringComparison.OrdinalIgnoreCase));
    }

    private sealed class CaptureLogger : ILogger
    {
        public List<(LogLevel Level, string Message)> Entries { get; } = [];

        public IDisposable BeginScope<TState>(TState state)
            where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Entries.Add((logLevel, formatter(state, exception)));

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }
}
