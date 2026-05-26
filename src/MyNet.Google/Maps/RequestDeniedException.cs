// -----------------------------------------------------------------------
// <copyright file="RequestDeniedException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Google.Maps;

public class RequestDeniedException : Exception
{
    public RequestDeniedException()
    {
    }

    public RequestDeniedException(string? message)
        : base(message) { }

    public RequestDeniedException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
