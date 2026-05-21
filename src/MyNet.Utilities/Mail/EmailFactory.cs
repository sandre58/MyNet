// -----------------------------------------------------------------------
// <copyright file="EmailFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Utilities.Mail;

/// <summary>
/// Factory for creating IEmail instances with a predefined sender. This is useful for dependency injection and allows for easier testing and flexibility in email creation, especially when the sender information is constant across the application.
/// </summary>
/// <param name="from">The email address of the sender.</param>
/// <param name="displayName">The display name of the sender.</param>
public class EmailFactory(string from, string displayName = "") : IEmailFactory
{
    /// <summary>
    /// Creates a new instance of an IEmail with the predefined sender information. This method utilizes the static From method of the Email class to create an email instance with the specified sender email address and display name.
    /// </summary>
    /// <returns>A new instance of an IEmail with the predefined sender information.</returns>
    public IEmail Create() => Email.From(from, displayName);
}
