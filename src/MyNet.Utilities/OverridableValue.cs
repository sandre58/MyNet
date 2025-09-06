// -----------------------------------------------------------------------
// <copyright file="OverridableValue.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MyNet.Utilities;

/// <summary>
/// Represents a value that can either inherit its value from another source or be overridden locally.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public class OverridableValue<T> : INotifyPropertyChanged
{
    private Func<T?>? _getInheritedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="OverridableValue{T}"/> class that will retrieve its inherited value using the provided function.
    /// </summary>
    /// <param name="getInheritedValue">A function that returns the inherited value.</param>
    public OverridableValue(Func<T?> getInheritedValue) => _getInheritedValue = getInheritedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="OverridableValue{T}"/> class without an inherited value. Use <see cref="Initialize(Func{T?})"/> to provide one later.
    /// </summary>
    public OverridableValue() { }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => PropertyChangedHandler += value;
        remove => PropertyChangedHandler -= value;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Roslynator", "RCS1159:Use EventHandler<T>", Justification = "INotifyPropertyChanged implementation")]
    private event PropertyChangedEventHandler? PropertyChangedHandler;

    /// <summary>
    /// Gets the current effective value: either the inherited value (when not overridden) or the override value.
    /// </summary>
    public T? Value => !IsInherited ? InheritedValue : OverrideValue;

    /// <summary>
    /// Gets the value explicitly set as override.
    /// </summary>
    public T? OverrideValue { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the current value is inherited (<c>true</c>) or overridden (<c>false</c>).
    /// </summary>
    public bool IsInherited { get; private set; }

    /// <summary>
    /// Gets the inherited value by invoking the configured inherited value provider, if any.
    /// </summary>
    public T? InheritedValue => _getInheritedValue is not null ? _getInheritedValue() : default;

    /// <summary>
    /// Sets the function used to obtain the inherited value.
    /// </summary>
    /// <param name="getInheritedValue">A function that returns the inherited value.</param>
    public void Initialize(Func<T?> getInheritedValue) => _getInheritedValue = getInheritedValue;

    /// <summary>
    /// Initializes the inherited value provider using an expression on a <see cref="INotifyPropertyChanged"/> item and subscribes to its PropertyChanged event.
    /// </summary>
    /// <typeparam name="TItem">The type of the item exposing property change notifications.</typeparam>
    /// <param name="item">The item that raises PropertyChanged events.</param>
    /// <param name="propertyExpression">An expression selecting the inherited property.</param>
    public void Initialize<TItem>(TItem item, Expression<Func<T>> propertyExpression)
        where TItem : INotifyPropertyChanged
    {
        _getInheritedValue = propertyExpression.Compile();

        item.PropertyChanged += (_, e) =>
        {
            var propertyName = propertyExpression.GetPropertyName();

            if (e.PropertyName != propertyName) return;
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(InheritedValue));
        };
    }

    /// <summary>
    /// Initializes the inherited value provider using a delegate and subscribes to specified property names on the source item.
    /// </summary>
    /// <typeparam name="TItem">The type of the item exposing property change notifications.</typeparam>
    /// <param name="item">The item that raises PropertyChanged events.</param>
    /// <param name="getInheritedValue">A function that returns the inherited value using the provided item.</param>
    /// <param name="properties">Property names that trigger updates when changed.</param>
    public void Initialize<TItem>(TItem item, Func<TItem, T?> getInheritedValue, params string[] properties)
        where TItem : INotifyPropertyChanged
    {
        _getInheritedValue = () => getInheritedValue(item);

        item.PropertyChanged += (_, e) =>
        {
            if (string.IsNullOrEmpty(e.PropertyName) || !properties.Contains(e.PropertyName)) return;
            OnPropertyChanged(nameof(Value));
            OnPropertyChanged(nameof(InheritedValue));
        };
    }

    /// <summary>
    /// Sets an override value and raises property change notifications.
    /// </summary>
    /// <param name="value">The value to set as override.</param>
    public void Override(T value)
    {
        OverrideValue = value;
        IsInherited = true;

        OnPropertyChanged(nameof(OverrideValue));
        OnPropertyChanged(nameof(Value));
    }

    /// <summary>
    /// Resets the value to inherit from the configured source and clears any override.
    /// </summary>
    public void Reset()
    {
        OverrideValue = default;
        IsInherited = false;

        OnPropertyChanged(nameof(OverrideValue));
        OnPropertyChanged(nameof(Value));
    }

    public override string? ToString() => Value?.ToString();

    /// <summary>
    /// Raises the PropertyChanged event for the specified property name.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected void OnPropertyChanged(string? propertyName) => PropertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
