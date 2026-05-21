// -----------------------------------------------------------------------
// <copyright file="ThreadCultureContext.cs" company="Stéphane ANDRE">
// Copyright (c) Stéphane ANDRE. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Globalization;

namespace MyNet.Globalization.Culture;

/// <summary>
/// A lightweight <see cref="ICultureContext"/> that always returns <see cref="CultureInfo.CurrentCulture"/>.
/// Used as the default culture context when the full <see cref="ICultureService"/> is not registered in DI,
/// e.g. during unit tests or when the localization stack is used without the globalization module.
/// </summary>
public sealed class ThreadCultureContext : ICultureContext
{
    /// <inheritdoc />
    public CultureInfo CurrentCulture => CultureInfo.CurrentCulture;
}
