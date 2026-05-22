// -----------------------------------------------------------------------
// <copyright file="EnforceGeneratedMetadataAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Behaviors.Metadata.Attributes;

/// <summary>
/// Indicates that metadata verification should be strict. When applied to an assembly,
/// the metadata provider generator will emit compile-time diagnostics (errors) when an
/// ObservableObject type is found but does not have a generated metadata provider.
/// This enables fail-fast detection of missing metadata configurations during compilation.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class EnforceGeneratedMetadataAttribute : Attribute;
