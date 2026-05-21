// -----------------------------------------------------------------------
// <copyright file="Slugifier.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Text.Slugification;

/// <summary>
/// Provides common slugifier transforms.
/// </summary>
public static class Slugifier
{
    /// <summary>
    /// Gets the default slugifier transform.
    /// </summary>
    public static ITextSlugifierTransform Default { get; } = new SlugifyTransform(new());

    /// <summary>
    /// Gets a kebab-case slugifier (lowercase, '-' separator).
    /// </summary>
    public static ITextSlugifierTransform KebabCase { get; } = Default;

    /// <summary>
    /// Gets a snake_case slugifier (lowercase, '_' separator).
    /// </summary>
    public static ITextSlugifierTransform SnakeCase { get; } = new SlugifyTransform(new() { Separator = '_' });

    /// <summary>
    /// Gets a slugifier preserving original case.
    /// </summary>
    public static ITextSlugifierTransform PreserveCase { get; } = new SlugifyTransform(new() { Lowercase = false });
}
