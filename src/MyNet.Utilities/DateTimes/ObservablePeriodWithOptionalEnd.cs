// -----------------------------------------------------------------------
// <copyright file="ObservablePeriodWithOptionalEnd.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace MyNet.Utilities.DateTimes;

/// <summary>
/// An observable variant of <see cref="PeriodWithOptionalEnd"/> that raises property change notifications
/// when the Start or End properties are changed.
/// </summary>
public class ObservablePeriodWithOptionalEnd(DateTime start, DateTime? end = null) : PeriodWithOptionalEnd(start, end), INotifyPropertyChanged
{
    /// <summary>
    /// Occurs when a property value changes. This forwards to the internal <see cref="PropertyChangedHandler"/>.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => PropertyChangedHandler += value;
        remove => PropertyChangedHandler -= value;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1159:Use EventHandler<T>", Justification = "INotifyPropertyChanged implementation")]
    private event PropertyChangedEventHandler? PropertyChangedHandler;

    /// <summary>
    /// Sets the interval and raises <see cref="PropertyChanged"/> for Start and End when they change.
    /// </summary>
    /// <param name="start">The new start date/time.</param>
    /// <param name="end">The new end date/time, or null for open-ended intervals.</param>
    public override void SetInterval(DateTime start, DateTime? end = null)
    {
        var oldStart = Start;
        var oldEnd = End;
        base.SetInterval(start, end);
        if (oldStart != Start)
            OnPropertyChanged(nameof(Start));
        if (oldEnd != End)
            OnPropertyChanged(nameof(End));
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event for the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected void OnPropertyChanged(string? propertyName) => PropertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
