// -----------------------------------------------------------------------
// <copyright file="TranslatableException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;

namespace MyNet.Utilities.Exceptions;

/// <summary>
/// Represents an exception that contains a resource key and format parameters intended for localization.
/// </summary>
/// <remarks>
/// The message passed to base <see cref="Exception"/> is formatted using the current culture and the provided parameters.
/// </remarks>
[System.Runtime.InteropServices.ComVisible(true)]
public class TranslatableException(string? message, Exception? innerException, string resourceKey, params object?[] stringFormatParameters) : Exception(string.Format(CultureInfo.CurrentCulture, message.OrEmpty(), stringFormatParameters), innerException)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatableException"/> class with no message.
    /// </summary>
    public TranslatableException()
        : this(string.Empty) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatableException"/> class with a message and an inner exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception that caused this exception.</param>
    public TranslatableException(string message, Exception innerException)
        : this(message, innerException, string.Empty) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatableException"/> class with a message.
    /// </summary>
    /// <param name="message">The exception message.</param>
    public TranslatableException(string message)
        : this(message, null, string.Empty) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatableException"/> class with an inner exception and a resource key plus optional formatting parameters.
    /// </summary>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="resourceKey">The resource key used to lookup a localized message.</param>
    /// <param name="stringFormatParameters">Parameters used to format the localized message.</param>
    public TranslatableException(Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : this(null, innerException, resourceKey, stringFormatParameters)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatableException"/> class with a message, resource key and optional formatting parameters.
    /// </summary>
    /// <param name="message">The message to format.</param>
    /// <param name="resourceKey">The resource key used to lookup a localized message.</param>
    /// <param name="stringFormatParameters">Parameters used to format the localized message.</param>
    public TranslatableException(string? message, string resourceKey, params object?[] stringFormatParameters)
        : this(message, null, resourceKey, stringFormatParameters)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatableException"/> class with a resource key and optional formatting parameters.
    /// </summary>
    /// <param name="resourceKey">The resource key used to lookup a localized message.</param>
    /// <param name="stringFormatParameters">Parameters used to format the localized message.</param>
    public TranslatableException(string resourceKey, params object?[] stringFormatParameters)
        : this(null, null, resourceKey, stringFormatParameters)
    {
    }

    /// <summary>
    /// Gets the resource key referencing the localized message for this exception.
    /// </summary>
    public string ResourceKey { get; } = resourceKey;

    /// <summary>
    /// Gets the parameters used to format the localized message.
    /// </summary>
    public object?[]? Parameters { get; } = stringFormatParameters;
}
