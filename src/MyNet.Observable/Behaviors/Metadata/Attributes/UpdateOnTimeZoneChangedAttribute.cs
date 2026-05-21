// -----------------------------------------------------------------------
// <copyright file="UpdateOnTimeZoneChangedAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Behaviors.Metadata.Attributes;

/// <summary>
/// Indicates that the property should be updated when the culture changes. This attribute is used to mark properties that need to be refreshed or updated when the culture of the application changes, allowing for dynamic updates to the user interface or other components that rely on culture-specific information. By applying this attribute to a property, developers can ensure that the property will be automatically updated in response to culture changes, improving the responsiveness and adaptability of the application to different cultural contexts.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class UpdateOnTimeZoneChangedAttribute : Attribute;
