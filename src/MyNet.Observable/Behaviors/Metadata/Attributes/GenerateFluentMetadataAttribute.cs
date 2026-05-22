// -----------------------------------------------------------------------
// <copyright file="GenerateFluentMetadataAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Behaviors.Metadata.Attributes;

/// <summary>
/// Indicates that metadata should be generated for a type via both the immutable provider and fluent API configuration (MetadataRegistry.For&lt;T&gt;()).
/// When applied to a class, the metadata provider generator will emit both a metadata provider and fluent configuration code.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class GenerateFluentMetadataAttribute : Attribute;
