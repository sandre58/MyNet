// -----------------------------------------------------------------------
// <copyright file="NamingConventionHelpers.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.UI.Locators.Conventions;

/// <summary>
/// Provides helper methods for naming conventions used in view and view model resolution, such as removing common suffixes and manipulating namespaces.
/// </summary>
internal static class NamingConventionHelpers
{
    /// <summary>
    /// Gets the list of recognized view suffixes used for type resolution.
    /// </summary>
    internal static readonly string[] ViewSuffixes =
    [
        "View", "Control", "UserControl", "Page", "Activity", "Window", "Fragment"
    ];

    private static readonly string[] ViewModelSuffixes =
    [
        "ViewModel"
    ];

    /// <summary>
    /// Removes any of the specified suffixes from the end of the given name, if present. Comparison is case-insensitive.
    /// </summary>
    /// <param name="name">The name from which to remove the suffix.</param>
    /// <param name="suffixes">An array of suffixes to remove.</param>
    /// <returns>The name without the suffix if a match was found; otherwise, the original name.</returns>
    public static string RemoveSuffix(string name, string[] suffixes)
    {
        foreach (var suffix in suffixes)
        {
            if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                return name[..^suffix.Length];
        }

        return name;
    }

    /// <summary>
    /// Removes common view model suffixes from the given type name to derive the base name for view resolution. This method is typically used to convert a view model type name into a corresponding view type name by stripping off known suffixes like "ViewModel". The resulting base name can then be used to locate the appropriate view based on naming conventions.
    /// </summary>
    /// <param name="type">The type from which to derive the base name.</param>
    /// <returns>The base name without the view model suffix.</returns>
    public static string GetBaseNameFromViewModel(Type type) => RemoveSuffix(type.Name, ViewModelSuffixes);

    /// <summary>
    /// Removes common view suffixes from the given type name to derive the base name for view model resolution. This method is typically used to convert a view type name into a corresponding view model type name by stripping off known suffixes like "View", "Control", "UserControl", "Page", "Activity", "Window", or "Fragment". The resulting base name can then be used to locate the appropriate view model based on naming conventions.
    /// </summary>
    /// <param name="type">The type from which to derive the base name.</param>
    /// <returns>The base name without the view suffix.</returns>
    public static string GetBaseNameFromView(Type type) => RemoveSuffix(type.Name, ViewSuffixes);

    /// <summary>
    /// Determines whether the specified type is a view type based on its name ending with one of the recognized view suffixes.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><see langword="true"/> if the type is a view type; otherwise, <see langword="false"/>.</returns>
    public static bool IsViewType(Type type) => GetBaseNameFromView(type) != type.Name;

    /// <summary>
    /// Replaces a segment of the namespace with a new value.
    /// </summary>
    /// <param name="ns">The original namespace.</param>
    /// <param name="from">The segment to replace.</param>
    /// <param name="to">The new segment.</param>
    /// <returns>The modified namespace if the original namespace is not null; otherwise, null.</returns>
    public static string? ReplaceNamespaceSegment(string? ns, string from, string to) => ns?.Replace(from, to, StringComparison.Ordinal);

    /// <summary>
    /// Gets the parent namespace of the given namespace.
    /// </summary>
    /// <param name="ns">The namespace.</param>
    /// <returns>The parent namespace if it exists; otherwise, the original namespace.</returns>
    public static string? GetParentNamespace(string? ns)
    {
        if (string.IsNullOrWhiteSpace(ns))
            return ns;

        var lastDot = ns.LastIndexOf('.');
        return lastDot > 0 ? ns[..lastDot] : ns;
    }
}
