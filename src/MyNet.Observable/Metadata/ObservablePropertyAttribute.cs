// -----------------------------------------------------------------------
// <copyright file="ObservablePropertyAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Observable.Metadata;

/// <summary>
/// Marks a backing field for source generation of an observable property that uses <see cref="ObservableObject.SetProperty{T}"/>.
/// The containing type must be <c>partial</c> and derive from <see cref="ObservableObject"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class ObservablePropertyAttribute : Attribute;
