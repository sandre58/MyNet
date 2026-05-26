// -----------------------------------------------------------------------
// <copyright file="HttpException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Http.Exceptions;

/// <summary>
/// Represents errors that occur during HTTP operations. This exception can be thrown when an HTTP request fails due to network issues, server errors, or other problems related to HTTP communication. It provides constructors for creating an instance with a custom message and an optional inner exception for more detailed error information.
/// </summary>
public class HttpException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class with no parameters. This constructor allows for the creation of an exception without providing a specific message or inner exception, which can be useful in scenarios where the error details are not known at the time of exception creation or when a generic HTTP error needs to be represented.
    /// </summary>
    public HttpException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class with a specified error message. This constructor allows for the creation of an exception that includes a custom message describing the error that occurred during an HTTP operation. The message can provide additional context or details about the nature of the error, making it easier for developers to understand and troubleshoot issues related to HTTP communication.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public HttpException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception. This constructor allows for the creation of an exception that includes both a custom message and an inner exception, providing more detailed information about the error that occurred during an HTTP operation.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="exception">The inner exception that is the cause of this exception.</param>
    public HttpException(string message, Exception? exception)
        : base(message, exception) { }
}
