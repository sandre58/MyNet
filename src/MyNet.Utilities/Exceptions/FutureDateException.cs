// -----------------------------------------------------------------------
// <copyright file="FutureDateException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Exceptions;

/// <summary>
/// Exception thrown when a date must be in the past but a future date was provided.
/// </summary>
[Serializable]
public class FutureDateException : TranslatableException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FutureDateException"/> class.
    /// </summary>
    public FutureDateException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FutureDateException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public FutureDateException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FutureDateException"/> class indicating the specified property must be in the past.
    /// </summary>
    /// <param name="property">The name of the property that has the invalid date.</param>
    public FutureDateException(string property)
        : base("The field {0} must be in past.", "FieldXMustBeInPastError", property) { }

    public FutureDateException(string? message, Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(message, innerException, resourceKey, stringFormatParameters)
    {
    }

    public FutureDateException(Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(innerException, resourceKey, stringFormatParameters)
    {
    }

    public FutureDateException(string? message, string resourceKey, params object?[] stringFormatParameters)
        : base(message, resourceKey, stringFormatParameters)
    {
    }

    public FutureDateException(string resourceKey, params object?[] stringFormatParameters)
        : base(resourceKey, stringFormatParameters)
    {
    }
}
