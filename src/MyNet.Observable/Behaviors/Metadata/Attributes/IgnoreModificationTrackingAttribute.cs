// -----------------------------------------------------------------------
// <copyright file="IgnoreModificationTrackingAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Behaviors.Metadata.Attributes;

/// <summary>
/// Indicates that a property, class, or interface should be excluded from modification tracking.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class IgnoreModificationTrackingAttribute : Attribute;
