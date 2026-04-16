// -----------------------------------------------------------------------
// <copyright file="SplashScreenTaskEventArgs.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.ViewModels.SplashScreen;

/// <summary>
/// Event arguments raised when a splash screen task starts or completes.
/// </summary>
/// <param name="message">The task message.</param>
/// <param name="stepIndex">The 1-based step index.</param>
/// <param name="totalSteps">The total number of steps.</param>
public sealed class SplashScreenTaskEventArgs(string message, int stepIndex, int totalSteps) : EventArgs
{
    /// <summary>Gets the task message.</summary>
    public string Message { get; } = message;

    /// <summary>Gets the 1-based step index.</summary>
    public int StepIndex { get; } = stepIndex;

    /// <summary>Gets the total number of steps.</summary>
    public int TotalSteps { get; } = totalSteps;
}

/// <summary>
/// Event arguments raised when a splash screen task fails after all retry attempts.
/// </summary>
/// <param name="message">The task message.</param>
/// <param name="stepIndex">The 1-based step index.</param>
/// <param name="totalSteps">The total number of steps.</param>
/// <param name="exception">The exception that caused the failure.</param>
public sealed class SplashScreenTaskFailedEventArgs(string message, int stepIndex, int totalSteps, Exception exception) : EventArgs
{
    /// <summary>Gets the task message.</summary>
    public string Message { get; } = message;

    /// <summary>Gets the 1-based step index.</summary>
    public int StepIndex { get; } = stepIndex;

    /// <summary>Gets the total number of steps.</summary>
    public int TotalSteps { get; } = totalSteps;

    /// <summary>Gets the exception that caused the failure.</summary>
    public Exception Exception { get; } = exception;
}
