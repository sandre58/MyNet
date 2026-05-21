// -----------------------------------------------------------------------
// <copyright file="SelectedWrapper.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using MyNet.Observable.Attributes;
using MyNet.Observable.Collections.Selection;

namespace MyNet.Observable;

/// <summary>
/// Wraps an item and provides selection state and logic for use in selectable collections.
/// Implements <see cref="ISelectable"/>.
/// </summary>
/// <typeparam name="T">The type of the wrapped item.</typeparam>
public class SelectedWrapper<T> : EditableWrapper<T>, ISelectable
{
    /// <summary>
    /// Gets or sets a value indicating whether the item can be selected.
    /// </summary>
    [CanBeValidated(false)]
    public virtual bool IsSelectable { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the item is currently selected.
    /// </summary>
    [CanBeValidated(false)]
    public bool IsSelected { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectedWrapper{T}"/> class.
    /// </summary>
    /// <param name="item">The item to wrap.</param>
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Fody compatibility")]
    public SelectedWrapper(T item)
        : base(item) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectedWrapper{T}"/> class with selection state.
    /// </summary>
    /// <param name="item">The item to wrap.</param>
    /// <param name="isSelected">Indicates whether the item is selected.</param>
    public SelectedWrapper(T item, bool isSelected)
        : this(item) => IsSelected = isSelected;

    /// <summary>
    /// Called when the selectable state changes. Deselects the item if it becomes unselectable.
    /// </summary>
    protected virtual void OnIsSelectableChanged()
    {
        if (!IsSelectable) IsSelected = false;
    }

    /// <summary>
    /// Creates a clone of this wrapper with the same selection and selectable state.
    /// </summary>
    /// <param name="item">The item to wrap in the clone.</param>
    /// <returns>A cloned instance of <see cref="SelectedWrapper{T}"/>.</returns>
    protected override EditableWrapper<T> CreateCloneInstance(T item) => new SelectedWrapper<T>(item, IsSelected) { IsSelectable = IsSelectable };
}
