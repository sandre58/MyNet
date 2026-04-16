// -----------------------------------------------------------------------
// <copyright file="SplashScreenTask.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyNet.UI.ViewModels.SplashScreen;

/// <summary>
/// Describes how a splash-screen task must be executed.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SplashScreenTask"/> class.
/// </remarks>
/// <param name="message">Factory that produces the localized status message.</param>
/// <param name="action">The asynchronous action to execute.</param>
/// <param name="canExecute">Optional predicate; the task is skipped when it returns <see langword="false"/>.</param>
/// <param name="runOnUiThread">When <see langword="true"/> the action is dispatched on the UI thread.</param>
/// <param name="continueOnError">When <see langword="true"/> failures are swallowed.</param>
/// <param name="retryCount">Number of additional attempts after the first failure.</param>
/// <param name="retryDelay">Delay between retries (defaults to 500 ms).</param>
public sealed class SplashScreenTask(
    Func<string> message,
    Func<CancellationToken, Task> action,
    Func<bool>? canExecute = null,
    bool runOnUiThread = false,
    bool continueOnError = false,
    int retryCount = 0,
    TimeSpan retryDelay = default)
{
    /// <summary>Gets the localized message shown while this task is running.</summary>
    public Func<string> Message { get; } = message;

    /// <summary>Gets the asynchronous action to run. Receives a <see cref="CancellationToken"/>.</summary>
    public Func<CancellationToken, Task> Action { get; } = action;

    /// <summary>Gets the condition that must be met for the task to execute.</summary>
    public Func<bool> CanExecute { get; } = canExecute ?? (() => true);

    /// <summary>Gets a value indicating whether the task is posted on the UI synchronization context.</summary>
    public bool RunOnUiThread { get; } = runOnUiThread;

    /// <summary>Gets a value indicating whether an exception thrown by this task is swallowed so the next task continues.</summary>
    public bool ContinueOnError { get; } = continueOnError;

    /// <summary>Gets the number of additional attempts after the first failure (0 = no retry).</summary>
    public int RetryCount { get; } = retryCount;

    /// <summary>Gets the delay between two consecutive retry attempts.</summary>
    public TimeSpan RetryDelay { get; } = retryDelay == TimeSpan.Zero ? TimeSpan.FromMilliseconds(500) : retryDelay;
}
