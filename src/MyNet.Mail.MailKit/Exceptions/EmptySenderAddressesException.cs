// -----------------------------------------------------------------------
// <copyright file="EmptySenderAddressesException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Mail.MailKit.Exceptions;

/// <summary>
/// Exception thrown when no sender addresses have been defined for a mail message.
/// </summary>
public class EmptySenderAddressesException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmptySenderAddressesException"/> class with a default error message.
    /// </summary>
    public EmptySenderAddressesException()
        : this("No sender addresses has been defined.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmptySenderAddressesException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public EmptySenderAddressesException(string? message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmptySenderAddressesException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public EmptySenderAddressesException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
