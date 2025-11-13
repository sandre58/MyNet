// -----------------------------------------------------------------------
// <copyright file="EditableWrapper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using MyNet.Utilities;

namespace MyNet.Observable;

/// <summary>
/// Provides an editable wrapper around an item with property change notifications, validation, and cloning support.
/// Optimized for performance with minimal allocations and efficient event handling.
/// </summary>
/// <typeparam name="T">The type of the wrapped item.</typeparam>
public class EditableWrapper<T> : EditableObject, ICloneable, ISettable, IIdentifiable<Guid>, IWrapper<T>
{
    private T _item;
    private INotifyPropertyChanged? _notifyPropertyChangedItem;

    /// <summary>
    /// Gets the unique identifier for this wrapper instance.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Gets or sets the wrapped item.
    /// </summary>
    public T Item
    {
        get => _item;
        protected set
        {
            if (EqualityComparer<T>.Default.Equals(_item, value))
                return;

            OnItemChanging();
            _item = value;
            OnItemChanged();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditableWrapper{T}"/> class.
    /// </summary>
    /// <param name="item">The item to wrap.</param>
    public EditableWrapper(T item)
    {
        Id = Guid.NewGuid();
        _item = item;

        // Optimize: Cache the INotifyPropertyChanged reference
        _notifyPropertyChangedItem = item as INotifyPropertyChanged;

        if (_notifyPropertyChangedItem != null)
        {
            _notifyPropertyChangedItem.PropertyChanged += Item_PropertyChanged;
        }
    }

    /// <summary>
    /// Called when the item has been changed.
    /// Subscribes to property change notifications if the item implements <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    protected virtual void OnItemChanged()
    {
        // Optimize: Use cached reference
        _notifyPropertyChangedItem = _item as INotifyPropertyChanged;

        if (_notifyPropertyChangedItem != null)
        {
            _notifyPropertyChangedItem.PropertyChanged += Item_PropertyChanged;
        }
    }

    /// <summary>
    /// Called before the item is about to be changed.
    /// Unsubscribes from property change notifications and disposes the item if necessary.
    /// </summary>
    protected virtual void OnItemChanging()
    {
        // Optimize: Use cached reference for unsubscription
        if (_notifyPropertyChangedItem != null)
        {
            _notifyPropertyChangedItem.PropertyChanged -= Item_PropertyChanged;
            _notifyPropertyChangedItem = null;
        }

        // Dispose if item implements IDisposable
        if (_item is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    /// <summary>
    /// Handles property changes from the wrapped item and forwards them.
    /// </summary>
    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
=> OnPropertyChanged(e.PropertyName);

    /// <summary>
    /// Creates a clone of this wrapper with a cloned or copied item.
    /// </summary>
    /// <returns>A new instance with a cloned item.</returns>
    public virtual object Clone()
    {
        // Optimize: Check for ICloneable first to avoid unnecessary boxing
        var clonedItem = _item is ICloneable cloneable
         ? (T)cloneable.Clone()
             : _item;

        return CreateCloneInstance(clonedItem);
    }

    /// <summary>
    /// Creates a new wrapper instance for cloning purposes.
    /// Override this method in derived classes to return the correct type.
    /// </summary>
    /// <param name="item">The item to wrap in the clone.</param>
    /// <returns>A new wrapper instance.</returns>
    protected virtual EditableWrapper<T> CreateCloneInstance(T item) => new(item);

    /// <summary>
    /// Sets the properties of this wrapper from another object.
    /// Supports setting from a raw item of type <typeparamref name="T"/> or another <see cref="EditableWrapper{T}"/>.
    /// </summary>
    /// <param name="from">The source object to copy from.</param>
    public void SetFrom(object? from)
    {
        if (from == null)
            return;

        // Optimize: Use pattern matching and early exit
        switch (from)
        {
            case T newItem:
                SetFromItem(newItem);
                break;

            case EditableWrapper<T> wrapper:
                SetFromItem(wrapper.Item);
                break;
        }
    }

    /// <summary>
    /// Sets the properties from a raw item.
    /// </summary>
    /// <param name="newItem">The item to copy properties from.</param>
    private void SetFromItem(T newItem)
    {
        // Optimize: Check ISettable first as it's more specific
        if (_item is ISettable settable)
        {
            settable.SetFrom(newItem);
        }
        else
        {
            // Fallback to DeepSet extension method
            _item?.DeepSet(newItem);
        }
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current wrapper.
    /// Equality is based on reference equality of the wrapped item.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>True if the objects wrap the same item; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        // Optimize: Combine null check and type check
        if (obj is not EditableWrapper<T> other)
            return false;

        // Optimize: Use ReferenceEquals for reference types, EqualityComparer for value types
        return typeof(T).IsValueType
          ? EqualityComparer<T>.Default.Equals(_item, other._item)
      : ReferenceEquals(_item, other._item);
    }

    /// <summary>
    /// Returns a hash code for this wrapper based on the wrapped item.
    /// </summary>
    /// <returns>A hash code for the current wrapper.</returns>
    public override int GetHashCode() => _item != null ? _item.GetHashCode() : 0;

    /// <summary>
    /// Cleans up resources and unsubscribes from events.
    /// </summary>
    protected override void Cleanup()
    {
        // Optimize: Use cached reference for cleanup
        if (_notifyPropertyChangedItem != null)
        {
            _notifyPropertyChangedItem.PropertyChanged -= Item_PropertyChanged;
            _notifyPropertyChangedItem = null;
        }

        base.Cleanup();
    }
}
