// -----------------------------------------------------------------------
// <copyright file="UpdateOnCultureChangedAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Behaviors.Metadata.Attributes;

/// <summary>
/// Indicates that a property should be updated when the culture changes. This attribute can be applied to properties of an ObservableObject to specify that they should be automatically updated when the culture changes, allowing for dynamic updates of culture-sensitive properties in an application. When a property is decorated with this attribute, it will be included in the list of properties that are notified when the culture changes, enabling seamless localization and internationalization support for properties in an application that relies on culture-specific information.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class UpdateOnCultureChangedAttribute : Attribute;
