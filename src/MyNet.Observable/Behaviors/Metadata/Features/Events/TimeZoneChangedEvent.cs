// -----------------------------------------------------------------------
// <copyright file="TimeZoneChangedEvent.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Behaviors.Metadata.Features.Events;

/// <summary>
/// Represents an event that is raised when the time zone of an observable object changes. This event contains information about the new time zone, allowing subscribers to react to time zone changes and update their behavior accordingly. By subscribing to this event, you can ensure that your observable objects respond appropriately to changes in time zone, such as updating displayed times or scheduling tasks according to the new time zone settings.
/// </summary>
/// <param name="TimeZone">The new time zone information.</param>
public record TimeZoneChangedEvent(TimeZoneInfo TimeZone);
