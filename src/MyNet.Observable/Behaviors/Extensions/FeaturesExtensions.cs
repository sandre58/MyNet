// -----------------------------------------------------------------------
// <copyright file="FeaturesExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using MyNet.Metadata;
using MyNet.Observable.Behaviors.Metadata;
using MyNet.Observable.Behaviors.Metadata.Features;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Fluent configuration extensions for observable metadata.
/// Authoring API: <see cref="MetadataRegistry.For{T}"/> → <see cref="PropertyBuilder{T,TProperty}"/>.
/// Batch API: <see cref="TypeMetadata"/> extension methods for multiple property names.
/// </summary>
public static class FeaturesExtensions
{
    extension(TypeMetadata metadata)
    {
        /// <summary>
        /// Configures the specified properties to refresh when the application culture changes.
        /// </summary>
        public TypeMetadata UpdateOnCultureChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                MetadataApplicators.ApplyUpdateOnCultureChanged(metadata.GetProperty(propertyName));

            return metadata;
        }

        /// <summary>
        /// Configures the specified properties to refresh when the application time zone changes.
        /// </summary>
        public TypeMetadata UpdateOnTimeZoneChanged(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                MetadataApplicators.ApplyUpdateOnTimeZoneChanged(metadata.GetProperty(propertyName));

            return metadata;
        }

        /// <summary>
        /// Configures the specified properties to ignore modification tracking.
        /// </summary>
        public TypeMetadata IgnoreModificationTracking(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
                MetadataApplicators.ApplyIgnoreModificationTracking(metadata.GetProperty(propertyName));

            return metadata;
        }
    }

    extension<T, TProperty>(PropertyBuilder<T, TProperty> builder)
    {
        /// <summary>
        /// Configures the property to react to changes of the specified event type.
        /// Prefer <see cref="UpdateOnCultureChanged"/> / <see cref="UpdateOnTimeZoneChanged"/> for built-in events.
        /// </summary>
        public PropertyBuilder<T, TProperty> ReactTo(Type eventType)
        {
            ArgumentNullException.ThrowIfNull(eventType);
            var feature = builder.Metadata.GetOrCreate<EventReactionFeature>();
            feature.Events.Add(eventType);
            return builder;
        }

        /// <summary>
        /// Configures the property to update when the culture changes.
        /// </summary>
        public PropertyBuilder<T, TProperty> UpdateOnCultureChanged()
        {
            MetadataApplicators.ApplyUpdateOnCultureChanged(builder.Metadata);
            return builder;
        }

        /// <summary>
        /// Configures the property to update when the time zone changes.
        /// </summary>
        public PropertyBuilder<T, TProperty> UpdateOnTimeZoneChanged()
        {
            MetadataApplicators.ApplyUpdateOnTimeZoneChanged(builder.Metadata);
            return builder;
        }

        /// <summary>
        /// Configures the property to ignore modification tracking.
        /// </summary>
        public PropertyBuilder<T, TProperty> IgnoreModificationTracking()
        {
            MetadataApplicators.ApplyIgnoreModificationTracking(builder.Metadata);
            return builder;
        }

        /// <summary>
        /// Configures the property to relay child property-changed notifications to the owner.
        /// </summary>
        public PropertyBuilder<T, TProperty> ForwardPropertyChanged(bool concatenatePropertyName = true)
        {
            MetadataApplicators.ApplyForwardProperty(builder.Metadata, concatenatePropertyName);
            return builder;
        }

        /// <summary>
        /// Configures the property to validate when the specified dependent properties change.
        /// </summary>
        public PropertyBuilder<T, TProperty> Validates(params string[] dependentProperties)
        {
            foreach (var dependentProperty in dependentProperties)
                MetadataApplicators.ApplyAlsoValidate(builder.Metadata, dependentProperty);

            return builder;
        }
    }
}
