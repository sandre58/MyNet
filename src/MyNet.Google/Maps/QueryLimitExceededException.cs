// -----------------------------------------------------------------------
// <copyright file="QueryLimitExceededException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Google.Maps;

[Serializable]
public class QueryLimitExceededException : System.Net.WebException
{
    public QueryLimitExceededException() { }

    public QueryLimitExceededException(string? message)
        : base(message) { }

    public QueryLimitExceededException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public QueryLimitExceededException(string? message, Exception? innerException, System.Net.WebExceptionStatus status, System.Net.WebResponse? response)
        : base(message, innerException, status, response)
    {
    }

    public QueryLimitExceededException(string? message, System.Net.WebExceptionStatus status)
        : base(message, status)
    {
    }
}
