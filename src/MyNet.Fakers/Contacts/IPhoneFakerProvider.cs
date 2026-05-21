// -----------------------------------------------------------------------
// <copyright file="IPhoneFakerProvider.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using MyNet.Globalization.Localization.Providers;

namespace MyNet.Fakers.Contacts;

/// <summary>
/// Provides culture-specific datasets for phone generation.
/// </summary>
public interface IPhoneFakerProvider : ICultureScoped
{
    /// <summary>
    /// Gets supported national phone patterns.
    /// </summary>
    IReadOnlyList<string> NumberFormats { get; }

    /// <summary>
    /// Gets supported mobile phone patterns.
    /// </summary>
    IReadOnlyList<string> MobileNumberFormats { get; }

    /// <summary>
    /// Gets supported international phone patterns.
    /// </summary>
    IReadOnlyList<string> InternationalNumberFormats { get; }
}
