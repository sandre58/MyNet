// -----------------------------------------------------------------------
// <copyright file="FileExtensionFilter.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNet.Utilities.IO.FileExtensions;

/// <summary>
/// Represents a file extension filter, which is a collection of file extension groups. Each group has a key and a list of extensions. The filter can be used to generate file filters for file dialogs, where each group represents a category of files (e.g., "Image Files", "Excel Files") and the extensions represent the specific file types that belong to that category. The filter can also provide a combined list of all extensions across all groups, which can be used for an "All Files" filter in a file dialog.
/// </summary>
/// <param name="groups">The collection of file extension groups to be included in the filter.</param>
public sealed class FileExtensionFilter(IEnumerable<FileExtensionGroup> groups)
{
    /// <summary>
    /// Gets the collection of file extension groups included in the filter. Each group represents a category of files and contains a key (which can be used for localization) and a list of file extensions that belong to that category. This property allows access to the individual groups and their associated extensions, which can be used to generate specific filters for file dialogs based on the categories defined in the filter.
    /// </summary>
    public IReadOnlyList<FileExtensionGroup> Groups { get; } = [.. groups];

    /// <summary>
    /// Gets a combined list of all file extensions across all groups in the filter. This property flattens the list of extensions from each group and returns a distinct set of extensions that can be used for an "All Files" filter in a file dialog. The extensions are returned as a collection of strings, where each string represents a file extension (e.g., ".jpg", ".xlsx").
    /// </summary>
    public IEnumerable<string> AllExtensions => Groups.SelectMany(g => g.Extensions)
        .Select(e => e.Value)
        .Distinct();

    /// <summary>
    /// Generates a filter string that can be used in file dialogs to filter files based on their extensions. The filter string is constructed by joining the groups and their extensions in a specific format. Each group is represented as "Group Name (ext1;ext2;...)", where "Group Name" is obtained by localizing the group's key (if a localization function is provided) and "ext1;ext2;..." is a semicolon-separated list of the group's extensions. The groups are then joined together with a pipe character ("|") to create the final filter string that can be used in file dialogs.
    /// </summary>
    /// <param name="localize">A function to localize the group names. If null, the group keys are used as-is.</param>
    /// <returns>A filter string that can be used in file dialogs.</returns>
    public string ToFilterString(Func<string, string>? localize = null)
        => string.Join("|",
            Groups.Select(g =>
            {
                var name = localize?.Invoke(g.Key) ?? g.Key;
                var exts = string.Join(";", g.Extensions.Select(e => $"*{e.Value}"));
                return $"{name} ({exts})";
            }));
}
