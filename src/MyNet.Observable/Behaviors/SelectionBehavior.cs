// -----------------------------------------------------------------------
// <copyright file="SelectionBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Provides selection state (IsSelected / IsSelectable) for an ObservableObject.
/// The behavior implements <see cref="ISelectable"/> and raises property changed
/// notifications on the owner when the selection state changes.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SelectionBehavior"/> class.
/// </remarks>
/// <param name="owner">The owner observable object.</param>
/// <param name="isSelectable">Initial value for IsSelectable (default true).</param>
public sealed class SelectionBehavior(ObservableObject owner, bool isSelectable = true) : SuspendableBehavior<ObservableObject>(owner), ISelectable
{
    private bool _isSelectable = isSelectable;
    private bool _isSelected;

    /// <inheritdoc />
    public bool IsSelectable
    {
        get => _isSelectable;
        set
        {
            if (IsDisposed || IsSuspended)
                return;

            if (_isSelectable == value)
                return;

            _isSelectable = value;

            if (!_isSelectable)
            {
                SetSelected(false);
            }

            NotifyChanged(nameof(IsSelectable));
        }
    }

    /// <inheritdoc />
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (IsDisposed || IsSuspended)
                return;

            if (!_isSelectable)
                return;

            SetSelected(value);
        }
    }

    /// <summary>
    /// Sets the IsSelected property to the specified value and raises a property changed notification if the value has changed. This method is used internally to update the selection state and notify the owner when the selection state changes. It checks if the new value is different from the current value, and if so, it updates the _isSelected field and calls NotifyChanged to raise a property changed notification for the IsSelected property.
    /// </summary>
    /// <param name="value">The new value for the IsSelected property.</param>
    private void SetSelected(bool value)
    {
        if (_isSelected == value)
            return;

        _isSelected = value;

        NotifyChanged(nameof(IsSelected));
    }

    /// <summary>
    /// Notifies the owner that a property has changed by raising the PropertyChanged event with the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    private void NotifyChanged(string propertyName) => Owner.NotifyPropertyChanged(propertyName);
}
