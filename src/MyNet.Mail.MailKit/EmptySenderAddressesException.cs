// -----------------------------------------------------------------------
// <copyright file="EmptySenderAddressesException.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace MyNet.Mail.MailKit;

public class EmptySenderAddressesException : Exception
{
    public EmptySenderAddressesException()
        : this("No sender addresses has been defined.") { }

    public EmptySenderAddressesException(string? message)
        : base(message) { }

    public EmptySenderAddressesException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
