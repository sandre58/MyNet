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
    private static readonly Lock TimeLocker = new();
    private static readonly Lock CurrentObject = new();
#else
    private static readonly object TimeLocker = new();
    private static readonly object CurrentObject = new();
#endif

    private static readonly Dictionary<PerformanceTraceLevel, Action<string>> GroupLogAction = new()
    {
        [PerformanceTraceLevel.Console] = x => Debug.WriteLine(x),
        [PerformanceTraceLevel.Information] = LogManager.Info,
        [PerformanceTraceLevel.Trace] = LogManager.Trace,
        [PerformanceTraceLevel.Debug] = LogManager.Debug,
        [PerformanceTraceLevel.Warning] = LogManager.Warning,
        [PerformanceTraceLevel.Error] = LogManager.Error
    };

    private readonly Dictionary<string, (PerformanceTraceLevel Level, TimeSpan Time)> _registeredTimes = [];

    private readonly string _title;
    private readonly PerformanceLoggerSettings _settings;
    private readonly Stopwatch _stopwatch;

    internal PerformanceLogger(string title, PerformanceLoggerSettings settings)
    {
        _title = title ?? throw new ArgumentNullException(nameof(title));
        _settings = settings;

        OperationGroups.Push(this);

        if (settings.ShowStartMessage)
            LogManager.Trace($"START - {_title}");

        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    internal static PerformanceLogger? Current => OperationGroups.Count == 0 ? null : OperationGroups.Peek();

    private TimeSpan TotalTime => _stopwatch.Elapsed;

    public void Dispose()
    {
        lock (CurrentObject)
        {
            Pop();

            _stopwatch.Stop();

            if (_settings.ShowEndMessage)
                LogManager.Trace($"END - {_title} : {_stopwatch.Elapsed}");

            if (Current == null)
            {
                TraceTimes();
            }
            else
            {
                Current.AddTime(_title, _stopwatch.Elapsed, _settings.ProvideTraceLevel(_stopwatch.Elapsed));
                Current.AddGroupTime(this);
            }
        }
    }

    internal void AddTime(string key, TimeSpan time, PerformanceTraceLevel level)
    {
        lock (TimeLocker)
        {
            _registeredTimes[key] = _registeredTimes.TryGetValue(key, out var value) ? (level, value.Time + time) : (level, time);
        }
    }

    private static void Pop()
    {
        if (OperationGroups.Count > 0)
            _ = OperationGroups.Pop();
    }

    private static void DisplayTime(KeyValuePair<string, (PerformanceTraceLevel Level, TimeSpan Time)> item) =>
        GroupLogAction[item.Value.Level]($"{item.Key} - {item.Value.Time}");

    private void AddGroupTime(PerformanceLogger group)
    {
        if (group._registeredTimes.Count == 0) return;

        KeyValuePair<string, (PerformanceTraceLevel, TimeSpan)>[] times;

        lock (TimeLocker)
        {
            times = [.. group._registeredTimes];
        }

        foreach (var time in times)
            AddTime($"{group._title} - {time.Key}", time.Value.Item2, time.Value.Item1);
    }

    private void TraceTimes()
    {
        var traceLevel = _settings.ProvideTraceLevel(TotalTime);
        var stars = new string('*', 20);
        GroupLogAction[traceLevel]($"{stars} {_title} {stars}");

        foreach (var item in _registeredTimes)
        {
            DisplayTime(item);
        }

        GroupLogAction[traceLevel]($"Total Time : {TotalTime}");
        GroupLogAction[traceLevel]($"{stars} {_title} {stars}");
    }
}

public record PerformanceLoggerSettings(bool ShowStartMessage, bool ShowEndMessage, Func<TimeSpan, PerformanceTraceLevel> ProvideTraceLevel);
