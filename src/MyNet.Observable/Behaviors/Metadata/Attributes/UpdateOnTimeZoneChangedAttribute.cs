// -----------------------------------------------------------------------
// <copyright file="UpdateOnTimeZoneChangedAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Behaviors.Metadata.Attributes;

/// <summary>
/// Indicates that the property should be updated when the time zone changes. This attribute is used to mark properties that need to be refreshed or updated when the application's time zone context changes, allowing dynamic updates to UI or other components that rely on time zone-specific information.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class UpdateOnTimeZoneChangedAttribute : Attribute;
