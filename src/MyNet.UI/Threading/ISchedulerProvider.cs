// -----------------------------------------------------------------------
// <copyright file="ISchedulerProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Reactive.Concurrency;

namespace MyNet.UI.Threading;

/// <summary>
/// Defines an interface for providing schedulers for background and UI thread operations.
/// </summary>
public interface ISchedulerProvider
{
    /// <summary>
    /// Gets the scheduler for background operations.
    /// </summary>
    IScheduler Background { get; }

    /// <summary>
    /// Gets the scheduler for UI thread operations.
    /// </summary>
    IScheduler Ui { get; }
}
