// -----------------------------------------------------------------------
// <copyright file="Log.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MyNet.Utilities.Logging;

/// <summary>
/// Provides a static logging utility that allows for creating loggers using a configurable logger factory, with a convenient generic type parameter for category-based logging.
/// </summary>
/// <typeparam name="T">The type to use as the logger category.</typeparam>
public static class Log<T>
{
    /// <summary>
    /// Gets a logger instance for the specified category type using the configured logger factory. This property provides a convenient way to access a logger without needing to call the Create method explicitly.
    /// </summary>
    public static ILogger Instance => Log.Factory.CreateLogger<T>();
}

/// <summary>
/// Provides a static logging utility that allows for creating loggers using a configurable logger factory.
/// </summary>
public static class Log
{
    /// <summary>
    /// Gets or sets the logger factory used to create loggers. By default, it is set to a null logger factory that produces no-op loggers.
    /// </summary>
    public static ILoggerFactory Factory { get; set; } = NullLoggerFactory.Instance;

    /// <summary>
    /// Creates a logger for the specified category type using the configured logger factory.
    /// </summary>
    /// <typeparam name="T">The type to use as the logger category.</typeparam>
    /// <returns>A logger instance for the specified category type.</returns>
    public static ILogger<T> Create<T>() => Factory.CreateLogger<T>();

    /// <summary>
    /// Creates a logger for the specified category name using the configured logger factory.
    /// </summary>
    /// <param name="category">The category name for the logger.</param>
    /// <returns>A logger instance for the specified category name.</returns>
    public static ILogger Create(string category) => Factory.CreateLogger(category);
}
