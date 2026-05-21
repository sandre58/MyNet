// -----------------------------------------------------------------------
// <copyright file="LoggerMessages.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.Extensions.Logging;

namespace MyNet.Utilities.Logging;

/// <summary>
/// Defines strongly-typed logging messages for the application. This class uses source generators to create efficient logging methods that avoid unnecessary allocations when logging is disabled.
/// </summary>
public static partial class LoggerMessages
{
    /// <summary>
    /// Logs an error message with the provided exception details.
    /// </summary>
    /// <param name="logger">The logger instance to use for logging.</param>
    /// <param name="exception">The exception to log.</param>
    [LoggerMessage(LogLevel.Error, "An error occurred. See details exception.")]
    public static partial void LogException(this ILogger logger, Exception exception);
}
