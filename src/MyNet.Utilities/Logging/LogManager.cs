// -----------------------------------------------------------------------
// <copyright file="LogManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;

namespace MyNet.Utilities.Logging;

/// <summary>
/// Class representing a Logger.
/// </summary>
public static class LogManager
{
    private static ILogger? _logger;

    public static void Initialize(ILogger logger) => _logger = logger;

    /// <summary>
    /// Information the specified message.
    /// </summary>
    /// <param name="message">The resource.</param>
    public static void Info(string message) => _logger?.Info(message);

    /// <summary>
    /// Traces the specified message.
    /// </summary>
    /// <param name="message">The resource.</param>
    public static void Trace(string message) => _logger?.Trace(message);

    /// <summary>
    /// Debugs the specified message.
    /// </summary>
    /// <param name="message">The resource.</param>
    public static void Debug(string message) => _logger?.Debug(message);

    /// <summary>
    /// Warnings the specified message.
    /// </summary>
    /// <param name="message">The resource.</param>
    public static void Warning(string message) => _logger?.Warning(message);

    /// <summary>
    /// Log Application Error.
    /// </summary>
    /// <param name="message">Non managed exception.</param>
    public static void Error(string message) => _logger?.Error(message);

    /// <summary>
    /// Log Application Error.
    /// </summary>
    /// <param name="ex">Non managed exception.</param>
    public static void Error(Exception ex) => _logger?.Error(ex);

    /// <summary>
    /// Log Critical Error that can crash application.
    /// </summary>
    /// <param name="ex">Critical non managed exception.</param>
    public static void Fatal(Exception ex) => _logger?.Fatal(ex);

    public static IDisposable MeasureTime(string title = "", PerformanceTraceLevel traceLevel = PerformanceTraceLevel.Trace) => MeasureTime(_ => traceLevel, title);

    public static IDisposable MeasureTime(Func<TimeSpan, PerformanceTraceLevel> provideTraceLevel, string title = "")
    {
        var settings = new PerformanceLoggerSettings(false, false, provideTraceLevel);
        var message = title;
        if (!string.IsNullOrEmpty(message)) return new PerformanceLogger(message, settings);
        var st = new StackTrace(new StackFrame(1));
        var method = st.GetFrame(0)?.GetMethod();

        if (method != null)
        {
            message = $"{method.DeclaringType}.{method.Name}({string.Join(", ", method.GetParameters().Select(x => x.Name))})";
        }

        return new PerformanceLogger(message, settings);
    }

    public static IDisposable MeasureTime(PerformanceLoggerSettings settings, string title = "")
    {
        var message = title;
        if (!string.IsNullOrEmpty(message)) return new PerformanceLogger(message, settings);
        var st = new StackTrace(new StackFrame(1));
        var method = st.GetFrame(0)?.GetMethod();

        if (method != null)
        {
            message = $"{method.DeclaringType}.{method.Name}({string.Join(", ", method.GetParameters().Select(x => x.Name))})";
        }

        return new PerformanceLogger(message, settings);
    }
}
