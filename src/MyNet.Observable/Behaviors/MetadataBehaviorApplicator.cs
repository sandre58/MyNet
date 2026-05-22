// -----------------------------------------------------------------------
// <copyright file="MetadataBehaviorApplicator.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using MyNet.Observable.Behaviors.Metadata;
using MyNet.Observable.Behaviors.Metadata.Features;
using MyNet.Utilities;
using MyNet.Utilities.Metadata;

namespace MyNet.Observable.Behaviors;

/// <summary>
/// Applies runtime behaviors declared in <see cref="MetadataRegistry"/> to <see cref="ObservableObject"/> instances.
/// </summary>
public static class MetadataBehaviorApplicator
{
    private static readonly ConditionalWeakTable<ObservableObject, object?> AppliedOwners = [];

    /// <summary>
    /// Applies metadata-driven behaviors for the owner instance (currently property-changed forwarding).
    /// </summary>
    public static void Apply(ObservableObject owner)
    {
        if (AppliedOwners.TryGetValue(owner, out _))
            return;

        AppliedOwners.Add(owner, null);

        for (var current = owner.GetType(); current is not null && typeof(ObservableObject).IsAssignableFrom(current); current = current.BaseType)
        {
            var typeMetadata = MetadataRegistry.Get(current);

            foreach (var propertyName in typeMetadata.WithFeature<PropertyChangedForwardingFeature>())
            {
                var feature = typeMetadata.GetFeatureOrDefault<PropertyChangedForwardingFeature>(propertyName);
                if (feature is null)
                    continue;

                ReplaceForwardingBehavior(owner, propertyName, feature.ConcatenatePropertyName);
            }
        }
    }

    /// <summary>
    /// Registers forwarding for a single property and records it in metadata when not already configured.
    /// </summary>
    public static void ApplyForwardProperty(ObservableObject owner, string propertyName, bool concatenatePropertyName = true)
    {
        ArgumentNullException.ThrowIfNull(owner);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        var propertyMetadata = MetadataRegistry.Get(owner.GetType()).GetProperty(propertyName);
        MetadataApplicators.ApplyForwardProperty(propertyMetadata, concatenatePropertyName);
        ReplaceForwardingBehavior(owner, propertyName, concatenatePropertyName);
    }

    private static void ReplaceForwardingBehavior(ObservableObject owner, string propertyName, bool concatenatePropertyName)
    {
        owner.Behaviors.Replace(
            new PropertyChangedForwardingBehavior(owner, propertyName, concatenatePropertyName),
            propertyName,
            nameof(PropertyChangedForwardingBehavior));
    }
}
