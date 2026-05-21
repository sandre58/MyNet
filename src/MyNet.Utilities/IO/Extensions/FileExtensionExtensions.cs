// -----------------------------------------------------------------------
// <copyright file="FileExtensionExtensions.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyNet.Utilities.IO.FileExtensions;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MyNet.Utilities;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class FileExtensionExtensions
{
    extension(FileExtensionGroup group)
    {
        /// <summary>
        /// Converts the <see cref="FileExtensionGroup"/> to a <see cref="FileFilter"/> that can be used in file dialogs. The method generates a filter pattern based on the extensions in the group and creates a title for the filter using the group's key, optionally translating it using the provided translation function. The resulting <see cref="FileFilter"/> contains both the title and the pattern, which can be used to filter files in file dialogs based on their extensions.
        /// </summary>
        /// <param name="translate">A function to translate the group's key.</param>
        /// <returns>A <see cref="FileFilter"/> representing the group's extensions.</returns>
        public FileFilter ToFilter(Func<string, string?>? translate = null)
        {
            var pattern = string.Join(";", group.Extensions.Select(e => $"*{e.Value}"));

            var title = translate?.Invoke(group.Key) ?? group.Key.OrEmpty();

            if (!string.IsNullOrWhiteSpace(pattern) && pattern != "*.*")
                title = $"{title} ({pattern})";

            return new(title, pattern);
        }

        /// <summary>
        /// Determines whether the specified file path has a valid extension based on the extensions defined in the <see cref="FileExtensionGroup"/>. The method checks if the file path is not null or whitespace, and then compares the file's extension against the list of extensions in the group, ignoring case sensitivity. If the file's extension matches any of the extensions in the group, the method returns true; otherwise, it returns false.
        /// </summary>
        /// <param name="filePath">The file path to check.</param>
        /// <returns>True if the file has a valid extension; otherwise, false.</returns>
        public bool IsValidFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;

            var ext = Path.GetExtension(filePath);
            return group.Extensions.Any(e =>
                string.Equals(e.Value, ext, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Concatenates the extensions of the current <see cref="FileExtensionGroup"/> with those of another <see cref="FileExtensionGroup"/>. The method creates a new instance of <see cref="FileExtensionGroup"/> that combines the extensions from both groups. The key for the new group can be specified as an optional parameter; if not provided, it defaults to the key of the current group. This allows you to create a new group that represents a combination of file extensions from two different groups, which can be useful for creating more comprehensive filters for file dialogs.
        /// </summary>
        /// <param name="second">The second <see cref="FileExtensionGroup"/> to concatenate with the current group.</param>
        /// <param name="key">The key for the new <see cref="FileExtensionGroup"/>. If not provided, the key of the current group is used.</param>
        /// <returns>A new <see cref="FileExtensionGroup"/> that combines the extensions from both groups.</returns>
        public FileExtensionGroup Concat(
            FileExtensionGroup second,
            string? key = null)
            => new(key ?? group.Key, group.Extensions.Concat(second.Extensions));
    }

    extension(IEnumerable<FileExtensionGroup> groups)
    {
        /// <summary>
        /// Converts a collection of <see cref="FileExtensionGroup"/> items to a filter string that can be used in file dialogs. The method generates a filter string by converting each group to a <see cref="FileFilter"/> and joining the titles and patterns together in the format expected by file dialogs. Each group's title is optionally translated using the provided translation function, and the resulting filter string can be used to filter files based on their extensions in file dialogs.
        /// </summary>
        /// <param name="translate">A function to translate the group's key.</param>
        /// <returns>A filter string that can be used in file dialogs.</returns>
        public string ToFilterString(Func<string, string?>? translate = null)
            => string.Join("|",
                groups.SelectMany(g =>
                {
                    var (title, pattern) = g.ToFilter(translate);
                    return new[] { title, pattern };
                }));
    }

    extension(IEnumerable<string> files)
    {
        /// <summary>
        /// Filters a collection of file paths based on the extensions defined in a <see cref="FileExtensionGroup"/>. The method creates a hash set of allowed extensions from the group for efficient lookup, and then filters the input file paths by checking if their extensions match any of the allowed extensions in the group, ignoring case sensitivity. The resulting collection contains only the file paths that have valid extensions according to the specified group.
        /// </summary>
        /// <param name="group">The file extension group to filter by.</param>
        /// <returns>A collection of file paths that have valid extensions according to the specified group.</returns>
        public IEnumerable<string> FilterByExtensions(FileExtensionGroup group)
        {
            var allowed = group.Extensions
                .Select(x => x.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return files.Where(file =>
            {
                var ext = Path.GetExtension(file);
                return allowed.Contains(ext);
            });
        }
    }
}

/// <summary>
/// Represents a file filter that can be used in file dialogs. The <see cref="FileFilter"/> record contains a title and a pattern, where the title is a user-friendly name for the filter (e.g., "Image Files") and the pattern is a string that defines the file extensions associated with the filter (e.g., "*.jpg;*.png"). This record can be used to create filters for file dialogs, allowing users to easily select files based on their extensions.
/// </summary>
/// <param name="Title">The title of the file filter.</param>
/// <param name="Pattern">The pattern of the file filter.</param>
public sealed record FileFilter(string Title, string Pattern);
