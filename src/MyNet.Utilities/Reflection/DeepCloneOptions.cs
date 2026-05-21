// -----------------------------------------------------------------------
// <copyright file="DeepCloneOptions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Reflection;

/// <summary>
/// Options for deep cloning operations, allowing customization of cloning behavior such as reference preservation.
/// </summary>
public sealed class DeepCloneOptions
{
    /// <summary>
    /// Gets a value indicating whether to preserve object references during the cloning process.
    /// </summary>
    public bool PreserveReferences { get; init; } = true;
}
