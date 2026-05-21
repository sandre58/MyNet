// -----------------------------------------------------------------------
// <copyright file="Progresser.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MyNet.Utilities.Progress;

/// <summary>
/// Concrete implementation of <see cref="IProgresser"/> that works with <see cref="ProgressMessage"/>.
/// String-based convenience overloads are available via <see cref="ProgresserExtensions"/>.
/// </summary>
public class Progresser : Progresser<ProgressMessage>, IProgresser;

/// <summary>
/// Generic implementation of <see cref="IProgresser{T}"/>.
/// Manages a stack of active <see cref="ProgressStep{T}"/> instances and
/// notifies registered <see cref="IProgress{T}"/> subscribers on every state change.
/// </summary>
/// <typeparam name="T">The type of messages associated with progress steps.</typeparam>
public class Progresser<T> : IProgresser<T>
{
    // ── Fields ───────────────────────────────────────────────────────────────

    /// <summary>Thread-safe set of progress subscribers (value is unused; the key is the subscriber).</summary>
    private readonly ConcurrentDictionary<IProgress<ProgressReport<T>>, byte> _subscribers = new();

    /// <summary>LIFO stack of active steps, from root (bottom) to innermost (top).</summary>
    private readonly ConcurrentStack<ProgressStep<T>> _steps = new();

    /// <summary>
    /// The root step of the current session. Stored separately so that
    /// <see cref="Report"/> can access it in O(1) without iterating the stack.
    /// Volatile to avoid stale reads across threads.
    /// </summary>
    private volatile ProgressStep<T>? _rootStep;

    // ── IProgresser<T> — Begin ───────────────────────────────────────────────

    /// <inheritdoc/>
    public IProgressStep<T> Begin(T message, Action? cancelAction = null)
    {
        DiscardCurrentSession();
        return CreateRootStep([], message, cancelAction);
    }

    /// <inheritdoc/>
    public IProgressStep<T> Begin(int numberOfSteps, T message, Action? cancelAction = null)
    {
        DiscardCurrentSession();
        return CreateRootStep(EqualWeights(numberOfSteps), message, cancelAction);
    }

    /// <inheritdoc/>
    public IProgressStep<T> Begin(IEnumerable<double> subStepWeightings, T message, Action? cancelAction = null)
    {
        DiscardCurrentSession();
        return CreateRootStep(subStepWeightings, message, cancelAction);
    }

    // ── IProgresser<T> — StartStep ───────────────────────────────────────────

    /// <inheritdoc/>
    public IProgressStep<T> StartStep(T message, bool canCancel = true)
    {
        var parent = GetCurrentStep();
        return new ProgressStep<T>(this, parent, parent.AllocateChildSlot(), message, [], null, canCancel);
    }

    /// <inheritdoc/>
    public IProgressStep<T> StartStep(int numberOfSteps, T message, bool canCancel = true)
    {
        var parent = GetCurrentStep();
        return new ProgressStep<T>(this, parent, parent.AllocateChildSlot(), message, EqualWeights(numberOfSteps), null, canCancel);
    }

    /// <inheritdoc/>
    public IProgressStep<T> StartStep(IEnumerable<double> subStepWeightings, T message, bool canCancel = true)
    {
        var parent = GetCurrentStep();
        return new ProgressStep<T>(this, parent, parent.AllocateChildSlot(), message, subStepWeightings, null, canCancel);
    }

    // ── IProgresser<T> — Subscription ────────────────────────────────────────

    /// <inheritdoc/>
    public void Subscribe(IProgress<ProgressReport<T>> progress) => _subscribers.TryAdd(progress, 0);

    /// <inheritdoc/>
    public void Unsubscribe(IProgress<ProgressReport<T>> progress) => _subscribers.TryRemove(progress, out _);

    // ── Internal API (called by ProgressStep<T>) ─────────────────────────────
    [SuppressMessage("ReSharper", "NonAtomicCompoundOperator", Justification = "ConcurrentStack is thread-safe for Push/Pop, and rootStep is only used for reporting which can tolerate some staleness.")]
    internal void Push(ProgressStep<T> step)
    {
        _steps.Push(step);

        // The first step pushed after DiscardCurrentSession() becomes the root.
        _rootStep ??= step;
    }

    internal void Pop()
    {
        _steps.TryPop(out _);
        if (_steps.IsEmpty)
            _rootStep = null;
    }

    internal void Report()
    {
        var root = _rootStep;
        if (root is null) return;

        // Snapshot the stack to avoid races during enumeration.
        // ConcurrentStack enumerates top → bottom (LIFO), so Reverse() gives root → leaf.
        var snapshot = _steps.ToArray();
        var messages = snapshot
            .Reverse()
            .Where(s => s.Message is not null)
            .Select(s => s.Message!)
            .ToList();

        var canCancel = snapshot.All(s => s.CanCancel);
        var report = new ProgressReport<T>(root.Progress, messages, root.CancelAction, canCancel);

        foreach (var subscriber in _subscribers.Keys)
            subscriber.Report(report);
    }

    // ── Private helpers ───────────────────────────────────────────────────────
    private static IEnumerable<double> EqualWeights(int count)
        => count <= 0
            ? []
            : Enumerable.Repeat(1.0 / count, count);

    /// <summary>Clears any ongoing session without triggering dispose logic on individual steps.</summary>
    private void DiscardCurrentSession()
    {
        _steps.Clear();
        _rootStep = null;
    }

    private ProgressStep<T> CreateRootStep(IEnumerable<double> weightings, T message, Action? cancelAction) =>
        new(this, parent: null, parentSlotIndex: -1, message, weightings, cancelAction, canCancel: true);

    private ProgressStep<T> GetCurrentStep()
        => _steps.TryPeek(out var top)
            ? top
            : throw new InvalidOperationException(
                "No active progress session. Call Begin() before StartStep().");
}
