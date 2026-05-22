// -----------------------------------------------------------------------
// <copyright file="ModificationTrackingBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using MyNet.Observable.Behaviors.Metadata.Features;
using MyNet.Utilities;
using MyNet.Utilities.Metadata;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Provides a behavior that tracks modifications to an object and its nested properties, including collections. It implements the <see cref="IModificationTrackingBehavior"/> interface to indicate whether the object has been modified since the last reset. The behavior listens for property changes and collection changes to automatically update the modification state. It also supports suspending modification tracking to allow for batch updates without triggering intermediate modification states.
/// </summary>
public sealed class ModificationTrackingBehavior : SuspendableBehavior<ObservableObject>, IModificationTrackingBehavior
{
    private readonly CollectionTrackingBehavior? _collections;
    private readonly bool _ownsCollections;
    private readonly HashSet<IModificationAware> _trackedChildren = [];
    private readonly Dictionary<IModificationAware, INotifyPropertyChanged> _childNotifiers = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ModificationTrackingBehavior"/> class with the specified owner. This constructor initializes the behavior and attaches it to the owner object, allowing it to track modifications to the owner and its nested properties. It also initializes a default collection tracking behavior to handle modifications to collections within the owner object, ensuring that changes to collections are properly tracked and reflected in the modification state of the owner.
    /// </summary>
    /// <param name="owner">The owner object.</param>
    public ModificationTrackingBehavior(ObservableObject owner)
        : this(owner, new(owner), ownsCollections: true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModificationTrackingBehavior"/> class.
    /// </summary>
    /// <param name="owner">The owner object.</param>
    /// <param name="collections">The collection tracking behavior.</param>
    public ModificationTrackingBehavior(ObservableObject owner, CollectionTrackingBehavior collections)
        : this(owner, collections, ownsCollections: false)
    {
    }

    private ModificationTrackingBehavior(ObservableObject owner, CollectionTrackingBehavior? collections, bool ownsCollections)
        : base(owner)
    {
        _collections = collections;
        _ownsCollections = ownsCollections;

        _collections?.CollectionChanged += OnCollectionChanged;

        AttachExistingProperties();
    }

    /// <inheritdoc />
    public bool IsModified { get; private set; }

    /// <summary>
    /// Determines whether the specified property should be tracked for modifications, using metadata from <see cref="MetadataRegistry"/> (no reflection on the mutation path).
    /// </summary>
    /// <param name="propertyName">The name of the property to check.</param>
    /// <returns><c>true</c> if the property should be tracked; otherwise, <c>false</c>.</returns>
    private bool ShouldTrack(string propertyName) => !MetadataRegistry.Get(Owner.GetType()).GetProperty(propertyName).TryGetFeature<ModificationTrackingFeature>(out var feature) || !feature.Ignore;

    /// <summary>
    /// Marks the object as modified.
    /// </summary>
    public void MarkAsModified()
    {
        if (IsDisposed || IsSuspended)
            return;

        if (IsModified)
            return;

        IsModified = true;

        RaiseIsModifiedChanged();
    }

    /// <summary>
    /// Resets the modification state.
    /// </summary>
    /// <param name="recursive">
    /// Indicates whether nested objects should also be reset.
    /// </param>
    public void ResetModified(bool recursive = true)
    {
        if (recursive)
        {
            foreach (var child in _trackedChildren)
                child.ResetModified();
        }

        if (!IsModified)
            return;

        IsModified = false;

        RaiseIsModifiedChanged();
    }

    /// <inheritdoc/>
    void IModificationAware.ResetModified() => ResetModified();

    /// <inheritdoc />
    public void OnPropertyChanged(PropertyMutationContext context)
    {
        if (IsDisposed || IsSuspended)
            return;

        if (string.IsNullOrWhiteSpace(context.PropertyName))
            return;

        if (!ShouldTrack(context.PropertyName))
            return;

        if (Equals(context.OldValue, context.NewValue))
            return;

        Detach(context.OldValue);
        Attach(context.NewValue);

        MarkAsModified();
    }

    #region Collections (via CollectionTrackingBehavior)

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (IsDisposed || IsSuspended)
            return;

        MarkAsModified();
    }

