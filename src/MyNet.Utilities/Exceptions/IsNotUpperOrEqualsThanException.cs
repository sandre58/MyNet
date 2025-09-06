// -----------------------------------------------------------------------
// <copyright file="IsNotUpperOrEqualsThanException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Exceptions;

/// <summary>
/// Exception thrown when a value is expected to be upper or equal than a target but is not.
/// </summary>
public class IsNotUpperOrEqualsThanException : TranslatableException
{
    public IsNotUpperOrEqualsThanException() { }

    public IsNotUpperOrEqualsThanException(string message, System.Exception innerException)
        : base(message, innerException) { }

    public IsNotUpperOrEqualsThanException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IsNotUpperOrEqualsThanException"/> class indicating the specified property must be upper than the provided target.
    /// </summary>
    /// <param name="property">The property name.</param>
    /// <param name="target">The target value the property must be greater than or equal to.</param>
    public IsNotUpperOrEqualsThanException(string property, object? target)
        : base($"the value of '{property}' must be upper than {target}.", "FieldXMustBeUpperOrEqualsThanYError", property, target) { }

    public IsNotUpperOrEqualsThanException(string? message, System.Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(message, innerException, resourceKey, stringFormatParameters)
    {
    }

    public IsNotUpperOrEqualsThanException(System.Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(innerException, resourceKey, stringFormatParameters)
    {
    }

    public IsNotUpperOrEqualsThanException(string? message, string resourceKey, params object?[] stringFormatParameters)
        : base(message, resourceKey, stringFormatParameters)
    {
    }

    public IsNotUpperOrEqualsThanException(string resourceKey, params object?[] stringFormatParameters)
        : base(resourceKey, stringFormatParameters)
    {
    }
}
