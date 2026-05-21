// -----------------------------------------------------------------------
// <copyright file="EventBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Utilities;
using MyNet.Utilities.Metadata;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Provides functionality to raise events and notify property changes for properties that are associated with a specific event type. This abstract class is designed to be inherited by concrete implementations that specify the event type they are interested in. When an event is raised using the RaiseEvent method, the class retrieves the metadata for the owner object and identifies all properties that are associated with the specified event type. It then notifies that these properties have changed, allowing any bindings or observers to react to the event. Additionally, if the owner object implements either the <see cref="IEventAware{TEvent}"/> or <see cref="IAsyncEventAware{TEvent}"/> interface, the corresponding OnEvent or OnEventAsync method will be invoked, allowing for further custom handling of the event. This class provides a structured way to manage events and their associated property changes within an observable object framework.
/// </summary>
/// <param name="owner">The owner object of this behavior.</param>
public abstract class EventBehavior<TEvent>(ObservableObject owner) : SuspendableBehavior(owner)
    where TEvent : class
{
    /// <summary>
    /// Raises an event of the specified type and notifies property changes for all properties that are associated with that event type. This method retrieves the metadata for the owner object and identifies all properties that are associated with the specified event type. It then calls NotifyPropertyChanged for each of those properties, allowing any bindings or observers to react to the event. Additionally, if the owner object implements either the <see cref="IEventAware{TEvent}"/> or <see cref="IAsyncEventAware{TEvent}"/> interface, the corresponding OnEvent or OnEventAsync method will be invoked, allowing for further custom handling of the event. This method provides a structured way to manage events and their associated property changes within an observable object framework.
    /// </summary>
    /// <param name="evt">The event instance to be raised.</param>
    protected void RaiseEvent(TEvent evt)
    {
        if (IsDisposed || IsSuspended)
            return;

        var typeMetadata = MetadataRegistry.Get(Owner.GetType());

        var properties = typeMetadata.WithFeature<TEvent>();

        foreach (var property in properties)
        {
            Owner.NotifyPropertyChanged(property);
        }

        switch (Owner)
        {
            case IEventAware<TEvent> aware:
                aware.OnEvent(evt);
                break;
            case IAsyncEventAware<TEvent> asyncAware:
                _ = asyncAware.OnEventAsync(evt);
                break;
        }
    }
}
