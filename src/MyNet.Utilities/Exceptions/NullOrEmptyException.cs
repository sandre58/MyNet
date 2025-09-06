// -----------------------------------------------------------------------
// <copyright file="NullOrEmptyException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Exceptions;

/// <summary>
/// Exception thrown when a required field is null or empty.
/// </summary>
public class NullOrEmptyException : TranslatableException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrEmptyException"/> class.
    /// </summary>
    public NullOrEmptyException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrEmptyException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public NullOrEmptyException(string message, System.Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrEmptyException"/> class indicating the specified property is required.
    /// </summary>
    /// <param name="property">The name of the required property.</param>
    public NullOrEmptyException(string property)
        : base("Field {0} is required.", "FieldXIsRequiredError", property) { }

    public NullOrEmptyException(string? message, System.Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(message, innerException, resourceKey, stringFormatParameters)
    {
    }

    public NullOrEmptyException(System.Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(innerException, resourceKey, stringFormatParameters)
    {
    }

    public NullOrEmptyException(string? message, string resourceKey, params object?[] stringFormatParameters)
        : base(message, resourceKey, stringFormatParameters)
    {
    }

    public NullOrEmptyException(string resourceKey, params object?[] stringFormatParameters)
        : base(resourceKey, stringFormatParameters)
    {
    }
}