    #endregion

    #region Attach / Detach

    /// <summary>
    /// Attaches to existing properties of the owner object that are modification-aware or collections. This method is called during initialization to ensure that the behavior is aware of any existing nested properties that may affect the modification state. It uses reflection to find all readable properties of the owner object and attaches to them if they implement the appropriate interfaces. This allows the behavior to track modifications to nested properties and collections from the moment it is attached, ensuring accurate modification tracking even for pre-existing data structures.
    /// </summary>
    private void AttachExistingProperties()
    {
        foreach (var property in Owner.GetType().GetPublicProperties())
        {
            if (!ShouldTrack(property.Name))
                continue;

            Attach(property.GetValue(Owner));
        }
    }

    /// <summary>
    /// Attaches to a value if it is modification-aware or a collection. This method is called when a property changes to attach to the new value if it implements the appropriate interfaces. It checks the type of the value and attaches to it if it is an IModificationAware or an INotifyCollectionChanged, allowing the behavior to track modifications to nested properties and collections. If the value is an enumerable, it also iterates through its items and attaches to any that are modification-aware, ensuring comprehensive tracking of modifications across complex object graphs.
    /// </summary>
    private void Attach(object? value)
    {
        if (value is IModificationAware child)
        {
            if (_trackedChildren.Add(child))
            {
                if (value is INotifyPropertyChanged npc)
                {
                    _childNotifiers[child] = npc;
                    npc.PropertyChanged += OnChildChanged;
                }
            }
        }
    }

    /// <summary>
    /// Detaches from a value if it is modification-aware or a collection. This method is called when a property changes to detach from the old value if it implements the appropriate interfaces. It checks the type of the value and detaches from it if it is an IModificationAware or an INotifyCollectionChanged, allowing the behavior to stop tracking modifications to nested properties and collections that are no longer relevant. If the value is an enumerable, it also iterates through its items and detaches from any that are modification-aware, ensuring that the behavior does not continue to track modifications for objects that have been removed from the object graph.
    /// </summary>
    private void Detach(object? value)
    {
        if (value is IModificationAware child)
        {
            if (_trackedChildren.Remove(child))
            {
                if (_childNotifiers.TryGetValue(child, out var npc))
                {
                    npc.PropertyChanged -= OnChildChanged;
                    _childNotifiers.Remove(child);
                }
            }
        }
    }

    /// <summary>
    /// Handles the PropertyChanged event of a child object that is modification-aware. This method is called when a child object that implements the IModificationAware interface raises a PropertyChanged event, allowing the behavior to react to changes in the child's properties. It checks if the child is currently modified, and if so, it marks the parent object as modified as well, ensuring that changes in nested properties are properly reflected in the overall modification state of the owner object.
    /// </summary>
    /// <param name="sender">The child object that raised the event.</param>
    /// <param name="e">The event data.</param>
    private void OnChildChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (IsDisposed || IsSuspended)
            return;

        if (sender is IModificationAware { IsModified: true })
        {
            MarkAsModified();
        }
    }

    #endregion

    #region Notifications

    /// <summary>
    /// Raises a notification that the IsModified property has changed. This method is called whenever the modification state of the owner object changes, allowing any observers to react to changes in the modification state. It uses the Owner's NotifyPropertyChanged method to raise a PropertyChanged event for the IsModified property, ensuring that any bindings or other observers that are monitoring this property will be notified of the change and can update their state accordingly.
    /// </summary>
    private void RaiseIsModifiedChanged() => Owner.NotifyPropertyChanged(nameof(IModificationAware.IsModified));

    #endregion

    #region Dispose

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        _collections?.CollectionChanged -= OnCollectionChanged;

        if (_ownsCollections)
            _collections?.Dispose();

        foreach (var notifier in _childNotifiers.Values)
        {
            notifier.PropertyChanged -= OnChildChanged;
        }

        _childNotifiers.Clear();
        _trackedChildren.Clear();

        base.DisposeManagedResources();
    }

    #endregion
}
