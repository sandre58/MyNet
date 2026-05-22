// -----------------------------------------------------------------------
// <copyright file="TimeZoneChangedBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Globalization.DateTime;
using MyNet.Observable.Behaviors.Metadata.Features.Events;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Defines a behavior that listens for changes in the system time zone and raises an event when the time zone changes. This behavior can be used in scenarios where an application needs to respond to changes in the system time zone, such as updating displayed times or adjusting scheduled tasks based on the new time zone. By subscribing to the TimeZoneChanged event, you can ensure that your application remains responsive and accurate when the system time zone is modified, providing a better user experience in applications that rely on date and time information.
/// </summary>
public sealed class TimeZoneChangedBehavior : EventBehavior<ObservableObject, TimeZoneChangedEvent>
{
    private readonly ITimeZoneService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeZoneChangedBehavior"/> class with the specified owner and time zone service. This constructor sets up the behavior to listen for time zone changes by subscribing to the TimeZoneChanged event of the provided time zone service. When the system time zone changes, the behavior will raise an event with the new time zone information, allowing the owner observable object to react accordingly. If the time zone service is null, an ArgumentNullException will be thrown to ensure that a valid service is provided for the behavior to function properly.
    /// </summary>
    /// <param name="owner">The owner object of this behavior.</param>
    /// <param name="timeZoneService">The time zone service to listen for changes.</param>
    /// <exception cref="ArgumentNullException">Thrown if the time zone service is null.</exception>
    public TimeZoneChangedBehavior(ObservableObject owner, ITimeZoneService timeZoneService)
        : base(owner)
    {
        _service = timeZoneService ?? throw new ArgumentNullException(nameof(timeZoneService));
        _service.TimeZoneChanged += OnTimeZoneChangedCallback;
    }

    /// <summary>
    /// Callback method that is invoked when the system time zone changes. This method checks if the behavior is disposed or suspended before raising an event with the new time zone information. If the behavior is active, it will call the RaiseEvent method with the new time zone provided in the event arguments, allowing any subscribers to react to the change in time zone accordingly. This ensures that the application remains responsive and accurate when the system time zone is modified, providing a better user experience in applications that rely on date and time information.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data containing the new time zone information.</param>
    private void OnTimeZoneChangedCallback(object? sender, TimeZoneChangedEventArgs e)
    {
        if (IsDisposed || IsSuspended)
            return;

        RaiseEvent(new(e.NewTimeZone));
    }

    /// <inheritdoc/>
    protected override void DisposeManagedResources()
    {
        _service.TimeZoneChanged -= OnTimeZoneChangedCallback;
        base.DisposeManagedResources();
    }
}
