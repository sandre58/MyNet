// -----------------------------------------------------------------------
// <copyright file="MailFaker.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using MyNet.Fakers.Identity;
using MyNet.Fakers.Internet;
using MyNet.Utilities.Generator;

namespace MyNet.Fakers.Contacts;

/// <summary>
/// Current implementation of <see cref="IMailFaker"/>.
/// </summary>
/// <param name="random">The random generator.</param>
/// <param name="identity">The identity faker.</param>
/// <param name="domainFaker">The domain faker.</param>
public sealed class MailFaker(IRandomGenerator random, IIdentityFaker identity, IDomainFaker domainFaker) : IMailFaker
{
    /// <inheritdoc />
    public string Email(string? host = null, CultureInfo? culture = null)
    {
        if (host is null)
        {
            host = domainFaker.Host(culture);

            if (string.IsNullOrWhiteSpace(host))
                host = $"example{random.Int(1, 999)}.com";
            else if (!host.Contains('.', StringComparison.Ordinal))
                host = $"{host}.{domainFaker.Domain(culture)}";
        }

        var user = identity.Username();

        return $"{user}@{host}";
    }
}
