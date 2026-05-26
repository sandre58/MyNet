// -----------------------------------------------------------------------
// <copyright file="MultipleHttpException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MyNet.Http.Exceptions;

/// <summary>
/// Represents an exception that encapsulates multiple HTTP errors, allowing for comprehensive error reporting in scenarios where multiple issues may occur during an HTTP operation.
/// </summary>
public class MultipleHttpException : Exception
{
    private readonly List<HttpError> _errors = [];

    /// <summary>
    /// Gets a read-only collection of <see cref="HttpError"/> instances that represent the individual errors associated with this exception. This property allows developers to access detailed information about each error that occurred during an HTTP operation, facilitating better error handling and debugging.
    /// </summary>
    public IReadOnlyCollection<HttpError> Errors => _errors.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleHttpException"/> class with no parameters. This constructor allows for the creation of an exception without providing specific error details, which can be useful in scenarios where the errors are not known at the time of exception creation or when a generic multiple HTTP error needs to be represented.
    /// </summary>
    public MultipleHttpException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleHttpException"/> class with a specified error message. This constructor allows for the creation of an exception that includes a custom message describing the overall error context, while the individual errors can be added later through the Errors property or other constructors that accept error details.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public MultipleHttpException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleHttpException"/> class with a collection of error messages. This constructor allows for the creation of an exception that includes multiple error messages, which are encapsulated as <see cref="HttpError"/> instances and added to the Errors collection. The provided error messages can offer detailed insights into the various issues that occurred during an HTTP operation, enabling developers to understand and address each specific error effectively.
    /// </summary>
    /// <param name="errors">A collection of error messages that describe the individual errors.</param>
    public MultipleHttpException(IEnumerable<string> errors)
        : this(string.Empty, errors)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleHttpException"/> class with a collection of <see cref="HttpError"/> instances. This constructor allows for the creation of an exception that includes detailed error information through the provided collection of <see cref="HttpError"/> objects, which are added to the Errors collection. Each <see cref="HttpError"/> can contain specific details about an individual error that occurred during an HTTP operation, enabling developers to access comprehensive error information for better debugging and error handling.
    /// </summary>
    /// <param name="errors">A collection of <see cref="HttpError"/> instances that describe the individual errors.</param>
    public MultipleHttpException(IEnumerable<HttpError> errors)
        : this(string.Empty, errors)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleHttpException"/> class with a specified error message and a collection of <see cref="HttpError"/> instances. This constructor allows for the creation of an exception that includes both a custom message describing the overall error context and a collection of detailed error information through the provided <see cref="HttpError"/> objects, which are added to the Errors collection. This combination of a general message and specific error details can help developers understand the broader context of the error while also providing insights into each individual issue that occurred during an HTTP operation.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">A collection of <see cref="HttpError"/> instances that describe the individual errors.</param>
    public MultipleHttpException(string message, IEnumerable<HttpError> errors)
        : base(message)
        => _errors.AddRange(errors);

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleHttpException"/> class with a specified error message and a collection of error messages. This constructor allows for the creation of an exception that includes both a custom message describing the overall error context and a collection of error messages, which are encapsulated as <see cref="HttpError"/> instances and added to the Errors collection. This approach provides a way to include multiple error details in a structured manner while also offering a general message to describe the overall error situation during an HTTP operation.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errors">A collection of error messages that describe the individual errors.</param>
    public MultipleHttpException(string message, IEnumerable<string> errors)
        : base(message)
        => _errors.AddRange(errors.Select(x => new HttpError(x)));

    /// <summary>
    /// Initializes a new instance of the <see cref="MultipleHttpException"/> class with a specified error message and an inner exception. This constructor allows for the creation of an exception that includes a custom message describing the overall error context and a reference to an inner exception that may provide additional details about the underlying cause of the error. The inner exception can be used to capture more specific information about the error that occurred during an HTTP operation, while the main message can offer a broader description of the issue at hand.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="exception">The exception that is the cause of the current exception.</param>
    public MultipleHttpException(string message, Exception exception)
        : base(message, exception)
    {
    }

    /// <summary>
    /// Returns a string that represents the current exception, including the main error message and the details of each individual error contained in the Errors collection. This method constructs a comprehensive error message by combining the main message of the exception with the messages from each <see cref="HttpError"/> in the Errors collection, providing a clear and detailed representation of all the errors that occurred during an HTTP operation. The resulting string can be used for logging, debugging, or displaying error information to users in a more informative way.
    /// </summary>
    /// <returns>A string representation of the current exception.</returns>
    public override string ToString()
    {
        var errorMessage = new StringBuilder();

        if (!string.IsNullOrEmpty(Message))
        {
            _ = errorMessage.AppendLine(Message);
        }

        foreach (var error in Errors)
        {
            _ = errorMessage.AppendLine(CultureInfo.CurrentCulture, $" - {error.Message}");
        }

        return errorMessage.ToString();
    }
}

/// <summary>
/// Represents an individual HTTP error, encapsulating a message and an optional code that provides specific details about the error that occurred during an HTTP operation. This class is designed to be used within the context of a <see cref="MultipleHttpException"/> to provide detailed information about each error that may have contributed to the overall exception, allowing for better error handling and debugging in scenarios where multiple issues may arise during HTTP communication.
/// </summary>
public class HttpError
{
    /// <summary>
    /// Gets the error message that describes the specific issue that occurred during an HTTP operation. This property provides a human-readable description of the error, which can be used for logging, debugging, or displaying error information to users. The message should ideally provide enough context to understand the nature of the error and how it may be resolved.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets an optional code that provides additional information about the specific error that occurred during an HTTP operation. This code can be used to categorize or identify the error in a more structured way, allowing for easier handling and processing of errors based on their codes. The code may correspond to standard HTTP status codes, application-specific error codes, or any other identifier that helps to clarify the nature of the error beyond the message alone.
    /// </summary>
    public string? Code { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpError"/> class with a specified error code and message. This constructor allows for the creation of an error instance that includes both a specific code to categorize the error and a descriptive message that provides details about the error that occurred during an HTTP operation. The combination of a code and message can help developers understand the nature of the error more clearly and facilitate better error handling based on the code.
    /// </summary>
    /// <param name="code">The error code that categorizes the error.</param>
    /// <param name="message">The error message that describes the specific issue.</param>
    public HttpError(string code, string message) => (Code, Message) = (code, message);

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpError"/> class with a specified error message. This constructor allows for the creation of an error instance that includes only a descriptive message, without a specific code. This can be useful in scenarios where an error code is not necessary or available, but a clear description of the error is still required for logging, debugging, or user feedback.
    /// </summary>
    /// <param name="message">The error message that describes the specific issue.</param>
    public HttpError(string message) => Message = message;

    /// <summary>
    /// Returns a string that represents the current error, which is the error message. This method provides a simple way to obtain a string representation of the error, which can be used for logging, debugging, or displaying error information to users. The resulting string will contain the message that describes the specific issue that occurred during an HTTP operation, allowing for better understanding and communication of the error details.
    /// </summary>
    /// <returns>A string that represents the current error.</returns>
    public override string ToString() => Message;
}
