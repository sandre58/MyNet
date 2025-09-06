// -----------------------------------------------------------------------
// <copyright file="InvalidEmailAddressException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Exceptions;

/// <summary>
/// Exception thrown when a field value is not a valid email address.
/// </summary>
[Serializable]
public class InvalidEmailAddressException : TranslatableException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEmailAddressException"/> class.
    /// </summary>
    public InvalidEmailAddressException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEmailAddressException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidEmailAddressException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidEmailAddressException"/> class indicating the specified property must be a valid email address.
    /// </summary>
    /// <param name="propertyName">The name of the invalid property.</param>
    public InvalidEmailAddressException(string propertyName)
        : base("Field {0} must be a valid email address.", "FieldXMustBeValidEmailAddressError", propertyName) { }

    public InvalidEmailAddressException(string? message, Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(message, innerException, resourceKey, stringFormatParameters)
    {
    }

    public InvalidEmailAddressException(Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(innerException, resourceKey, stringFormatParameters)
    {
    }

    public InvalidEmailAddressException(string? message, string resourceKey, params object?[] stringFormatParameters)
        : base(message, resourceKey, stringFormatParameters)
    {
    }

    public InvalidEmailAddressException(string resourceKey, params object?[] stringFormatParameters)
        : base(resourceKey, stringFormatParameters)
    {
    }
}
