// -----------------------------------------------------------------------
// <copyright file="EventReactionFeature.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MyNet.Observable.Behaviors.Metadata.Features;

/// <summary>
/// Represents a feature that defines a set of events that an ObservableObject can react to. This feature allows you to specify which events the object should listen for and respond to, enabling a flexible and extensible event-driven architecture. By associating this feature with an ObservableObject, you can easily manage and handle reactions to specific events without tightly coupling the event handling logic to the object's core functionality. This promotes separation of concerns and makes it easier to maintain and extend the behavior of the object in response to various events.
/// </summary>
public sealed class EventReactionFeature
{
    /// <summary>
    /// Gets the set of event types that the ObservableObject can react to. This property holds a collection of Type objects representing the events that the object is interested in. By populating this set with specific event types, you can define the events that the ObservableObject should listen for and respond to, allowing for a customizable and extensible event-driven behavior. The object can then use this information to determine which events to subscribe to and how to react when those events are raised.
    /// </summary>
    public HashSet<Type> Events { get; } = [];
}
