// -----------------------------------------------------------------------
// <copyright file="FileExtension.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;

namespace MyNet.IO.FileExtensions;

/// <summary>
/// Represents a file extension, ensuring it is normalized (lowercase and starts with a dot).
/// </summary>
public sealed class FileExtension
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileExtension"/> class with the specified extension value. The constructor normalizes the extension by trimming whitespace, converting it to lowercase, and ensuring it starts with a dot. If the provided value is null, empty, or consists only of whitespace, an <see cref="ArgumentException"/> is thrown.
    /// </summary>
    /// <param name="value">The file extension value.</param>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="value"/> is null, empty, or consists only of whitespace.</exception>
    public FileExtension(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Extension cannot be null or empty.", nameof(value));

        Value = Normalize(value);
    }

    /// <summary>
    /// Gets the normalized file extension value. The value is stored in lowercase and starts with a dot, ensuring consistency when comparing or using file extensions.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Normalizes the file extension by trimming whitespace, converting it to lowercase, and ensuring it starts with a dot. This method is used internally to maintain a consistent format for file extensions, making it easier to compare and use them in file operations.
    /// </summary>
    /// <param name="extension">The file extension to normalize.</param>
    /// <returns>The normalized file extension.</returns>
    private static string Normalize(string extension)
    {
        extension = extension.Trim().ToLower(CultureInfo.CurrentCulture);
        return extension.StartsWith('.') ? extension : $".{extension}";
    }

    /// <summary>
    /// Returns a string representation of the file extension, which is the normalized value. This method overrides the default ToString implementation to provide a meaningful representation of the file extension when it is used in contexts such as logging or displaying information to the user.
    /// </summary>
    /// <returns>The normalized file extension value.</returns>
    public override string ToString() => Value;

    /// <summary>
    /// Determines whether the specified object is equal to the current file extension. Two <see cref="FileExtension"/> instances are considered equal if their normalized values are the same. This method overrides the default Equals implementation to provide a meaningful equality comparison based on the normalized file extension value.
    /// </summary>
    /// <param name="obj">The object to compare with the current file extension.</param>
    /// <returns><c>true</c> if the specified object is equal to the current file extension; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => obj is FileExtension other && Value == other.Value;

    /// <summary>
    /// Returns a hash code for the file extension, which is based on the normalized value. This method overrides the default GetHashCode implementation to ensure that the hash code is consistent with the equality comparison defined in the Equals method, allowing <see cref="FileExtension"/> instances to be used effectively in hash-based collections such as dictionaries or hash sets.
    /// </summary>
    /// <returns>A hash code for the current file extension.</returns>
    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
}
