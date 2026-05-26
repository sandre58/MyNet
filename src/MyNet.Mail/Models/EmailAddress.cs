// -----------------------------------------------------------------------
// <copyright file="EmailAddress.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Mail.Models;

public class EmailAddress
{
    public EmailAddress() { }

    public EmailAddress(string emailAddress, string name = "") => (Name, Address) = (name, emailAddress);

    public string Name { get; } = string.Empty;

    public string Address { get; } = string.Empty;
}
