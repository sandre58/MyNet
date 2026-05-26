// -----------------------------------------------------------------------
// <copyright file="ProgressStep.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MyNet.Primitives;

namespace MyNet.Utilities.Progress;

/// <summary>
/// Internal implementation of <see cref="IProgressStep{T}"/>.
/// Each instance represents one node in the progress tree.
/// </summary>
internal sealed class ProgressStep<T> : IProgressStep<T>
{
    private readonly Progresser<T> _progresser;

    /// <summary>Direct reference to the parent step (null for the root step).</summary>
    private readonly ProgressStep<T>? _parent;

    /// <summary>
    /// Index of the child slot in <see cref="_parent"/> that this step occupies.
    /// -1 when there is no parent or when the parent has no pre-allocated slot for this step.
    /// </summary>
    private readonly int _parentSlotIndex;

    /// <summary>Pre-allocated child slots with their relative weightings.</summary>
    private readonly List<ProgressStepValue> _children = [];

    /// <summary>Points to the next unallocated child slot index.</summary>
    private int _nextChildSlot;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressStep{T}"/> class, pushes it onto the progresser stack,
    /// and fires an initial report so subscribers see the new message immediately.
    /// </summary>
    internal ProgressStep(
        Progresser<T> progresser,
        ProgressStep<T>? parent,
        int parentSlotIndex,
        T? message,
        IEnumerable<double> subStepWeightings,
        Action? cancelAction,
        bool canCancel)
    {
        _progresser = progresser;
        _parent = parent;
        _parentSlotIndex = parentSlotIndex;
        CancelAction = cancelAction;
        CanCancel = canCancel;
        Message = message;

        // Normalize and store sub-step weightings so they always sum to 1.
        var weightings = subStepWeightings.ToList();
        var total = weightings.Sum();
        foreach (var w in weightings)
            _children.Add(new(total > 0 ? w / total : 1.0 / weightings.Count));

        _progresser.Push(this);

        // Fire an initial report so subscribers see the new message right away.
        _progresser.Report();
    }

    /// <inheritdoc/>
    public double Progress { get; private set; }

    /// <inheritdoc/>
    public bool IsCompleted { get; private set; }

    /// <inheritdoc/>
    public T? Message { get; private set; }

    /// <inheritdoc/>
    public bool CanCancel { get; }

    /// <inheritdoc/>
    public Action? CancelAction { get; }

    // ── Internal child-tracking API (used by Progresser<T>) ─────────────────

    /// <summary>
    /// Allocates the next child slot and returns its index.
    /// Returns -1 when there are no more pre-allocated slots available.
    /// </summary>
    internal int AllocateChildSlot()
        => _nextChildSlot < _children.Count ? _nextChildSlot++ : -1;

    /// <summary>
    /// Updates the progress of the child slot at <paramref name="slotIndex"/> and recomputes
    /// this step's own aggregate progress.
    /// </summary>
    internal void SetChildProgress(int slotIndex, double progress)
    {
        if (slotIndex < 0 || slotIndex >= _children.Count) return;
        _children[slotIndex].Progress = progress;
        RecomputeProgress();
    }

    // ── IProgressStep<T> ────────────────────────────────────────────────────

    /// <inheritdoc/>
    public void UpdateMessage(T? message)
    {
        if (Equals(Message, message)) return;
        Message = message;
        _progresser.Report();
    }

    /// <inheritdoc/>
    public void UpdateProgress(double value)
    {
        var clamped = Math.Clamp(value, 0.0, 1.0);
        if (Progress.IsCloseTo(clamped)) return;

        Progress = clamped;

        if (_parent is not null)
            _parent.SetChildProgress(_parentSlotIndex, Progress);
        else
            _progresser.Report();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        UpdateProgress(1.0);
        _progresser.Pop();
    }

    // ── Private helpers ──────────────────────────────────────────────────────
    private void RecomputeProgress()
    {
        var aggregate = _children.Sum(c => c.Weighting * c.Progress);
        UpdateProgress(aggregate);
    }

    // ── Nested type ──────────────────────────────────────────────────────────
    [DebuggerDisplay("Weighting={Weighting}, Progress={Progress}")]
    private sealed class ProgressStepValue(double weighting)
    {
        public double Weighting { get; } = weighting;

        public double Progress { get; set; }
    }
}
