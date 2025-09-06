// -----------------------------------------------------------------------
// <copyright file="NotEnoughDiskSpaceException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Exceptions;

/// <summary>
/// Exception thrown when there is not enough disk space to complete an operation.
/// </summary>
[Serializable]
public class NotEnoughDiskSpaceException : TranslatableException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotEnoughDiskSpaceException"/> class with a default message.
    /// </summary>
    public NotEnoughDiskSpaceException()
        : base("Not enough disk space.", "NotEnoughSpaceDisk") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotEnoughDiskSpaceException"/> class with a specified message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public NotEnoughDiskSpaceException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotEnoughDiskSpaceException"/> class with a specified message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public NotEnoughDiskSpaceException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotEnoughDiskSpaceException"/> class with a specified message, inner exception, resource key, and format parameters.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="resourceKey">The resource key for the error message.</param>
    /// <param name="stringFormatParameters">The format parameters for the error message.</param>
    public NotEnoughDiskSpaceException(string? message, Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(message, innerException, resourceKey, stringFormatParameters)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotEnoughDiskSpaceException"/> class with a specified inner exception, resource key, and format parameters.
    /// </summary>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="resourceKey">The resource key for the error message.</param>
    /// <param name="stringFormatParameters">The format parameters for the error message.</param>
    public NotEnoughDiskSpaceException(Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(innerException, resourceKey, stringFormatParameters)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotEnoughDiskSpaceException"/> class with a specified message, resource key, and format parameters.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="resourceKey">The resource key for the error message.</param>
    /// <param name="stringFormatParameters">The format parameters for the error message.</param>
    public NotEnoughDiskSpaceException(string? message, string resourceKey, params object?[] stringFormatParameters)
        : base(message, resourceKey, stringFormatParameters)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotEnoughDiskSpaceException"/> class with a specified resource key and format parameters.
    /// </summary>
    /// <param name="resourceKey">The resource key for the error message.</param>
    /// <param name="stringFormatParameters">The format parameters for the error message.</param>
    public NotEnoughDiskSpaceException(string resourceKey, params object?[] stringFormatParameters)
        : base(resourceKey, stringFormatParameters)
    {
    }
}
