// -----------------------------------------------------------------------
// <copyright file="CultureChangedEvent.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Observable.Behaviors.Metadata.Features.Events;

/// <summary>
/// Represents an event that is raised when the culture of an observable object changes. This event contains information about the new culture, allowing subscribers to react to culture changes and update their behavior accordingly. By subscribing to this event, you can ensure that your observable objects respond appropriately to changes in culture, such as updating displayed text or formatting data according to the new culture settings.
/// </summary>
/// <param name="Culture">The new culture information.</param>
public record CultureChangedEvent(CultureInfo Culture);
