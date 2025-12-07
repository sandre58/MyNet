// -----------------------------------------------------------------------
// <copyright file="PerformanceLogger.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

#if NET9_0_OR_GREATER
using System.Threading;
#endif

namespace MyNet.Utilities.Logging;

public sealed class PerformanceLogger : IDisposable
{
    private static readonly Stack<PerformanceLogger> OperationGroups = new();

#if NET9_0_OR_GREATER
    private static readonly Lock StackLocker = new();
#else
    private static readonly object StackLocker = new();
#endif

    private static readonly Dictionary<PerformanceTraceLevel, Action<string>> LogActions = new()
    {
        [PerformanceTraceLevel.None] = _ => { },
        [PerformanceTraceLevel.Console] = x => Debug.WriteLine(x),
        [PerformanceTraceLevel.Information] = LogManager.Info,
        [PerformanceTraceLevel.Trace] = LogManager.Trace,
        [PerformanceTraceLevel.Debug] = LogManager.Debug,
        [PerformanceTraceLevel.Warning] = LogManager.Warning,
        [PerformanceTraceLevel.Error] = LogManager.Error
    };

    private readonly Dictionary<string, (PerformanceTraceLevel Level, TimeSpan Time)> _registeredTimes = [];

#if NET9_0_OR_GREATER
    private readonly Lock _timesLocker = new();
#else
    private readonly object _timesLocker = new();
#endif

    private readonly string _title;
    private readonly PerformanceLoggerSettings _settings;
    private readonly Stopwatch _stopwatch;

    internal PerformanceLogger(string title, PerformanceLoggerSettings settings)
    {
        _title = title ?? throw new ArgumentNullException(nameof(title));
        _settings = settings;

        lock (StackLocker)
        {
            OperationGroups.Push(this);
        }

        if (settings.ShowStartMessage)
            LogManager.Trace($"START - {_title}");

        _stopwatch = Stopwatch.StartNew();
    }

    internal static PerformanceLogger? Current
    {
        get
        {
            lock (StackLocker)
            {
                return OperationGroups.Count == 0 ? null : OperationGroups.Peek();
            }
        }
    }

    private TimeSpan TotalTime => _stopwatch.Elapsed;

    public void Dispose()
    {
        _stopwatch.Stop();
        var elapsed = _stopwatch.Elapsed;

        if (_settings.ShowEndMessage)
            LogManager.Trace($"END - {_title} : {elapsed}");

        PerformanceLogger? parent;
        lock (StackLocker)
        {
            if (OperationGroups.Count > 0 && OperationGroups.Peek() == this)
                OperationGroups.Pop();

            parent = OperationGroups.Count == 0 ? null : OperationGroups.Peek();
        }

        if (parent == null)
        {
            TraceTimes();
        }
        else
        {
            var traceLevel = _settings.ProvideTraceLevel(elapsed);
            parent.AddTime(_title, elapsed, traceLevel);
            parent.MergeGroupTimes(this);
        }
    }

    internal void AddTime(string key, TimeSpan time, PerformanceTraceLevel level)
    {
        lock (_timesLocker)
        {
            if (_registeredTimes.TryGetValue(key, out var existing))
            {
                _registeredTimes[key] = (level, existing.Time + time);
            }
            else
            {
                _registeredTimes[key] = (level, time);
            }
        }
    }

    private static string FormatTimeSpan(TimeSpan time) => time.TotalDays >= 1
            ? $"{time.TotalDays:F2}d"
            : time.TotalHours >= 1
            ? $"{time.TotalHours:F2}h"
            : time.TotalMinutes >= 1
            ? $"{time.TotalMinutes:F2}m"
            : time.TotalSeconds >= 1
            ? $"{time.TotalSeconds:F2}s"
            : time.TotalMilliseconds >= 1 ? $"{time.TotalMilliseconds:F2}ms" : $"{time.TotalMicroseconds:F2}μs";

    private void MergeGroupTimes(PerformanceLogger group)
    {
        if (group._registeredTimes.Count == 0) return;

        KeyValuePair<string, (PerformanceTraceLevel Level, TimeSpan Time)>[] times;

        lock (group._timesLocker)
        {
            times = [.. group._registeredTimes];
        }

        foreach (var (key, (level, time)) in times)
        {
            AddTime($"{group._title} - {key}", time, level);
        }
    }

    private void TraceTimes()
    {
        var traceLevel = _settings.ProvideTraceLevel(TotalTime);

        KeyValuePair<string, (PerformanceTraceLevel Level, TimeSpan Time)>[] times;
        lock (_timesLocker)
        {
            times = [.. _registeredTimes];
        }

        if (times.Length == 0)
        {
            LogActions[traceLevel]($"{_title} : {FormatTimeSpan(TotalTime)}");
            return;
        }

        var stars = new string('*', 20);
        LogActions[traceLevel]($"{stars} {_title} {stars}");

        foreach (var (key, (level, time)) in times)
        {
            LogActions[level]($"{key} - {FormatTimeSpan(time)}");
        }

        LogActions[traceLevel]($"Total Time : {FormatTimeSpan(TotalTime)}");
        LogActions[traceLevel]($"{stars} {_title} {stars}");
    }
}

public record PerformanceLoggerSettings(bool ShowStartMessage, bool ShowEndMessage, Func<TimeSpan, PerformanceTraceLevel> ProvideTraceLevel);
