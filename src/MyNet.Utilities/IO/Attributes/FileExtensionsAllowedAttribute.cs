// -----------------------------------------------------------------------
// <copyright file="FileExtensionsAllowedAttribute.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using MyNet.Utilities.IO.FileExtensions;
using MyNet.Utilities.Localization;

namespace MyNet.Utilities.IO.Attributes;

/// <summary>
/// Validation attribute that restricts a file path property to a specific set of allowed file extensions.
/// Extensions are normalized (case-insensitive, leading dot always added) so that both <c>txt</c> and <c>.txt</c>
/// are treated identically.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class FileExtensionsAllowedAttribute : ValidationAttribute
{
    private readonly HashSet<string> _extensions;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileExtensionsAllowedAttribute"/> class with the specified allowed file extensions. The extensions are normalized to ensure consistent validation, and the error message is set to a resource string that indicates the field must contain one of the allowed extensions. The constructor throws exceptions if the provided extensions array is null or empty, ensuring that at least one valid extension is specified for validation.
    /// </summary>
    /// <param name="extensions">The allowed file extensions.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="extensions"/> array is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="extensions"/> array is empty.</exception>
    public FileExtensionsAllowedAttribute(params string[] extensions)
    {
        ArgumentNullException.ThrowIfNull(extensions);

        _extensions = extensions
            .Select(Normalize)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (_extensions.Count == 0)
            throw new ArgumentException("At least one extension must be provided.", nameof(extensions));

        ErrorMessageResourceName = nameof(InternalResources.FieldXMustContainsAllowedExtensionsYError);
        ErrorMessageResourceType = typeof(InternalResources);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileExtensionsAllowedAttribute"/> class with the specified allowed file extensions. This constructor accepts an array of <see cref="FileExtension"/> objects, which are then normalized and stored in a hash set for validation. The error message is set to a resource string that indicates the field must contain one of the allowed extensions. The constructor ensures that at least one valid extension is specified for validation by throwing an exception if the provided array is null or empty.
    /// </summary>
    /// <param name="extensions">The allowed file extensions.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="extensions"/> array is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="extensions"/> array is empty.</exception>
    public FileExtensionsAllowedAttribute(params FileExtension[] extensions)
        : this(extensions.Select(x => x.Value).ToArray())
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether null or empty file paths are considered valid. If set to <c>true</c>, null or empty values will pass validation; if set to <c>false</c>, they will fail validation. The default value is <c>true</c>, allowing for optional file paths. This property provides flexibility in validation scenarios where a file path may not be required, while still enforcing the allowed extensions when a value is provided.
    /// </summary>
    public bool AllowEmpty { get; set; } = true;

    /// <summary>
    /// Gets the collection of allowed file extensions that are used for validation. The extensions are stored in a hash set for efficient lookup and are normalized to ensure consistent validation. This property provides access to the set of allowed extensions, which can be used for reference or in error messages to indicate which extensions are valid for the associated file path property.
    /// </summary>
    public IReadOnlyCollection<string> Extensions => _extensions;

    /// <summary>
    /// Formats the error message to include the field name and the list of allowed extensions. The error message is constructed using the current culture and the resource string defined in the constructor, which indicates that the field must contain one of the allowed extensions. The list of allowed extensions is joined into a single string with a pipe character ("|") as a separator, providing a clear indication of which extensions are valid for the associated file path property.
    /// </summary>
    /// <param name="name">The name of the field being validated.</param>
    /// <returns>The formatted error message.</returns>
    public override string FormatErrorMessage(string name)
        => string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, string.Join(" | ", _extensions));

    /// <summary>
    /// Determines whether the specified value is valid based on the allowed file extensions. The method checks if the value is null or empty (if allowed), and if it is a string, it extracts the file extension and checks if it is in the set of allowed extensions. The validation is case-insensitive and treats extensions with or without a leading dot as equivalent. If the value is not a string or if the extension is not allowed, the method returns <c>false</c>, indicating that the value is invalid.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <returns><c>true</c> if the value is valid; otherwise, <c>false</c>.</returns>
    public override bool IsValid(object? value)
    {
        if (value is null)
            return AllowEmpty;

        if (value is not string path)
            return false;

        if (string.IsNullOrWhiteSpace(path))
            return AllowEmpty;

        var extension = Normalize(Path.GetExtension(path));

        return _extensions.Contains(extension);
    }

    /// <summary>
    /// Normalizes a file extension by trimming whitespace, converting it to lowercase, and ensuring it starts with a dot. This method is used internally to maintain a consistent format for file extensions, making it easier to compare and validate them against the set of allowed extensions. If the input is null or consists only of whitespace, an empty string is returned. If the input is a wildcard ("*"), it is returned as-is to allow for any extension. Otherwise, the method ensures that the extension starts with a dot and is in lowercase for consistent validation.
    /// </summary>
    /// <param name="extension">The file extension to normalize.</param>
    /// <returns>The normalized file extension.</returns>
    private static string Normalize(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return string.Empty;

        extension = extension.Trim().ToLowerCase();

        if (extension == "*")
            return "*";

        if (!extension.StartsWith('.'))
            extension = "." + extension;

        return extension;
    }
}
