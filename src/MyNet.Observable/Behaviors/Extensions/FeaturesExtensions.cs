// -----------------------------------------------------------------------
// <copyright file="FeaturesExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using MyNet.Observable.Behaviors.Metadata;
using MyNet.Observable.Behaviors.Metadata.Features;
using MyNet.Observable.Behaviors.Metadata.Features.Events;
using MyNet.Utilities;
using MyNet.Utilities.Metadata;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Observable;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class FeaturesExtensions
{
    extension(TypeMetadata metadata)
    {
        /// <summary>
        /// Configures the specified properties to update when the culture changes. This method takes an array of property names and applies the UpdateOnCultureChanged configuration to each of them. By calling this method, you can easily configure multiple properties to update their values whenever the culture changes in the application, ensuring that they reflect the new culture settings appropriately. This is particularly useful for properties that are culture-sensitive, such as those that display formatted dates, numbers, or localized strings, allowing for proper localization and formatting based on the current culture.
        /// </summary>
        /// <param name="propertyNames">The names of the properties to configure.</param>
        /// <returns>The updated type metadata.</returns>
        public TypeMetadata UpdateOnCultureChanged(params string[] propertyNames)
        {
            propertyNames.ForEach(propertyName => metadata.GetProperty(propertyName).UpdateOnCultureChanged());

            return metadata;
        }

        /// <summary>
        /// Configures the specified properties to update when the time zone changes. This method takes an array of property names and applies the UpdateOnTimeZoneChanged configuration to each of them. By calling this method, you can easily configure multiple properties to update their values whenever the time zone changes in the application, ensuring that they reflect the new time zone settings appropriately. This is particularly useful for properties that are time zone-sensitive, such as those that display dates and times, allowing for proper display of date and time information based on the current time zone.
        /// </summary>
        /// <param name="propertyNames">The names of the properties to configure.</param>
        /// <returns>The updated type metadata.</returns>
        public TypeMetadata UpdateOnTimeZoneChanged(params string[] propertyNames)
        {
            propertyNames.ForEach(propertyName => metadata.GetProperty(propertyName).UpdateOnTimeZoneChanged());

            return metadata;
        }

        /// <summary>
        /// Configures the specified properties to ignore modification tracking. This method takes an array of property names and applies the IgnoreModificationTracking configuration to each of them. By calling this method, you can easily configure multiple properties to ignore modification tracking, meaning that changes to these properties will not be tracked or trigger any modification-related behaviors in the application. This is particularly useful for properties that do not require tracking of changes, allowing for improved performance and reduced overhead when working with modification tracking in an application.
        /// </summary>
        /// <param name="propertyNames">The names of the properties to configure.</param>
        /// <returns>The updated type metadata.</returns>
        public TypeMetadata IgnoreModificationTracking(params string[] propertyNames)
        {
            propertyNames.ForEach(propertyName => metadata.GetProperty(propertyName).IgnoreModificationTracking());

            return metadata;
        }
    }

    extension<T, TProperty>(PropertyBuilder<T, TProperty> builder)
    {
        /// <summary>
        /// Configures the property to react to changes of the specified event type. This method adds a feature to the property metadata that indicates that the property's value should be updated whenever an event of the specified type is raised in the application. By calling this method with a specific event type, you can ensure that the property's value will be refreshed to reflect any changes that occur as a result of that event, allowing for dynamic updates and responsiveness in an application based on specific events that may affect the property's value.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to react to.</typeparam>
        /// <returns>The updated property builder.</returns>
        public PropertyBuilder<T, TProperty> ReactTo<TEvent>()
        {
            var feature = builder.Metadata.GetOrCreate<EventReactionFeature>();
            feature.Events.Add(typeof(TEvent));
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
        /// <param name="dependentProperties">The names of the properties that the current property depends on for validation.</param>
        public PropertyBuilder<T, TProperty> Validates(params string[] dependentProperties)
        {
            foreach (var dependentProperty in dependentProperties)
            {
                MetadataApplicators.ApplyAlsoValidate(builder.Metadata, dependentProperty);
            }

            return builder;
        }
    }

    extension(PropertyMetadata property)
    {
        /// <summary>
        /// Configures the property to react to changes of the specified event type. This method adds a feature to the property metadata that indicates that the property's value should be updated whenever an event of the specified type is raised in the application. By calling this method with a specific event type, you can ensure that the property's value will be refreshed to reflect any changes that occur as a result of that event, allowing for dynamic updates and responsiveness in an application based on specific events that may affect the property's value.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event to react to.</typeparam>
        /// <returns>The property metadata instance for chaining.</returns>
        public PropertyMetadata ReactTo<TEvent>()
        {
            var feature = property.GetOrCreate<EventReactionFeature>();
            feature.Events.Add(typeof(TEvent));

            return property;
        }

        /// <summary>
        /// Configures the property to update when the culture changes.
        /// </summary>
        public PropertyMetadata UpdateOnCultureChanged()
        {
            MetadataApplicators.ApplyUpdateOnCultureChanged(property);
            return property;
        }

        /// <summary>
        /// Configures the property to update when the time zone changes.
        /// </summary>
        public PropertyMetadata UpdateOnTimeZoneChanged()
        {
            MetadataApplicators.ApplyUpdateOnTimeZoneChanged(property);
            return property;
        }

        /// <summary>
        /// Configures the property to ignore modification tracking.
        /// </summary>
        public PropertyMetadata IgnoreModificationTracking()
        {
            MetadataApplicators.ApplyIgnoreModificationTracking(property);
            return property;
        }

        /// <summary>
        /// Configures the property to relay child property-changed notifications to the owner.
        /// </summary>
        public PropertyMetadata ForwardPropertyChanged(bool concatenatePropertyName = true)
        {
            MetadataApplicators.ApplyForwardProperty(property, concatenatePropertyName);
            return property;
        }

        /// <summary>
        /// Configures the property to validate when the specified dependent properties change.
        /// </summary>
        /// <param name="dependentProperties">The names of the properties that the current property depends on for validation.</param>
        public PropertyMetadata Validates(params string[] dependentProperties)
        {
            foreach (var dependentProperty in dependentProperties)
            {
                MetadataApplicators.ApplyAlsoValidate(property, dependentProperty);
            }

            return property;
        }
    }
}
