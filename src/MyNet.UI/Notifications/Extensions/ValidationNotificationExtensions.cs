// -----------------------------------------------------------------------
// <copyright file="ValidationNotificationExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Observable;
using MyNet.Observable.Behaviors;

#pragma warning disable IDE0130
namespace MyNet.UI.Notifications;
#pragma warning restore IDE0130

/// <summary>
/// Bridges <see cref="IValidationBehavior"/> with <see cref="INotificationPublisher"/>.
/// </summary>
public static class ValidationNotificationExtensions
{
    extension(ObservableObject owner)
    {
        /// <summary>
        /// Validates the object and publishes validation errors when validation fails.
        /// </summary>
        /// <param name="notificationPublisher">Optional publisher; when null, validation still runs but no notification is emitted.</param>
        /// <param name="errors">Optional explicit errors; when null, uses errors from <see cref="IValidationBehavior"/>.</param>
        /// <returns><see langword="true"/> when validation succeeds; otherwise <see langword="false"/>.</returns>
        public bool TryValidateAndNotify(
            INotificationPublisher? notificationPublisher,
            IEnumerable<string>? errors = null)
        {
            if (owner.Validate())
                return true;

            notificationPublisher?.PublishErrors(errors ?? owner.GetValidationErrors());

            return false;
        }

        /// <summary>
        /// Gets validation error messages from the registered <see cref="IValidationBehavior"/>, if any.
        /// </summary>
        public IReadOnlyCollection<string> GetValidationErrors() =>
            owner.Behaviors.TryGet<IValidationBehavior>(out var behavior) ? behavior.Errors : [];
    }
}
