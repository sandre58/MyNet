// -----------------------------------------------------------------------
// <copyright file="DefaultSchedulerProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Reactive.Concurrency;

namespace MyNet.UI.Threading;

/// <summary>
/// Provides fallback schedulers for environments that do not register a framework-specific scheduler provider.
/// </summary>
public sealed class DefaultSchedulerProvider : ISchedulerProvider
{
    /// <inheritdoc />
    public IScheduler Background { get; } = TaskPoolScheduler.Default;

    /// <inheritdoc />
    public IScheduler Ui { get; } = CurrentThreadScheduler.Instance;
}
