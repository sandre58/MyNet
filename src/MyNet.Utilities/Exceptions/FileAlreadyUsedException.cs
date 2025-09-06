// -----------------------------------------------------------------------
// <copyright file="FileAlreadyUsedException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Utilities.Exceptions;

/// <summary>
/// Exception thrown when a file is already in use by another process.
/// </summary>
[Serializable]
public class FileAlreadyUsedException : TranslatableException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileAlreadyUsedException"/> class.
    /// </summary>
    public FileAlreadyUsedException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileAlreadyUsedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public FileAlreadyUsedException(string message, Exception innerException)
        : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileAlreadyUsedException"/> class indicating that the specified file is used by another process.
    /// </summary>
    /// <param name="filename">The name of the file in use.</param>
    public FileAlreadyUsedException(string filename)
        : base("File {0} is used by another process", "FileXAlreadyUsedError", filename) { }

    public FileAlreadyUsedException(string? message, Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(message, innerException, resourceKey, stringFormatParameters)
    {
    }

    public FileAlreadyUsedException(Exception? innerException, string resourceKey, params object?[] stringFormatParameters)
        : base(innerException, resourceKey, stringFormatParameters)
    {
    }

    public FileAlreadyUsedException(string? message, string resourceKey, params object?[] stringFormatParameters)
        : base(message, resourceKey, stringFormatParameters)
    {
    }

    public FileAlreadyUsedException(string resourceKey, params object?[] stringFormatParameters)
        : base(resourceKey, stringFormatParameters)
    {
    }
}
