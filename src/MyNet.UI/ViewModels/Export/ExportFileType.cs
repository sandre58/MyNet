// -----------------------------------------------------------------------
// <copyright file="ExportFileType.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyNet.UI.ViewModels.Export;

/// <summary>
/// Represents export file constraints and dialog filter information.
/// </summary>
public sealed class ExportFileType
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExportFileType"/> class.
    /// </summary>
    /// <param name="defaultExtension">The default extension used for generated file names (for example ".csv").</param>
    /// <param name="dialogFilter">The dialog filter string.</param>
    /// <param name="allowedExtensions">Optional allowed extensions used for destination validation.</param>
    public ExportFileType(string defaultExtension, string dialogFilter, IEnumerable<string>? allowedExtensions = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(defaultExtension);
        ArgumentException.ThrowIfNullOrWhiteSpace(dialogFilter);

        DefaultExtension = NormalizeExtension(defaultExtension);
        DialogFilter = dialogFilter;
        AllowedExtensions = [.. (allowedExtensions ?? [defaultExtension]).Select(NormalizeExtension).Distinct(StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>
    /// Gets the default extension.
    /// </summary>
    public string DefaultExtension { get; }

    /// <summary>
    /// Gets the dialog filter string.
    /// </summary>
    public string DialogFilter { get; }

    /// <summary>
    /// Gets allowed extensions.
    /// </summary>
    public IReadOnlyCollection<string> AllowedExtensions { get; }

    /// <summary>
    /// Determines whether the specified file path has an allowed extension.
    /// </summary>
    /// <param name="path">The file path to validate.</param>
    /// <returns><see langword="true"/> when the extension is allowed; otherwise <see langword="false"/>.</returns>
    public bool IsValidPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        var extension = NormalizeExtension(Path.GetExtension(path));
        return AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    private static string NormalizeExtension(string extension) => string.IsNullOrWhiteSpace(extension) ? string.Empty : extension.StartsWith('.') ? extension : $".{extension}";
}
