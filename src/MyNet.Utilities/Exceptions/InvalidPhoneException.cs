// -----------------------------------------------------------------------
// <copyright file="InvalidPhoneException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Exceptions;

/// <summary>
/// Exception thrown when a field value is not a valid phone number.
/// </summary>
[Serializable]
public class InvalidPhoneException : TranslatableException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPhoneException"/> class.
    /// </summary>
    public InvalidPhoneException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPhoneException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidPhoneException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPhoneException"/> class indicating the specified property must be a valid phone number.
    /// </summary>
    /// <param name="propertyName">The name of the invalid property.</param>
    public InvalidPhoneException(string propertyName)
        : base("Field {0} must be a valid phone number.", "FieldXMustBeValidPhoneNumberError", propertyName) { }

    public InvalidPhoneException(string? message, Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(message, innerException, resourceKey, stringFormatParameters)
    {
    }

    public InvalidPhoneException(Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(innerException, resourceKey, stringFormatParameters)
    {
    }

    public InvalidPhoneException(string? message, string resourceKey, params object?[] stringFormatParameters)
        : base(message, resourceKey, stringFormatParameters)
    {
    }

    public InvalidPhoneException(string resourceKey, params object?[] stringFormatParameters)
        : base(resourceKey, stringFormatParameters)
    {
    }
}
