// -----------------------------------------------------------------------
// <copyright file="ExemptFromGeneratedMetadataAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Metadata;

/// <summary>
/// Excludes a type from <see cref="EnforceGeneratedMetadataAttribute"/> diagnostics when it derives from
/// <see cref="ObservableObject"/> but intentionally has no metadata attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ExemptFromGeneratedMetadataAttribute : Attribute;
