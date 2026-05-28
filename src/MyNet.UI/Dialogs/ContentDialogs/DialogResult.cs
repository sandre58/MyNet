// -----------------------------------------------------------------------
// <copyright file="DialogResult.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.UI.Dialogs.ContentDialogs;

/// <summary>
/// Provides non-generic factory helpers for the common <see cref="DialogResult{T}"/> of <see cref="bool"/>.
/// </summary>
public static class DialogResult
{
    /// <summary>Returns a successful <c>bool</c> result (Value = <see langword="true"/>).</summary>
    public static DialogResult<bool> Ok() => DialogResult<bool>.Success(true);

    /// <summary>Returns a cancelled <c>bool</c> result.</summary>
    public static DialogResult<bool> Cancel() => DialogResult<bool>.Cancelled();

    /// <summary>Returns a dismissed <c>bool</c> result.</summary>
    public static DialogResult<bool> Dismiss() => DialogResult<bool>.Dismissed();
}

/// <summary>
/// Represents the result of a dialog operation, indicating whether the dialog was successful, canceled, dismissed, or returned a typed value.
/// </summary>
/// <typeparam name="T">The type of the value returned by the dialog.</typeparam>
public class DialogResult<T>
{
    /// <summary>
    /// Gets the outcome of the dialog operation (Success, Canceled, or Dismissed).
    /// </summary>
    public DialogOutcome Outcome { get; init; }

    /// <summary>
    /// Gets a value indicating whether the dialog was confirmed (user clicked OK/Accept).
    /// </summary>
    public bool IsSuccess => Outcome == DialogOutcome.Success;

    /// <summary>
    /// Gets a value indicating whether the dialog was explicitly canceled (user clicked Cancel).
    /// </summary>
    public bool IsCancelled => Outcome == DialogOutcome.Cancelled;

    /// <summary>
    /// Gets a value indicating whether the dialog was dismissed without an explicit action (e.g. overlay click, Escape key, or forced close).
    /// </summary>
    public bool IsDismissed => Outcome == DialogOutcome.Dismissed;

    /// <summary>
    /// Gets the value returned by the dialog. Only meaningful when <see cref="IsSuccess"/> is <see langword="true"/>.
    /// </summary>
    public T? Value { get; init; }

    /// <summary>Creates a successful result carrying <paramref name="value"/>.</summary>
    public static DialogResult<T> Success(T value) => new() { Outcome = DialogOutcome.Success, Value = value };

    /// <summary>Creates a canceled result (user explicitly canceled).</summary>
    public static DialogResult<T> Cancelled() => new() { Outcome = DialogOutcome.Cancelled };

    /// <summary>Creates a dismissed result (closed without explicit action).</summary>
    public static DialogResult<T> Dismissed() => new() { Outcome = DialogOutcome.Dismissed };
}

/// <summary>
/// Defines the possible outcomes of a dialog operation.
/// </summary>
public enum DialogOutcome
{
    /// <summary>The user confirmed the dialog (OK / Accept).</summary>
    Success,

    /// <summary>The user explicitly canceled the dialog (Cancel button).</summary>
    Cancelled,

    /// <summary>The dialog was closed without an explicit action (overlay click, Escape, forced close).</summary>
    Dismissed
}
