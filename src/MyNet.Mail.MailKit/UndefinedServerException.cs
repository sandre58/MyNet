// -----------------------------------------------------------------------
// <copyright file="UndefinedServerException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Mail.MailKit;

public class UndefinedServerException : Exception
{
    public UndefinedServerException()
        : this("No server has been defined.") { }

    public UndefinedServerException(string? message)
        : base(message) { }

    public UndefinedServerException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
