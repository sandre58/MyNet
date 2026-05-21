// -----------------------------------------------------------------------
// <copyright file="IsSingletonAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Attributes;

/// <summary>
/// Indicates that a class should be registered as a singleton in the dependency injection container.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class IsSingletonAttribute : Attribute;
