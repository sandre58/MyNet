// -----------------------------------------------------------------------
// <copyright file="IProgresser.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Utilities.Progress;

/// <summary>
/// Specialization of <see cref="IProgresser{T}"/> that uses <see cref="ProgressMessage"/> as the message type.
/// </summary>
public interface IProgresser : IProgresser<ProgressMessage>;

/// <summary>
/// Factory and registry for hierarchical progress tracking.
/// </summary>
/// <typeparam name="T">The type of messages associated with progress steps.</typeparam>
/// <remarks>
/// A <em>session</em> is a tree of steps rooted by a call to <see cref="Begin(T, Action?)"/>.
/// Child steps are created via the <c>StartStep</c> overloads and must be disposed before control
/// returns to the parent. Subscribers receive a <see cref="ProgressReport{T}"/> each time
/// any step changes its progress or message.
/// <para>
/// Typical usage:
/// <code>
/// using var root = progresser.Begin(3, myMessage, cancelToken);
/// DoPhase1();
/// using (progresser.StartStep(phaseMsg)) { ... }
/// using (progresser.StartStep(phaseMsg)) { ... }
/// </code>
/// </para>
/// </remarks>
public interface IProgresser<T>
{
    // ── Root step factory ────────────────────────────────────────────────────

    /// <summary>
    /// Starts a new top-level progress session with a single implicit step.
    /// Any previous session is discarded.
    /// </summary>
    /// <param name="message">Message associated with the root step.</param>
    /// <param name="cancelAction">Optional action to invoke when the user requests cancellation.</param>
    IProgressStep<T> Begin(T message, Action? cancelAction = null);

    /// <summary>
    /// Starts a new top-level progress session pre-divided into <paramref name="numberOfSteps"/> equal sub-steps.
    /// Any previous session is discarded.
    /// </summary>
    /// <param name="numberOfSteps">Number of equal-weight child steps expected.</param>
    /// <param name="message">Message associated with the root step.</param>
    /// <param name="cancelAction">Optional action to invoke when the user requests cancellation.</param>
    IProgressStep<T> Begin(int numberOfSteps, T message, Action? cancelAction = null);

    /// <summary>
    /// Starts a new top-level progress session with custom sub-step weightings.
    /// Any previous session is discarded.
    /// </summary>
    /// <param name="subStepWeightings">
    /// Relative weights for each expected child step. Values do not need to sum to 1 —
    /// the implementation normalizes them automatically.
    /// </param>
    /// <param name="message">Message associated with the root step.</param>
    /// <param name="cancelAction">Optional action to invoke when the user requests cancellation.</param>
    IProgressStep<T> Begin(IEnumerable<double> subStepWeightings, T message, Action? cancelAction = null);

    // ── Child step factory ───────────────────────────────────────────────────

    /// <summary>
    /// Pushes a new child step onto the current session.
    /// The step is linked to the parent's next available sub-step slot.
    /// </summary>
    /// <param name="message">Message associated with this step.</param>
    /// <param name="canCancel">
    /// When <see langword="false"/> the <see cref="ProgressReport{T}.CanCancel"/> flag
    /// propagated to subscribers will be <see langword="false"/> for the lifetime of this step,
    /// regardless of the root cancel action.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no root step has been created with <see cref="Begin(T, Action?)"/>.
    /// </exception>
    IProgressStep<T> StartStep(T message, bool canCancel = true);

    /// <summary>
    /// Pushes a new child step divided into <paramref name="numberOfSteps"/> equal sub-steps.
    /// </summary>
    /// <param name="numberOfSteps">Number of equal-weight child steps expected within this step.</param>
    /// <param name="message">Message associated with this step.</param>
    /// <param name="canCancel">Whether this step supports cancellation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no root step has been created with <see cref="Begin(T, Action?)"/>.
    /// </exception>
    IProgressStep<T> StartStep(int numberOfSteps, T message, bool canCancel = true);

    /// <summary>
    /// Pushes a new child step with custom sub-step weightings.
    /// </summary>
    /// <param name="subStepWeightings">Relative weights for each expected nested child step.</param>
    /// <param name="message">Message associated with this step.</param>
    /// <param name="canCancel">Whether this step supports cancellation.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no root step has been created with <see cref="Begin(T, Action?)"/>.
    /// </exception>
    IProgressStep<T> StartStep(IEnumerable<double> subStepWeightings, T message, bool canCancel = true);

    // ── Subscription ─────────────────────────────────────────────────────────

    /// <summary>Registers a subscriber that receives <see cref="ProgressReport{T}"/> on every state change.</summary>
    /// <param name="progress">The subscriber to register.</param>
    void Subscribe(IProgress<ProgressReport<T>> progress);

    /// <summary>Removes a previously registered subscriber.</summary>
    /// <param name="progress">The subscriber to remove.</param>
    void Unsubscribe(IProgress<ProgressReport<T>> progress);
}
