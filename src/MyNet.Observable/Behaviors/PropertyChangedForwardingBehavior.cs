// -----------------------------------------------------------------------
// <copyright file="PropertyChangedForwardingBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Threading;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Relays PropertyChanged notifications from a wrapper object (a property of the owner)
/// to the owner. The behavior watches a named property on the owner; when that
/// property's value implements <see cref="INotifyPropertyChanged"/>, the behavior
/// subscribes to its PropertyChanged and forwards notifications to the owner by
/// calling <see cref="ObservableObject.NotifyPropertyChanged(string, object?, object?)"/> with either
/// the child property name or a qualified name (<c>Wrapper.Property</c>) depending
/// on configuration.
/// </summary>
public sealed class PropertyChangedForwardingBehavior : SuspendableBehavior<ObservableObject>, IPropertyChangedBehavior
{
    private readonly string _propertyName;
    private readonly bool _concatenatePropertyName;
    private readonly bool _raiseInitialSnapshot;
    private readonly Lock _gate = new();
    private INotifyPropertyChanged? _source;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyChangedForwardingBehavior"/> class.
    /// </summary>
    /// <param name="owner">Owner observable object.</param>
    /// <param name="propertyName">The owner property that contains the wrapper source.</param>
    /// <param name="concatenatePropertyName">True to emit notifications like Wrapper.Name; false to emit only Name.</param>
    /// <param name="raiseInitialSnapshot">When <c>true</c>, relays a snapshot of the source's public properties after each attach.</param>
    public PropertyChangedForwardingBehavior(
        ObservableObject owner,
        string propertyName,
        bool concatenatePropertyName = true,
        bool raiseInitialSnapshot = true)
        : base(owner)
    {
        _propertyName = propertyName;
        _concatenatePropertyName = concatenatePropertyName;
        _raiseInitialSnapshot = raiseInitialSnapshot;

        Attach(ObservableObjectPropertyAccess.GetPropertyValue(Owner, _propertyName) as INotifyPropertyChanged);
    }

    /// <inheritdoc />
    public void OnPropertyChanged(PropertyMutationContext context)
    {
        if (IsDisposed || IsSuspended)
            return;

        if (!string.Equals(context.PropertyName, _propertyName, StringComparison.Ordinal))
            return;

        Attach(context.NewValue as INotifyPropertyChanged);
    }

    /// <summary>
    /// Attaches to the specified source object by subscribing to its PropertyChanged event.
    /// </summary>
    /// <param name="value">The source object to attach to.</param>
    private void Attach(INotifyPropertyChanged? value)
    {
        INotifyPropertyChanged? attached;

        lock (_gate)
        {
            if (ReferenceEquals(_source, value))
                return;

            _source?.PropertyChanged -= OnSourcePropertyChanged;

            _source = value;

            attached = _source;

            attached?.PropertyChanged += OnSourcePropertyChanged;
        }

        if (attached is not null && _raiseInitialSnapshot)
            RaiseInitialSnapshot(attached);
    }

    /// <summary>
    /// Relays current values of the source's public properties (attach-time snapshot).
    /// </summary>
    private void RaiseInitialSnapshot(INotifyPropertyChanged source)
    {
        if (IsDisposed || IsSuspended)
            return;

        foreach (var (childPropertyName, _) in NotifyPropertyChangedSourceSnapshot.EnumerateValues(source))
            RelayPropertyChanged(childPropertyName);
    }

    /// <summary>
    /// Handles PropertyChanged events from the source object and relays them to the owner.
    /// </summary>
    /// <param name="sender">The source object that raised the event.</param>
    /// <param name="e">The event arguments containing the name of the property that changed.</param>
    private void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (IsDisposed || IsSuspended)
            return;

        if (string.IsNullOrWhiteSpace(e.PropertyName))
            return;

        lock (_gate)
        {
            if (!ReferenceEquals(sender, _source))
                return;
        }

        RelayPropertyChanged(e.PropertyName);
    }

    private void RelayPropertyChanged(string childPropertyName)
    {
        if (IsDisposed || IsSuspended)
            return;

        var relayedName = _concatenatePropertyName ? $"{_propertyName}.{childPropertyName}" : childPropertyName;
        Owner.NotifyPropertyChanged(relayedName, UnknownValue.Instance, UnknownValue.Instance);
    }

    /// <inheritdoc/>
    protected override void DisposeManagedResources()
    {
        lock (_gate)
        {
            _source?.PropertyChanged -= OnSourcePropertyChanged;
            _source = null;
        }

        base.DisposeManagedResources();
    }
}
