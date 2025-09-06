// -----------------------------------------------------------------------
// <copyright file="IsNotLowerOrEqualsThanException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Exceptions;

/// <summary>
/// Exception thrown when a value is expected to be lower or equal than a target but is not.
/// </summary>
public class IsNotLowerOrEqualsThanException : TranslatableException
{
    public IsNotLowerOrEqualsThanException() { }

    public IsNotLowerOrEqualsThanException(string message, Exception innerException)
        : base(message, innerException) { }

    public IsNotLowerOrEqualsThanException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IsNotLowerOrEqualsThanException"/> class indicating the specified property must be lower or equal than the provided target.
    /// </summary>
    /// <param name="property">The property name.</param>
    /// <param name="target">The target value the property must be lower or equal to.</param>
    public IsNotLowerOrEqualsThanException(string property, object? target)
        : base("the value of {0} must be lower than {1}.", "FieldXMustBeLowerOrEqualsThanYError", property, target is DateTime date1 ? date1.ToLocalTime() : target) { }

    public IsNotLowerOrEqualsThanException(string? message, Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(message, innerException, resourceKey, stringFormatParameters)
    {
    }

    public IsNotLowerOrEqualsThanException(Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(innerException, resourceKey, stringFormatParameters)
    {
    }

    public IsNotLowerOrEqualsThanException(string? message, string resourceKey, params object?[] stringFormatParameters)
        : base(message, resourceKey, stringFormatParameters)
    {
    }

    public IsNotLowerOrEqualsThanException(string resourceKey, params object?[] stringFormatParameters)
        : base(resourceKey, stringFormatParameters)
    {
    }
}
