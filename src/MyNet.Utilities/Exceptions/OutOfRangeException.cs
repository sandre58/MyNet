// -----------------------------------------------------------------------
// <copyright file="OutOfRangeException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Exceptions;

/// <summary>
/// Exception thrown when a value is outside the allowed range.
/// </summary>
public class OutOfRangeException : TranslatableException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OutOfRangeException"/> class with a default message.
    /// </summary>
    public OutOfRangeException()
        : base("the specified value is out of range.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OutOfRangeException"/> class with a specified message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public OutOfRangeException(string message, System.Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OutOfRangeException"/> class with a specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public OutOfRangeException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OutOfRangeException"/> class indicating the specified property value is outside the given range.
    /// </summary>
    /// <param name="property">The property name.</param>
    /// <param name="min">Minimum allowed value.</param>
    /// <param name="max">Maximum allowed value.</param>
    public OutOfRangeException(string property, object min, object max)
        : base("the field '{0}' is out of range.", "FieldXMustBeBetweenYAndZError", property, min, max) { }

    public OutOfRangeException(string? message, System.Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(message, innerException, resourceKey, stringFormatParameters)
    {
    }

    public OutOfRangeException(System.Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(innerException, resourceKey, stringFormatParameters)
    {
    }

    public OutOfRangeException(string? message, string resourceKey, params object?[] stringFormatParameters)
        : base(message, resourceKey, stringFormatParameters)
    {
    }

    public OutOfRangeException(string resourceKey, params object?[] stringFormatParameters)
        : base(resourceKey, stringFormatParameters)
    {
    }
}
