// -----------------------------------------------------------------------
// <copyright file="MailSmtpServiceFactory.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MyNet.Mail.Smtp;

public class MailSmtpServiceFactory : IMailServiceFactory
{
    public IMailService Create(SmtpClientOptions options) => new MailSmtpService(options);
}
