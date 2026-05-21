// -----------------------------------------------------------------------
// <copyright file="ModificationTrackingFeature.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Observable.Behaviors.Metadata.Features;

/// <summary>
/// Defines a feature that can be associated with a property to indicate whether modifications to the property should be ignored for tracking purposes. This feature can be used in scenarios where you want to exclude certain properties from being tracked for changes, allowing for more efficient change tracking and reducing unnecessary overhead when monitoring modifications to properties in an application. By setting the Ignore property to true, you can specify that changes to the associated property should not be tracked, which can be useful in cases where the property is not relevant for change tracking or when you want to optimize performance by excluding certain properties from tracking in an application.
/// </summary>
public sealed class ModificationTrackingFeature
{
    /// <summary>
    /// Gets or sets a value indicating whether modifications to the associated property should be ignored for tracking purposes. When set to true, changes to the property will not be tracked, allowing for more efficient change tracking and reducing unnecessary overhead when monitoring modifications to properties in an application. This can be useful in scenarios where the property is not relevant for change tracking or when you want to optimize performance by excluding certain properties from tracking in an application.
    /// </summary>
    public bool Ignore { get; set; }
}
