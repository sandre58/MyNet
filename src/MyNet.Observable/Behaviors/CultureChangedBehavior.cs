// -----------------------------------------------------------------------
// <copyright file="CultureChangedBehavior.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Globalization.Culture;
using MyNet.Observable.Behaviors.Metadata.Features.Events;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Defines a behavior that listens for culture change events from an ICultureService and raises an event with the new culture information when the culture changes. This behavior can be used to automatically update properties or perform actions in response to changes in the application's culture settings, allowing for a more dynamic and responsive user interface that adapts to different cultural contexts. By subscribing to the CultureChanged event of the ICultureService, this behavior ensures that any relevant updates or changes are triggered whenever the culture changes, enabling developers to easily implement culture-aware features in their applications.
/// </summary>
public sealed class CultureChangedBehavior : EventBehavior<CultureChangedEvent>
{
    private readonly ICultureService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="CultureChangedBehavior"/> class with the specified owner and culture service. This constructor sets up the behavior to listen for culture change events from the provided ICultureService and raises an event with the new culture information when the culture changes. By passing in the owner and culture service, this behavior can be easily integrated into an observable object, allowing it to react to culture changes and update its properties or perform actions accordingly without modifying the original class or creating new dependencies.
    /// </summary>
    /// <param name="owner">The owner object of this behavior.</param>
    /// <param name="cultureService">The culture service to listen for changes.</param>
    /// <exception cref="ArgumentNullException">Thrown if the culture service is null.</exception>
    public CultureChangedBehavior(ObservableObject owner, ICultureService cultureService)
        : base(owner)
    {
        _service = cultureService ?? throw new ArgumentNullException(nameof(cultureService));
        _service.CultureChanged += OnCultureChangedCallback;
    }

    /// <summary>
    /// Handles the CultureChanged event from the ICultureService. This method is called whenever the culture changes, and it raises an event with the new culture information. If the behavior is disposed or suspended, it will not raise the event. By implementing this callback method, the behavior ensures that any relevant updates or changes are triggered in response to culture changes, allowing developers to easily implement culture-aware features in their applications without modifying the original class or creating new dependencies.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data containing the new culture information.</param>
    private void OnCultureChangedCallback(object? sender, CultureChangedEventArgs e)
    {
        if (IsDisposed || IsSuspended)
            return;

        RaiseEvent(new(e.NewCulture));
    }

    /// <inheritdoc/>
    protected override void DisposeManagedResources()
    {
        _service.CultureChanged -= OnCultureChangedCallback;
        base.DisposeManagedResources();
    }
}
