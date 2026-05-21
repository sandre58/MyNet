// -----------------------------------------------------------------------
// <copyright file="ProgressManager.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Utilities.Progress;

/// <summary>
/// Static ambient-context facade that provides application-wide access to a single <see cref="IProgresser"/> instance.
/// </summary>
/// <remarks>
/// <para>
/// This class implements the <em>ambient context</em> pattern and should be used as a last resort
/// when dependency injection is not available (e.g. in static helpers or deeply nested call stacks).
/// In all other situations prefer injecting <see cref="IProgresser"/> directly and calling the
/// extension methods provided by <see cref="ProgresserExtensions"/>.
/// </para>
/// <para>
/// Call <see cref="Initialize"/> once at application startup before using any other member.
/// All methods return <see langword="null"/> when no progresser has been registered or when
/// StartStep variants are called outside an active session.
/// </para>
/// </remarks>
public static class ProgressManager
{
    private static IProgresser? _progresser;

    // ── Initialization ───────────────────────────────────────────────────────

    /// <summary>Registers the global <see cref="IProgresser"/> instance to use for progress tracking.</summary>
    /// <param name="progresser">The progresser to register.</param>
    public static void Initialize(IProgresser progresser) => _progresser = progresser;

    // ── Begin (root step) ────────────────────────────────────────────────────

    /// <summary>Starts a new single-step session with an optional message.</summary>
    public static IProgressStep<ProgressMessage>? Begin(string message = "", params object[] parameters)
        => _progresser?.Begin(message, parameters);

    /// <summary>Starts a new session divided into <paramref name="numberOfSteps"/> equal sub-steps.</summary>
    public static IProgressStep<ProgressMessage>? Begin(int numberOfSteps, string message = "", params object[] parameters)
        => _progresser?.Begin(numberOfSteps, message, parameters);

    /// <summary>Starts a new session with custom sub-step weightings.</summary>
    public static IProgressStep<ProgressMessage>? Begin(IEnumerable<double> subStepWeightings, string message = "", params object[] parameters)
        => _progresser?.Begin(subStepWeightings, message, parameters);

    /// <summary>Starts a new cancellable session with an optional message.</summary>
    public static IProgressStep<ProgressMessage>? Begin(Action cancelAction, string message = "", params object[] parameters)
        => _progresser?.Begin(cancelAction, message, parameters);

    /// <summary>Starts a new cancellable session divided into <paramref name="numberOfSteps"/> equal sub-steps.</summary>
    public static IProgressStep<ProgressMessage>? Begin(int numberOfSteps, Action cancelAction, string message = "", params object[] parameters)
        => _progresser?.Begin(numberOfSteps, cancelAction, message, parameters);

    /// <summary>Starts a new cancellable session with custom sub-step weightings.</summary>
    public static IProgressStep<ProgressMessage>? Begin(IEnumerable<double> subStepWeightings, Action cancelAction, string message = "", params object[] parameters)
        => _progresser?.Begin(subStepWeightings, cancelAction, message, parameters);

    // ── StartStep (child step) ───────────────────────────────────────────────

    /// <summary>
    /// Pushes a cancellable child step onto the current session.
    /// Returns <see langword="null"/> when no progresser is registered or no session is active.
    /// </summary>
    public static IProgressStep<ProgressMessage>? StartStep(string message = "", params object[] parameters)
        => TryStartStep(() => _progresser!.StartStep(message, parameters));

    /// <summary>
    /// Pushes a cancellable child step divided into <paramref name="numberOfSteps"/> equal sub-steps.
    /// Returns <see langword="null"/> when no progresser is registered or no session is active.
    /// </summary>
    public static IProgressStep<ProgressMessage>? StartStep(int numberOfSteps, string message = "", params object[] parameters)
        => TryStartStep(() => _progresser!.StartStep(numberOfSteps, message, parameters));

    /// <summary>
    /// Pushes a cancellable child step with custom sub-step weightings.
    /// Returns <see langword="null"/> when no progresser is registered or no session is active.
    /// </summary>
    public static IProgressStep<ProgressMessage>? StartStep(IEnumerable<double> subStepWeightings, string message = "", params object[] parameters)
        => TryStartStep(() => _progresser!.StartStep(subStepWeightings, message, parameters));

    /// <summary>
    /// Pushes a non-cancellable child step onto the current session.
    /// Returns <see langword="null"/> when no progresser is registered or no session is active.
    /// </summary>
    public static IProgressStep<ProgressMessage>? StartStepUncancellable(string message = "", params object[] parameters)
        => TryStartStep(() => _progresser!.StartStepUncancellable(message, parameters));

    /// <summary>
    /// Pushes a non-cancellable child step divided into <paramref name="numberOfSteps"/> equal sub-steps.
    /// Returns <see langword="null"/> when no progresser is registered or no session is active.
    /// </summary>
    public static IProgressStep<ProgressMessage>? StartStepUncancellable(int numberOfSteps, string message = "", params object[] parameters)
        => TryStartStep(() => _progresser!.StartStepUncancellable(numberOfSteps, message, parameters));

    /// <summary>
    /// Pushes a non-cancellable child step with custom sub-step weightings.
    /// Returns <see langword="null"/> when no progresser is registered or no session is active.
    /// </summary>
    public static IProgressStep<ProgressMessage>? StartStepUncancellable(IEnumerable<double> subStepWeightings, string message = "", params object[] parameters)
        => TryStartStep(() => _progresser!.StartStepUncancellable(subStepWeightings, message, parameters));

    // ── Private helpers ───────────────────────────────────────────────────────

    /// <summary>
    /// Invokes <paramref name="factory"/> and returns its result.
    /// Returns <see langword="null"/> when no progresser is registered or when the factory throws an
    /// <see cref="InvalidOperationException"/> (i.e. <c>StartStep</c> was called without a prior <c>Begin</c>).
    /// </summary>
    private static IProgressStep<ProgressMessage>? TryStartStep(Func<IProgressStep<ProgressMessage>> factory)
    {
        if (_progresser is null) return null;
        try
        {
            return factory();
        }
        catch (InvalidOperationException)
        {
            // No active session: StartStep was called without a prior Begin().
            // This is accepted as a no-op in the static facade to simplify caller code.
            return null;
        }
    }
}
