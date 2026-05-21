// -----------------------------------------------------------------------
// <copyright file="IDomainFakerProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Globalization.Localization.Providers;

namespace MyNet.Fakers.Internet;

/// <summary>
/// Provides fake domain name data for applications.
/// </summary>
public interface IDomainFakerProvider : ICultureScoped
{
    /// <summary>
    /// Gets a list of domain name formats that can be used to generate random domain names.
    /// </summary>
    IReadOnlyList<string> Domains { get; }

    /// <summary>
    /// Gets a list of host names that can be used to generate random domain names.
    /// </summary>
    IReadOnlyList<string> Hosts { get; }
}
