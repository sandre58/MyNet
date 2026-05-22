// -----------------------------------------------------------------------
// <copyright file="MetadataApplicators.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Observable.Behaviors.Metadata.Features;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using MyNet.Utilities.Metadata;

namespace MyNet.Observable.Behaviors.Metadata;

/// <summary>
/// Applies observable metadata features to <see cref="PropertyMetadata"/> instances.
/// Used by the metadata source generator and by fluent configuration extensions.
/// </summary>
public static class MetadataApplicators
{
    /// <summary>
    /// Configures the property to ignore modification tracking.
    /// </summary>
    public static void ApplyIgnoreModificationTracking(PropertyMetadata metadata)
        => metadata.SetFeature(new ModificationTrackingFeature { Ignore = true });

    /// <summary>
    /// Configures the property to refresh when the application culture changes.
    /// </summary>
    public static void ApplyUpdateOnCultureChanged(PropertyMetadata metadata)
        => metadata.GetOrCreate<EventReactionFeature>().Events.Add(typeof(CultureChangedEvent));

    /// <summary>
    /// Configures the property to refresh when the application time zone changes.
    /// </summary>
    public static void ApplyUpdateOnTimeZoneChanged(PropertyMetadata metadata)
        => metadata.GetOrCreate<EventReactionFeature>().Events.Add(typeof(TimeZoneChangedEvent));

    /// <summary>
    /// Adds a validation dependency on another property.
    /// </summary>
    public static void ApplyAlsoValidate(PropertyMetadata metadata, string dependentPropertyName)
        => metadata.GetOrCreate<ValidationDependencyFeature>().Dependents.Add(dependentPropertyName);

    /// <summary>
    /// Configures property-changed forwarding from a wrapper property to the owner.
    /// </summary>
    public static void ApplyForwardProperty(PropertyMetadata metadata, bool concatenatePropertyName = true)
        => metadata.SetFeature(new PropertyChangedForwardingFeature { ConcatenatePropertyName = concatenatePropertyName });
}
