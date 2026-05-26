// -----------------------------------------------------------------------
// <copyright file="TranslatableException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MyNet.Primitives.Exceptions;

/// <summary>
/// Represents an exception that contains a resource key and format parameters intended for localization.
/// </summary>
/// <remarks>
/// The message passed to base <see cref="Exception"/> is formatted using the current culture and the provided parameters.
/// </remarks>
[ComVisible(true)]
public class TranslatableException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatableException"/> class with a specified resource key and format parameters. The message passed to base <see cref="Exception"/> is formatted using the current culture and the provided parameters.
    /// </summary>
    /// <param name="resourceKey">The resource key that identifies the localized message associated with this exception.</param>
    /// <param name="parameters">The parameters to be used for formatting the resource string associated with the ResourceKey.</param>
    public TranslatableException(string resourceKey, params object?[] parameters)
    {
        ResourceKey = resourceKey;
        Parameters = parameters;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatableException"/> class with a specified resource key, inner exception, and format parameters. The message passed to base <see cref="Exception"/> is formatted using the current culture and the provided parameters.
    /// </summary>
    /// <param name="resourceKey">The resource key that identifies the localized message associated with this exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="parameters">The parameters to be used for formatting the resource string associated with the ResourceKey.</param>
    public TranslatableException(string resourceKey, Exception? innerException, params object?[] parameters)
        : base(null, innerException)
    {
        ResourceKey = resourceKey;
        Parameters = parameters;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatableException"/> class with default values for ResourceKey and Parameters. This constructor is intended for use when the exception will be initialized with specific values for ResourceKey and Parameters after construction.
    /// </summary>
    internal TranslatableException()
    {
        ResourceKey = string.Empty;
        Parameters = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatableException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    internal TranslatableException(string message)
        : base(message)
    {
        ResourceKey = string.Empty;
        Parameters = [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslatableException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    internal TranslatableException(string message, Exception innerException)
        : base(message, innerException)
    {
        ResourceKey = string.Empty;
        Parameters = [];
    }

    /// <summary>
    /// Gets the resource key that identifies the localized message associated with this exception. This key will be used by the localization system to retrieve the appropriate message template based on the current culture.
    /// </summary>
    public string ResourceKey { get; }

    /// <summary>
    /// Gets the parameters to be used for formatting the resource string associated with the ResourceKey. These parameters will be passed to the localization system to generate the final message based on the current culture.
    /// </summary>
    public IReadOnlyList<object?> Parameters { get; }

    /// <summary>
    /// Gets the localized message by formatting the resource string associated with the ResourceKey using the provided Parameters.
    /// </summary>
    public override string Message => ResourceKey;
}
